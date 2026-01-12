//using Newtonsoft.Json;
//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.DailyTicket;
//using portal.Models.Views.DailyTicket.Form;
//using portal.Repository.VP.DT;
//using portal.Repository.VP.EM;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Controllers.View.DailyTicket
//{
//    public class DailyEquipmentController : BaseController
//    {

//        [HttpGet]
//        public ActionResult Panel(byte dtco, int ticketId, bool forEdit)
//        {
//            using var db = new VPContext();
//            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            
//            var modelTicket = new TicketForm(ticket, true);
//            var form = (JobTicketFormViewModel)modelTicket.DynaimicTicket;

//            ViewBag.ViewOnly = form.Access == DB.SessionAccess.View ? true : false;
//            var model = form.Equipments;

//            return PartialView("../DT/Type/Job/Equipment/List/Panel", model);
//        }

//        [HttpGet]
//        public ActionResult Table(byte dtco, int ticketId)
//        {
//            using var db = new VPContext();
//            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

//            var modelTicket = new TicketForm(ticket, true);
//            var form = (JobTicketFormViewModel)modelTicket.DynaimicTicket;

//            ViewBag.ViewOnly = form.Access == DB.SessionAccess.View ? true : false;
//            var model = form.Equipments;
//            return PartialView("../DT/Type/Job/Equipment/List/Table", model);
//        }

//        [HttpGet]
//        public PartialViewResult Add(byte dtco, int ticketId)
//        {
//            using var repo = new DailyEquipmentRepository();
//            using var db = new VPContext();
//            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
//            var model = DailyEquipmentRepository.Init(ticket);
//            model = repo.Create(model, ModelState);

//            var dailyEquipment = db.DailyEquipments.Where(f => f.DTCo == dtco && f.TicketId == ticketId && f.LineNum == model.LineNum).FirstOrDefault();
//            var result = new DailyEquipmentViewModel(dailyEquipment);
//            ViewBag.tableRow = "True";
//            return PartialView("../DT/Type/Job/Equipment/List/TableRow", result);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Update(DailyEquipmentViewModel model)
//        {
//            if (model == null)
//            {
//                throw new System.ArgumentNullException(nameof(model));
//            }
//            using var repo = new DailyEquipmentRepository();
//            var result = repo.ProcessUpdate(model, ModelState);
//            //var result = repo.GetDailyEquipment(model.Co, model.TicketId, model.LineNum, "Equipment");

//            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

//            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Delete(byte dtco, int ticketId, int lineNum)
//        {
//            using var db = new VPContext();
//            var delObj = db.DailyEquipments.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId && f.LineNum == lineNum);
//            if (delObj != null)
//            {
//                db.DailyEquipments.Remove(delObj);
//                db.SaveChanges(ModelState);
//            }
//            else
//            {
//                ModelState.AddModelError("", "Could not delete, line not found.");
//            }
//            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
//        }

//        [HttpGet]
//        public JsonResult Validate(DailyEquipmentViewModel model)
//        {
//            this.TryValidateModel(model);
//            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
//        }
//    }
//}