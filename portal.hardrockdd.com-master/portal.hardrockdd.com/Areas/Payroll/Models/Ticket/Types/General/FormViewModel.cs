using DB.Infrastructure.ViewPointDB.Data;
using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Areas.Payroll.Models.Ticket.General
{

    public class FormViewModel
    {
        public FormViewModel()
        {

        }

        public FormViewModel(DailyTicket ticket)
        {
            if (ticket == null)
                return;

			DTCo = ticket.DTCo;
			TicketId = ticket.TicketId;

			Ticket = new TicketViewModel(ticket);
            Employees = new EmployeeEntryListViewModel(ticket);
			StatusLogs = new StatusLogListViewModel(ticket);

			UniqueAttchID = ticket.Attachment.UniqueAttchID;
		}


		[Display(Name = "Co")]
		public byte DTCo { get; set; }

		[Display(Name = "Ticket Id")]
		public int TicketId { get; set; }

		public TicketViewModel Ticket { get; set; }

        public EmployeeEntryListViewModel Employees { get; set; }

		public StatusLogListViewModel StatusLogs { get; set; }

		public System.Guid? UniqueAttchID { get; set; }

	}
}