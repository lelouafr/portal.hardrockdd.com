using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.DailyTicket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.DailyTicket
{
    public class TicketPendingSummaryController : BaseController
    {
        [HttpGet]
        [Route("Tickets/Pending")]
        public ViewResult Index()
        {
            var results = new DailyTicketListSummaryViewModel(DB.TimeSelectionEnum.All);
            results.Tickets.Add(new DailyTicketSummaryViewModel());

            ViewBag.Title = "Pending Tickets";
            //ViewBag.TableRow = "true";
            //ViewBag.ViewOnly = "true";
            ViewBag.TicketDataController = "TicketPendingSummary";
            ViewBag.TicketDataAction = "Data";
            return View("../DT/Summary/Index", results);
        }

        [HttpGet]
        public PartialViewResult Table()
        {
            var results = new DailyTicketListSummaryViewModel(DB.TimeSelectionEnum.All);
            results.Tickets.Add(new DailyTicketSummaryViewModel());
            //ViewBag.TableRow = "true";
            //ViewBag.ViewOnly = "true";
            ViewBag.TicketDataController = "TicketPendingSummary";
            ViewBag.TicketDataAction = "Data";
            return PartialView("../DT/Summary/List/Table", results);
        }

        [HttpGet]
        public ActionResult Data(DB.TimeSelectionEnum timeSelection)
        {
            using var db = new VPContext();
            var companies = db.HQCompanyParms.ToList();

            var division = StaticFunctions.GetCurrentDivision();

			var statusList = new List<DB.DailyTicketStatusEnum>
            {
                DB.DailyTicketStatusEnum.Draft,
                DB.DailyTicketStatusEnum.Submitted,
                DB.DailyTicketStatusEnum.Rejected,
                DB.DailyTicketStatusEnum.UnPosted
            };

            var results = new DailyTicketListSummaryViewModel(companies, statusList, timeSelection);
            
            JsonResult result = Json(new
            {
                data = results.Tickets.ToArray()
            }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }
    }
}