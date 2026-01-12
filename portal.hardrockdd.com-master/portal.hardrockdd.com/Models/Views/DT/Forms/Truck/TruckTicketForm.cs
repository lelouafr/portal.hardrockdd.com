using DB.Infrastructure.ViewPointDB.Data;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.DailyTicket.Form
{

    public class TruckTicketFormViewModel : TicketForm
    {
        public TruckTicketFormViewModel()
        {

        }

        public TruckTicketFormViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket) : base(ticket)
        {
            Ticket = new DailyTruckTicketViewModel(ticket);
            Employees = new DailyTruckEmployeeListViewModel(ticket);
            StatusLogs = new DailyStatusLogListViewModel(ticket);
            UniqueAttchID = ticket.Attachment.UniqueAttchID;
            Audit = new Views.Equipment.Audit.EquipmentAuditMeterFormViewModel(ticket);
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
        public DailyTruckTicketViewModel Ticket { get; set; }

        public DailyTruckEmployeeListViewModel Employees { get; set; }

        public DailyStatusLogListViewModel StatusLogs { get; set; }

        public System.Guid? UniqueAttchID { get; set; }

        public Views.Equipment.Audit.EquipmentAuditMeterFormViewModel Audit { get; set; }
    }
}