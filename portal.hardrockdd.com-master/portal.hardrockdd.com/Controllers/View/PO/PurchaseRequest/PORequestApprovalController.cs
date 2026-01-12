using portal.Models.Views.Purchase.Request;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View.DailyTicket
{
    //[AuthorizePosition(PositionCodes = "CFO,COO,PRES,FIN-AP,IT-DIR,FIN-APMGR,FIN-AR,FIN-CTRL,FLD-CL,HR-MGR,IT-DIR,OP-DM,OP-EQADM,OP-EQMGR,OP-GM,OP-SFMGR,SHP-MGR,SHP-SUP")]
    [ControllerAuthorize]
    public class PORequestApprovalController : BaseController
    {
        [HttpGet]
        [Route("PORequest/Approval")]
        public ActionResult Index()
        {
            //using var db =  new DB.Infrastructure.ViewPointDB.Data.VPContext();
            //var userId = StaticFunctions.GetUserId();
            //var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            //var results = new PORequestSummaryListViewModel(user, DB.POListTypeEnum.Assigned, DB.PORequestStatusEnum.Submitted);

            var results = new PORequestSummaryListViewModel();
            results.List.Add(new PORequestSummaryViewModel());

            return View("../PO/Request/Approval/Index", results);
        }

        [HttpGet]
        public ActionResult Table()
        {
            //using var db =  new DB.Infrastructure.ViewPointDB.Data.VPContext();
            //var userId = StaticFunctions.GetUserId();
            //var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            //var results = new PORequestSummaryListViewModel(user, DB.POListTypeEnum.Assigned, DB.PORequestStatusEnum.Submitted);

            var results = new PORequestSummaryListViewModel();
            results.List.Add(new PORequestSummaryViewModel());
            ViewBag.Controller = "UserPORequest";

            return PartialView("../PO/Request/Approval/List/Table", results);
        }

        [HttpGet]
        public ActionResult Data()
        {
            using var db =  new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.Include("AssignedPORequests").Include("AssignedPORequests.Request").Include("AssignedPORequests.Request.Lines").FirstOrDefault(f => f.Id == userId);
            var results = new PORequestSummaryListViewModel(user, DB.POListTypeEnum.Assigned, DB.PORequestStatusEnum.Submitted);
            var subResults = new PORequestSummaryListViewModel(user, DB.POListTypeEnum.Assigned, DB.PORequestStatusEnum.SupApproved);

            results.List.AddRange(subResults.List);
            JsonResult result = Json(new
            {
                data = results.List.ToArray()
            }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }
    }
}