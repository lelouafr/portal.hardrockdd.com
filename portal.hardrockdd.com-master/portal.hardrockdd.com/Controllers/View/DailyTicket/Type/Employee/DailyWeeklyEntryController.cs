using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.DailyTicket;
using portal.Models.Views.DailyTicket.Form;
using portal.Repository.VP.DT;
using System.Linq;
using System.Web.Mvc;
using System;

namespace portal.Controllers.View.DailyTicket
{
    public class DailyWeeklyEntryController : BaseController
    {

        [HttpGet]
        public ActionResult Panel(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            if (ticket.DailyEmployeeTicket != null)
                ticket.DailyEmployeeTicket.CreateDefaultEntries();
            db.SaveChanges(ModelState);

            var modelTicket = new TicketForm(ticket, true);
            var form = (EmployeeTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.ViewOnly = form.Access == DB.SessionAccess.View ? true : false;
            var model = new DailyWeeklyEntryListViewModel(ticket);
            return PartialView("../DT/Type/Employee/Entry/List/Panel", model);
        }

        [HttpGet]
        public ActionResult Table(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            if (ticket.DailyEmployeeTicket != null)
                ticket.DailyEmployeeTicket.CreateDefaultEntries();
            db.SaveChanges(ModelState);

            var modelTicket = new TicketForm(ticket, true);
            var form = (EmployeeTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.ViewOnly = form.Access == DB.SessionAccess.View ? true : false;
            var model = new DailyWeeklyEntryListViewModel(ticket);
            return PartialView("../DT/Type/Employee/Entry/List/Table", model);
        }

        [HttpGet]
        public PartialViewResult Add(byte dtco, int ticketId)
        {
            using var db = new VPContext();

            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var entry = ticket.DailyEmployeeTicket.AddHoursEntry((DateTime)ticket.WorkDate);
            db.SaveChanges(ModelState);

            var result = new DailyWeeklyEmployeeEntryViewModel(entry);

            var cal = db.Calendars.Where(f => f.Week == ticket.DailyEmployeeTicket.WeekId).ToList();
            ViewBag.StartDate = cal.Min(m => m.Date);
            ViewBag.EndDate = cal.Max(m => m.Date);
            ViewBag.tableRow = "True";
            ViewBag.ViewOnly = false;

            return PartialView("../DT/Type/Employee/Entry/List/TableRow", result);
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
            var delObj = db.DailyEmployeeEntries.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId && f.LineNum == lineNum);

            if (delObj != null)
            {
                var workDate = delObj.WorkDate;

                if (!db.DailyEmployeeEntries.Any(f => f.DTCo == dtco && f.TicketId == ticketId && f.LineNum != delObj.LineNum && f.tEmployeeId == delObj.EmployeeId && f.tWorkDate == delObj.WorkDate))
                {
                    var perdiems = db.DailyEmployeePerdiems.Where(f => f.DTCo == dtco && f.TicketId == ticketId && f.tEmployeeId == delObj.EmployeeId && f.tWorkDate == delObj.WorkDate).ToList();
                    foreach (var perdiem in perdiems)
                    {
                        db.DailyEmployeePerdiems.Remove(perdiem);
                    }
                }
                db.DailyEmployeeEntries.Remove(delObj);
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