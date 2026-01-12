using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.DailyTicket.Form;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View.DailyTicket
{
    public class DailyHolidayTicketFormController : BaseController
    {
        [HttpGet]
        [Route("Ticket/Holiday/{ticketId}-{dtco}")]
        public ActionResult Index(byte dtco, int ticketId, bool partialView = false)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var modelTicket = new TicketForm(ticket, true);
            var model = (HolidayTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.TicketForm = modelTicket;

            ViewBag.ViewOnly = model.Access == DB.SessionAccess.View ? true : false;
            ViewBag.UnSumbitAllowed = modelTicket.CanUnSubmit;
            ViewBag.Partial = partialView;

            //ViewBag.StartDate = ticket.WorkDate.Value.WeekStartDate(System.DayOfWeek.Friday);
            //ViewBag.EndDate = ticket.WorkDate.Value.WeekEndDate(System.DayOfWeek.Friday);

            if (model.Access == DB.SessionAccess.Denied)
            {
                return RedirectToAction("Denied", "Error");
            }
            if (!partialView)
                return View("../DT/Type/Holiday/Index", model);
            else
                return PartialView("../DT/Type/Holiday/PartialIndex", model);
        }

    }
}