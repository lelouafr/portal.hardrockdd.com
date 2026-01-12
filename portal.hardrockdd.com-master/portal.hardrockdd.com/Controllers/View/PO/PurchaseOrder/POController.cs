using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Purchase.Order;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View.PurchasOrders
{
    [ControllerAuthorize]
    public class PurchaseOrderController : BaseController
    {

        [HttpGet]
        [Route("PO/{po}-{poco}")]
        public ActionResult Index(byte poco, string po)
        {
            using var db = new VPContext();
            var userId = StaticFunctions.GetUserId();
            var result = db.PurchaseOrders.FirstOrDefault(f => f.POCo == poco && f.PO == po);
            
           
            var model = new PurchaseOrderFormViewModel(result);
            //ViewBag.ViewOnly = result.WorkFlows.Any(w => w.AssignedTo == userId && w.Status == w.Status && w.Active == "Y") ? "False" : "True";
            ViewBag.ViewOnly = true;
            ViewBag.Partial = false;
            return View("../PO/PurchaseOrder/Form/Index", model);
        }

        [HttpGet]
        public ActionResult PartialIndex(byte poco, string po)
        {
            using var db = new VPContext();
            var userId = StaticFunctions.GetUserId();
            var result = db.PurchaseOrders.FirstOrDefault(f => f.POCo == poco && f.PO == po);


            var model = new PurchaseOrderFormViewModel(result);
            //ViewBag.ViewOnly = result.WorkFlows.Any(w => w.AssignedTo == userId && w.Status == result.Status && w.Active == "Y") ? "False" : "True";
            ViewBag.ViewOnly = true;
            ViewBag.Partial = true;
            return PartialView("../PO/PurchaseOrder/Form/PartialIndex", model);
        }
        
        [HttpGet]
        public ActionResult Form(byte poco, string po)
        {
            using var db = new VPContext();
            var result = db.PurchaseOrders.FirstOrDefault(f => f.POCo == poco && f.PO == po);
            var model = new PurchaseOrderViewModel(result);
            //var userId = StaticFunctions.GetUserId();
            //ViewBag.ViewOnly = result.WorkFlows.Any(w => w.AssignedTo == userId && w.Status == w.Status && w.Active == "Y") ? "False" : "True";
            ViewBag.ViewOnly = true;
            return PartialView("../PO/PurchaseOrder/Form/Header/Form", model);
        }
    }
}