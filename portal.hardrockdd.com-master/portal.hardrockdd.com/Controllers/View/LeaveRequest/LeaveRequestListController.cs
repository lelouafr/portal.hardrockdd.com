using portal.Models.Views.Payroll.Leave;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.DailyTicket
{
    //[AuthorizePosition(PositionCodes = "HR-MGR,HR-PRMGR,FIN-CTRL,CFO,COO,PRES,IT-DIR,OP-GM,OP-DM")]
    public class LeaveRequestListController : BaseController
    {
        [HttpGet]
        [Route("LeaveRequest/All")]
        public ActionResult Index()
        {
            using var db =  new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var emp = user.Employee.FirstOrDefault();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emp.HRCo);
            var results = new LeaveRequestSummaryListViewModel(company);

            ViewBag.Controller = "LeaveRequestList";

            return View("../LeaveRequest/Index", results);
        }

        [HttpGet]
        public ActionResult Table()
        {
            using var db =  new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var emp = user.Employee.FirstOrDefault();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emp.HRCo);
            var results = new LeaveRequestSummaryListViewModel(company);

            ViewBag.Controller = "LeaveRequestList";

            return PartialView("../LeaveRequest/Table", results);
        }

    }
}