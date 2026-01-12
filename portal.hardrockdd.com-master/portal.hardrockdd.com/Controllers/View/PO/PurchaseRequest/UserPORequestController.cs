using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Purchase.Request;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.DailyTicket
{
    //[CustomAuthorize(Roles = "PORequest, Admin")]    
    //[AuthorizePosition(PositionCodes = "CFO,COO,PRES,FIN-AP,IT-DIR,FIN-APMGR,FIN-AR,FIN-CTRL,FLD-CL,HR-MGR,IT-DIR,OP-DM,OP-EQADM,OP-EQMGR,OP-GM,OP-SFMGR,SHP-MGR,SHP-SUP")]
    [ControllerAuthorize]
    public class UserPORequestController : BaseController
    {
        [HttpGet]
        [Route("PORequest/User")]
        public ActionResult Index()
        {
            var results = new PORequestSummaryListViewModel();
            results.List.Add(new PORequestSummaryViewModel());

            ViewBag.Controller = "UserPORequest";
            ViewBag.Data = "Data";

            return View("../PO/Request/Summary/Index", results);
        }

        [HttpGet]
        public ActionResult Table()
        {
            var results = new PORequestSummaryListViewModel();
            results.List.Add(new PORequestSummaryViewModel());

            ViewBag.Controller = "UserPORequest";
            ViewBag.Data = "Data";

            return PartialView("../PO/Request/Summary/List/Table", results);
        }

        [HttpGet]
        public ActionResult Data()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var results = new PORequestSummaryListViewModel(user, DB.POListTypeEnum.User);

            JsonResult result = Json(new
            {
                data = results.List.ToArray()
            }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }


        [HttpGet]
        [Route("PORequest/User/Pending")]
        public ActionResult PendingIndex()
        {
            var results = new PORequestSummaryListViewModel();
            results.List.Add(new PORequestSummaryViewModel());

            ViewBag.Controller = "UserPORequest";
            ViewBag.Data = "PendingData";

            return View("../PO/Request/Summary/Index", results);
        }

        [HttpGet]
        public ActionResult PendingTable()
        {
            var results = new PORequestSummaryListViewModel();
            results.List.Add(new PORequestSummaryViewModel());

            ViewBag.Controller = "UserPORequest";
            ViewBag.Data = "PendingData";

            return PartialView("../PO/Request/Summary/List/Table", results);
        }

        [HttpGet]
        public ActionResult PendingData()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            //var user = db.WebUsers
            //    .Include("WorkFlows")
            //    .Include("WorkFlows.Sequence")
            //    .Include("WorkFlows.Sequence.WorkFlow")
            //    .Include("WorkFlows.Sequence.WorkFlow.PORequests")
            //    .FirstOrDefault(f => f.Id == userId);

            //var list1 = user.WorkFlows
            //    .Where(f => f.AssignedTo == userId && f.Sequence.Active)
            //    .Select(s => s.Sequence.WorkFlow)
            //    .Distinct()
            //    .SelectMany(s => s.PORequests)
            //    .ToList();

            //var wf = new List<WorkFlow>();
            var poList = db.WorkFlowUsers
                .Include("Sequence")
                .Include("Sequence.WorkFlow")
                .Where(f => f.AssignedTo == userId && f.Sequence.Active)
                .Select(s => s.Sequence.WorkFlow)
                .Distinct()
                .SelectMany(s => s.PORequests)
                .ToList();
            var results = new PORequestSummaryListViewModel(poList);

            JsonResult result = Json(new
            {
                data = results.List.ToArray()
            }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }
    }
}