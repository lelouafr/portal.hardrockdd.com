using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Dashboard;
using portal.Models.Views.Payroll;
using portal.Repository.VP.HQ;
using portal.Repository.VP.PR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Controllers.View.Payroll
{
    public class DirectReportSummaryController : BaseController
    {
        [HttpGet]
        [Route("Employee/Review")]
        public ActionResult Index()
        {
            return View("../DT/Report/Supervisor/Index");
        }

        // GET: Bid
        [HttpGet]
        public ActionResult Dashboard()
        {
            return PartialView("../DT/Report/Supervisor/List/Panel");
        }

        // GET: Bid
        [HttpGet]
        public ActionResult Panel(int WeekId)
        {
            return PartialView("../DT/Report/Supervisor/List/Panel");
        }

        [HttpGet]
        public ActionResult Table()
        {
            using var db = new VPContext();
            var dateFilter = DateTime.Now.AddDays(-7).Date;
            var weekId = db.Calendars.FirstOrDefault(f => f.Date == dateFilter).Week ?? 0;

            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId).Employee.FirstOrDefault();
            var result = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == user.PRCo);
            var model = new DirectReportSummaryListViewModel(result, db, weekId);
            return PartialView("../DT/Reports/Supervisor/List/Table", model);
        }

    }
}