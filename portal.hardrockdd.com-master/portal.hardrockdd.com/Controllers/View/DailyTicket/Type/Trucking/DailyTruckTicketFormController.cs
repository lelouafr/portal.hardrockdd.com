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
//    public class DailyTruckTicketFormController : BaseController
//    {
//        [HttpGet]
//        [Route("Ticket/Truck/{ticketId}-{dtco}")]
//        public ActionResult Index(byte dtco, int ticketId, bool partialView = false)
//        {
//            using var db = new VPContext();

//            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
//            var modelTicket = new TicketForm(ticket, true);
//            var model = (TruckTicketFormViewModel)modelTicket.DynaimicTicket;

//            ViewBag.ViewOnly = model.Access == DB.SessionAccess.View ? true : false;
//            ViewBag.TicketForm = modelTicket;
//            ViewBag.Partial = partialView;

//            if (model.Access == DB.SessionAccess.Denied)
//            {
//                return RedirectToAction("Denied", "Error");
//            }
//            if (!partialView)
//                return View("../DT/Type/Trucking/Index", model);
//            else
//                return PartialView("../DT/Type/Trucking/PartialIndex", model);
//        }

//        //[HttpPost]
//        //[ValidateAntiForgeryToken]
//        //public ActionResult Update(Models.Views.DailyTicket.DailyTruckTicketViewModel model)
//        //{
//        //    using var repo = new DailyTruckTicketRepository();
//        //    var result = repo.ProcessUpdate(model, ModelState);

//        //    //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

//        //    return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
//        //}

//        //[HttpGet]
//        //public JsonResult ValidateFieldTicket(Models.Views.DailyTicket.DailyTruckTicketViewModel model)
//        //{
//        //    if (model == null)
//        //    {
//        //        throw new System.ArgumentNullException(nameof(model));
//        //    }
//        //    ModelState.Clear();
//        //    TryValidateModel(model);
//        //    if (!ModelState.IsValid)
//        //    {
//        //        var errorModel = ModelState.ModelErrors();
//        //        return Json(new { success = ModelState.IsValidJson(), errorModel }, JsonRequestBehavior.AllowGet);
//        //        ;
//        //    }
//        //    return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
//        //}
//    }
//}