using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Payroll.Models.Ticket.TimeOff
{

    public class FormViewModel
    {
        public FormViewModel()
        {

        }

        public FormViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket)
        {
            Ticket = new TicketViewModel(ticket);
            Entries = new EmployeeEntryListViewModel(ticket);
			StatusLogs = new StatusLogListViewModel(ticket);

			DTCo = ticket.DTCo;
			TicketId = ticket.TicketId;
			UniqueAttchID = ticket.Attachment.UniqueAttchID;
		}

        public bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new System.ArgumentNullException(nameof(modelState));
            }
            var ok = true;
            var maxDate = Ticket.WorkDate.Value.WeekEndDate(DayOfWeek.Saturday);
            var futureDate =Entries.List.Any(f => f.WorkDate > maxDate);
            if (futureDate)
            {
                modelState.AddModelError("", "You may not enter Time off for a date outside of current week");
                ok &= false;
            }
            return ok;
        }

		[Display(Name = "Co")]
		public byte DTCo { get; set; }

		[Display(Name = "Ticket Id")]
		public int TicketId { get; set; }

		public TicketViewModel Ticket { get; set; }

        public EmployeeEntryListViewModel Entries { get; set; }

		public StatusLogListViewModel StatusLogs { get; set; }

		public System.Guid? UniqueAttchID { get; set; }
	}
}