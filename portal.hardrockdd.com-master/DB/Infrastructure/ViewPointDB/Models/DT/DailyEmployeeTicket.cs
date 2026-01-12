using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class DailyEmployeeTicket
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

        public DailyEmployeeEntry AddHoursEntry(DateTime workDate)
        {
            var entry = new DailyEmployeeEntry
            {
                db = db,
                DailyTicket = DailyTicket,
                PREmployee = PREmployee,
                EarnCode = PREmployee.EarnCode,

                DTCo = DTCo,
                TicketId = TicketId,
                LineNum = DailyTicket.DailyEmployeeEntries.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1,

                tWorkDate = workDate,
                PRCo = PREmployee.PRCo,
                tEmployeeId = EmployeeId,
                PhaseGroupId = DailyTicket.HQCompanyParm.PhaseGroupId,
                CostTypeId = 1,
                tPhaseId = "   100-  -",
                EarnCodeId = PREmployee.EarnCodeId,
                tEntryTypeId = (int)EntryTypeEnum.Admin,
            };
            var perdiem = DailyTicket.DailyEmployeePerdiems.FirstOrDefault(f => f.PRCo == entry.PRCo && f.EmployeeId == entry.EmployeeId && f.WorkDate == entry.WorkDate);
            if (perdiem == null)
                perdiem = this.AddPerdiem((DateTime)entry.WorkDate);


            entry.PerdiemLineNum = perdiem?.LineNum;

            DailyTicket.DailyEmployeeEntries.Add(entry);

            return entry;
        }

        public DailyEmployeePerdiem AddPerdiem(DateTime workDate)
        {
            var perdiem = DailyTicket.AddPerdiem(PREmployee, workDate);
            perdiem.WorkDate = workDate;

            return perdiem;
        }

        public void CreateDefaultEntries()
        {
            var calanderDays = db.Calendars.Where(f => f.Week == WeekId).ToList();
            foreach (var day in calanderDays)
            {
                var entry = DailyTicket.DailyEmployeeEntries.FirstOrDefault(f => f.tWorkDate == day.Date);
                if (entry == null)
                    entry = AddHoursEntry(day.Date);

                var perdeim = DailyTicket.DailyEmployeePerdiems.FirstOrDefault(f => f.tWorkDate == day.Date);
                if (perdeim == null)
                    perdeim = AddPerdiem(day.Date);
                entry.PerdiemLineNum ??= perdeim.LineNum;
            }
        }
    }
}