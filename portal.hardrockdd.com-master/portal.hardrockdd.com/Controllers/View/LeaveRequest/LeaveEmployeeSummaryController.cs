using portal.Models.Views.Payroll.Leave;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.DailyTicket
{
    public class LeaveEmployeeSummaryController : BaseController
    {
        [HttpGet]
        public ActionResult Panel(int prco, int employeeId)
        {
            using var db =  new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var emp = db.Employees.FirstOrDefault(f => f.PRCo == prco && f.EmployeeId == employeeId);
            var results = new LeaveEmployeeSummaryListViewModel(emp);

            ViewBag.Controller = "LeaveEmployeeSummary";

            return View("../LeaveRequest/Summary/Panel", results);
        }

        [HttpGet]
        public ActionResult Table(int prco, int employeeId)
        {
            using var db =  new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var emp = db.Employees.FirstOrDefault(f => f.PRCo == prco && f.EmployeeId == employeeId);
            var results = new LeaveEmployeeSummaryListViewModel(emp);

            ViewBag.Controller = "LeaveEmployeeSummary";

            return PartialView("../LeaveRequest/Summary/Table", results);
        }

    }
}