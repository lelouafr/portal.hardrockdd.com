using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Code;
using portal.Models.Views.DailyTicket;
using portal.Models.Views.DailyTicket.Form;
using portal.Repository.VP.DT;
using portal.Repository.VP.WP;
using System;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;

namespace portal.Controllers.View.DailyTicket
{
    public class DailyTicketController : BaseController
    {
        [HttpGet]
        [Route("Ticket/{ticketId}-{dtco}")]
        public ActionResult Index(byte dtco, int ticketId)
        {
            return RedirectToAction("Index", "DailyTicketForm", new { Area = "",dtco, ticketId });
        }

        [HttpGet]
        public ActionResult PartialIndex(byte dtco, int ticketId)
        {
            return RedirectToAction("PartialIndex", "DailyTicketForm", new { Area = "",dtco, ticketId, partialView = true });
        }

        [HttpGet]
        public ActionResult PopupForm(byte dtco, int ticketId)
        {
            return RedirectToAction("PopupForm", "DailyTicketForm", new { Area = "",dtco, ticketId, partialView = true });
        }

        [HttpGet]
        public ActionResult DailyTicketCrewList(byte prco, string crewId, int weekId)
        {
            using var db = new VPContext();

            var dates = db.Calendars.Where(f => f.Week == weekId).Select(s => s.Date).ToList();
            var tickets = db.vDailyTickets.Where(f => f.WorkDate >= dates.Min() && f.WorkDate <= dates.Max() && f.Status <= 4 && f.CrewId == crewId).ToList();

            return PartialView("../DT/Summary/TabList/Panel", tickets);
        }
    }
}