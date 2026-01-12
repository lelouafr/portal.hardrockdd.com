using DB.Infrastructure.ViewPointDB.Data;
using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Payroll.Models.Ticket.Crew
{

    public class FormViewModel
    {
        public FormViewModel()
        {

        }

        public FormViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket)
        {
            Ticket = new TicketViewModel(ticket);
            Employees = new EmployeeEntryListViewModel(ticket);
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
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == Ticket.DTCo && f.TicketId == Ticket.TicketId);
            var audit = ticket.CreatedUser.ActiveEquipmentAudits().FirstOrDefault();
            if (audit != null && audit.IsAuditLate(ticket))
            {
                modelState.AddModelError("", "You have a Late Equipment Audit, must complete Audit before submitting Ticket!");
            }

            return ok;
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