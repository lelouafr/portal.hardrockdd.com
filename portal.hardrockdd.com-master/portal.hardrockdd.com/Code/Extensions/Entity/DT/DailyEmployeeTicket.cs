using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class DailyEmployeeTicket
    {
        private VPEntities _db;

        public VPEntities db
        {
            set
            {
                _db = value;
            }
            get
            {
                if (_db == null)
                {
                    _db = VPEntities.GetDbContextFromEntity(this);

                    if (_db == null)
                        db = DailyTicket.db;

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }

        public DailyEmployeeEntry AddHoursEntry(DateTime workDate)
        {
            var entry = new DailyEmployeeEntry
            {
                DTCo = DTCo,
                TicketId = TicketId,
                WorkDate = workDate,
                PRCo = PREmployee.PRCo,
                tEmployeeId = EmployeeId,
                PhaseGroupId = DailyTicket.HQCompanyParm.PhaseGroupId,
                CostTypeId = 1,
                tPhaseId = "   100-  -",
                EarnCodeId = PREmployee.EarnCodeId,
                tEntryTypeId = (int)DB.EntryTypeEnum.Admin,
                LineNum = DailyTicket.DailyEmployeeEntries.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1,
                DailyTicket = DailyTicket,
            };

            DailyTicket.DailyEmployeeEntries.Add(entry);

            return entry;
        }

        public DailyEmployeePerdiem AddPerdiem(DateTime workDate)
        {
            //var perdiem = new DailyEmployeePerdiem
            //{
            //    DTCo = DTCo,
            //    TicketId = TicketId,
            //    PerDiemId = (int)DB.PerdiemEnum.No,
            //    WorkDate = workDate,
            //    LineNum = DailyTicket.DailyEmployeePerdiems.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1,
            //    DailyTicket = DailyTicket,
            //    ModifiedBy = StaticFunctions.GetUserId(),
            //    ModifiedOn = DateTime.Now,
            //};
            //if (PREmployee != null)
            //{
            //    perdiem.PREmployee = PREmployee;
            //    perdiem.PRCo = PREmployee.PRCo;
            //    perdiem.tEmployeeId = PREmployee.EmployeeId;
            //}

            //DailyTicket.DailyEmployeePerdiems.Add(perdiem);
            var perdiem = DailyTicket.AddPerdiem(PREmployee);
            perdiem.WorkDate = workDate;

            return perdiem;
        }

        public void CreateDefaultEntries()
        {
            var calanderDays = db.Calendars.Where(f => f.Week == WeekId).ToList();
            foreach (var day in calanderDays)
            {
                var entry = DailyTicket.DailyEmployeeEntries.FirstOrDefault(f => f.tWorkDate == day.Date);
                var perdeim = DailyTicket.DailyEmployeePerdiems.FirstOrDefault(f => f.tWorkDate == day.Date);
                if (entry == null)
                    entry = AddHoursEntry(day.Date);
                if (perdeim == null)
                    perdeim = AddPerdiem(day.Date);
                entry.PerdiemLineNum = perdeim.LineNum;
            }
        }
    }
}