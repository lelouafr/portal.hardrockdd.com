using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Payroll.Review;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View.DailyTicket
{
    [ControllerAuthorize]
    public class PayrollEmployeeEntryFormController : BaseController
    {


        [HttpGet]
        public ActionResult Panel(byte prco, int EmployeeId, int WeekId)
        {
            using var db = new VPContext();
            var employee = db.Employees.Where(f => f.EmployeeId == EmployeeId && f.PRCo == prco).FirstOrDefault();
            var model = new PayrollEmployeeReviewFormViewModel(employee, WeekId, db);
            return PartialView("../PR/Entries/Review/Forms/Panel", model);
        }


        [HttpGet]
        public ActionResult Form(byte prco, int EmployeeId, int WeekId)
        {
            using var db = new VPContext();
            var employee = db.Employees.Where(f => f.EmployeeId == EmployeeId && f.PRCo == prco).FirstOrDefault();
            var model = new PayrollEmployeeReviewFormViewModel(employee, WeekId, db);
            return PartialView("../PR/Entries/Review/Forms/Form", model);
        }


        //[HttpGet]
        //public ActionResult Table(byte Co, int EmployeeId, int WeekId)
        //{
        //    using var db = new VPContext();
        //    var calendar = db.Calendars.Where(f => f.Week == WeekId).FirstOrDefault();
        //    var employee = db.Employees.Where(f => f.EmployeeId == EmployeeId && f.PRCo == Co).FirstOrDefault();
        //    var model = new PayrollEmployeeTicketListViewModel(employee, WeekId);
        //    return PartialView(model);
        //}
    }
}