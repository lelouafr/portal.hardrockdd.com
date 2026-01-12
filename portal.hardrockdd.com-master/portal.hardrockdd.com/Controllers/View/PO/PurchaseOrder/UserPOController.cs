using portal.Models.Views.Purchase.Order;
using portal.Models.Views.Purchase.Request;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.DailyTicket
{
    //[AuthorizePosition(PositionCodes = "CFO,COO,PRES,COO,FIN-AP,FIN-APMGR,FIN-AR,FIN-ARMGR,FIN-CTRL,FLD-CDLDRV,FLD-CL,FLD-LBR,FLD-WL,HR-MGR,HR-PRMGR,IT-DIR,OF-GA,OP-DM,OP-ENGD,OP-EQADM,OP-EQMGR,OP-GM,OP-PM,OP-SF,OP-SFMGR,OP-SLS,OP-SLSMGR,OP-SUP,PRES,SHP-MGR,SHP-SUP,TRUCKING")]

    [ControllerAuthorize]
    public class UserPOController : BaseController
    {
        [HttpGet]
        [Route("PO/User")]
        public ActionResult Index()
        {
            var results = new PurchaseOrderSummaryListViewModel();
            results.List.Add(new PurchaseOrderSummaryViewModel());
            ViewBag.Controller = "UserPO";

            return View("../PO/PurchaseOrder/Summary/Index", results);
        }

        [HttpGet]
        public ActionResult Table()
        {
            var results = new PurchaseOrderSummaryListViewModel();
            results.List.Add(new PurchaseOrderSummaryViewModel());

            ViewBag.Controller = "UserPO";

            return PartialView("../PO/PurchaseOrder/Summary/List/Table", results);
        }

        [HttpGet]
        public ActionResult Data()
        {
            using var db =  new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var results = new PurchaseOrderSummaryListViewModel(user);

            JsonResult result = Json(new
            {
                data = results.List.ToArray()
            }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }
    }
}