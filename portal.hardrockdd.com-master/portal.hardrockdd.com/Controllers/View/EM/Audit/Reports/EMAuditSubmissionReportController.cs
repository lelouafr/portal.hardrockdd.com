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
    public class EMAuditSubmissionReportController : BaseController
    {
        [HttpGet]
        [Route("Report/EMAuditSubmissionReport")]
        public ViewResult Index()
        {
            using var db = new VPContext();
            var dateFilter = db.PayrollEntries.Max(max => max.PREndDate).AddDays(1);
            var weekId = db.Calendars.FirstOrDefault(f => f.Date == dateFilter).Week ?? 0;

            var results = new AuditSubmissionReportListViewModel(weekId);

            return View("../EM/Audit/Report/WeekSubmission/Index", results);
        }

        [HttpGet]
        public PartialViewResult Table(int weekId)
        {
            var results = new AuditSubmissionReportListViewModel(weekId);

            return PartialView("../EM/Audit/Report/WeekSubmission/List/Table", results);
        }
    }
}