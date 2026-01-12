using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.DailyTicket;
using portal.Models.Views.Payroll;
using portal.Repository.VP.DT;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View.DailyTicket
{
    [ControllerAuthorize]
    public class PayrollEmployeeTicketController : BaseController
    {


        //[HttpGet]
        //public ActionResult Form(byte Co, int EmployeeId, int WeekId)
        //{
        //    using var db = new VPContext();
        //    var employee = db.Employees.Where(f => f.EmployeeId == EmployeeId && f.PRCo == Co).FirstOrDefault();
        //    var model = new PayrollEmployeeTicketListViewModel(employee, WeekId);
        //    return PartialView(model);
        //}


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