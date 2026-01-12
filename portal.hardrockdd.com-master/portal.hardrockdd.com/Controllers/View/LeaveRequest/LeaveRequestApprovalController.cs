using portal.Models.Views.Payroll.Leave;
using portal.Models.Views.Purchase.Request;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View.DailyTicket
{
    public class LeaveRequestApprovalController : BaseController
    {
        [HttpGet]
        [Route("LeaveRequest/Approval")]
        public ActionResult Index()
        {
            using var db =  new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var results = new LeaveRequestSummaryListViewModel(user, DB.LeaveListTypeEnum.Assigned, DB.LeaveRequestStatusEnum.Submitted);
            ViewBag.Controller = "LeaveRequestApproval";

            return View("../LeaveRequest/Approval/Index", results);
        }

        [HttpGet]
        public ActionResult Table()
        {
            using var db =  new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var results = new LeaveRequestSummaryListViewModel(user, DB.LeaveListTypeEnum.Assigned, DB.LeaveRequestStatusEnum.Submitted);

            ViewBag.Controller = "LeaveRequestApproval";

            return PartialView("../LeaveRequest/Approval/Table", results);
        }
    }
}