//using Newtonsoft.Json;
//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.DailyTicket.Form;
//using portal.Repository.VP.DT;
//using portal.Repository.VP.EM;
//using portal.Repository.VP.JC;
//using portal.Repository.VP.PR;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Controllers.View.DailyTicket
//{
//    public class DailyShopTicketFormController : BaseController
//    {
//        [HttpGet]
//        [Route("Ticket/Shop/{ticketId}-{dtco}")]
//        public ActionResult Index(byte dtco, int ticketId, bool partialView = false)
//        {
//            using var db = new VPContext();

//            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
//            var modelTicket = new TicketForm(ticket, true);
//            var model = (ShopTicketFormViewModel)modelTicket.DynaimicTicket;

//            ViewBag.ViewOnly = model.Access == DB.SessionAccess.View ? true : false;
//            ViewBag.TicketForm = modelTicket;
//            ViewBag.Partial = partialView;

//            if (model.Access == DB.SessionAccess.Denied)
//                return RedirectToAction("Denied", "Error");

//            if (!partialView)
//                return View("../DT/Type/Shop/Index", model);
//            else
//                return PartialView("../DT/Type/Shop/PartialIndex", model);
//        }
//    }
//}