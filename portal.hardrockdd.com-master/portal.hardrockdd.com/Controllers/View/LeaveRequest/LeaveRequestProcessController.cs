using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Payroll.Leave;
using portal.Repository.VP.HQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View.DailyTicket
{
    //[AuthorizePosition(PositionCodes = "HR-MGR,HR-PRMGR,FIN-CTRL,CFO,COO,PRES,IT-DIR")]
    public class LeaveRequestProcessController : BaseController
    {
        [HttpGet]
        [Route("LeaveRequest/Process")]
        public ActionResult Index()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var approvedResults = new LeaveRequestSummaryListViewModel(user, DB.LeaveListTypeEnum.All, DB.LeaveRequestStatusEnum.Approved);
            var submittedResults = new LeaveRequestSummaryListViewModel(user, DB.LeaveListTypeEnum.All, DB.LeaveRequestStatusEnum.Submitted);

            var results = approvedResults;
            results.List.AddRange(submittedResults.List);
            ViewBag.Controller = "LeaveRequesProcess";

            return View("../LeaveRequestProcess/Index", results);
        }

        [HttpGet]
        public ActionResult Table()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var approvedResults = new LeaveRequestSummaryListViewModel(user, DB.LeaveListTypeEnum.All, DB.LeaveRequestStatusEnum.Approved);
            var submittedResults = new LeaveRequestSummaryListViewModel(user, DB.LeaveListTypeEnum.All, DB.LeaveRequestStatusEnum.Submitted);

            var results = approvedResults;
            results.List.AddRange(submittedResults.List);
            ViewBag.Controller = "LeaveRequesProcess";

            return PartialView("../LeaveRequestProcess/Table", results);
        }


        [HttpPost]
        public ActionResult AddtoBatchList(List<LeaveRequestViewModel> List)
        {
            if (List == null)
            {
                //throw new ArgumentNullException(nameof(List));
                ModelState.AddModelError("", "Empty List");
                var error_jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                return Json(new { success = ModelState.IsValidJson(), ModelState = error_jsonModelState });
            }
            //using var db = new VPContext();
            //using var batchRepo = new BatchRepository();

            //var mth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).Date;

            //var batch = batchRepo.FindCreate("LeaveHB", "Leave Entry", mth);
            //batch = db.Batches.FirstOrDefault(f => f.Co == batch.Co && f.Mth == batch.Mth && f.BatchId == batch.BatchId);

            //var batchSeqId = BatchHeaderRepository.NextId(batch);
            //foreach (var request in List)
            //{
            //    var requestDB = db.LeaveRequests.FirstOrDefault(f => f.Co == request.Co && 
            //                                                      f.RequestId == request.RequestId && 
            //                                                      (f.Status == (int)DB.LeaveRequestStatusEnum.Submitted || 
            //                                                       f.Status == (int)DB.LeaveRequestStatusEnum.Approved));
            //    if (requestDB != null)
            //    {

            //        if (requestDB.ApprovedBy == null)
            //        {
            //            requestDB.ApprovedBy = StaticFunctions.GetUserId();
            //            requestDB.ApprovedOn = DateTime.Now;
            //        }

            //        var poBatch = BatchHeaderRepository.Init(batch, requestDB);
            //        poBatch.BatchSeq = batchSeqId;
            //        batchSeqId++;
            //        foreach (var item in requestDB.Lines)
            //        {
            //            if (item.GLAcct != null)
            //            {
            //                var poItem = BatchItemRepository.Init(poBatch, item);
            //                poBatch.Items.Add(poItem);
            //            }
            //        }
            //        requestDB.Status = (int)DB.LeaveRequestStatusEnum.Processed;
            //        if (requestDB.SubmittedBy == null)
            //        {
            //            requestDB.SubmittedBy = StaticFunctions.GetUserId();
            //            requestDB.SubmittedOn = DateTime.Now;
            //        }
            //        requestDB.ProcessedBy = StaticFunctions.GetUserId();
            //        requestDB.ProcessedOn = DateTime.Now;
            //        requestDB.BatchId = poBatch.BatchId;
            //        requestDB.BatchSeq = poBatch.BatchSeq;
            //        requestDB.StatusLogs.Add(RequestStatusLogRepository.Init(requestDB));
            //        batch.LeaveBatchs.Add(poBatch);
            //        RequestWorkFlowRepository.GenerateWorkFlow(requestDB);
            //        db.SaveChanges(ModelState);
                    
            //    }
            //    ModelState.Clear();
            //}

            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }
    }
}