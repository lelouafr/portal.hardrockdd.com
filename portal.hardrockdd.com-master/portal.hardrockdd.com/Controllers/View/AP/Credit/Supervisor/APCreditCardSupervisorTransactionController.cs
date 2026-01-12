using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.CreditCard.Supervisor;
using System;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers
{
    [ControllerAuthorize]
    public class APCreditCardSupervisorTransactionController : BaseController
    {
        [HttpGet]
        [Route("AP/Credit/Supervisor/Transactions")]///{co}-{employeeId}-{mth}
        public ActionResult Index()
        {
            using var db = new VPContext();           
            var currentEmp = StaticFunctions.GetCurrentEmployee();
            var emp = db.Employees
                .Include("CreditCardTransactions")
                .Include("DirectReports")
                .Include("DirectReports.CreditCardTransactions")
                .FirstOrDefault(f => f.PRCo == currentEmp.PRCo && f.EmployeeId == currentEmp.EmployeeId);
            var cld = db.Calendars.FirstOrDefault(f => f.Date.Year == DateTime.Now.Year && f.Date.Month == DateTime.Now.Month && f.Date.Day == 1);
            var mth = cld.Date;

            var model = new FormViewModel(emp, mth);

            return View("../AP/CreditCard/Supervisor/Index", model);
        }


        [HttpGet]
        public ActionResult TransTable(byte prco, int supervisorId, DateTime mth)
        {
            using var db = new VPContext();
            var emp = db.Employees.FirstOrDefault(f => f.PRCo == prco && f.EmployeeId == supervisorId);
           
            var model = new TransactionListViewModel(emp, mth);

            return PartialView("../AP/CreditCard/Supervisor/List/Table", model);
        }

    }
}