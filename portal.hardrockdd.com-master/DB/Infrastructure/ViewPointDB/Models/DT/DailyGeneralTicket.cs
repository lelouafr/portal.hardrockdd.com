using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class DailyGeneralTicket
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

        public DailyEmployeeEntry AddHolidayHoursEntry(DateTime workDate, Employee? emp)
        {
            var entry = new DailyEmployeeEntry
            {
                db = db,
                DailyTicket = DailyTicket,
                PREmployee = emp,

                DTCo = DTCo,
                TicketId = TicketId,
                WorkDate = workDate,
                PRCo = emp?.PRCo,
                tEmployeeId = emp?.EmployeeId,
                Value = 8,
                EarnCodeId = (short)(emp.EarnCodeId != 1 ? 18 : 17),
                tEntryTypeId = (int)EntryTypeEnum.Admin,
                LineNum = DailyTicket.DailyEmployeeEntries.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1,
            };
            var perdeim = DailyTicket.DailyEmployeePerdiems.FirstOrDefault(f => f.WorkDate == entry.WorkDate);
            entry.PerdiemLineNum = perdeim?.LineNum;

            DailyTicket.DailyEmployeeEntries.Add(entry);

            return entry;
        }

        public DailyEmployeeEntry AddHoursEntry()
        {
            var entry = new DailyEmployeeEntry
            {
                db = db,
                DailyTicket = DailyTicket,

                DTCo = DTCo,
                TicketId = TicketId,
                LineNum = DailyTicket.DailyEmployeeEntries.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1,

                tWorkDate = DateTime.Now.Date,
                tEntryTypeId = (int)EntryTypeEnum.Admin,

                ModifiedBy = db.CurrentUserId,
                ModifiedOn = DateTime.Now,
            };
            var perdeim = DailyTicket.DailyEmployeePerdiems.FirstOrDefault(f => f.WorkDate == entry.WorkDate);
            if (perdeim == null)
                perdeim = AddPerdiem((DateTime)entry.WorkDate, null);
            entry.PerdiemLineNum ??= perdeim?.LineNum;

            DailyTicket.DailyEmployeeEntries.Add(entry);

            return entry;
        }

        public DailyEmployeePerdiem AddPerdiem(DateTime workDate, Employee emp)
        {
            var perdiem = new DailyEmployeePerdiem
            {
                DTCo = DTCo,
                TicketId = TicketId,
                PerDiemId = (int)PerdiemEnum.No,
                WorkDate = workDate,
                LineNum = DailyTicket.DailyEmployeePerdiems.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1,
                DailyTicket = DailyTicket,
                ModifiedBy = db.CurrentUserId,
                ModifiedOn = DateTime.Now,
            };
            if (emp != null)
            {
                perdiem.PREmployee = emp;
                perdiem.PRCo = emp.PRCo;
                perdiem.tEmployeeId = emp.EmployeeId;
            }

            DailyTicket.DailyEmployeePerdiems.Add(perdiem);

            return perdiem;
        }

    }
}