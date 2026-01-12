using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Areas.Payroll.Models.Ticket.Holiday
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