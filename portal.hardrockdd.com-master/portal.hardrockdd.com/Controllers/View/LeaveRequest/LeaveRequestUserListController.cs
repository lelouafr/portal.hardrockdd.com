using portal.Models.Views.Payroll.Leave;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.DailyTicket
{
    public class LeaveRequestUserListController : BaseController
    {
        [HttpGet]
        [Route("LeaveRequest/User")]
        public ActionResult Index()
        {
            using var db =  new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var results = new LeaveRequestSummaryListViewModel(user, DB.LeaveListTypeEnum.User);

            ViewBag.Controller = "LeaveRequestUserList";

            return View("../LeaveRequest/Index", results);
        }

        [HttpGet]
        public ActionResult Table()
        {
            using var db =  new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var results = new LeaveRequestSummaryListViewModel(user, DB.LeaveListTypeEnum.User);

            ViewBag.Controller = "LeaveRequestUserList";

            return PartialView("../LeaveRequest/Table", results);
        }

    }
}