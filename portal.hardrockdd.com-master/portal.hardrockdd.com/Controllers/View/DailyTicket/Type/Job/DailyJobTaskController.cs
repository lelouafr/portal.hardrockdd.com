//using Newtonsoft.Json;
//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.DailyTicket;
//using portal.Repository.VP.DT;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Controllers.View.DailyTicket
//{
//    public class DailyJobTaskController : BaseController
//    {

//        [HttpGet]
//        public ActionResult Panel(byte dtco, int ticketId, bool forEdit)
//        {
//            using var db = new VPContext();
//            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
//            ViewBag.ViewOnly = !forEdit;
//            var model = new DailyJobTaskListViewModel(ticket);
//            return PartialView("../DT/Type/Job/Task/List/Panel", model);
//        }

//        [HttpGet]
//        public ActionResult Table(byte dtco, int ticketId, bool forEdit)
//        {
//            using var db = new VPContext();
//            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
//            //ViewBag.PhaseList = Repository.VP.JC.JobPhaseRepository.GetSelectList((new Repository.VP.JC.JobPhaseRepository().GetJobPhases(co, ticket.DailyJobTicket.JobId, co, "Production", true)));
//            ViewBag.ViewOnly = !forEdit;

//            //ViewBag.TaskList = TaskRepository.GetSelectList((new TaskRepository().GetTasks(co)));

//            var model = new DailyJobTaskListViewModel(ticket);
//            return PartialView("../DT/Type/Job/Task/List/Table", model);
//        }

//        [HttpGet]
//        public ActionResult Add(byte dtco, int ticketId, string phaseId)
//        {
//            //if (phaseId == null || phaseId == "" || phaseId == "null")
//            //{
//            //    return Json(new { success ="false" });
//            //}
//            var db = new VPContext();
//            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
//            var lineNum = DailyJobTaskRepository.AddDailyJobTasks(ticket, phaseId);
//            db.SaveChanges(ModelState);
//            db.Dispose();

//            db = new VPContext();
//            ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
//            var DailyJobTasks = ticket.DailyJobTasks.Where(f => f.LineNum == lineNum).ToList();
//            var result = DailyJobTasks.Select(s => new DailyJobTaskViewModel(s)).ToList();

//            ViewBag.tableRow = "True";
//            db.Dispose();
//            return PartialView("../DT/Type/Job/Task/List/Row/TablePhaseRow", result);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Update(DailyJobTaskViewModel model)
//        {
//            if (model == null)
//            {
//                throw new System.ArgumentNullException(nameof(model));
//            }
//            using var repo = new DailyJobTaskRepository();
//            var result = repo.ProcessUpdate(model, ModelState);
//            //var result = repo.GetDailyJobTask(model.Co, model.TicketId, model.LineNum, model.SeqId, "Task");

//            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

//            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Delete(byte dtco, int ticketId, int lineNum)
//        {
//            using var repo = new DailyJobTaskRepository();
//            var model = repo.GetDailyJobTasks(dtco, ticketId, lineNum);
//            if (model != null)
//            {
//                foreach (var obj in model)
//                {
//                    repo.Delete(obj);
//                }
//            }
//            return Json(new { success = "true", model });
//        }

//        [HttpGet]
//        public JsonResult Validate(DailyJobTaskViewModel model)
//        {
//            using var db = new VPContext();
//            using var repo = new DailyJobTaskRepository();
//            var DailyJobTask = db.DailyJobTasks.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId && f.LineNum == model.LineNum && f.SeqId == model.SeqId);
//            if (DailyJobTask != null)
//            {
//                model = new DailyJobTaskViewModel(DailyJobTask);
//                ModelState.Clear();
//                this.TryValidateModel(model);

//            }
//            if (!ModelState.IsValid)
//            {
//                return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);                
//            }
//            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
//        }
//    }
//}