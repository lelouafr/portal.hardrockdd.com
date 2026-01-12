using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.DailyTicket;
using portal.Models.Views.DailyTicket.Form;
using portal.Repository.VP.DT;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View.DailyTicket
{
    public class DailyWeeklyPerdiemController : BaseController
    {

        [HttpGet]
        public ActionResult Panel(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

            var modelTicket = new TicketForm(ticket, true);
            var form = (EmployeeTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.ViewOnly = form.Access == DB.SessionAccess.View ? true : false;
            var model = new DailyWeeklyEntryListViewModel(ticket);
            return PartialView("../DT/Type/Employee/PerDiem/List/Panel", model);
        }

        [HttpGet]
        public ActionResult Table(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

            var modelTicket = new TicketForm(ticket, true);
            var form = (EmployeeTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.ViewOnly = form.Access == DB.SessionAccess.View ? true : false;
            var model = new DailyWeeklyEntryListViewModel(ticket);
            return PartialView("../DT/Type/Employee/PerDiem/List/Table", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(DailyEmployeePerdiemViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });
            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }


        [HttpGet]
        public JsonResult Validate(DailyEmployeePerdiemViewModel model)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId);
            var entry = ticket.DailyEmployeePerdiems.FirstOrDefault(f => f.LineNum == model.LineNum);
            if (entry != null)
            {
                model = new DailyEmployeePerdiemViewModel(entry);

                ModelState.Clear();
                this.TryValidateModel(model);
            }

            if (!ModelState.IsValid)
            {
                var errorModel = ModelState.ModelErrors();
                    return Json(new { success = ModelState.IsValidJson(), errorModel }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        }
    }
}