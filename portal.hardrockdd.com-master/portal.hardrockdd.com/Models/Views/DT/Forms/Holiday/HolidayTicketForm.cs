using System.Web.Mvc;

namespace portal.Models.Views.DailyTicket.Form
{

    public class HolidayTicketFormViewModel : TicketForm
    {
        public HolidayTicketFormViewModel()
        {

        }

        public HolidayTicketFormViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket) : base(ticket)
        {
            Ticket = new DailyHolidayTicketViewModel(ticket);
            Entries = new DailyHolidayEntryListViewModel(ticket);
            StatusLogs = new DailyStatusLogListViewModel(ticket);
            UniqueAttchID = ticket.Attachment.UniqueAttchID;
        }
               
        public DailyHolidayTicketViewModel Ticket { get; set; }

        public DailyHolidayEntryListViewModel Entries { get; set; }

        public DailyStatusLogListViewModel StatusLogs { get; set; }

        public System.Guid? UniqueAttchID { get; set; }
    }
}