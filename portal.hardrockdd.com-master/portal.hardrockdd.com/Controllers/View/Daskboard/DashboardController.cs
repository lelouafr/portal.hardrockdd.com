using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.DailyTicket;
using System;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.Daskboard
{
    public class DashboardController : BaseController
    {
        
        [HttpGet]
        public ActionResult UserTickets()
        {
            using var db = new VPContext();
            var userId = StaticFunctions.GetUserId();
            var dateFlt = DateTime.Now.AddDays(-120);
            var tickets = db.DailyTickets.Where(f => f.CreatedBy == userId &&
                                                     f.tWorkDate >= dateFlt &&
                                                     f.tStatusId != (int)DB.DailyTicketStatusEnum.Deleted &&
                                                     f.tStatusId != (int)DB.DailyTicketStatusEnum.Canceled &&
                                                     f.tStatusId != (int)DB.DailyTicketStatusEnum.Processed &&
                                                     f.tStatusId != (int)DB.DailyTicketStatusEnum.UnPosted
                                                     ).ToList();
            var vtickets = db.vDailyTickets.Where(f => f.CreatedBy == userId &&
                                                     f.WorkDate >= dateFlt).ToList();

            var results = new DailyTicketsUserSummary(tickets, vtickets);
            ViewBag.EndDate = DateTime.Now.WeekEndDate(DayOfWeek.Friday);
            return PartialView("UserTicket/Panel", results);
        }

        [HttpGet]
        public ActionResult UserTicketsTable()
        {
            using var db = new VPContext();
            var userId = StaticFunctions.GetUserId();
            var dateFlt = DateTime.Now.AddDays(-120);
            var tickets = db.DailyTickets.Where(f => f.CreatedBy == userId &&
                                                     f.tWorkDate >= dateFlt &&
                                                     f.tStatusId != (int)DB.DailyTicketStatusEnum.Deleted &&
                                                     f.tStatusId != (int)DB.DailyTicketStatusEnum.Canceled &&
                                                     f.tStatusId != (int)DB.DailyTicketStatusEnum.Processed &&
                                                     f.tStatusId != (int)DB.DailyTicketStatusEnum.UnPosted
                                                     ).ToList();
            var vtickets = db.vDailyTickets.Where(f => f.CreatedBy == userId &&
                                                     f.WorkDate >= dateFlt).ToList();

            var results = new DailyTicketsUserSummary(tickets, vtickets);

            return PartialView("UserTicket/Table", results);
        }

    }
}