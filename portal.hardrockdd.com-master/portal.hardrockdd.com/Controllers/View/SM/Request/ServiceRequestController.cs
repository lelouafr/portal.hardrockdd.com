using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Net.Mail;

namespace portal.Controllers.VP
{
    public class ServiceRequestController : BaseController
    {
        #region New Service Request
        [HttpGet]
        public ActionResult CreateServiceRequest()
        {
            using var db = new VPContext();
            var userId = StaticFunctions.GetUserId();
            var newObj = db.SMRequests.FirstOrDefault(f => f.RequestBy == userId && f.StatusId == 0);
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            if (newObj == null)
            {
                newObj = new SMRequest
                {
                    RequestBy = StaticFunctions.GetUserId(),
                    RequestDate = DateTime.Now,
                    SMCo = company.HQCo,
                    Status = DB.SMRequestStatusEnum.Draft,
                    RequestId = db.SMRequests.DefaultIfEmpty().Max(max => max == null ? 0 : max.RequestId) + 1,
                    RequestType = DB.SMRequestTypeEnum.Equipment,    
                    
                    RequestUser = user,
                    HQCompanyParm = company,
                };
                newObj.GenerateWorkFlow();
                newObj.WorkFlow.CreateSequence(newObj.StatusId);
                newObj.WorkFlow.AddUser(StaticFunctions.GetUserId());
                db.SMRequests.Add(newObj);
                db.SaveChanges(ModelState);
            }
            return RedirectToAction("ServiceRequestIndex", new { smco = newObj.SMCo, requestId = newObj.RequestId });

        }
        #endregion

        #region Service Request Master
        [HttpGet]
        [Route("Service/Request/List")]
        public ActionResult ServiceRequestListIndex()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var requests = db.SMRequests.ToList().Where(f => f.Status != DB.SMRequestStatusEnum.Draft && f.Status != DB.SMRequestStatusEnum.Canceled).ToList();
            var results = new portal.Models.Views.SM.Request.ServiceRequestListViewModel(requests);
            return View("../SM/Request/Summary/List/Index", results);
        }

