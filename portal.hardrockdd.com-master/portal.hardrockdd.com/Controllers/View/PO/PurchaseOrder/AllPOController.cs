using portal.Models.Views.Purchase.Order;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.PurchasOrders
{
    //[AuthorizePosition(PositionCodes = "CFO,COO,PRES,IT-DIR,FIN-AP,FIN-APMGR,FIN-AR,FIN-ARMGR,FIN-CTRL,HR-MGR,OF-GA,OP-DM,OP-ENGD,OP-EQADM,OP-EQMGR,OP-GM,OP-PM,OP-SF,OP-SFMGR,OP-SLS,OP-SLSMGR,OP-SUP,SHP-MGR,SHP-SUP")]

    [ControllerAuthorize]
    public class AllPOController : BaseController
    {
        [HttpGet]
        [Route("PO/All")]
        public ActionResult Index()
        {
            var results = new PurchaseOrderSummaryListViewModel();
            results.List.Add(new PurchaseOrderSummaryViewModel());

            ViewBag.Controller = "AllPO";

            return View("../PO/PurchaseOrder/Summary/Index", results);
        }

        [HttpGet]
        public ActionResult Table()
        {
            var results = new PurchaseOrderSummaryListViewModel();
            results.List.Add(new PurchaseOrderSummaryViewModel());

            ViewBag.Controller = "AllPO";

            return PartialView("../PO/PurchaseOrder/Summary/List/Table", results);
        }


        [HttpGet]
        public ActionResult Data()
        {
            using var db =  new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var emp = user.Employee.FirstOrDefault();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emp.HRCo);
            var results = new PurchaseOrderSummaryListViewModel(company);

            JsonResult result = Json(new
            {
                data = results.List.ToArray()
            }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }
    }
}