using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.DailyTicket;
using portal.Models.Views.DailyTicket.Form;
using portal.Repository.VP.DT;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View.DailyTicket
{
    public class DailyEmployeeTimeOffEntryController : BaseController
    {

        [HttpGet]
        public ActionResult Panel(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

            var modelTicket = new TicketForm(ticket, true);
            var form = (EmployeeTimeOffTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.ViewOnly = form.Access == DB.SessionAccess.View ? true : false;

            ViewBag.ViewOnly = true;
            var model = form.Entries;
            return PartialView("../DT/Type/TimeOff/Entry/List/Panel", model);
        }

        [HttpGet]
        public ActionResult Table(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

            var modelTicket = new TicketForm(ticket, true);
            var form = (EmployeeTimeOffTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.StartDate = ticket.WorkDate.Value.WeekStartDate(System.DayOfWeek.Saturday);
            ViewBag.EndDate = ticket.WorkDate.Value.WeekEndDate(System.DayOfWeek.Saturday);
            ViewBag.ViewOnly = form.Access == DB.SessionAccess.View ? true : false;
            ViewBag.ViewOnly = true;
            var model = form.Entries;
            return PartialView("../DT/Type/TimeOff/Entry/List/Table", model);
        }

        [HttpGet]
        public PartialViewResult Add(byte dtco, int ticketId)
        {
            ViewBag.tableRow = "True";
            ViewBag.ViewOnly = false;
            using var db = new VPContext();

            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

            var cld = ticket.GeneralTicket.DailyTicket.Calendar;
            var newEntry = new DailyEmployeeEntry
            {
                DTCo = ticket.DTCo,
                TicketId = ticket.TicketId,
                WorkDate = cld?.Date,
                PRCo = ticket.GeneralTicket.Employee.PRCo,
                tEmployeeId = ticket.GeneralTicket.Employee.EmployeeId,
                EarnCodeId = ticket.GeneralTicket.Employee.EarnCodeId,
                tEntryTypeId = (int)DB.EntryTypeEnum.Admin,
                DailyTicket = ticket,
            };
            newEntry.LineNum = ticket.DailyEmployeeEntries.
                                DefaultIfEmpty()
                              .Max(f => f == null ? 0 : f.LineNum) + 1;
            ticket.DailyEmployeeEntries.Add(newEntry);
            db.SaveChanges();

            var entry = db.DailyEmployeeEntries.FirstOrDefault(f => f.DTCo == newEntry.DTCo && f.TicketId == newEntry.TicketId && f.LineNum == newEntry.LineNum);
            var result = new DailyEmployeeTimeOffViewModel(entry);

            return PartialView("../DT/Type/TimeOff/Entry/List/TableRow", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(DailyEmployeeTimeOffViewModel model)
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

            var timeOffEntry = new DailyEmployeeTimeOffViewModel(entry);
            timeOffEntry.Validate(ModelState);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
    }
}