using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Payroll;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View.Employee
{
    public class WeeklyEmployeeSummaryController: BaseController
    {
        [HttpGet]
        [Route("Employee/Entries")]
        public ActionResult Index()
        {
            using var db = new VPContext();

            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var model = new PayrollReviewSummaryListViewModel(db, user);
            return View("../DT/Summary/User/Index", model);
        }

        [HttpGet]
        public ActionResult PartialIndex()
        {
            using var db = new VPContext();

            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var model = new PayrollReviewSummaryListViewModel(db, user);
            return PartialView("../DT/Summary/User/PartialIndex", model);
        }

        [HttpGet]
        public ActionResult Table()
        {
            using var db = new VPContext();

            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var model = new PayrollReviewSummaryListViewModel(db, user);
            return PartialView("../DT/Summary/User/List/Table", model);
        }

        [HttpGet]
        public ActionResult Panel()
        {
            using var db = new VPContext();

            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var model = new PayrollReviewSummaryListViewModel(db, user);
            return PartialView("../DT/Summary/User/List/Panel", model);
        }

    }
}