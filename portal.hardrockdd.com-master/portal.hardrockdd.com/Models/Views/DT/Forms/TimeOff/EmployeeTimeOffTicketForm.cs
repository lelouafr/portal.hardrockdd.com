using System;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.DailyTicket.Form
{

    public class EmployeeTimeOffTicketFormViewModel : TicketForm
    {
        public EmployeeTimeOffTicketFormViewModel()
        {

        }

        public EmployeeTimeOffTicketFormViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket) : base(ticket)
        {
            Ticket = new DailyEmployeeTimeOffTicketViewModel(ticket);
            Entries = new DailyEmployeeTimeOffListViewModel(ticket);
            StatusLogs = new DailyStatusLogListViewModel(ticket);
            Audit = new Views.Equipment.Audit.EquipmentAuditMeterFormViewModel(ticket);
        }

        public bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new System.ArgumentNullException(nameof(modelState));
            }
            var ok = true;
            var maxDate = Ticket.WorkDate.Value.WeekEndDate(DayOfWeek.Saturday);
            var futureDate =Entries.Entries.Any(f => f.WorkDate > maxDate);
            if (futureDate)
            {
                modelState.AddModelError("", "You may not enter Time off for a date outside of current week");
                ok &= false;
            }
            return ok;
        }

        public DailyEmployeeTimeOffTicketViewModel Ticket { get; set; }

        public DailyEmployeeTimeOffListViewModel Entries { get; set; }

        public DailyStatusLogListViewModel StatusLogs { get; set; }

        public Views.Equipment.Audit.EquipmentAuditMeterFormViewModel Audit { get; set; }
    }
}