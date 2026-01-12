//using Newtonsoft.Json;
//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.DailyTicket.Form;
//using portal.Repository.VP.DT;
//using portal.Repository.VP.JC;
//using portal.Repository.VP.PR;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Controllers.View.DailyTicket
//{
//    public class DailyTruckTicketController : BaseController
//    {

//        [HttpGet]
//        public ActionResult Form(byte dtco, int ticketId)
//        {
//            using var db = new VPContext();
//            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
//            var modelTicket = new TicketForm(ticket, true);
//            var model = (TruckTicketFormViewModel)modelTicket.DynaimicTicket;

//            ViewBag.ViewOnly = model.Access == DB.SessionAccess.View ? true : false;
//            ViewBag.TicketForm = modelTicket;
//            return PartialView("../DT/Type/Trucking/Form/Form", model.Ticket);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Update(Models.Views.DailyTicket.DailyTruckTicketViewModel model)
//        {
//            using var repo = new DailyTruckTicketRepository();
//            var result = repo.ProcessUpdate(model, ModelState);

//            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

//            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
//        }

//        [HttpGet]
//        public JsonResult Validate(Models.Views.DailyTicket.DailyTruckTicketViewModel model)
//        {
//            using var db = new VPContext();
//            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId);

//            using var repo = new DailyTicketRepository();
//            var form = new Models.Views.DailyTicket.Form.TruckTicketFormViewModel(ticket);
//            model = new Models.Views.DailyTicket.DailyTruckTicketViewModel(ticket);
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