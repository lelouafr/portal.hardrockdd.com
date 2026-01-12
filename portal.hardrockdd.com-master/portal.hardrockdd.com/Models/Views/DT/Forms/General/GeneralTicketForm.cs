using System.Web.Mvc;

namespace portal.Models.Views.DailyTicket.Form
{

    public class GeneralTicketFormViewModel : TicketForm
    {
        public GeneralTicketFormViewModel()
        {

        }

        public GeneralTicketFormViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket) : base(ticket)
        {
            Ticket = new DailyGeneralTicketViewModel(ticket);
            Employees = new DailyEmployeeEntryListViewModel(ticket);
            StatusLogs = new DailyStatusLogListViewModel(ticket);
        }

        public DailyGeneralTicketViewModel Ticket { get; set; }

        public DailyEmployeeEntryListViewModel Employees { get; set; }

        public DailyStatusLogListViewModel StatusLogs { get; set; }
    }
}