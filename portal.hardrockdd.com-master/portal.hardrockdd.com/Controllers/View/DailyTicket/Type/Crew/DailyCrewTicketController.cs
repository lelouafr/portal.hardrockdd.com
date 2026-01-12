//using Newtonsoft.Json;
//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.DailyTicket.Form;
//using portal.Repository.VP.DT;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Controllers.View.DailyTicket
//{
//    public class DailyCrewTicketController : BaseController
//    {

//        [HttpGet]
//        public ActionResult Form(byte dtco, int ticketId)
//        {
//            using var db = new VPContext();
//            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
//            var modelTicket = new TicketForm(ticket, true);
//            var model = (CrewTicketFormViewModel)modelTicket.DynaimicTicket;

//            ViewBag.TicketForm = modelTicket;
//            ViewBag.ViewOnly = model.Access == DB.SessionAccess.View;
//            return PartialView("../DT/Type/Crew/Form/Form", model.Ticket);
//        }

//        [HttpGet]
//        public ActionResult Panel(byte dtco, int ticketId)
//        {
//            using var db = new VPContext();
//            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
//            var modelTicket = new TicketForm(ticket, true);
//            var model = (CrewTicketFormViewModel)modelTicket.DynaimicTicket;

//            ViewBag.TicketForm = modelTicket;
//            ViewBag.ViewOnly = model.Access == DB.SessionAccess.View ? true : false;
//            return PartialView("../DT/Type/Crew/Form/Panel", model.Ticket);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Update(Models.Views.DailyTicket.DailyCrewTicketViewModel model)
//        {
//            using var db = new VPContext();
//            var updObj = db.DailyShopTickets.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId);
//            var result = DailyShopTicketRepository.ProcessUpdate(model, updObj);
//            db.SaveChanges(ModelState);
//            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

//            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
//        }

//        [HttpGet]
//        public JsonResult Validate(Models.Views.DailyTicket.DailyCrewTicketViewModel model)
//        {
//            using var db = new VPContext();
//            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId);

//            using var repo = new DailyTicketRepository();
//            var form = new CrewTicketFormViewModel(ticket);
//            model = new Models.Views.DailyTicket.DailyCrewTicketViewModel(ticket);
//            form.Validate(ModelState);

//            if (!ModelState.IsValid)
//            {
//                var errorModel = ModelState.ModelErrors();
//                    return Json(new { success = ModelState.IsValidJson(), errorModel }, JsonRequestBehavior.AllowGet);
//            }
//            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
//        }
//    }
//}