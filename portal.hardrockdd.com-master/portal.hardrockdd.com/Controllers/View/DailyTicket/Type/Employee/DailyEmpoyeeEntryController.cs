using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.DailyTicket;
using portal.Models.Views.DailyTicket.Form;
using portal.Repository.VP.DT;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View.DailyTicket
{
    public class DailyEmployeeEntryController : BaseController
    {

        [HttpGet]
        public ActionResult Panel(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

            var modelTicket = new TicketForm(ticket, true);
            var form = (EmployeeTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.ViewOnly = form.Access == DB.SessionAccess.View ? true : false;
            var model = new DailyEmployeeEntryListViewModel(ticket);
            return PartialView("UNKNOWN", model);
        }

        [HttpGet]
        public ActionResult Table(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

            var modelTicket = new TicketForm(ticket, true);
            var form = (EmployeeTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.ViewOnly = form.Access == DB.SessionAccess.View ? true : false;
            var model = new DailyEmployeeEntryListViewModel(ticket);
            return PartialView("UNKNOWN", model);
        }

        [HttpGet]
        public PartialViewResult Add(byte dtco, int ticketId)
        {
            using var db = new VPContext();

            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);            
            var entry = ticket.GeneralTicket.AddHoursEntry();
            db.SaveChanges(ModelState);

            var result = new DailyEmployeeEntryViewModel(entry);

            ViewBag.tableRow = "True";
            ViewBag.ViewOnly = false;
            return PartialView("UNKNOWN", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(DailyEmployeeEntryViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });
            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(byte dtco, int ticketId, int lineNum)
        {
            using var db = new VPContext();
            var delObj = db.DailyEmployeeEntries
                        .Where(f => f.DTCo == dtco && f.TicketId == ticketId && f.LineNum == lineNum)
                        .FirstOrDefault();
            if (delObj != null)
            {
                db.DailyEmployeeEntries.Remove(delObj);
                var delList = db.DailyEmployeePerdiems.Where(f => f.DTCo == dtco && f.TicketId == ticketId && f.tWorkDate == delObj.WorkDate && f.tEmployeeId == delObj.tEmployeeId).ToList();
                db.DailyEmployeePerdiems.RemoveRange(delList);
                db.SaveChanges(ModelState);
            }
            else
            {
                ModelState.AddModelError("", "Could not delete, line not found.");
            }
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Validate(DailyEmployeeEntryViewModel model)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId);
            var entry = ticket.DailyEmployeeEntries.FirstOrDefault(f => f.LineNum == model.LineNum);
            model = new DailyEmployeeEntryViewModel(entry);

            ModelState.Clear();
            TryValidateModel(model);
            model.Validate(ModelState);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
    }
}