        [HttpGet]
        public PartialViewResult ServiceRequestListTable()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var requests = db.SMRequests.ToList().Where(f => f.Status != DB.SMRequestStatusEnum.Draft && f.Status != DB.SMRequestStatusEnum.Canceled).ToList();
            var results = new portal.Models.Views.SM.Request.ServiceRequestListViewModel(requests);
            return PartialView("../SM/Request/Summary/List/Table", results);
        }


        [HttpGet]
        [Route("Service/Request/User/List")]
        public ActionResult ServiceRequestUserListIndex()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();

            var requests = db.SMRequests.Where(f => f.RequestBy == userId).ToList();
            var results = new portal.Models.Views.SM.Request.ServiceRequestListViewModel(requests);
            return View("../SM/Request/Summary/List/Index", results);
        }

        [HttpGet]
        public PartialViewResult ServiceRequestUserListTable()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();

            var requests = db.SMRequests.Where(f => f.RequestBy == userId).ToList();
            var results = new portal.Models.Views.SM.Request.ServiceRequestListViewModel(requests);
            return PartialView("../SM/Request/Summary/List/Table", results);
        }


        [HttpGet]
        [Route("Service/Request/All/List")]
        public ActionResult ServiceRequestAllListIndex()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();

            var requests = db.SMRequests.ToList();
            var results = new portal.Models.Views.SM.Request.ServiceRequestListViewModel(requests);
            return View("../SM/Request/Summary/List/Index", results);
        }

        [HttpGet]
        public PartialViewResult ServiceRequestAllListTable()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();

            var requests = db.SMRequests.ToList();
            var results = new portal.Models.Views.SM.Request.ServiceRequestListViewModel(requests);
            return PartialView("../SM/Request/Summary/List/Table", results);
        }
        #endregion

        #region Equipment Service Request Master
        [HttpGet]
        [Route("Service/Equipment/Request/List")]
        public ActionResult ServiceEquipmentRequestListIndex()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var request = db.SMRequests.ToList().Where(f => f.Status != DB.SMRequestStatusEnum.Draft && f.Status != DB.SMRequestStatusEnum.Canceled).ToList();
            var results = new portal.Models.Views.SM.Request.Equipment.EquipmentListViewModel(request);
            return View("../SM/Equipment/Summary/Index", results);
        }

        [HttpGet]
        public PartialViewResult ServiceEquipmentRequestListTable()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var request = db.SMRequests.ToList().Where(f => f.Status != DB.SMRequestStatusEnum.Draft && f.Status != DB.SMRequestStatusEnum.Canceled).ToList();
            var results = new portal.Models.Views.SM.Request.Equipment.EquipmentListViewModel(request);
            return PartialView("../SM/Equipment/Summary/List/Table", results);
        }

        //var requests = db.SMRequestLines.Where(f => f.Request.RequestTypeId == (int)DB.SMRequestTypeEnum.Equipment).ToList();


        [HttpGet]
        [Route("Service/Equipment/Request/All")]
        public ActionResult ServiceEquipmentAllListIndex()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext(); 
            var requests = db.SMRequestLines.Where(f => f.Request.RequestTypeId == (int)DB.SMRequestTypeEnum.Equipment && f.tEquipmentId != null).ToList();
            var results = new portal.Models.Views.SM.RequestLine.EquipmentListLineViewModel(requests);
            return View("../SM/Equipment/Summary/RequestLine/Index", results);
        }

        [HttpGet]
        public PartialViewResult ServiceEquipmentAllListTable()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext(); 
            var requests = db.SMRequestLines.Where(f => f.Request.RequestTypeId == (int)DB.SMRequestTypeEnum.Equipment && f.tEquipmentId != null).ToList();
            var results = new portal.Models.Views.SM.RequestLine.EquipmentListLineViewModel(requests);
            return PartialView("../SM/Equipment/Summary/RequestLine/Table", results);
        }
        #endregion

        #region Service Request Form

        [HttpGet]
        [Route("Service/Request/{requestId}-{smco}")]
        public ViewResult ServiceRequestIndex(byte smco, int requestId)
        {
            using var db = new VPContext();
            var entity = db.SMRequests.FirstOrDefault(f => f.SMCo == smco && f.RequestId == requestId);

            switch (entity.RequestType)
            {
                case DB.SMRequestTypeEnum.Equipment:
                    //portal.Models.Views.SM.Request.Equipment.Forms.FormViewModel
                    return View("../SM/Request/Equipment/Form/Index", new portal.Models.Views.SM.Request.Forms.FormViewModel(entity));
                    
                default:
                    var result = new portal.Models.Views.SM.Request.Forms.FormViewModel(entity);
                    return View("../SM/Request/Forms/Index", result);
                    
            }
        }

        [HttpGet]
        public PartialViewResult ServiceRequestPanel(byte smco, int requestId)
        {
            using var db = new VPContext();
            var entity = db.SMRequests.FirstOrDefault(f => f.SMCo == smco && f.RequestId == requestId);

            var result = new portal.Models.Views.SM.Request.Forms.FormViewModel(entity);
            result.WorkFlowActions.WorkFlowActions.ForEach(e => e.ActionRedirect = "Reload");
            switch (entity.RequestType)
            {
                case DB.SMRequestTypeEnum.Equipment:
                    //portal.Models.Views.SM.Request.Equipment.Forms.FormViewModel
                    return PartialView("../SM/Request/Equipment/Form/Panel", result);

                default:
                    return PartialView("../SM/Request/Forms/Panel", result);

            }
        }

        [HttpGet]
        public PartialViewResult ServiceRequestForm(byte smco, int requestId)
        {
            using var db = new VPContext();
            var entity = db.SMRequests.FirstOrDefault(f => f.SMCo == smco && f.RequestId == requestId);

            var result = new portal.Models.Views.SM.Request.Forms.FormViewModel(entity);
            result.WorkFlowActions.WorkFlowActions.ForEach(e => e.ActionRedirect = "Reload");
            switch (entity.RequestType)
            {
                case DB.SMRequestTypeEnum.Equipment:
                    //portal.Models.Views.SM.Request.Equipment.Forms.FormViewModel
                    return PartialView("../SM/Request/Equipment/Form/Form", result);

                default:
                    return PartialView("../SM/Request/Forms/Form", result);

            }
        }

        #endregion

        #region Request Actions
        [HttpGet]
        public ActionResult RequestActionPanel(byte smco, int requestId)
        {
            using var db = new VPContext();
            var entity = db.SMRequests.FirstOrDefault(f => f.SMCo == smco && f.RequestId == requestId);
            var model = new portal.Models.Views.SM.Request.Forms.FormViewModel(entity);
            var userId = StaticFunctions.GetUserId();
            ViewBag.ViewOnly = entity.WorkFlow.Sequences.SelectMany(s => s.AssignedUsers).Any(w => w.AssignedTo == userId && w.Sequence.Status == entity.StatusId && w.Sequence.Active == true) ? "False" : "True";

            return PartialView("../BD/Bid/Forms/Action/Panel", model);
        }

        [HttpGet]
        public ActionResult RequestActionForm(byte smco, int requestId)
        {
            using var db = new VPContext();
            var entity = db.SMRequests.FirstOrDefault(f => f.SMCo == smco && f.RequestId == requestId);
            var model = new portal.Models.Views.SM.Request.Forms.FormViewModel(entity);
            var userId = StaticFunctions.GetUserId();
            ViewBag.ViewOnly = entity.WorkFlow.Sequences.SelectMany(s => s.AssignedUsers).Any(w => w.AssignedTo == userId && w.Sequence.Status == entity.StatusId && w.Sequence.Active == true) ? "False" : "True";

            return PartialView("../BD/Bid/Forms/Action/Form", model);
        }

        //[HttpGet]
        //public ActionResult RequestUpdateStatus(byte smco, int requestId, int gotoStatusId, string ActionRedirect)
        //{
        //    using var db = new VPContext();
        //    var entity = db.SMRequests.FirstOrDefault(f => f.SMCo == smco && f.RequestId == requestId);
        //    entity.Status = (DB.SMRequestStatusEnum)gotoStatusId;
        //    foreach (var item in entity.Lines.Where(f => f.Equipment == null).ToList())
        //    {
        //        entity.Lines.Remove(item);
        //    }
        //    db.SaveChanges(ModelState);
        //    if (ActionRedirect == "Reload")
        //    {
        //        return RedirectToAction("ServiceRequestIndex", new { smco, requestId });
        //    }
        //    else if (ActionRedirect == "Home")
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //    return RedirectToAction("Index", "Home");
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RequestUpdateStatus(byte smco, int requestId, int gotoStatusId, string ActionRedirect)
        {
            using var db = new VPContext();
            var entity = db.SMRequests.FirstOrDefault(f => f.SMCo == smco && f.RequestId == requestId);

            // Remove lines without equipment
            foreach (var item in entity.Lines.Where(f => f.Equipment == null).ToList())
            {
                entity.Lines.Remove(item);
            }

            entity.Status = (DB.SMRequestStatusEnum)gotoStatusId;
            db.SaveChanges(ModelState);

            // ========== AUTO-CREATE WORK ORDER ON SUBMIT ==========
            if (gotoStatusId == (int)DB.SMRequestStatusEnum.Submitted)
            {
                // Call the existing CreateWorkOrderFromRequest logic
                AutoCreateWorkOrder(db, smco, requestId);
            }
            // ======================================================

            var url = Url.Action("Index", "Home", new { Area = "" });
            if (ActionRedirect == "Reload")
            {
                url = Url.Action("ServiceRequestIndex", new { smco, requestId });
            }

            return Json(new { url, success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        // ========== ADD THIS NEW PRIVATE METHOD ==========
        private void AutoCreateWorkOrder(VPContext db, byte smco, int requestId)
        {
            var request = db.SMRequests.FirstOrDefault(f => f.SMCo == smco && f.RequestId == requestId);

            if (request == null)
                return;

            var equipmentLines = request.Lines.Where(l => l.tEquipmentId != null && l.WorkOrderId == null).ToList();

            if (!equipmentLines.Any())
                return;

            var firstEquipment = equipmentLines.First().Equipment;
            byte emco = firstEquipment?.EMCo ?? 1;

            // Generate unique Work Order ID using timestamp
            string workOrderIdStr = DateTime.Now.ToString("MMddHHmmss");

            var workOrder = new EMWorkOrder
            {
                EMCo = emco,
                WorkOrderId = workOrderIdStr,
                EquipmentId = firstEquipment?.EquipmentId,
                Description = $"From Service Request #{requestId}",
                ShopGroupId = (byte)(equipmentLines.FirstOrDefault()?.ShopGroupId ?? 1),
                DateCreated = DateTime.Now,
                Notes = request.Comments,
                Complete = "N"
            };

            db.EMWorkOrders.Add(workOrder);
            db.SaveChanges(ModelState);

            if (!ModelState.IsValid)
                return;

            short itemNum = 1;
            foreach (var line in equipmentLines)
            {
                var woItem = new EMWorkOrderItem
                {
                    EMCo = emco,
                    WorkOrderId = workOrder.WorkOrderId,
                    WOItem = itemNum,
                    EquipmentId = line.tEquipmentId,
                    Description = line.Description ?? line.RequestComments ?? "Service Request Item",
                    DateCreated = DateTime.Now,
                    Notes = line.RequestComments,
                    EMGroup = 1,
                    InHseSubFlag = "I",
                    RepairType = "1",
                    Priority = "N"
                };

                woItem.CostCodeId = "300";
                woItem.StatusCode = db.EMWorkOrderStatusCodes.FirstOrDefault(s => s.StatusCodeId == "1");

                db.EMWorkOrderItems.Add(woItem);

                line.EMCo = emco;
                line.WorkOrderId = workOrder.WorkOrderId;
                line.WOItemId = itemNum;

                itemNum++;
            }

            db.SaveChanges(ModelState);
        }



        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWorkOrderFromRequest(byte smco, int requestId)
        {
            using var db = new VPContext();
            var request = db.SMRequests.FirstOrDefault(f => f.SMCo == smco && f.RequestId == requestId);

            if (request == null)
            {
                return Json(new { success = "false", errorModel = new { Error = "Service Request not found" } });
            }

            var equipmentLines = request.Lines.Where(l => l.tEquipmentId != null).ToList();

            if (!equipmentLines.Any())
            {
                return Json(new { success = "false", errorModel = new { Error = "No equipment lines found in request" } });
            }

            var firstEquipment = equipmentLines.First().Equipment;
            byte emco = firstEquipment?.EMCo ?? 1;

            // Generate unique Work Order ID using timestamp
            string workOrderIdStr = DateTime.Now.ToString("MMddHHmmss");

            var workOrder = new EMWorkOrder
            {
                EMCo = emco,
                WorkOrderId = workOrderIdStr,
                EquipmentId = firstEquipment?.EquipmentId,
                Description = $"From Service Request #{requestId}",
                ShopGroupId = (byte)(equipmentLines.FirstOrDefault()?.ShopGroupId ?? 1),
                DateCreated = DateTime.Now,
                Notes = request.Comments,
                Complete = "N"
            };

            db.EMWorkOrders.Add(workOrder);
            db.SaveChanges(ModelState);

            if (!ModelState.IsValid)
            {
                return Json(new { success = "false", errorModel = ModelState.ModelErrors() });
            }

            short itemNum = 1;
            foreach (var line in equipmentLines)
            {
                var woItem = new EMWorkOrderItem
                {
                    EMCo = emco,
                    WorkOrderId = workOrder.WorkOrderId,
                    WOItem = itemNum,
                    EquipmentId = line.tEquipmentId,
                    Description = line.Description ?? line.RequestComments ?? "Service Request Item",
                    DateCreated = DateTime.Now,
                    Notes = line.RequestComments,
                    EMGroup = 1,
                    InHseSubFlag = "I",
                    RepairType = "1",
                    Priority = line.IsEmergancy == true ? "E" : "N"
                };

                woItem.CostCodeId = "300";
                woItem.StatusCode = db.EMWorkOrderStatusCodes.FirstOrDefault(s => s.StatusCodeId == "1");
                
                db.EMWorkOrderItems.Add(woItem);

                line.EMCo = emco;
                line.WorkOrderId = workOrder.WorkOrderId;
                line.WOItemId = itemNum;

                itemNum++;
            }

            db.SaveChanges(ModelState);

            var url = Url.Action("Index", "EMWorkOrder", new { Area = "", emco = workOrder.EMCo, workOrderId = workOrder.WorkOrderId });
            return Json(new { success = ModelState.IsValid ? "true" : "false", url, errorModel = ModelState.ModelErrors() });
        }

        #region Service Request Info Form
        [HttpGet]
        public PartialViewResult ServiceRequestInfoForm(byte smco, int requestId)
        {
            using var db = new VPContext();
            var entity = db.SMRequests.FirstOrDefault(f => f.SMCo == smco && f.RequestId == requestId);

            var result = new portal.Models.Views.SM.Request.Forms.InfoViewModel(entity);
            return PartialView("../SM/Request/Forms/Info/Form", result);
        }

        [HttpGet]
        public PartialViewResult ServiceRequestInfoPanel(byte smco, int requestId)
        {
            using var db = new VPContext();
            var entity = db.SMRequests.FirstOrDefault(f => f.SMCo == smco && f.RequestId == requestId);

            var result = new portal.Models.Views.SM.Request.Forms.InfoViewModel(entity);
            return PartialView("../SM/Request/Forms/Info/Panel", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ServiceRequestInfoUpdate(portal.Models.Views.SM.Request.Forms.InfoViewModel model)
        {
            using var db = new VPContext();
            var entity = db.SMRequests.FirstOrDefault(f => f.SMCo == model.SMCo && f.RequestId == model.RequestId);
            if (entity != null)
            {
                entity.RequestType = model.RequestType;
                entity.Comments = model.Comments;
                db.SaveChanges(ModelState);
            }

            var result = new portal.Models.Views.SM.Request.Forms.InfoViewModel(entity);
            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult ServiceRequestInfoValidate(portal.Models.Views.SM.Request.Forms.InfoViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Service Request Equipment List
        [HttpGet]
        public PartialViewResult RequestEquipmentLineTable(byte smco, int requestId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var request = db.SMRequests.FirstOrDefault(f => f.SMCo == smco && f.RequestId == requestId);

            var result = new portal.Models.Views.SM.Request.Forms.EquipmentLineListViewModel(request);
            return PartialView("../SM/Request/Equipment/Form/List/Table", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RequestEquipmentLineUpdate(portal.Models.Views.SM.Request.Forms.EquipmentLineViewModel model)
        {
            if (ModelState.IsValid && model != null)
            {
                using var db = new VPContext();
                var entity = db.SMRequestLines.FirstOrDefault(f => f.SMCo == model.SMCo && f.RequestId == model.RequestId && f.LineId == model.LineId);
                if (entity != null)
                {
                    // Equipment handling
                    if (entity.tEquipmentId != model.EquipmentId)
                    {
                        var equipment = db.Equipments.FirstOrDefault(f => f.EquipmentId == model.EquipmentId);
                        entity.tEquipmentId = model.EquipmentId;
                        entity.Equipment = null;
                        entity.Equipment = equipment;
                        entity.ShopGroupId = equipment?.ShopGroup;
                    }
                    if (entity.tEquipmentId != null && entity.Equipment == null)
                    {
                        var equipment = db.Equipments.FirstOrDefault(f => f.EquipmentId == entity.tEquipmentId);
                        entity.EMCo = equipment.EMCo;
                        entity.Equipment = equipment;
                        entity.ShopGroupId = equipment.ShopGroup;
                    }

                    // ========== EXISTING FIELDS (budSMRL) ==========
                    entity.RequestComments = model.RequestComments;
                    entity.AssignedLocation = model.AssignedLocation;
                    entity.EMOdoReading = model.Mileage;
                    entity.EMHourReading = model.Hours;
                    entity.IsEmergancy = model.IsEmergency;

                    // ========== CUSTOM FIELDS (budSMRLCustom) - using raw SQL helper ==========
                    var customData = new SMRequestLineCustom
                    {
                        SMCo = model.SMCo,
                        RequestId = model.RequestId,
                        LineId = model.LineId,
                        PriorityId = (byte?)model.PriorityId,
                        RequestTypeId = model.MaintenanceRequestTypeId
                    };
                    SMCustomData.SaveCustomData(customData, db.CurrentUserId);
                    // ================================================
                }

                db.SaveChanges(ModelState);
                model = new portal.Models.Views.SM.Request.Forms.EquipmentLineViewModel(entity);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }


        [HttpGet]
        public PartialViewResult RequestEquipmentLineAdd(byte smco, int requestId)
        {
            using var db = new VPContext();
            var request = db.SMRequests.FirstOrDefault(f => f.SMCo == smco && f.RequestId == requestId);
            var newObj = new SMRequestLine
            {
                SMCo = request.SMCo,
                RequestId = request.RequestId,
                LineId = request.Lines.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineId) + 1,
                Status = DB.SMRequestLineStatusEnum.Draft,
                Request = request,

            };
            newObj.AddWorkFlow();
            newObj.WorkFlow.AddSequence(0);
            newObj.WorkFlow.AddUser(StaticFunctions.GetUserId());

            request.Lines.Add(newObj);
            db.SaveChanges(ModelState);

            var result = new portal.Models.Views.SM.Request.Forms.EquipmentLineViewModel(newObj);
            ViewBag.tableRow = "True";
            return PartialView("../SM/Request/Equipment/Form/List/TableRow", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RequestEquipmentLineDelete(byte smco, int requestId, int lineId)
        {
            using var db = new VPContext();

            var request = db.SMRequests.FirstOrDefault(f => f.SMCo == smco && f.RequestId == requestId);
            var entity = request.Lines.FirstOrDefault(f => f.LineId == lineId);
            if (entity != null)
            {
                var workflow = entity.WorkFlow;
                request.Lines.Remove(entity);
                db.SaveChanges(ModelState);


                if (workflow != null)
                {
                    workflow = db.WorkFlows.FirstOrDefault(f => f.WorkFlowId == workflow.WorkFlowId && f.WFCo == workflow.WFCo);
                    if (workflow != null)
                        db.WorkFlows.Remove(workflow);
                    db.SaveChanges(ModelState);
                }
            }

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }
        #endregion

        #region Search
        [HttpGet]
        public PartialViewResult Search(byte smco)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var request = db.SMRequests.ToList();
            var results = new portal.Models.Views.SM.Request.ServiceRequestListViewModel(request);

            return PartialView("../SM/Category/Search/Panel", results);
        }

        [HttpGet]
        public PartialViewResult SearchTable(byte smco)
        {
            using var db = new VPContext();
            var request = db.SMRequests.ToList();
            var results = new portal.Models.Views.SM.Request.ServiceRequestListViewModel(request);

            return PartialView("../SM/Category/Search/Table", results);
        }

        [HttpPost]
        public JsonResult SearchReturn(portal.Models.Views.SM.Request.ServiceRequestViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return Json(new { success = "true", value = model.RequestId, errorModel = ModelState.ModelErrors() });
        }
        #endregion

        #region Equipment Type Service Actions
        #region Equipment Form

        [HttpGet]
        [Route("Service/Equipment/Requests/{equipmentId}-{emco}")]
        public ViewResult ServiceEquipmentIndex(byte emco, string equipmentId)
        {
            using var db = new VPContext();
            var entity = db.Equipments.FirstOrDefault(f => f.EMCo == emco && f.EquipmentId == equipmentId);

            var result = new portal.Models.Views.SM.Request.Equipment.Forms.FormViewModel(entity);
            return View("../SM/Equipment/Form/Index", result);
        }

        [HttpGet]
        public PartialViewResult ServiceEquipmentPanel(byte emco, string equipmentId)
        {
            using var db = new VPContext();
            var entity = db.Equipments.FirstOrDefault(f => f.EMCo == emco && f.EquipmentId == equipmentId);

            var result = new portal.Models.Views.SM.Request.Equipment.Forms.FormViewModel(entity);
            return PartialView("../SM/Equipment/Form/Panel", result);
        }

        [HttpGet]
        public PartialViewResult ServiceEquipmenttForm(byte emco, string equipmentId)
        {
            using var db = new VPContext();
            var entity = db.Equipments.FirstOrDefault(f => f.EMCo == emco && f.EquipmentId == equipmentId);

            var result = new portal.Models.Views.SM.Request.Equipment.Forms.FormViewModel(entity);
            return PartialView("../SM/Equipment/Form/Form", result);
        }
        #endregion

        #region Equipment Service Info Form
        [HttpGet]
        public PartialViewResult EquipmentInfoForm(byte emco, string equipmentId)
        {
            using var db = new VPContext();
            var entity = db.Equipments.FirstOrDefault(f => f.EMCo == emco && f.EquipmentId == equipmentId);

            var result = new portal.Models.Views.SM.Request.Equipment.Forms.InfoViewModel(entity);
            return PartialView("../SM/Request/Equipment/Forms/Info/Form", result);
        }

        [HttpGet]
        public PartialViewResult EquipmentInfoPanel(byte emco, string equipmentId)
        {
            using var db = new VPContext();
            var entity = db.Equipments.FirstOrDefault(f => f.EMCo == emco && f.EquipmentId == equipmentId);

            var result = new portal.Models.Views.SM.Request.Equipment.Forms.InfoViewModel(entity);
            return PartialView("../SM/Request/Equipment/Forms/Info/Panel", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EquipmentInfoUpdate(portal.Models.Views.SM.Request.Forms.InfoViewModel model)
        {
            using var db = new VPContext();
            var entity = db.SMRequests.FirstOrDefault(f => f.SMCo == model.SMCo && f.RequestId == model.RequestId);
            if (entity != null)
            {
                entity.RequestType = model.RequestType;
                entity.Comments = model.Comments;
                db.SaveChanges(ModelState);
            }

            var result = new portal.Models.Views.SM.Request.Forms.InfoViewModel(entity);
            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult EquipmentInfoValidate(portal.Models.Views.SM.Request.Forms.InfoViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Equipment Service Request Line List

        [HttpGet]
        public PartialViewResult EquipmentRequestLineTable(byte emco, string equipmentId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var entity = db.Equipments.FirstOrDefault(f => f.EMCo == emco && f.EquipmentId == equipmentId);

            var result = new portal.Models.Views.SM.Request.Equipment.Forms.RequestLineListViewModel(entity);
            return PartialView("../SM/Equipment/Form/Forms/LineList/Table", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EquipmentRequestLineUpdate(portal.Models.Views.SM.Request.Equipment.Forms.RequestLineViewModel model)
        {
            if (ModelState.IsValid && model != null)
            {
                using var db = new VPContext();
                var entity = db.SMRequestLines.FirstOrDefault(f => f.SMCo == model.SMCo && f.RequestId == model.RequestId && f.LineId == model.LineId);
                if (entity != null)
                {
                    entity.EMWorkOrderAdd = model.AddToWorkOrder;
                    if (model.IsComplete)
                    {
                        entity.Status = DB.SMRequestLineStatusEnum.Completed;
                    }
                    else
                    {
                        entity.Status = DB.SMRequestLineStatusEnum.Pending;
                    }
                    db.SaveChanges(ModelState);
                }
                model = new portal.Models.Views.SM.Request.Equipment.Forms.RequestLineViewModel(entity);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        #endregion

        #region Equipment Request Line Form

        [HttpGet]
        public ActionResult EquipmentRequestLinePanel(byte smco, int requestId, int lineId)
        {
            using var db = new VPContext();
            var entity = db.SMRequestLines.FirstOrDefault(f => f.SMCo == smco && f.RequestId == requestId && f.LineId == lineId);

            if (entity != null)
            {
                var result = new portal.Models.Views.SM.Request.Equipment.Forms.Line.FormViewModel(entity, this);

                if (result.Info.Status == DB.SMRequestLineStatusEnum.Draft)
                {
                    return RedirectToAction("ServiceRequestPanel", new { smco = entity.SMCo, requestId = entity.RequestId });
                }
                return PartialView("../SM/Equipment/Form/Forms/RequestLine/Panel", result);

            }
            return PartialView("");
        }

        [HttpGet]
        public ActionResult EquipmentRequestLineForm(byte smco, int requestId, int lineId)
        {
            using var db = new VPContext();
            var entity = db.SMRequestLines.FirstOrDefault(f => f.SMCo == smco && f.RequestId == requestId && f.LineId == lineId);

            var result = new portal.Models.Views.SM.Request.Equipment.Forms.Line.FormViewModel(entity, this);
            return PartialView("../SM/Equipment/Form/Forms/RequestLine/Form", result);
        }
        #endregion

        #region Equipment Service Request Line Info Form
        [HttpGet]
        public PartialViewResult EquipmentRequestLineInfoForm(byte smco, int requestId, int lineId)
        {
            using var db = new VPContext();
            var entity = db.SMRequestLines.FirstOrDefault(f => f.SMCo == smco && f.RequestId == requestId && f.LineId == lineId);

            var result = new portal.Models.Views.SM.Request.Equipment.Forms.Line.InfoViewModel(entity);
            return PartialView("../SM/Equipment/Form/Forms/RequestLine/Info/Form", result);
        }

        [HttpGet]
        public PartialViewResult EquipmentRequestLineInfoPanel(byte smco, int requestId, int lineId)
        {
            using var db = new VPContext();
            var entity = db.SMRequestLines.FirstOrDefault(f => f.SMCo == smco && f.RequestId == requestId && f.LineId == lineId);

            var result = new portal.Models.Views.SM.Request.Equipment.Forms.Line.InfoViewModel(entity);
            return PartialView("../SM/Equipment/Form/Forms/RequestLine/Info/Panel", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EquipmentRequestLineInfoUpdate(portal.Models.Views.SM.Request.Equipment.Forms.Line.InfoViewModel model)
        {
            using var db = new VPContext();

            model = model.ProcessUpdate(db, ModelState);
            db.SaveChanges(ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult EquipmentRequestLineInfoValidate(portal.Models.Views.SM.Request.Forms.InfoViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Equipment Request Line Actions
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EquipmentRequestLineUpdateStatus(byte smco, int requestId, int lineId, int gotoStatusId, string ActionRedirect)
        {
            using var db = new VPContext();
            var entity = db.SMRequestLines.FirstOrDefault(f => f.SMCo == smco && f.RequestId == requestId && f.LineId == lineId);
            if (entity != null)
            {

                entity.Status = (DB.SMRequestLineStatusEnum)gotoStatusId;

                db.SaveChanges(ModelState);

            }
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });

        }

        #endregion
        #endregion

        #region Service Request Line Summary
        [Route("Service/Request/Line/User/Summary")]
        public ActionResult ServiceRequestLineUserListIndex()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var curTmpUser = StaticFunctions.GetCurrentUser();
            var curUser = db.WebUsers.FirstOrDefault(f => f.Id == curTmpUser.Id);

            var requests = curUser.SMServiceRequests.ToList();
            var results = new portal.Models.Views.SM.Request.Summary.ServiceRequestLineListViewModel(requests);
            return View("../SM/Request/Line/Summary/User/Index", results);
        }

        [HttpGet]
        public PartialViewResult ServiceRequestLineUserListTable()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var curTmpUser = StaticFunctions.GetCurrentUser();
            var curUser = db.WebUsers.FirstOrDefault(f => f.Id == curTmpUser.Id);

            var requests = curUser.SMServiceRequests.ToList();
            var results = new portal.Models.Views.SM.Request.Summary.ServiceRequestLineListViewModel(requests);
            return PartialView("../SM/Request/Line/Summary/User/List/Table", results);
        }
        #endregion

        #region Email

        #endregion


        #region Foreman Dashboard

        [HttpGet]
        [Route("Service/Foreman/Dashboard")]
        public ActionResult ForemanDashboard()
        {
            try
            {
                using var db = new VPContext();

                // Get request lines from the last 30 days with equipment
                var thirtyDaysAgo = DateTime.Now.AddDays(-30);

                var lines = db.SMRequestLines
                    .Where(l => l.tEquipmentId != null)
                    .ToList()
                    .Where(l => l.Request.Status != DB.SMRequestStatusEnum.Draft
                             && l.Request.Status != DB.SMRequestStatusEnum.Canceled
                             && l.Request.RequestDate >= thirtyDaysAgo)
                    .OrderByDescending(l => l.Request.RequestDate)
                    .ToList();

                var model = new portal.Models.Views.SM.Request.ForemanDashboardViewModel(lines);
                return View("~/Views/SM/Request/Foreman/Index.cshtml", model);
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message + "<br><br>Stack: " + ex.StackTrace, "text/html");
            }
        }


        [HttpGet]
        public JsonResult GetMechanics(byte emco = 1)
        {
            using var db = new VPContext();

            var mechanics = db.Employees
                .Where(e => e.PRCo == emco && e.ActiveYN == "Y")
                .OrderBy(e => e.LastName)
                .Take(100)
                .Select(e => new { Value = e.EmployeeId.ToString(), Text = e.FirstName + " " + e.LastName })
                .ToList();

            return Json(mechanics, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AssignMechanic(byte smco, int requestId, int lineId, byte emco, string workOrderId, int mechanicId)
        {
            using var db = new VPContext();

            // Update Service Request Line
            var line = db.SMRequestLines.FirstOrDefault(l =>
                l.SMCo == smco && l.RequestId == requestId && l.LineId == lineId);

            if (line != null)
            {
                line.AsignedEmployeeId = mechanicId;
                //line.Status = DB.SMRequestLineStatusEnum.Assigned;
            }

            // Update Work Order Item if exists
            if (!string.IsNullOrEmpty(workOrderId))
            {
                var woItem = db.EMWorkOrderItems.FirstOrDefault(w =>
                    w.EMCo == emco && w.WorkOrderId == workOrderId && w.WOItem == line.WOItemId);

                if (woItem != null)
                {
                    woItem.MechanicId = mechanicId;
                }
            }

            db.SaveChanges(ModelState);

            return Json(new { success = ModelState.IsValid });
        }

        #endregion



        #region Mechanic Dashboard

        [HttpGet]
        [Route("Service/Mechanic/Dashboard")]
        public ActionResult MechanicDashboard()
        {
            try
            {
                using var db = new VPContext();

                // Get current user's employee info
                var currentEmployee = StaticFunctions.GetCurrentEmployee();
                int? employeeId = currentEmployee?.EmployeeId;
                string mechanicName = currentEmployee?.FullName ?? "Unknown";

                // Get work order items assigned to this mechanic (not completed, from last 90 days)
                var ninetyDaysAgo = DateTime.Now.AddDays(-90);

                var items = db.EMWorkOrderItems
                    .Where(i => i.MechanicId == employeeId
                             && i.DateCreated >= ninetyDaysAgo)
                    .OrderByDescending(i => i.DateCreated)
                    .ToList();

                var model = new portal.Models.Views.SM.Request.MechanicDashboardViewModel(items, mechanicName);
                return View("~/Views/SM/Request/Mechanic/Index.cshtml", model);
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message + "<br><br>Stack: " + ex.StackTrace, "text/html");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CompleteWorkOrderItem(byte emco, string workOrderId, short woItem, string notes)
        {
            try
            {
                using var db = new VPContext();

                // Find the work order item
                var item = db.EMWorkOrderItems.FirstOrDefault(i =>
                    i.EMCo == emco && i.WorkOrderId == workOrderId && i.WOItem == woItem);

                if (item == null)
                {
                    return Json(new { success = false, error = "Work order item not found" });
                }

                // Mark as complete
                item.DateCompl = DateTime.Now;

                // Append completion notes if provided
                if (!string.IsNullOrEmpty(notes))
                {
                    item.Notes = string.IsNullOrEmpty(item.Notes)
                        ? notes
                        : item.Notes + "\n\n[Completion Notes " + DateTime.Now.ToShortDateString() + "]: " + notes;
                }

                // Update linked Service Request Line status if exists
                var srLine = db.SMRequestLines.FirstOrDefault(l =>
                    l.EMCo == emco && l.WorkOrderId == workOrderId && l.WOItemId == woItem);

                if (srLine != null)
                {
                    srLine.Status = DB.SMRequestLineStatusEnum.Completed;
                }

                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        #endregion


        // =============================================================================
        // ADD THIS METHOD TO: Controllers/View/SM/Request/ServiceRequestController.cs
        // (Replace or add alongside the existing CompleteWorkOrderItem method)
        // =============================================================================

        /// <summary>
        /// Complete work order with full documentation
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CompleteWorkOrderWithDetails(
            byte emco,
            string workOrderId,
            short woItem,
            char completionStatus,
            string repairsCompleted,
            string partsReplaced,
            string partsBackordered,
            string backorderETA,
            string temporaryRepairs,
            string recommendedFutureWork)
        {
            try
            {
                using var db = new VPContext();

                var currentEmployee = StaticFunctions.GetCurrentEmployee();
                int? mechanicId = currentEmployee?.EmployeeId;

                // Save work performed documentation
                var workPerformed = new WorkPerformedRecord
                {
                    EMCo = emco,
                    WorkOrderId = workOrderId,
                    WOItem = woItem,
                    CompletionStatus = completionStatus,
                    RepairsCompleted = repairsCompleted,
                    PartsReplaced = partsReplaced,
                    PartsBackordered = partsBackordered,
                    BackorderETA = string.IsNullOrEmpty(backorderETA) ? null : (DateTime?)DateTime.Parse(backorderETA),
                    TemporaryRepairs = temporaryRepairs,
                    RecommendedFutureWork = recommendedFutureWork,
                    CompletedBy = mechanicId
                };

                EMWorkPerformedData.SaveWorkPerformed(workPerformed);

                // Only mark work order as complete if status is "C" (Completed)
                if (completionStatus == 'C')
                {
                    // Update Work Order Item
                    var woItemRecord = db.EMWorkOrderItems.FirstOrDefault(i =>
                        i.EMCo == emco && i.WorkOrderId == workOrderId && i.WOItem == woItem);

                    if (woItemRecord != null)
                    {
                        woItemRecord.DateCompl = DateTime.Now;

                        // Append notes
                        var notes = $"[Completed {DateTime.Now:MM/dd/yyyy}]\n{repairsCompleted}";
                        if (!string.IsNullOrEmpty(partsReplaced))
                            notes += $"\nParts: {partsReplaced}";
                        if (!string.IsNullOrEmpty(recommendedFutureWork))
                            notes += $"\nRecommended: {recommendedFutureWork}";

                        woItemRecord.Notes = string.IsNullOrEmpty(woItemRecord.Notes)
                            ? notes
                            : woItemRecord.Notes + "\n\n" + notes;
                    }

                    // Update linked Service Request Line status
                    var srLine = db.SMRequestLines.FirstOrDefault(l =>
                        l.EMCo == emco && l.WorkOrderId == workOrderId && l.WOItemId == woItem);

                    if (srLine != null)
                    {
                        srLine.Status = DB.SMRequestLineStatusEnum.Completed;
                    }

                    db.SaveChanges();
                }
                else
                {
                    // For partial/follow-up, just save notes but don't complete
                    var woItemRecord = db.EMWorkOrderItems.FirstOrDefault(i =>
                        i.EMCo == emco && i.WorkOrderId == workOrderId && i.WOItem == woItem);

                    if (woItemRecord != null)
                    {
                        var statusText = completionStatus == 'P' ? "Partially Completed" : "Requires Follow-up";
                        var notes = $"[{statusText} {DateTime.Now:MM/dd/yyyy}]\n{repairsCompleted}";
                        if (!string.IsNullOrEmpty(partsBackordered))
                            notes += $"\nBackordered: {partsBackordered}";
                        if (!string.IsNullOrEmpty(temporaryRepairs))
                            notes += $"\nTemp Repair: {temporaryRepairs}";

                        woItemRecord.Notes = string.IsNullOrEmpty(woItemRecord.Notes)
                            ? notes
                            : woItemRecord.Notes + "\n\n" + notes;
                    }

                    db.SaveChanges();
                }

                return Json(new { success = true, status = completionStatus });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        #region Cost Entry

        /// <summary>
        /// Cost Entry page for a work order
        /// </summary>
        [HttpGet]
        [Route("Service/CostEntry/{emco}/{workOrderId}")]
        public ActionResult CostEntry(byte emco, string workOrderId, short woItem = 1)
        {
            try
            {
                using var db = new VPContext();

                var wo = db.EMWorkOrders.FirstOrDefault(w => w.EMCo == emco && w.WorkOrderId == workOrderId);
                if (wo == null)
                    return HttpNotFound("Work Order not found");

                var woItemRecord = db.EMWorkOrderItems.FirstOrDefault(w => w.EMCo == emco && w.WorkOrderId == workOrderId && w.WOItem == woItem);

                ViewBag.EMCo = emco;
                ViewBag.WorkOrderId = workOrderId;
                ViewBag.WOItem = woItem;
                ViewBag.EquipmentId = woItemRecord?.EquipmentId ?? wo.EquipmentId;
                ViewBag.EquipmentDescription = woItemRecord?.Equipment?.Description ?? wo.Equipment?.Description;
                ViewBag.WorkOrderDescription = woItemRecord?.Description ?? wo.Description;

                ViewBag.Labor = EMCostEntryData.GetLabor(emco, workOrderId, woItem);
                ViewBag.Parts = EMCostEntryData.GetParts(emco, workOrderId, woItem);
                ViewBag.OtherCosts = EMCostEntryData.GetOtherCosts(emco, workOrderId, woItem);
                ViewBag.Summary = EMCostEntryData.GetCostSummary(emco, workOrderId, woItem);

                return View("~/Views/EM/CostEntry/Index.cshtml");
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Get cost summary as JSON
        /// </summary>
        [HttpGet]
        public JsonResult GetCostSummary(byte emco, string workOrderId, short woItem = 1)
        {
            var summary = EMCostEntryData.GetCostSummary(emco, workOrderId, woItem);
            return Json(summary, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Add labor entry
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddLabor(byte emco, string workOrderId, short woItem, string employeeName, DateTime workDate, decimal hours, decimal hourlyRate, string description)
        {
            try
            {
                var entry = new LaborEntry
                {
                    EmployeeName = employeeName,
                    WorkDate = workDate,
                    Hours = hours,
                    HourlyRate = hourlyRate,
                    Description = description
                };

                var enteredBy = StaticFunctions.GetUserId();
                var id = EMCostEntryData.AddLabor(emco, workOrderId, woItem, entry, enteredBy);
                var summary = EMCostEntryData.GetCostSummary(emco, workOrderId, woItem);

                return Json(new { success = true, laborId = id, summary });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Delete labor entry
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteLabor(int laborId, byte emco, string workOrderId, short woItem)
        {
            try
            {
                EMCostEntryData.DeleteLabor(laborId);
                var summary = EMCostEntryData.GetCostSummary(emco, workOrderId, woItem);
                return Json(new { success = true, summary });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Add part entry
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddPart(byte emco, string workOrderId, short woItem, string partNumber, string partDescription, decimal quantity, decimal unitCost, string vendor)
        {
            try
            {
                var entry = new PartEntry
                {
                    PartNumber = partNumber,
                    PartDescription = partDescription,
                    Quantity = quantity,
                    UnitCost = unitCost,
                    Vendor = vendor
                };

                var enteredBy = StaticFunctions.GetUserId();
                var id = EMCostEntryData.AddPart(emco, workOrderId, woItem, entry, enteredBy);
                var summary = EMCostEntryData.GetCostSummary(emco, workOrderId, woItem);

                return Json(new { success = true, partId = id, summary });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Delete part entry
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeletePart(int partId, byte emco, string workOrderId, short woItem)
        {
            try
            {
                EMCostEntryData.DeletePart(partId);
                var summary = EMCostEntryData.GetCostSummary(emco, workOrderId, woItem);
                return Json(new { success = true, summary });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Add other cost entry
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddOtherCost(byte emco, string workOrderId, short woItem, char costType, string description, decimal amount, string vendor, string invoiceNumber)
        {
            try
            {
                var entry = new OtherCostEntry
                {
                    CostType = costType,
                    Description = description,
                    Amount = amount,
                    Vendor = vendor,
                    InvoiceNumber = invoiceNumber
                };

                var enteredBy = StaticFunctions.GetUserId();
                var id = EMCostEntryData.AddOtherCost(emco, workOrderId, woItem, entry, enteredBy);
                var summary = EMCostEntryData.GetCostSummary(emco, workOrderId, woItem);

                return Json(new { success = true, otherCostId = id, summary });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Delete other cost entry
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteOtherCost(int otherCostId, byte emco, string workOrderId, short woItem)
        {
            try
            {
                EMCostEntryData.DeleteOtherCost(otherCostId);
                var summary = EMCostEntryData.GetCostSummary(emco, workOrderId, woItem);
                return Json(new { success = true, summary });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        #endregion
    }
}