using Microsoft.AspNet.Identity;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Controllers.View.Payroll
{
    public class PayrollEmployeeSummaryController : BaseController
    {
               
        // GET: Bid
        [HttpGet]
        public ActionResult Panel(byte prco, int EmployeeId, int WeekId)
        {
            using var db = new VPContext();
            var calendar = db.Calendars.Where(f => f.Week == WeekId).FirstOrDefault();
            var employee = db.Employees.Where(f => f.EmployeeId == EmployeeId && f.PRCo == prco).FirstOrDefault();
            var model = new PayrollEmployeeReviewListViewModel(employee, calendar, db);

            if (!User.IsInRole("Payroll"))
            {
                ViewBag.DisplayOnly = true;
            }
            //ViewBag.DisplayOnly = true;

            return PartialView(model);
        }

        [HttpGet]
        public ActionResult Table(byte prco, int EmployeeId, int WeekId)
        {
            using var db = new VPContext();
            var calendar = db.Calendars.Where(f => f.Week == WeekId).FirstOrDefault();
            var employee = db.Employees.Where(f => f.EmployeeId == EmployeeId && f.PRCo == prco).FirstOrDefault();
            var model = new PayrollEmployeeReviewListViewModel(employee, calendar, db);

            if (!User.IsInRole("Payroll"))
            {
                ViewBag.DisplayOnly = true;
            }
            return PartialView(model);
        }
    }
}