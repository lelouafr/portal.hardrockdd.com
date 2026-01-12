using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.DailyTicket.Form;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View.DailyTicket
{
    public class DailyEmployeeTicketFormController : BaseController
    {
        [HttpGet]
        [Route("Ticket/Employee/{ticketId}-{dtco}")]
        public ActionResult Index(byte dtco, int ticketId, bool partialView = false)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            if (ticket.DailyEmployeeTicket != null)
                ticket.DailyEmployeeTicket.CreateDefaultEntries();
            db.BulkSaveChanges();
            var modelTicket = new TicketForm(ticket, true);
            var model = (EmployeeTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.TicketForm = modelTicket;

            ViewBag.ViewOnly = model.Access == DB.SessionAccess.View;
            ViewBag.UnSumbitAllowed = modelTicket.CanUnSubmit;
            ViewBag.Partial = partialView;

            //ViewBag.StartDate = ticket.WorkDate.Value.WeekStartDate(System.DayOfWeek.Friday);
            //ViewBag.EndDate = ticket.WorkDate.Value.WeekEndDate(System.DayOfWeek.Friday);

            if (model.Access == DB.SessionAccess.Denied)
            {
                return RedirectToAction("Denied", "Error");
            }
            if (!partialView)
                return View("../DT/Type/Employee/Index", model);
            else
                return PartialView("../DT/Type/Employee/PartialIndex", model);
        }

    }
}