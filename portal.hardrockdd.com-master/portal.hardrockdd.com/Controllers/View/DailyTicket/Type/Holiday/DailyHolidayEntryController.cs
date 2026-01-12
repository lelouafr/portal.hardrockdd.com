using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.DailyTicket;
using portal.Models.Views.DailyTicket.Form;
using portal.Repository.VP.DT;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;

namespace portal.Controllers.View.DailyTicket
{
    public class DailyHolidayEntryController : BaseController
    {

        [HttpGet]
        public ActionResult Panel(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

            var modelTicket = new TicketForm(ticket, true);
            var form = (HolidayTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.ViewOnly = form.Access == DB.SessionAccess.View ? true : false;
            var model = new DailyHolidayEntryListViewModel(ticket);
            return PartialView("../DT/Type/Holiday/Entry/List/Panel", model);
        }

        [HttpGet]
        public ActionResult Table(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

            var modelTicket = new TicketForm(ticket, true);
            var form = (HolidayTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.ViewOnly = form.Access == DB.SessionAccess.View ? true : false;
            var model = new DailyHolidayEntryListViewModel(ticket);
            return PartialView("../DT/Type/Holiday/Entry/List/Table", model);
        }

        [HttpGet]
        public PartialViewResult Add(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var newEntry = ticket.GeneralTicket.AddHolidayHoursEntry((DateTime)ticket.WorkDate, null);
           
            var results = new List<DailyHolidayEntryViewModel>
            {
                new DailyHolidayEntryViewModel(newEntry)
            };

            ViewBag.tableRow = "True";
            ViewBag.ViewOnly = false;
            return PartialView("../DT/Type/Holiday/Entry/List/Row/EmployeeRow", results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(DailyHolidayEntryViewModel model)
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
                var delList = db.DailyEmployeePerdiems.Where(f => f.DTCo == dtco && f.TicketId == ticketId && f.WorkDate == delObj.WorkDate && f.tEmployeeId == delObj.tEmployeeId).ToList();
                db.DailyEmployeePerdiems.RemoveRange(delList);
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
        public JsonResult Validate(DailyHolidayEntryViewModel model)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId);
            var entry = ticket.DailyEmployeeEntries.FirstOrDefault(f => f.LineNum == model.LineNum);
            model = new DailyHolidayEntryViewModel(entry);

            ModelState.Clear();
            TryValidateModel(model);
            model.Validate(ModelState);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
    }
}