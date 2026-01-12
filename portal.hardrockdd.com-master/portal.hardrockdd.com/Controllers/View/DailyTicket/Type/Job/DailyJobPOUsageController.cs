//using Newtonsoft.Json;
//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.DailyTicket;
//using portal.Models.Views.DailyTicket.Form;
//using portal.Repository.VP.DT;
//using portal.Repository.VP.DT.Ticket;
//using portal.Repository.VP.PR;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Controllers.View.DailyTicket
//{
//    public class DailyJobPOUsageController : BaseController
//    {
//        [HttpGet]
//        public ActionResult Panel(byte dtco, int ticketId)
//        {
//            using var db = new VPContext();
//            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

//            var modelTicket = new TicketForm(ticket, true);
//            var form = (JobTicketFormViewModel)modelTicket.DynaimicTicket;

//            ViewBag.ViewOnly = form.Access == DB.SessionAccess.View ? true : false;
//            var model = form.POUsages;
//            return PartialView("../DT/Type/Job/POUsage/List/Panel", model);
//        }

//        [HttpGet]
//        public ActionResult Table(byte dtco, int ticketId)
//        {
//            using var db = new VPContext();
//            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

//            var modelTicket = new TicketForm(ticket, true);
//            var form = (JobTicketFormViewModel)modelTicket.DynaimicTicket;

//            ViewBag.ViewOnly = form.Access == DB.SessionAccess.View ? true : false;
//            var model = form.POUsages;
//            return PartialView("../DT/Type/Job/POUsage/List/Table", model);
//        }

//        [HttpGet]
//        public PartialViewResult Add(byte dtco, int ticketId)
//        {
//            using var db = new VPContext();

//            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

//            var entity = DailyPOUsageRepository.Init(ticket);
//            ticket.POUsages.Add(entity);

//            var result = new DailyPOUsageViewModel(entity);
//            ViewBag.tableRow = "True";
//            db.SaveChanges(ModelState);
//            return PartialView("../DT/Type/Job/POUsage/List/TableRow", result);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Update(DailyPOUsageViewModel model)
//        {
//            if (model == null)
//            {
//                throw new System.ArgumentNullException(nameof(model));
//            }
//            using var db = new VPContext();

//            var result = DailyPOUsageRepository.ProcessUpdate(model, db);
//            db.SaveChanges(ModelState);

//            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Delete(byte dtco, int ticketId, int lineId)
//        {
//            using var db = new VPContext();
//            var updObj = db.DailyPOUsages.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId && f.LineId == lineId);
//            if (updObj != null)
//            {
//                db.DailyPOUsages.Remove(updObj);
//                db.SaveChanges(ModelState);
//            }
//            var model = new DailyPOUsageViewModel(updObj);
//            return Json(new { success = ModelState.IsValidJson(), model });
//        }

//        [HttpGet]
//        public JsonResult Validate(DailyPOUsageViewModel model)
//        {
//            if (model == null)
//                throw new System.ArgumentNullException(nameof(model));

          
//            ModelState.Clear();
//            TryValidateModel(model);

//            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
//        }
//    }
//}