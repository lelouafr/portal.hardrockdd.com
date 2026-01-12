using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.DailyTicket;
using portal.Models.Views.EM.Audit.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Controllers.View.Equipment.Audit.Reports
{
    public class EMAuditWeeklySubmissionReportController : BaseController
    {
        [HttpGet]
        [Route("Report/EMAuditWeeklySubmissionReport")]
        public ViewResult Index()
        {
            using var db = new VPContext();
            var dateFilter = DateTime.Now.Date;
            var weekId = db.Calendars.FirstOrDefault(f => f.Date == dateFilter).Week ?? 0;

            var results = new AuditWeeklyReportListViewModel(weekId);
            
            return View("../EM/Audit/Report/WeeklySubmission/Index", results);
        }

        [HttpGet]
        public PartialViewResult Table()
        {
            using var db = new VPContext();
            var dateFilter = DateTime.Now.Date;
            var weekId = db.Calendars.FirstOrDefault(f => f.Date == dateFilter).Week ?? 0;
            var results = new AuditWeeklyReportListViewModel(weekId);

            return PartialView("../EM/Audit/Report/WeeklySubmission/List/Table", results);
        }
    }
}