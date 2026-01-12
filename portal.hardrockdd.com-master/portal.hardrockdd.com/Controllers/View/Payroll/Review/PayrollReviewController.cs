using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
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

    //[AuthorizePosition(PositionCodes = "CFO,COO,PRES,FIN-CTRL,HR-MGR,HR-PRMGR,IT-DIR")]
    public class PayrollReviewController : BaseController
    {
        [HttpGet]
        [Route("Payroll/Review")]
        public ActionResult Index()
        {
            using var db = new VPContext();
            var dateFilter = db.PayrollEntries.Max(max => max.PREndDate).AddDays(1);
            var weekId = db.Calendars.FirstOrDefault(f => f.Date == dateFilter).Week ?? 0;

            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId).Employee.FirstOrDefault();
            var result = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == user.PRCo);
            RequestLineRepository.SplitRequest(weekId, result.HQCo, db);
            var model = new PayrollReviewSummaryListViewModel(result, db, weekId, false);
            return View("../PR/Entries/Review/Index", model);
        }

        [HttpGet]
        public ActionResult Table(int WeekId)
        {
            using var db = new VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId).Employee.FirstOrDefault();
            var result = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == user.PRCo);
            var model = new PayrollReviewSummaryListViewModel(result, db, WeekId, false);
            return PartialView("../PR/Entries/Review/List/Table", model);
        }

        [HttpGet]
        public ActionResult Panel(int WeekId)
        {
            using var db = new VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId).Employee.FirstOrDefault();
            var result = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == user.PRCo);
            var model = new PayrollReviewSummaryListViewModel(result, db, WeekId, false);
            return PartialView("../PR/Entries/Review/List/Panel", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePayrollStatus(byte prco, int employeeId, int weekId, int payrollStatus)
        {
            using var db = new VPContext();
            foreach (var entry in db.DTPayrollHours.Where(f => f.PRCo == prco && f.EmployeeId == employeeId && 
                                                                           f.Calendar.Week == weekId &&
                                                                           f.StatusId != (int)DB.PayrollEntryStatusEnum.Posted &&
                                                                           f.StatusId != (int)DB.PayrollEntryStatusEnum.Reversal)
                                                    .ToList())
            {
                entry.Status = (DB.PayrollEntryStatusEnum)payrollStatus;
            }
            foreach (var entry in db.DTPayrollPerdiems.Where(f => f.PRCo == prco && f.EmployeeId == employeeId &&
                                                                           f.Calendar.Week == weekId &&
                                                                           f.StatusId != (int)DB.PayrollEntryStatusEnum.Posted &&
                                                                           f.StatusId != (int)DB.PayrollEntryStatusEnum.Reversal)
                                                    .ToList())
            {
                entry.Status = (DB.PayrollEntryStatusEnum)payrollStatus;
            }
            db.SaveChanges(ModelState);
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddtoBatch(byte prco, int employeeId, int weekId, int payWeekId)
        {
            using var db = new VPContext();
            using var batchRepo = new BatchRepository();
            //using var tempTimeRepo = new TimeTempEntryRepository();

            var calendar = db.Calendars.Where(w => w.Week == payWeekId).ToList();
            var prEndDate = calendar.Max(max => max.Date);

            var batch = batchRepo.FindCreatePR("PRTB", "PR Entry", prco, prEndDate);
            batch = db.Batches.FirstOrDefault(f => f.Co == batch.Co && f.Mth == batch.Mth && f.BatchId == batch.BatchId);
            var newEntreis = PRBatchTimeEntryRepository.GenerateEntries(db, batch, employeeId, weekId);
            db.PRBatchTimeEntries.AddRange(newEntreis);
            db.SaveChanges(ModelState);
            if (ModelState.IsValid)
            {
                PRBatchTimeEntryRepository.GenerateRetroOTEntries(db, batch, employeeId, weekId);
                db.SaveChanges(ModelState);
            }

            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }


        [HttpPost]
        public ActionResult AddtoBatchList(List<ProcessBatchEmployeeViewModel> List, int payWeekId)
        {
            if (List == null) 
                return Json(new { success = "false", errorModel = ModelState.ModelErrors() });

            using var db = new VPContext();
            using var batchRepo = new BatchRepository();
            //using var tempTimeRepo = new TimeTempEntryRepository();
            var calendar = db.Calendars.Where(w => w.Week == payWeekId).ToList();
            var prEndDate = calendar.Max(max => max.Date);
            var prco = List.FirstOrDefault().PRCo;
            var batch = batchRepo.FindCreatePR("PRTB", "PR Entry", prco, prEndDate);
            batch = db.Batches.FirstOrDefault(f => f.Co == batch.Co && f.Mth == batch.Mth && f.BatchId == batch.BatchId);

            var weeks = List.GroupBy(g => g.WeekId)
                .Select(s => new {
                    WeekId = s.Key, 
                    EmployeeIds = s.Select(e => e.EmployeeId).ToList()}
                ).ToList();
            foreach (var item in weeks)
			{
                db.Database.CommandTimeout = 600;
				var entries = PRBatchTimeEntryRepository.GenerateEntries(db, batch, item.EmployeeIds, item.WeekId);
				db.BulkSaveChanges();
				db.PRBatchTimeEntries.AddRange(entries);
                db.BulkSaveChanges();
                //db.SaveChanges(ModelState);


				if (ModelState.IsValid && item.WeekId < payWeekId && entries.Any())
				{
					PRBatchTimeEntryRepository.GenerateRetroOTEntries(db, batch, item.EmployeeIds, item.WeekId);
					db.BulkSaveChanges();
					//foreach (var emplId in item.EmployeeIds)
     //               {
     //                   PRBatchTimeEntryRepository.GenerateRetroOTEntries(db, batch, emplId, item.WeekId);
     //                   db.BulkSaveChanges();
     //                   //db.SaveChanges(ModelState);
     //               }
                }
                if (!ModelState.IsValid)
				{
					return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
				}
				ModelState.Clear();
			}
            //foreach (var item in List)
            //{
            //    var newEntreis = PRBatchTimeEntryRepository.GenerateEntries(db, batch, item.EmployeeId, item.WeekId);
            //    db.PRBatchTimeEntries.AddRange(newEntreis);
            //    db.SaveChanges(ModelState);
            //    if (ModelState.IsValid && item.WeekId < payWeekId)
            //    {
            //        PRBatchTimeEntryRepository.GenerateRetroOTEntries(db, batch, item.EmployeeId, item.WeekId);
            //        db.SaveChanges(ModelState);
            //    }
            //    if (!ModelState.IsValid)
            //    {
            //        return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
            //    }
            //    ModelState.Clear();
            //}

            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

    }
}