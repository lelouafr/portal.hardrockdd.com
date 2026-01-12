using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class DailyEmployeePerdiem
    {
        public bool IsSaveRequired { get; set; }

        private VPEntities db
        {
            get
            {

                var db = VPEntities.GetDbContextFromEntity(this);

                if (db == null && this.DailyTicket != null)
                    db = this.DailyTicket.db;

                if (db == null)
                    throw new NullReferenceException("GetDbContextFromEntity is null");

                return db;
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
                    //DailyTicket.WorkDate = value;
                    tWorkDate = value;
                }
            }
        }

        public int PayrolPerdiem
        {
            get
            {
                return (PerDiemIdAdj ?? PerDiemId) ?? 0;
            }
        }

        public DB.PerdiemEnum Perdiem
        {
            get
            {
                return (DB.PerdiemEnum)(PerDiemId ?? (int)DB.PerdiemEnum.No);
            }
            set
            {
                //value ??= DB.PerdiemEnum.No;
                //PerDiemIdAdj = (int)value ?? (int)DB.PerdiemEnum.No;
                PerDiemId = (int)(value);
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
                    PREmployee = obj;
                }
                else
                {
                    PRCo = null;
                    tEmployeeId = null;
                    PREmployee = null;
                }
            }
        }

        public void GenerateDTPayrollPerdiem()
        {
            UpdateEmployeeInfo(EmployeeId);

            var entryList = DailyTicket.DailyEmployeeEntries.Where(f => f.EmployeeId == EmployeeId && f.WorkDate == WorkDate).ToList();
            foreach (var entry in entryList)
            {
                var perdiemEntry = InitDTPayRollHourPerdiem(entry);
                DailyTicket.DTPayrollPerdiems.Add(perdiemEntry);
            }
            if (!entryList.Any())
            {
                var perdiemEntry = InitDTPayRollHourPerdiem();
                DailyTicket.DTPayrollPerdiems.Add(perdiemEntry);
            }


        }

        public DTPayrollPerdiem InitDTPayRollHourPerdiem()
        {
            //var lineNum = DailyTicket.DTPayrollPerdiems.Where(f => f.EmployeeId == EmployeeId && f.WorkDate == WorkDate).DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1;
            var payrollPerdiem = new DTPayrollPerdiem()
            {
                //DailyTicket = DailyTicket,
                //Employee = PREmployee,

                PRCo = PREmployee.PRCo,
                EmployeeId = PREmployee.EmployeeId,
                //LineNum = lineNum,
                WorkDate = (DateTime)WorkDate,
                DTCo = DTCo,
                TicketId = TicketId,
                TicketLineNum = LineNum,
                EntryType = DB.EntryTypeEnum.Admin,
                EarnCodeId = 20,

                PerdiemId = PayrolPerdiem,
                ModifiedOn = DailyTicket.ModifiedOn,
                ModifiedBy = DailyTicket.ModifiedBy,
                Comments = Comments,
                Status = (int)DB.PayrollEntryStatusEnum.Accepted,


            };

            return payrollPerdiem;
        }

        public DTPayrollPerdiem InitDTPayRollHourPerdiem(DailyEmployeeEntry entry)
        {
            //var lineNum = DailyTicket.DTPayrollPerdiems.Where(f => f.EmployeeId == EmployeeId && f.WorkDate == WorkDate).DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1;
            var payrollPerdiem = new DTPayrollPerdiem()
            {
                //DailyTicket = DailyTicket,
                //Employee = PREmployee,
                PRCo = PREmployee.PRCo,
                EmployeeId = PREmployee.EmployeeId,
                WorkDate = (DateTime)WorkDate,
                //LineNum = lineNum,

                DTCo = DTCo,
                TicketId = TicketId,
                TicketLineNum = LineNum,
                EntryType = entry.EntryType,
                EarnCodeId = 20,

                PerdiemId = PayrolPerdiem,
                ModifiedOn = DailyTicket.ModifiedOn,
                ModifiedBy = DailyTicket.ModifiedBy,
                Comments = Comments,
                Status = (int)DB.PayrollEntryStatusEnum.Accepted,


            };

            switch (entry.EntryType)
            {
                case DB.EntryTypeEnum.Admin:
                    break;
                case DB.EntryTypeEnum.Job:
                    payrollPerdiem.Job = entry.Job;
                    payrollPerdiem.JCCo = entry.Job.JCCo;
                    payrollPerdiem.JobId = entry.Job.JobId;
                    payrollPerdiem.PhaseGroupId = entry.PhaseGroupId;
                    payrollPerdiem.PhaseId = entry.PhaseId;

                    if (entry.Job.JobType == DB.JCJobTypeEnum.ShopYard)
                    {
                        payrollPerdiem.Job = null;
                        payrollPerdiem.JCCo = null;
                        payrollPerdiem.JobId = null;
                        payrollPerdiem.PhaseId = null;
                        payrollPerdiem.EntryTypeId = (int)DB.EntryTypeEnum.Admin;
                    }
                    break;
                case DB.EntryTypeEnum.Equipment:
                    payrollPerdiem.Equipment = entry.Equipment;
                    payrollPerdiem.EMCo = entry.Equipment.EMCo;
                    payrollPerdiem.EquipmentId = entry.Equipment.EquipmentId;
                    break;
                default:
                    break;
            }


            return payrollPerdiem;
        }

        public void UpdateFromModel(Models.Views.DailyTicket.DailyCrewEmployeeViewModel model)
        {
            if (model == null)
                return;

            EmployeeId = model.EmployeeId;
            PerDiemId = (int)model.Perdiem;
            PerDiemIdAdj = null;
            Comments = model.Comments;
        }

        public void UpdateFromModel(Models.Views.DailyTicket.DailyShopEmployeeViewModel model)
        {
            if (model == null)
                return;

            EmployeeId = model.EmployeeId;
            PerDiemId = (int)model.Perdiem;
            PerDiemIdAdj = null;
            Comments = model.Comments;
        }

        public void UpdateFromModel(Models.Views.DailyTicket.DailyTruckEmployeeViewModel model)
        {
            if (model == null)
                return;

            EmployeeId = model.EmployeeId;
            PerDiemId = (int)model.Perdiem;
            PerDiemIdAdj = null;
            Comments = model.Comments;
        }
    }
}