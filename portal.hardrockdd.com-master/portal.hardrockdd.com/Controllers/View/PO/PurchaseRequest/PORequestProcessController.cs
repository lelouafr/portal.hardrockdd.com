using DB.Infrastructure.ViewPointDB.Data;
using Newtonsoft.Json;
using portal.Models.Views.Purchase.Request;
using portal.Repository.VP.HQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View.DailyTicket
{
    //[CustomAuthorize(Roles = "POProcess, Admin")]
    //[AuthorizePosition(PositionCodes = "CFO,COO,PRES,FIN-AP,FIN-APMGR,FIN-CTRL,IT-DIR")]
    [ControllerAuthorize]
    public class PORequestProcessController : BaseController
    {
        [HttpGet]
        [Route("PORequest/Process")]
        public ActionResult Index()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);
            var results = new PORequestSummaryListViewModel(company, DB.PORequestStatusEnum.Approved);
            //results.List.AddRange(submittedResults.List);
            ViewBag.Controller = "PORequesProcess";

            return View("../PO/Request/Process/Index", results);
        }

        [HttpGet]
        public ActionResult Table()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);
            var results = new PORequestSummaryListViewModel(company, DB.PORequestStatusEnum.Approved);
            ViewBag.Controller = "PORequesProcess";

            return PartialView("../PO/Request/Process/List/Table", results);
        }

        [HttpGet]
        public ActionResult Data()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);
            var results = new PORequestSummaryListViewModel(company, DB.PORequestStatusEnum.Approved);

            JsonResult result = Json(new
            {
                data = results.List.ToArray()
            }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }

        [HttpPost]
        public ActionResult AddtoBatchList(List<PORequestViewModel> List)
        {
            if (List == null)
            {
                //throw new ArgumentNullException(nameof(List));
                ModelState.AddModelError("", "Empty List");
                var error_jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                return Json(new { success = ModelState.IsValidJson(), ModelState = error_jsonModelState });
            }
            using var db = new VPContext();
            using var batchRepo = new BatchRepository();

            var mth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).Date;

            ModelState.Clear();
            foreach (var item in List)
            {
                var request = db.PORequests.FirstOrDefault(f => f.POCo == item.POCo && f.RequestId == item.RequestId);

                var model = new PORequestSummaryViewModel(request);
                if (model.ContainsErrors)
                {
                    ModelState.AddModelError("", "Please Review Request there is an error");
                }

            }
            if (ModelState.IsValid)
            {
                Batch batch = null;
                foreach (var request in List)
                {
                    var requestDB = db.PORequests.FirstOrDefault(f => f.POCo == request.POCo && 
                                                                      f.RequestId == request.RequestId && 
                                                                      (f.tStatusId == (int)DB.PORequestStatusEnum.Submitted || 
                                                                       f.tStatusId == (int)DB.PORequestStatusEnum.Approved));
                    if (requestDB != null)
                    {
                        batch = requestDB.AddToBatch(batch);
                        db.BulkSaveChanges();
                        if (batch != null)
                        {
                            batch.InUseBy = null;
                            db.BulkSaveChanges();
                        }
                    }
                }
            }
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }
    }
}