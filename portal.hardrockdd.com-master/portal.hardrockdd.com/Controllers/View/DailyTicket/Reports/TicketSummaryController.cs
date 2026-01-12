using portal.Models.Views.DailyTicket;
using System;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.DailyTicket
{
    public class TicketSummaryController : BaseController
    {
        [HttpGet]
        [Route("Tickets/All")]
        public ViewResult Index()
        {
            var results = new DailyTicketListSummaryViewModel(DB.TimeSelectionEnum.LastMonth);
            results.Tickets.Add(new DailyTicketSummaryViewModel());

            ViewBag.Title = "All Tickets";
            ViewBag.TicketDataController = "TicketSummary";
            ViewBag.TicketDataAction = "Data";
            return View("../DT/Summary/Index", results);
        }

        [HttpGet]
        public PartialViewResult Table(DB.TimeSelectionEnum timeSelection)
        {
            //using var db = new VPContext();
            //var companies = db.HQCompanyParms.ToList();
            //var results = new DailyTicketsUserSummary(companies);

            var results = new DailyTicketListSummaryViewModel(timeSelection);
            results.Tickets.Add(new DailyTicketSummaryViewModel());
            //ViewBag.TableRow = "true";
            //ViewBag.ViewOnly = "true";
            ViewBag.TicketDataController = "TicketSummary";
            ViewBag.TicketDataAction = "Data";
            return PartialView("../DT/Summary/List/Table", results);
        }

        //[HttpGet]
        //public PartialViewResult TableData(TableViewModel model)
        //{
        //    if (model == null)
        //    {
        //        throw new System.ArgumentNullException(nameof(model));
        //    }
        //    ViewBag.TableRow = "true";
        //    ViewBag.ViewOnly = "true";
        //    ViewBag.Controller = "TicketSummary";
        //    using var repo = new DailyTicketSummaryRepository();
        //    var result = repo.TicketSummary("", model.PageSize, model.CurrentPage, model.SearchString, out _, out _);
        //    result.TableInfo.TableId = "DailyTicketSummary";
        //    result.TableInfo.SetupPages();
        //    return PartialView("~/Views/DailyTicket/Ticket/Table.cshtml", result);
        //}

        [HttpGet]
        public ActionResult Data(DB.TimeSelectionEnum timeSelection)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var companies = db.HQCompanyParms.ToList();
            var results = new DailyTicketListSummaryViewModel(companies, timeSelection);

            //var json = JsonConvert.SerializeObject(results.Tickets, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
            JsonResult result = Json(new
            {
                data = results.Tickets.ToArray()
            }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;// Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult JobData(DB.TimeSelectionEnum timeSelection, byte jcco, string jobId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var job = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
            var results = new DailyTicketListSummaryViewModel(job, db, timeSelection);

            //var json = JsonConvert.SerializeObject(results.Tickets, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
            JsonResult result = Json(new
            {
                data = results.Tickets.ToArray()
            }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;// Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }
    }
}