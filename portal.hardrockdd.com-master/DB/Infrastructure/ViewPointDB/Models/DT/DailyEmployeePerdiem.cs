using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class DailyEmployeePerdiem
    {
        public bool IsSaveRequired { get; set; }

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

        public PerdiemEnum PerDiem
        {
            get
            {
                return (PerdiemEnum)(PerDiemId ?? (int)PerdiemEnum.No);
            }
            set
            {
                PerDiemId = (int)value;
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
            Comments ??= "";
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
                EntryType = EntryTypeEnum.Admin,
                EarnCodeId = 20,

                PerdiemId = PayrolPerdiem,
                ModifiedOn = DailyTicket.ModifiedOn,
                ModifiedBy = DailyTicket.ModifiedBy,
                Comments = Comments.Length >= 2000 ? Comments.Substring(0, 1999) : Comments,
                Status = (int)PayrollEntryStatusEnum.Accepted,


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
                Status = (int)PayrollEntryStatusEnum.Accepted,


            };
            if (payrollPerdiem.Comments != null && payrollPerdiem.Comments.Length >= 2000)
            {
                payrollPerdiem.Comments = Comments.Substring(0, 1999);
            }
            switch (entry.EntryType)
            {
                case EntryTypeEnum.Admin:
                    break;
                case EntryTypeEnum.Job:
                    payrollPerdiem.Job = entry.Job;
                    payrollPerdiem.JCCo = entry.Job.JCCo;
                    payrollPerdiem.JobId = entry.Job.JobId;
                    payrollPerdiem.PhaseGroupId = entry.PhaseGroupId;
                    payrollPerdiem.PhaseId = entry.PhaseId;

                    if (entry.Job.JobType == JCJobTypeEnum.ShopYard)
                    {
                        payrollPerdiem.Job = null;
                        payrollPerdiem.JCCo = null;
                        payrollPerdiem.JobId = null;
                        payrollPerdiem.PhaseId = null;
                        payrollPerdiem.EntryTypeId = (int)EntryTypeEnum.Admin;
                    }
                    break;
                case EntryTypeEnum.Equipment:
                    payrollPerdiem.Equipment = entry.Equipment;
                    payrollPerdiem.EMCo = entry.Equipment.EMCo;
                    payrollPerdiem.EquipmentId = entry.Equipment.EquipmentId;
                    break;
                default:
                    break;
            }


            return payrollPerdiem;
        }

        //public void UpdateFromModel(Models.Views.DailyTicket.DailyCrewEmployeeViewModel model)
        //{
        //    if (model == null)
        //        return;

        //    EmployeeId = model.EmployeeId;
        //    PerDiemId = (int)model.Perdiem;
        //    PerDiemIdAdj = null;
        //    Comments = model.Comments;
        //}

        //public void UpdateFromModel(Models.Views.DailyTicket.DailyShopEmployeeViewModel model)
        //{
        //    if (model == null)
        //        return;

        //    EmployeeId = model.EmployeeId;
        //    PerDiemId = (int)model.Perdiem;
        //    PerDiemIdAdj = null;
        //    Comments = model.Comments;
        //}

        //public void UpdateFromModel(Models.Views.DailyTicket.DailyTruckEmployeeViewModel model)
        //{
        //    if (model == null)
        //        return;

        //    EmployeeId = model.EmployeeId;
        //    PerDiemId = (int)model.Perdiem;
        //    PerDiemIdAdj = null;
        //    Comments = model.Comments;
        //}
    }
}