//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.DailyTicket;
//using System.Linq;
//using System.Web.Mvc;


//namespace portal.Controllers.View.DailyTicket
//{
//    public class DailyStatusLogController : BaseController
//    {
//        [HttpGet]
//        public ActionResult Table(byte dtco, int ticketId)
//        {
//            using var db = new VPContext();
//            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

//            var results = new DailyStatusLogListViewModel(ticket);
//            return PartialView("../DT/StatusLog/List/Table", results);
//        }
//    }
//}