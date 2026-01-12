using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Purchase.Order;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View.PurchasOrders
{
    [ControllerAuthorize]
    public class POItemController : BaseController
    {

        [HttpGet]
        public ActionResult FormPanel(byte poco, string po, int poItem)
        {
            using var db = new VPContext();
            var result = db.PurchaseOrderItems.FirstOrDefault(f => f.POCo == poco && f.PO == po && f.POItemId == poItem);
            var model = new PurchaseOrderItemViewModel(result);
            var userId = StaticFunctions.GetUserId();
            //ViewBag.ViewOnly = result.Request.WorkFlows.Any(w => w.AssignedTo == userId && w.Status == w.Status && w.Active == "Y") ? "False" : "True";
            ViewBag.ViewOnly = true;
            return PartialView("../PO/PurchaseOrder/Form/Items/Form/Panel", model);
        }
        [HttpGet]
        public ActionResult Form(byte poco, string po, int poItem)
        {
            using var db = new VPContext();
            var result = db.PurchaseOrderItems.FirstOrDefault(f => f.POCo == poco && f.PO == po && f.POItemId == poItem);
            var model = new PurchaseOrderItemViewModel(result);
            var userId = StaticFunctions.GetUserId();
            //ViewBag.ViewOnly = result.Request.WorkFlows.Any(w => w.AssignedTo == userId && w.Status == w.Status && w.Active == "Y") ? "False" : "True";
            ViewBag.ViewOnly = true;
            return PartialView("../PO/PurchaseOrder/Form/Items/Form/Form", model);
        }


        [HttpGet]
        public ActionResult Panel(byte poco, string po)
        {
            using var db = new VPContext();
            var result = db.PurchaseOrders.FirstOrDefault(f => f.POCo == poco && f.PO == po);
            var model = new PurchaseOrderItemListViewModel(result);
            var userId = StaticFunctions.GetUserId();
            //ViewBag.ViewOnly = result.WorkFlows.Any(w => w.AssignedTo == userId && w.Status == w.Status && w.Active == "Y") ? "False" : "True";
            ViewBag.ViewOnly = true;
            return PartialView("../PO/PurchaseOrder/Form/Items/Panel", model);
        }

        [HttpGet]
        public ActionResult Table(byte poco, string po)
        {
            using var db = new VPContext();
            var result = db.PurchaseOrders.FirstOrDefault(f => f.POCo == poco && f.PO == po);
            var model = new PurchaseOrderItemListViewModel(result);
            var userId = StaticFunctions.GetUserId();
            //ViewBag.ViewOnly = result.WorkFlows.Any(w => w.AssignedTo == userId && w.Status == w.Status && w.Active == "Y") ? "False" : "True";
            ViewBag.ViewOnly = true;
            return PartialView("../PO/PurchaseOrder/Form/Items/Table", model);
        }            
    }
}