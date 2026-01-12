using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class DailyEmployeeEntry
    {

        private VPContext _db;

        public VPContext db
        {
            set
            {
                _db = value;
            }
            get
            {
                _db ??= DailyTicket.db;
                _db ??= VPContext.GetDbContextFromEntity(this);
                return _db;
            }
        }


        public DateTime? WorkDate
        {
            get
            {
                return tWorkDate;
            }
            set
            {
                if (value != tWorkDate)
                {
                    tWorkDate = value;
                }
            }
        }

        public string CrewId
        {
            get
            {
                if (DailyTicket.DailyShopTicket != null)
                    return DailyTicket.DailyShopTicket.CrewId;
                if (DailyTicket.DailyTruckTicket != null)
                    return DailyTicket.DailyTruckTicket.CrewId;

                return "";
            }
        }

        public decimal PayrollValue
        {
            get
            {
                return (ValueAdj ?? Value) ?? 0;
            }
        }

        public int? EntryTypeId
        {
            get
            {
                return tEntryTypeId;
            }
            set
            {
                if (value != tEntryTypeId)
                {
                    UpdateEntryType((EntryTypeEnum)(value ?? 0));
                }
            }
        }

        public EntryTypeEnum EntryType
        {
            get
            {
                return (EntryTypeEnum)(this.tEntryTypeId ?? 0);
            }
            set
            {
                EntryTypeId = (int)(value);
            }
        }

        public string JobId
        {
            get
            {
                return tJobId;
            }
            set
            {
                if (value != tJobId)
                {
                    UpdateJobInfo(value);
                }
            }
        }

        public string PhaseId
        {
            get
            {
                return tPhaseId;
            }
            set
            {
                if (value != tPhaseId)
                {
                    UpdateJobPhaseInfo(value);
                }
            }
        }

        public string EquipmentId
        {
            get
            {
                return tEquipmentId;
            }
            set
            {
                if (value != tEquipmentId)
                {
                    UpdateEquipmentInfo(value);
                }
            }
        }

        public int? EmployeeId
        {
            get
            {
                return tEmployeeId;
            }
            set
            {
                if (value != tEmployeeId)
                {
                    UpdateEmployeeInfo(value);
                }
            }
        }

        private void UpdateEmployeeInfo(int? newVal)
        {
            var origEmployeeId = tEmployeeId;

            if (PREmployee == null && tEmployeeId != null)
            {
                var obj = db.Employees.FirstOrDefault(f => f.EmployeeId == tEmployeeId);
                tEmployeeId = obj.EmployeeId;
                PRCo = obj.PRCo;
                PREmployee = obj;
            }

            if (tEmployeeId != newVal)
            {
                var obj = db.Employees.FirstOrDefault(f => f.EmployeeId == newVal);
                if (obj != null)
                {
                    PRCo = obj.PRCo;
                    tEmployeeId = obj.EmployeeId;
                    EarnCodeId = obj.EarnCodeId;
                    PREmployee = obj;
                }
                else
                {
                    PRCo = null;
                    tEmployeeId = null;
                    PREmployee = null;
                    EarnCodeId = null;
                }

                foreach (var perdiem in DailyTicket.DailyEmployeePerdiems.Where(f => f.LineNum == PerdiemLineNum).ToList())
                {
                    perdiem.PRCo = PRCo;
                    perdiem.EmployeeId = EmployeeId;
                    perdiem.PREmployee = PREmployee;
                }

                foreach (var entry in DailyTicket.DailyEmployeeEntries.Where(f => f.tEmployeeId == origEmployeeId && f.tEmployeeId != null && f.LineNum != LineNum).ToList())
                {
                    entry.PRCo = PRCo;
                    entry.tEmployeeId = EmployeeId;
                    entry.EarnCodeId = EarnCodeId;
                    entry.PREmployee = PREmployee;
                }

                //Clear up multiple Employee Entries in Perdiem Table
                if (DailyTicket.DailyEmployeePerdiems.Where(f => f.EmployeeId == tEmployeeId && f.tWorkDate == WorkDate).Count() > 1)
                {
                    var perdiemList = DailyTicket.DailyEmployeePerdiems.Where(f => f.tEmployeeId == EmployeeId && f.tWorkDate == WorkDate).ToList();
                    var firstPerdiem = perdiemList.FirstOrDefault();

                    foreach (var perdiem in perdiemList)
                    {
                        if (perdiem.LineNum != PerdiemLineNum)
                        {
                            var entries = DailyTicket.DailyEmployeeEntries.Where(f => f.PerdiemLineNum == perdiem.LineNum).ToList();
                            entries.ForEach(e => e.PerdiemLineNum = PerdiemLineNum);
                            DailyTicket.DailyEmployeePerdiems.Remove(perdiem);
                            //db.DailyEmployeePerdiems.Remove(perdiem);
                        }
                    }
                }
            }

            if (PREmployee != null)
            {
                if (PREmployee?.EarnCode?.Method != EarnCode?.Method)
                {
                    EarnCodeId = PREmployee.EarnCodeId;
                    EarnCode = PREmployee.EarnCode;
                }
            }
        }

        private void UpdateEntryType(EntryTypeEnum newValue)
        {
            switch (newValue)
            {
                case EntryTypeEnum.Admin:
                    tEquipmentId = null;
                    Equipment = null;
                    tJobId = null;
                    Job = null;
                    JobId = null;
                    PhaseId = null;

                    break;
                case EntryTypeEnum.Equipment:
                    tJobId = null;
                    Job = null;
                    tPhaseId = null;
                    break;
                case EntryTypeEnum.Job:
                    tEquipmentId = null;
                    Equipment = null;
                    break;
                default:
                    break;
            }
            tEntryTypeId = (int)newValue;
        }

        private void UpdateJobInfo(string newValue)
        {
            if (Job == null && tJobId != null)
            {
                var job = db.Jobs.FirstOrDefault(f => f.JobId == tJobId);
                tJobId = job.JobId;
                JCCo = job.JCCo;
                PhaseGroupId = job.JCCompanyParm.HQCompanyParm.PhaseGroupId;
                Job = job;
            }

            if (tJobId != newValue)
            {
                var job = db.Jobs.FirstOrDefault(f => f.JobId == newValue);
                if (job != null)
                {
                    tJobId = job.JobId;
                    JCCo = job.JCCo;
                    PhaseGroupId = job.JCCompanyParm.HQCompanyParm.PhaseGroupId;
                    Job = job;
                    JobPhase = job?.AddMasterPhase(tPhaseId);
                    PhaseId = JobPhase?.PhaseId;
                }
                else
                {
                    tJobId = null;
                    JCCo = null;
                    PhaseGroupId = null;
                    Job = null;
                }
            }
        }

        private void UpdateJobPhaseInfo(string newValue)
        {
            if (Job == null || string.IsNullOrEmpty(newValue))
            {
                tPhaseId = null;
                JobPhase = null;
                return;
            }

            if (JobPhase == null && tPhaseId != null)
            {
                var phase = Job.Phases.FirstOrDefault(f => f.PhaseId == tPhaseId);
                tPhaseId = phase.PhaseId;
                JCCo = phase.JCCo;
                PhaseId = phase.PhaseId;
                JobPhase = phase;
            }

            if (tPhaseId != newValue)
            {
                var phase = Job.Phases.FirstOrDefault(f => f.PhaseId == newValue);
                if (phase == null) //Phase is missing from Job Phase List *Add It*
                {
                    phase = Job.AddMasterPhase(newValue);
                }

                if (phase != null)
                {
                    JCCo = phase.JCCo;
                    PhaseGroupId = phase.PhaseGroupId;
                    tPhaseId = phase.PhaseId;
                    JobPhase = phase;

                    if (phase.JobPhaseCosts.Count == 0)
                    {
                        phase = Job.AddMasterPhase(PhaseId, true);
                    }
                }
                else
                {
                    JCCo = null;
                    PhaseGroupId = null;
                    tPhaseId = null;
                    JobPhase = null;
                }
            }
        }

        private void UpdateEquipmentInfo(string newValue)
        {

            if (Equipment == null && tEquipmentId != null)
            {
                var equipment = db.Equipments.FirstOrDefault(f => f.EquipmentId == tEquipmentId);
                tEquipmentId = equipment.EquipmentId;
                EMCo = equipment.EMCo;
                Equipment = equipment;
            }

            if (tEquipmentId != newValue)
            {
                var equipment = db.Equipments.FirstOrDefault(f => f.EquipmentId == newValue);
                if (equipment != null)
                {
                    tEquipmentId = equipment.EquipmentId;
                    EMCo = equipment.EMCo;
                    Equipment = equipment;
                }
                else
                {
                    tEquipmentId = null;
                    EMCo = null;
                    Equipment = null;
                }
            }
        }

        public void GenerateDTPayrollEntry()
        {
            UpdateEmployeeInfo(EmployeeId);
            var hourEntry = InitDTPayRollHourEntry();
            hourEntry.Hours = Math.Round(hourEntry.Hours ?? 0, 2);
            if (!string.IsNullOrEmpty(CrewId))
            {
                PREmployee.CrewId = CrewId;
            }

            DailyTicket.DTPayrollHours.Add(hourEntry);
        }

        public DTPayrollHour InitDTPayRollHourEntry()
        {
            //var lineNum = DailyTicket.DTPayrollHours.Where(f => f.EmployeeId == EmployeeId && f.WorkDate == WorkDate).DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1;
            var entry = new DTPayrollHour()
            {
                //DailyTicket = DailyTicket,
                // Employee = PREmployee,

                PRCo = PREmployee.PRCo,
                EmployeeId = PREmployee.EmployeeId,
                WorkDate = (DateTime)WorkDate,
                //LineNum = lineNum,
                DTCo = DTCo,
                TicketId = TicketId,
                TicketLineNum = LineNum,
                EntryTypeId = EntryTypeId,
                EarnCodeId = EarnCodeId,

                Hours = PayrollValue,
                ModifiedOn = DailyTicket.ModifiedOn,
                ModifiedBy = DailyTicket.ModifiedBy,
                Comments =  Comments,
                Status = (int)PayrollEntryStatusEnum.Accepted,


            };


            if (entry.Comments != null && entry.Comments.Length >= 2000)
            {
                entry.Comments = Comments.Substring(0, 1999);
            }
            ////Correct for missing phase 
            //if (entry.DailyTicket.FormId == (int)DTFormEnum.CrewTicket && model.JobId == null)
            //{
            //    model.JobId = entry.DailyTicket.DailyShopTicket.JobId;
            //    if (model.PhaseId == null)
            //        model.PhaseId = "   100-  -";
            //}

            switch (EntryType)
            {
                case EntryTypeEnum.Admin:
                    break;
                case EntryTypeEnum.Job:
                    entry.Job = Job;
                    entry.JCCo = Job.JCCo;
                    entry.JobId = Job.JobId;
                    entry.PhaseGroupId = PhaseGroupId;
                    entry.PhaseId = PhaseId;

                    if (Job.JobType == JCJobTypeEnum.ShopYard)
                    {
                        entry.Job = null;
                        entry.JCCo = null;
                        entry.JobId = null;
                        entry.PhaseId = null;
                        entry.EntryTypeId = (int)EntryTypeEnum.Admin;
                    }
                    break;
                case EntryTypeEnum.Equipment:
                    entry.Equipment = Equipment;
                    entry.EMCo = Equipment.EMCo;
                    entry.EquipmentId = Equipment.EquipmentId;
                    break;
                default:
                    break;
            }


            return entry;
        }

        //public void UpdateFromModel(Models.Views.DailyTicket.DailyCrewEmployeeViewModel model)
        //{
        //    if (model == null)
        //        return;

        //    /****Write the changes to object****/
        //    if (EntryType != model.EntryTypeId)
        //    {
        //        EntryType = model.EntryTypeId;
        //    }
        //    else
        //    {
        //        switch (EntryType)
        //        {
        //            case EntryTypeEnum.Admin:
        //                break;
        //            case EntryTypeEnum.Job:
        //                JobId = model.JobId;
        //                PhaseId = model.PhaseId;
        //                break;
        //            case EntryTypeEnum.Equipment:
        //                EquipmentId = model.EquipmentId;
        //                break;
        //            default:
        //                break;
        //        }
        //        EmployeeId = model.EmployeeId;
        //        Value = model.Value;
        //        Comments = model.Comments;
        //    }


        //    //Find Perdiem Line for update
        //    var perdiem = GetPerdiem();
        //    perdiem.UpdateFromModel(model);

        //}


        //public void UpdateFromModel(Models.Views.DailyTicket.DailyShopEmployeeViewModel model)
        //{
        //    if (model == null)
        //        return;

        //    /****Write the changes to object****/
        //    if (EntryType != model.EntryTypeId)
        //    {
        //        EntryType = model.EntryTypeId;
        //    }
        //    else
        //    {
        //        switch (EntryType)
        //        {
        //            case EntryTypeEnum.Admin:
        //                break;
        //            case EntryTypeEnum.Job:
        //                JobId = model.JobId;
        //                PhaseId = model.PhaseId;
        //                break;
        //            case EntryTypeEnum.Equipment:
        //                EquipmentId = model.EquipmentId;
        //                break;
        //            default:
        //                break;
        //        }
        //        EmployeeId = model.EmployeeId;
        //        Value = model.Value;
        //        Comments = model.Comments;
        //    }


        //    //Find Perdiem Line for update
        //    var perdiem = GetPerdiem();
        //    perdiem.UpdateFromModel(model);

        //}


        //public void UpdateFromModel(Models.Views.DailyTicket.DailyTruckEmployeeViewModel model)
        //{
        //    if (model == null)
        //        return;

        //    /****Write the changes to object****/
        //    if (EntryType != model.EntryTypeId)
        //    {
        //        EntryType = model.EntryTypeId;
        //    }
        //    else
        //    {
        //        switch (EntryType)
        //        {
        //            case EntryTypeEnum.Admin:
        //                break;
        //            case EntryTypeEnum.Job:
        //                JobId = model.JobId;
        //                PhaseId = model.PhaseId;
        //                break;
        //            case EntryTypeEnum.Equipment:
        //                EquipmentId = model.EquipmentId;
        //                break;
        //            default:
        //                break;
        //        }
        //        EmployeeId = model.EmployeeId;
        //        Value = model.Value;
        //        Comments = model.Comments;
        //    }


        //    //Find Perdiem Line for update
        //    var perdiem = GetPerdiem();
        //    perdiem.UpdateFromModel(model);

        //}

        public DailyEmployeePerdiem GetPerdiem()
        {
            var perdiem = db.DailyEmployeePerdiems.FirstOrDefault(f => f.DTCo == DTCo && f.TicketId == TicketId && f.LineNum == PerdiemLineNum);
            var saveRequired = perdiem == null;
            if (perdiem == null && PREmployee != null)
            {
                perdiem = db.DailyEmployeePerdiems.FirstOrDefault(f => f.DTCo == DTCo && f.TicketId == TicketId && f.PRCo == PREmployee.PRCo && f.tEmployeeId == PREmployee.EmployeeId);
                if (perdiem == null)
                    perdiem = DailyTicket.AddPerdiem(PREmployee);
            }
            else if (perdiem == null)
                perdiem = DailyTicket.AddPerdiem();

            if (PerdiemLineNum != perdiem.LineNum)
            {
                saveRequired = true;
                PerdiemLineNum = perdiem.LineNum;
            }

            perdiem.IsSaveRequired = saveRequired;

            return perdiem;
        }

    }
}