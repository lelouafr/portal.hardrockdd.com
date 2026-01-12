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
            foreach (var item in entity.Lines.Where(f => f.Equipment == null).ToList())
            {
                entity.Lines.Remove(item);
            }
            entity.Status = (DB.SMRequestStatusEnum)gotoStatusId;
            db.SaveChanges(ModelState);
            var url = Url.Action("Index", "Home", new { Area = "" });
            if (ActionRedirect == "Reload")
            {
                url = Url.Action("ServiceRequestIndex", new { smco, requestId });
            }

            return Json(new { url, success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }
        #endregion

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
                    if (entity.tEquipmentId != model.EquipmentId)
                    {
                        var equipment = db.Equipments.FirstOrDefault(f => f.EquipmentId == entity.tEquipmentId);
                        entity.tEquipmentId = model.EquipmentId;
                        entity.Equipment = null;
                        entity.Equipment = equipment;
                        entity.ShopGroupId = equipment?.ShopGroup;
                    }
                    if (entity.tEquipmentId != null && entity.Equipment == null)
                    {
                        var equipment = db.Equipments.FirstOrDefault(f =>  f.EquipmentId == entity.tEquipmentId);
                        entity.EMCo = equipment.EMCo;
                        entity.Equipment = equipment;
                        entity.ShopGroupId = equipment.ShopGroup;
                    }
                    entity.RequestComments = model.RequestComments;
                    entity.IsEquipmentDisabled = model.IsEquipmentDisabled;

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
    }
}