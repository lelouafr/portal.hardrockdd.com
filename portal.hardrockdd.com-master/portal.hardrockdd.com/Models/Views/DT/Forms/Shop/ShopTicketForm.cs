using DB.Infrastructure.ViewPointDB.Data;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.DailyTicket.Form
{

    public class ShopTicketFormViewModel : TicketForm
    {
        public ShopTicketFormViewModel()
        {

        }

        public ShopTicketFormViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket) : base(ticket)
        {
            Ticket = new DailyShopTicketViewModel(ticket);
            Employees = new DailyShopEmployeeListViewModel(ticket);
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
        public DailyShopTicketViewModel Ticket { get; set; }

        public DailyShopEmployeeListViewModel Employees { get; set; }

        public DailyStatusLogListViewModel StatusLogs { get; set; }

        public System.Guid? UniqueAttchID { get; set; }

        public Views.Equipment.Audit.EquipmentAuditMeterFormViewModel Audit { get; set; }
    }
}