//using portal.Models.Views.DailyTicket;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Controllers.View
//{
//    public class DailyTicketRejectController : BaseController
//    {
//        // GET: DailyTicketReject
//        [HttpGet]
//        public ActionResult Index(byte dtco, int ticketId)
//        {
//            using var db =  new DB.Infrastructure.ViewPointDB.Data.VPContext();
//            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
//            var model = new DailyTicketRejectViewModel(ticket);

//            return PartialView("../DT/Reject/Model", model);
//        }


//        [HttpGet]
//        public ActionResult Validate(DailyTicketRejectViewModel model)
//        {
//            ModelState.Clear();
//            TryValidateModel(model);

//            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
//        }

//    }
//}