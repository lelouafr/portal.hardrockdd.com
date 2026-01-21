using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.EM.WorkOrder;
using portal.Models.Views.EM.WorkOrder.Forms;
using portal.Models.Views.EM.WorkOrder.Item;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.EM
{
    public class EMWorkOrderController : BaseController
    {
        #region Work Order List
        [HttpGet]
        [Route("Equipment/WorkOrders")]
        public ActionResult WorkOrderIndex()
        {
            using var db = new VPContext();
            var workOrders = db.EMWorkOrders
                .Include(w => w.Equipment)
                .Where(w => w.Complete == "N")
                .ToList();

            var results = new WorkOrderListViewModel(workOrders);
            return View("../EM/WorkOrder/Summary/Index", results);
        }

        [HttpGet]
        public PartialViewResult WorkOrderTable()
        {
            using var db = new VPContext();
            var workOrders = db.EMWorkOrders
                .Include(w => w.Equipment)
                .Where(w => w.Complete == "N")
                .ToList();

            var results = new WorkOrderListViewModel(workOrders);
            return PartialView("../EM/WorkOrder/Summary/List/Table", results);
        }

        [HttpGet]
        public ActionResult WorkOrderData()
        {
            using var db = new VPContext();
            var workOrders = db.EMWorkOrders
                .Include(w => w.Equipment)
                .Where(w => w.Complete == "N")
                .ToList();

            var results = new WorkOrderListViewModel(workOrders);
            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }

        [HttpGet]
        [Route("Equipment/WorkOrders/All")]
        public ActionResult WorkOrderAllIndex()
        {
            using var db = new VPContext();
            var workOrders = db.EMWorkOrders
                .Include(w => w.Equipment)
                .ToList();

            var results = new WorkOrderListViewModel(workOrders);
            ViewBag.Title = "All Work Orders";
            return View("../EM/WorkOrder/Summary/Index", results);
        }

        [HttpGet]
        [Route("Equipment/WorkOrders/Completed")]
        public ActionResult WorkOrderCompletedIndex()
        {
            using var db = new VPContext();
            var workOrders = db.EMWorkOrders
                .Include(w => w.Equipment)
                .Where(w => w.Complete == "Y")
                .ToList();

            var results = new WorkOrderListViewModel(workOrders);
            ViewBag.Title = "Completed Work Orders";
            return View("../EM/WorkOrder/Summary/Index", results);
        }
        #endregion

        #region Work Order Form
        [HttpGet]
        [Route("Equipment/WorkOrder/{workOrderId}-{emco}")]
        public ActionResult Index(byte emco, string workOrderId)
        {
            using var db = new VPContext();
            var workOrder = db.EMWorkOrders
                .Include(w => w.Equipment)
                .Include(w => w.Items)
                .Include(w => w.Parts)
                .Include(w => w.EMCompany)
                .FirstOrDefault(f => f.EMCo == emco && f.WorkOrderId == workOrderId);

            if (workOrder == null)
            {
                return HttpNotFound();
            }

            var model = new FormViewModel(workOrder, this);
            return View("../EM/WorkOrder/Forms/Index", model);
        }

        [HttpGet]
        public PartialViewResult WorkOrderPanel(byte emco, string workOrderId)
        {
            using var db = new VPContext();
            var workOrder = db.EMWorkOrders
                .Include(w => w.Equipment)
                .Include(w => w.Items)
                .Include(w => w.Parts)
                .FirstOrDefault(f => f.EMCo == emco && f.WorkOrderId == workOrderId);

            var model = new FormViewModel(workOrder, this);
            return PartialView("../EM/WorkOrder/Forms/Panel", model);
        }

        [HttpGet]
        public PartialViewResult WorkOrderForm(byte emco, string workOrderId)
        {
            using var db = new VPContext();
            var workOrder = db.EMWorkOrders
                .Include(w => w.Equipment)
                .FirstOrDefault(f => f.EMCo == emco && f.WorkOrderId == workOrderId);

            var model = new InfoViewModel(workOrder);
            return PartialView("../EM/WorkOrder/Forms/Info/Form", model);
        }
        #endregion

        #region Create Work Order
        [HttpGet]
        public ActionResult Create()
        {
            using var db = new VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            
            var model = new WorkOrderCreateViewModel
            {
                EMCo = comp.HQCo,
                DateCreated = DateTime.Now
            };
            
            return PartialView("../EM/WorkOrder/Add/Index", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WorkOrderCreateViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            ModelState.Clear();
            TryValidateModel(model);

            if (ModelState.IsValid)
            {
                using var db = new VPContext();
                
                // Generate next Work Order ID
                var maxId = db.EMWorkOrders
                    .Where(w => w.EMCo == model.EMCo)
                    .Select(w => w.WorkOrderId)
                    .DefaultIfEmpty("0")
                    .Max();
                
                int nextId = 1;
                if (int.TryParse(maxId, out int currentMax))
                {
                    nextId = currentMax + 1;
                }

                var workOrder = new EMWorkOrder
                {
                    EMCo = model.EMCo,
                    WorkOrderId = nextId.ToString(),
                    EquipmentId = model.EquipmentId,
                    Description = model.Description,
                    ShopGroupId = 1, // Default shop group
                    DateCreated = DateTime.Now,
                    DateDue = model.DateDue,
                    Notes = model.Notes,
                    Complete = "N"
                };

                db.EMWorkOrders.Add(workOrder);
                db.SaveChanges(ModelState);

                if (ModelState.IsValid)
                {
                    var url = Url.Action("Index", "EMWorkOrder", new { Area = "", emco = workOrder.EMCo, workOrderId = workOrder.WorkOrderId });
                    return Json(new { success = true, url });
                }
            }

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }
        #endregion

        #region Update Work Order Info
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateInfo(InfoViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            using var db = new VPContext();
            var workOrder = db.EMWorkOrders.FirstOrDefault(f => f.EMCo == model.EMCo && f.WorkOrderId == model.WorkOrderId);
            
            if (workOrder != null)
            {
                workOrder.EquipmentId = model.EquipmentId;
                workOrder.Description = model.Description;
                workOrder.ShopId = model.ShopId;
                workOrder.MechanicID = model.MechanicID;
                workOrder.DateDue = model.DateDue;
                workOrder.DateSched = model.DateSched;
                workOrder.Notes = model.Notes;
                
                db.SaveChanges(ModelState);
            }

            var result = new InfoViewModel(workOrder);
            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult ValidateInfo(InfoViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Complete / Cancel Work Order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Complete(byte emco, string workOrderId)
        {
            using var db = new VPContext();
            var workOrder = db.EMWorkOrders.FirstOrDefault(f => f.EMCo == emco && f.WorkOrderId == workOrderId);
            
            if (workOrder != null)
            {
                workOrder.Complete = "Y";
                db.SaveChanges(ModelState);
            }

            var model = new WorkOrderViewModel(workOrder);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reopen(byte emco, string workOrderId)
        {
            using var db = new VPContext();
            var workOrder = db.EMWorkOrders.FirstOrDefault(f => f.EMCo == emco && f.WorkOrderId == workOrderId);
            
            if (workOrder != null)
            {
                workOrder.Complete = "N";
                db.SaveChanges(ModelState);
            }

            var model = new WorkOrderViewModel(workOrder);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(byte emco, string workOrderId)
        {
            using var db = new VPContext();

            // Clear references from Service Request Lines first
            var requestLines = db.SMRequestLines.Where(l => l.EMCo == emco && l.WorkOrderId == workOrderId).ToList();
            foreach (var line in requestLines)
            {
                line.EMCo = null;
                line.WorkOrderId = null;
                line.WOItemId = null;
            }

            // Remove parts first
            var parts = db.EMWorkOrderParts.Where(p => p.EMCo == emco && p.WorkOrderId == workOrderId).ToList();
            db.EMWorkOrderParts.RemoveRange(parts);

            // Remove items
            var items = db.EMWorkOrderItems.Where(i => i.EMCo == emco && i.WorkOrderId == workOrderId).ToList();
            db.EMWorkOrderItems.RemoveRange(items);

            // Remove work order
            var workOrder = db.EMWorkOrders.FirstOrDefault(f => f.EMCo == emco && f.WorkOrderId == workOrderId);
            if (workOrder != null)
            {
                db.EMWorkOrders.Remove(workOrder);
            }

            db.SaveChanges(ModelState);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        #endregion

        #region Work Order Items

        [HttpGet]
        public PartialViewResult ItemsTable(byte emco, string workOrderId)
        {
            using var db = new VPContext();
            var workOrder = db.EMWorkOrders
                .Include(w => w.Items)
                .Include(w => w.Items.Select(i => i.Equipment))
                .FirstOrDefault(f => f.EMCo == emco && f.WorkOrderId == workOrderId);

            var result = new WorkOrderItemListViewModel(workOrder);
            return PartialView("../EM/WorkOrder/Forms/Items/Table", result);
        }

        [HttpGet]
        public PartialViewResult ItemAdd(byte emco, string workOrderId)
        {
            using var db = new VPContext();
            var workOrder = db.EMWorkOrders
                .Include(w => w.EMCompany)
                .Include(w => w.EMCompany.LaborCostCode)
                .FirstOrDefault(f => f.EMCo == emco && f.WorkOrderId == workOrderId);

            if (workOrder == null)
            {
                return PartialView("../EM/WorkOrder/Forms/Items/TableRow", new WorkOrderItemViewModel());
            }

            // Get next item number
            var maxItem = db.EMWorkOrderItems
                .Where(i => i.EMCo == emco && i.WorkOrderId == workOrderId)
                .Select(i => i.WOItem)
                .DefaultIfEmpty((short)0)
                .Max();

            var newItem = new EMWorkOrderItem
            {
                EMCo = emco,
                WorkOrderId = workOrderId,
                WOItem = (short)(maxItem + 1),
                EquipmentId = workOrder.EquipmentId,
                Description = "New Item",
                DateCreated = DateTime.Now,
                EMGroup = workOrder.EMCompany.EMGroupId,
                InHseSubFlag = "I",
                RepairType = "1",
                Priority = "N",
                CostCodeId = workOrder.EMCompany.LaborCostCode?.CostCodeId ?? "300"
            };

            newItem.StatusCode = db.EMWorkOrderStatusCodes.FirstOrDefault(s => s.StatusCodeId == "1");

            db.EMWorkOrderItems.Add(newItem);
            db.SaveChanges(ModelState);

            var result = new WorkOrderItemViewModel(newItem);
            ViewBag.TableRow = "True";
            return PartialView("../EM/WorkOrder/Forms/Items/TableRow", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ItemUpdate(WorkOrderItemViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            using var db = new VPContext();
            var item = db.EMWorkOrderItems.FirstOrDefault(f => 
                f.EMCo == model.EMCo && 
                f.WorkOrderId == model.WorkOrderId && 
                f.WOItem == model.WorkOrderItemId);
            
            if (item != null)
            {
                item.EquipmentId = model.EquipmentId;
                item.Description = model.Description;
                item.MechanicId = model.MechanicId;
                item.DateDue = model.DateDue;
                item.DateSched = model.DateSched;
                item.Notes = model.Notes;
                
                db.SaveChanges(ModelState);
                model = new WorkOrderItemViewModel(item);
            }

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ItemDelete(byte emco, string workOrderId, short woItem)
        {
            using var db = new VPContext();
            var item = db.EMWorkOrderItems.FirstOrDefault(f => 
                f.EMCo == emco && 
                f.WorkOrderId == workOrderId && 
                f.WOItem == woItem);
            
            if (item != null)
            {
                // Remove associated parts first
                //var parts = db.EMWorkOrderParts.Where(p => 
                //    p.EMCo == emco && 
                //    p.WorkOrderId == workOrderId && 
                //    p.WOItem == woItem);
                //db.EMWorkOrderParts.RemoveRange(parts);
                
                db.EMWorkOrderItems.Remove(item);
                db.SaveChanges(ModelState);
            }

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ItemComplete(byte emco, string workOrderId, short woItem)
        {
            using var db = new VPContext();
            var item = db.EMWorkOrderItems.FirstOrDefault(f => 
                f.EMCo == emco && 
                f.WorkOrderId == workOrderId && 
                f.WOItem == woItem);
            
            if (item != null)
            {
                item.DateCompl = DateTime.Now;
                db.SaveChanges(ModelState);
            }

            var model = new WorkOrderItemViewModel(item);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }
        #endregion

        #region Work Order Parts
        [HttpGet]
        public PartialViewResult PartsTable(byte emco, string workOrderId)
        {
            using var db = new VPContext();
            var workOrder = db.EMWorkOrders
                .Include(w => w.Parts)
                .FirstOrDefault(f => f.EMCo == emco && f.WorkOrderId == workOrderId);

            // TODO: Create PartsListViewModel
            return PartialView("../EM/WorkOrder/Forms/Parts/Table");
        }
        #endregion

        #region Search
        [HttpGet]
        public PartialViewResult Search(byte co)
        {
            using var db = new VPContext();
            var workOrders = db.EMWorkOrders
                .Include(w => w.Equipment)
                .Where(w => w.EMCo == co && w.Complete == "N")
                .ToList();

            var results = new WorkOrderListViewModel(workOrders);
            return PartialView("../EM/WorkOrder/Search/Panel", results);
        }

        [HttpGet]
        public PartialViewResult SearchTable(byte co)
        {
            using var db = new VPContext();
            var workOrders = db.EMWorkOrders
                .Include(w => w.Equipment)
                .Where(w => w.EMCo == co && w.Complete == "N")
                .ToList();

            var results = new WorkOrderListViewModel(workOrders);
            return PartialView("../EM/WorkOrder/Search/Table", results);
        }
        #endregion
    }
}
