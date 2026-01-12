//using Newtonsoft.Json;
//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.DailyTicket.Form;
//using portal.Repository.VP.DT;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Controllers.View.DailyTicket
//{
//    public class DailyJobTicketController : BaseController
//    {

//        [HttpGet]
//        public ActionResult Form(byte dtco, int ticketId)
//        {
//            using var db = new VPContext();
//            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
//            var modelTicket = new TicketForm(ticket, true);
//            var model = (JobTicketFormViewModel)modelTicket.DynaimicTicket;
//            ViewBag.ViewOnly = model.Access == DB.SessionAccess.View ? true : false;
//            ViewBag.TicketForm = modelTicket;
//            return PartialView("../DT/Type/Job/Form/Form", model.Ticket);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Update(Models.Views.DailyTicket.DailyJobTicketViewModel model)
//        {
//            using var db = new VPContext();
//            var updObj = DailyJobTicketRepository.ProcessUpdate(model, db);
//            db.SaveChanges(ModelState);
//            var result = new Models.Views.DailyTicket.DailyJobTicketViewModel(updObj);

//            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
//        }

//        [HttpGet]
//        public JsonResult Validate(Models.Views.DailyTicket.DailyJobTicketViewModel model)
//        {
//            using var db = new VPContext();
//            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId);

//            using var repo = new DailyTicketRepository();
//            var form = new Models.Views.DailyTicket.Form.JobTicketFormViewModel(ticket);
//            model = new Models.Views.DailyTicket.DailyJobTicketViewModel(ticket);
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