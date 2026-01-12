//using Newtonsoft.Json;
//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.DailyTicket.Form;
//using portal.Repository.VP.DT;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Controllers.View.DailyTicket
//{
//    public class DailyShopTicketController : BaseController
//    {

//        [HttpGet]
//        public ActionResult Form(byte dtco, int ticketId)
//        {
//            using var db = new VPContext();
//            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
//            var modelTicket = new TicketForm(ticket, true);
//            var model = (ShopTicketFormViewModel)modelTicket.DynaimicTicket;

//            ViewBag.ViewOnly = model.Access == DB.SessionAccess.View ? true : false;
//            ViewBag.TicketForm = modelTicket;

//            return PartialView("../DT/Type/Shop/Form/Form", model.Ticket);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Update(Models.Views.DailyTicket.DailyShopTicketViewModel model)
//        {
           
//            using var db = new VPContext();
//            var updObj = db.DailyShopTickets.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId);
//            if (updObj != null)
//            {
//                updObj.UpdateFromModel(model);
//                db.SaveChanges(ModelState);
//                model = new Models.Views.DailyTicket.DailyShopTicketViewModel(updObj);
//            }
//            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

//            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
//        }

//        [HttpGet]
//        public JsonResult Validate(Models.Views.DailyTicket.DailyShopTicketViewModel model)
//        {
//            using var db = new VPContext();
//            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId);

//            var form = new Models.Views.DailyTicket.Form.ShopTicketFormViewModel(ticket);
//            model = new Models.Views.DailyTicket.DailyShopTicketViewModel(ticket);
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