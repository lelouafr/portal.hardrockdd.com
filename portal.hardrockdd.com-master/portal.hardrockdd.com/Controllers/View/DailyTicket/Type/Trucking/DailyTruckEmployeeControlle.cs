//using Newtonsoft.Json;
//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.DailyTicket;
//using portal.Models.Views.DailyTicket.Form;
//using portal.Repository.VP.DT;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Controllers.View.DailyTicket
//{
//    public class DailyTruckEmployeeController : BaseController
//    {

//        [HttpGet]
//        public ActionResult Panel(byte dtco, int ticketId)
//        {
//            using var db = new VPContext();
//            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

//            var modelTicket = new TicketForm(ticket, true);
//            var form = (TruckTicketFormViewModel)modelTicket.DynaimicTicket;

//            ViewBag.ViewOnly = form.Access == DB.SessionAccess.View ? true : false;
//            var model = form.Employees;

//            return PartialView("../DT/Type/Trucking/Entry/List/Table", model);
//        }

//        [HttpGet]
//        public ActionResult Table(byte dtco, int ticketId)
//        {
//            using var db = new VPContext();
//            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

//            var modelTicket = new TicketForm(ticket, true);
//            var form = (TruckTicketFormViewModel)modelTicket.DynaimicTicket;
            
//            ViewBag.ViewOnly = form.Access == DB.SessionAccess.View ? true : false;
//            var model = form.Employees;
//            return PartialView("../DT/Type/Trucking/Entry/List/Table", model);
//        }

//        [HttpGet]
//        public PartialViewResult Add(byte dtco, int ticketId)
//        {
//            ViewBag.tableRow = "True";
//            using var db = new VPContext();
//            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

//            var emp = DailyEmployeeEntryRepository.Init(ticket);
//            var perdiem = DailyEmployeePerdiemRepository.Init(ticket, emp);
//            ticket.DailyEmployeeEntries.Add(emp);
//            ticket.DailyEmployeePerdiems.Add(perdiem);
//            emp.PerdiemLineNum = perdiem.LineNum;
//            db.SaveChanges(ModelState);

//            var entry = db.DailyEmployeeEntries.Where(f => f.DTCo == dtco && f.TicketId == ticketId && f.LineNum == emp.LineNum).FirstOrDefault();
//            if (entry == null)
//            {
//                entry = db.DailyEmployeeEntries.Local.Where(f => f.DTCo == dtco && f.TicketId == ticketId && f.LineNum == emp.LineNum).FirstOrDefault();
//            }
//            var result = new DailyTruckEmployeeViewModel(entry, ticket);
//            var results = new List<DailyTruckEmployeeViewModel>
//            {
//                result
//            };

//            return PartialView("../DT/Type/Trucking/Entry/List/Row/EmployeeRow", results);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Update(DailyTruckEmployeeViewModel model)
//        {
//            //if (model == null)
//            //{
//            //    throw new System.ArgumentNullException(nameof(model));
//            //}
//            //using (var repo = new DailyEmployeeEntryRepository())
//            //{
//            //    _ = repo.ProcessUpdate(model, ModelState);
//            //}
//            //if (model.EmployeeId != null && model.Perdiem != null)
//            //{
//            //    using var repo = new DailyEmployeePerdiemRepository();
//            //    _ = repo.ProcessUpdate(model, ModelState);
//            //}
//            if (model == null)
//            {
//                throw new System.ArgumentNullException(nameof(model));
//            }
//            if (model.EmployeeId != null && model.Perdiem != null)
//            {
//                using var repo = new DailyEmployeePerdiemRepository();
//                _ = repo.ProcessUpdate(model, ModelState);
//            }
//            using var entryRepo = new DailyEmployeeEntryRepository();
//            var entryModel = entryRepo.ProcessUpdate(model, ModelState);

//            using var db = new VPContext();
//            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId);
//            var entry = db.DailyEmployeeEntries.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId && f.LineNum == model.LineNum);
//            var result = new DailyTruckEmployeeViewModel(entry, ticket);
//            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

//            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Delete(byte dtco, int ticketId, int lineNum)
//        {
//            using var db = new VPContext();
//            var delObj = db.DailyEmployeeEntries
//                        .Where(f => f.DTCo == dtco && f.TicketId == ticketId && f.LineNum == lineNum)
//                        .FirstOrDefault();
//            if (delObj != null)
//            {
//                var delList = db.DailyEmployeePerdiems.Where(f => f.DTCo == dtco && f.TicketId == ticketId && f.tWorkDate == delObj.WorkDate && f.tEmployeeId == delObj.tEmployeeId).ToList();
//                db.DailyEmployeePerdiems.RemoveRange(delList);
//                db.DailyEmployeeEntries.Remove(delObj);
//                db.SaveChanges(ModelState);
//            }
//            else
//            {
//                ModelState.AddModelError("", "Could not delete, line not found.");
//            }
//            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
//        }

//        [HttpGet]
//        public JsonResult Validate(DailyTruckEmployeeViewModel model)
//        {
//            using var db = new VPContext();
//            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId);
//            var entry = ticket.DailyEmployeeEntries.FirstOrDefault(f => f.LineNum == model.LineNum);
//            model = new DailyTruckEmployeeViewModel(entry, ticket);

//            ModelState.Clear();
//            this.TryValidateModel(model);
//            model.Validate(ModelState);

//            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
//        }
//    }
//}