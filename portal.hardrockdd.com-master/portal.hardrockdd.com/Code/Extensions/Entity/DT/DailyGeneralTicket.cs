using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class DailyGeneralTicket
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

        public DailyEmployeeEntry AddHoursEntry(DateTime workDate, Employee emp)
        {
            var entry = new DailyEmployeeEntry
            {
                DTCo = DTCo,
                TicketId = TicketId,
                WorkDate = workDate,
                PRCo = emp.PRCo,
                tEmployeeId = emp.EmployeeId,
                Value = 8,
                EarnCodeId = (short)(emp.EarnCodeId != 1 ? 18 : 17),
                tEntryTypeId = (int)DB.EntryTypeEnum.Admin,
                LineNum = DailyTicket.DailyEmployeeEntries.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1,
                DailyTicket = DailyTicket,


                PREmployee = emp,
            };

            DailyTicket.DailyEmployeeEntries.Add(entry);

            return entry;
        }

        public DailyEmployeePerdiem AddPerdiem(DateTime workDate, Employee emp)
        {
            var perdiem = new DailyEmployeePerdiem
            {
                DTCo = DTCo,
                TicketId = TicketId,
                PerDiemId = (int)DB.PerdiemEnum.No,
                WorkDate = workDate,
                LineNum = DailyTicket.DailyEmployeePerdiems.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineNum) + 1,
                DailyTicket = DailyTicket,
                ModifiedBy = StaticFunctions.GetUserId(),
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