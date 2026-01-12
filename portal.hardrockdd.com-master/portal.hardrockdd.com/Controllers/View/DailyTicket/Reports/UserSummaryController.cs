using portal.Models.Views.DailyTicket;
using System;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.DailyTicket
{
    [ControllerAuthorize]
    public class UserSummaryController : BaseController
    {
        [HttpGet]
        [Route("Tickets/Users")]
        public ViewResult Index()
        {
            var results = new DailyTicketListSummaryViewModel(DB.TimeSelectionEnum.All);
            results.Tickets.Add(new DailyTicketSummaryViewModel());

            ViewBag.Title = "Your Tickets";
            ViewBag.TicketDataController = "UserSummary";
            ViewBag.TicketDataAction = "Data";
            return View("../DT/Summary/Index", results);
        }

        [HttpGet]
        public PartialViewResult Table()
        {
            var results = new DailyTicketListSummaryViewModel(DB.TimeSelectionEnum.All);
            results.Tickets.Add(new DailyTicketSummaryViewModel());

            ViewBag.TicketDataController = "UserSummary";
            ViewBag.TicketDataAction = "Data";
            return PartialView("../DT/Summary/List/Table", results);
        }

        [HttpGet]
        public ActionResult Data(DB.TimeSelectionEnum timeSelection)
        {
            //var asofDate = DateTime.Now.AddYears(-99);
            using var db =  new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var results = new DailyTicketListSummaryViewModel(user, timeSelection);

            ViewBag.TicketDataController = "UserSummary";
            ViewBag.TicketDataAction = "Data";

            //var json = JsonConvert.SerializeObject(results.Tickets, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });

            return Json(new { data = results.Tickets }, JsonRequestBehavior.AllowGet);
        }
    }
}