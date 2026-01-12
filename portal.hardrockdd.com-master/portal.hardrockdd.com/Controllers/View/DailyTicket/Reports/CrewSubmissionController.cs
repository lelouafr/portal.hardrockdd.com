using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.DailyTicket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Controllers.View.DailyTicket.Reports
{
    public class CrewSubmissionController: BaseController
    {
        [HttpGet]
        [Route("Report/CrewSubmission")]
        public ViewResult Index()
        {
            using var db = new VPContext();
            var dateFilter = db.PayrollEntries.Max(max => max.PREndDate).AddDays(1);
            var weekId = db.Calendars.FirstOrDefault(f => f.Date == dateFilter).Week ?? 0;

            var results = new DailyCrewSubmissionListViewModel(weekId);

            return View("../DT/CrewSubmission/Index", results);
        }

        [HttpGet]
        public PartialViewResult Table(int weekId)
        {
            var results = new DailyCrewSubmissionListViewModel(weekId);

            return PartialView("../DT/CrewSubmission/List/Table", results);
        }
    }
}