//using Newtonsoft.Json;
//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.DailyTicket;
//using portal.Models.Views.DailyTicket.Form;
//using portal.Repository.VP.DT;
//using portal.Repository.VP.PR;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Controllers.View.DailyTicket
//{
//    public class DailyJobEmployeeController : BaseController
//    {
//        [HttpGet]
//        public ActionResult Panel(byte dtco, int ticketId, bool forEdit)
//        {
//            using var db = new VPContext();
//            var ticket = db.DailyTickets.Where(f => f.DTCo == dtco && f.TicketId == ticketId).FirstOrDefault();

//            var modelTicket = new TicketForm(ticket, true);
//            var form = (JobTicketFormViewModel)modelTicket.DynaimicTicket;

//            ViewBag.ViewOnly = form.Access == DB.SessionAccess.View ? true : false;
//            var model = form.Employees;
//            return PartialView("../DT/Type/Job/Employee/List/Panel", model);
//        }

//        [HttpGet]
//        public ActionResult Table(byte dtco, int ticketId, bool forEdit)
//        {
//            using var db = new VPContext();
//            var ticket = db.DailyTickets.Where(f => f.DTCo == dtco && f.TicketId == ticketId).FirstOrDefault();

//            var modelTicket = new TicketForm(ticket, true);
//            var form = (JobTicketFormViewModel)modelTicket.DynaimicTicket;

//            ViewBag.ViewOnly = form.Access == DB.SessionAccess.View ? true : false;
//            var model = form.Employees;
//            return PartialView("../DT/Type/Job/Employee/List/Table", model);
//        }

//        [HttpGet]
//        public PartialViewResult Add(byte dtco, int ticketId)
//        {
//            using var db = new VPContext();
//            DailyJobEmployeeViewModel result = null;
//            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
//            if (ticket != null)
//            {
//                var emp = ticket.DailyJobTicket.AddEmployee();
//                db.SaveChanges(ModelState);
//                result = new DailyJobEmployeeViewModel(emp);
//            }
            
//            ViewBag.tableRow = "True";
            
//            return PartialView("../DT/Type/Job/Employee/List/TableRow", result);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Update(DailyJobEmployeeViewModel model)
//        {
//            if (model == null)
//                throw new System.ArgumentNullException(nameof(model));

//            using var db = new VPContext();
//            var updObj = db.DailyJobEmployees.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId && f.LineNum == model.LineNum);
//            if (updObj != null)
//            {
//                updObj.UpdateFromModel(model);
//                db.SaveChanges(ModelState);
//            }
//            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Delete(byte dtco, int ticketId, int lineNum)
//        {
//            //using var repo = new DailyJobEmployeeRepository();
//            //var model = repo.GetDailyJobEmployee(co, ticketId, lineNum);
//            using var db = new VPContext();
//            var updObj = db.DailyJobEmployees.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId && f.LineNum == lineNum);
//            if (updObj != null)
//            {
//                db.DailyJobEmployees.Remove(updObj);
//                db.SaveChanges(ModelState);
//            }
//            return Json(new { success = ModelState.IsValidJson() });
//        }

//        [HttpGet]
//        public JsonResult Validate(DailyJobEmployeeViewModel model)
//        {
//            if (model == null)
//                throw new System.ArgumentNullException(nameof(model));

//            //if (model.EmployeeId == null)
//            //{
//            //    using var db = new VPContext();
//            //    var fieldEmployee = db.DailyJobEmployees.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId && f.LineNum == model.LineNum);
//            //    if (fieldEmployee != null)
//            //    {
//            //        var obj = new DailyJobEmployeeViewModel(fieldEmployee)
//            //        {
//            //            Comments = model.Comments
//            //        };
//            //        model = obj;
//            //    }
//            //}

//            ModelState.Clear();
//            TryValidateModel(model);

//            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
//        }
//    }
//}