using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;

namespace portal.Areas.HumanResource.Controllers
{
    [RouteArea("HumanResource")]
    public class PositionRequestController : portal.Controllers.BaseController
    {
        #region Create Request
        [HttpGet]
        public JsonResult ValidateCreate(Models.PositionRequest.CreateRequestViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult Create()
        {
            using var db = new VPContext();
            var results = new Models.PositionRequest.CreateRequestViewModel(db);
            return PartialView("Create/_Index", results);
        }
        [HttpGet]
        public PartialViewResult CreateForm()
        {
            using var db = new VPContext();
            var results = new Models.PositionRequest.CreateRequestViewModel(db);
            return PartialView("Create/_Form", results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(Models.PositionRequest.CreateRequestViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);
            if (ModelState.IsValid)
            {
                using var db = new VPContext();
                var request = db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo).HRCompanyParm.AddPositionRequest(model.ToDBObject());
                request.Status = DB.HRPositionRequestStatusEnum.Submitted;
                db.SaveChanges(ModelState);

                return Json(new { success = ModelState.IsValidJson() });
            }

            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region List
        [HttpGet]
        [Route("Position/Requests")]
        public ViewResult AllIndex()
        {
            using var db = new VPContext();
            var results = new Models.PositionRequest.RequestListViewModel(db);

            return View("List/All/Index", results);
        }

        [HttpGet]
        public PartialViewResult AllTable()
        {
            using var db = new VPContext();
            var results = new Models.PositionRequest.RequestListViewModel(db);

            return PartialView("List/All/_Table", results);
        }


        [HttpGet]
        [Route("Position/Request/Open")]
        public ViewResult OpenIndex()
        {
            using var db = new VPContext();
            var list = db.HRPositionRequests.Where(f => f.tStatusId == (int)DB.HRPositionRequestStatusEnum.Submitted ||
                                                        f.tStatusId == (int)DB.HRPositionRequestStatusEnum.HRApproved ||
                                                        f.tStatusId == (int)DB.HRPositionRequestStatusEnum.ManagementReviewed ||
                                                        f.tStatusId == (int)DB.HRPositionRequestStatusEnum.HRReview 
                                                        ).ToList();
            var results = new Models.PositionRequest.RequestListViewModel(list);

            return View("List/Open/Index", results);
        }

        [HttpGet]
        public PartialViewResult OpenTable()
        {
            using var db = new VPContext();
            var list = db.HRPositionRequests.Where(f => f.tStatusId == (int)DB.HRPositionRequestStatusEnum.Submitted ||
                                                        f.tStatusId == (int)DB.HRPositionRequestStatusEnum.HRApproved ||
                                                        f.tStatusId == (int)DB.HRPositionRequestStatusEnum.ManagementReviewed ||
                                                        f.tStatusId == (int)DB.HRPositionRequestStatusEnum.HRReview
                                                        ).ToList();
            var results = new Models.PositionRequest.RequestListViewModel(list);

            return PartialView("List/Open/_Table", results);
        }


        [HttpGet]
        [Route("Position/Request/User")]
        public ViewResult UserIndex()
        {
            using var db = new VPContext();
            var userId = db.CurrentUserId;
            var request = db.HRPositionRequests.Where(f => f.WorkFlow.Sequences.Any(seq => seq.Active && seq.AssignedUsers.Any(f => f.AssignedTo == userId))).ToList();
            var results = new Models.PositionRequest.RequestListViewModel(request);

            return View("List/User/Index", results);
        }

        [HttpGet]
        public PartialViewResult UserTable()
        {
            using var db = new VPContext();
            var userId = db.CurrentUserId;
            var request = db.HRPositionRequests.Where(f => f.WorkFlow.Sequences.Any(seq => seq.Active && seq.AssignedUsers.Any(f => f.AssignedTo == userId))).ToList();
            var results = new Models.PositionRequest.RequestListViewModel(request);

            return PartialView("List/User/_Table", results);
        }
        #endregion

        #region Main form Index
        [HttpGet]
        [Route("Position/Request/Form/{requestId}-{hrco}")]
        public ViewResult Index(byte hrco, int requestId)
        {
            using var db = new VPContext();
            var request = db.HRPositionRequests.FirstOrDefault(f => f.HRCo == hrco && f.RequestId == requestId);
            var model = new Models.PositionRequest.RequestFormViewModel(request);

            return View("Form/Index", model);
        }

        [HttpGet]
        [Route("Position/Request/Popup/{requestId}-{hrco}")]
        public ActionResult PopupForm(byte hrco, int requestId)
        {
            using var db = new VPContext();
            var request = db.HRPositionRequests.FirstOrDefault(f => f.HRCo == hrco && f.RequestId == requestId);
            var model = new Models.PositionRequest.RequestFormViewModel(request);

            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";
            return PartialView("Form/Index", model);
        }

        #endregion

        #region Request Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(byte hrco, int requestId, int statusId)
        {
            using var db = new VPContext();
            var request = db.HRPositionRequests.FirstOrDefault(f => f.HRCo == hrco && f.RequestId == requestId);

            var model = new Models.PositionRequest.RequestFormViewModel(request);
            if (request != null)
            {
                var status = (DB.HRPositionRequestStatusEnum)statusId;

                if (status != DB.HRPositionRequestStatusEnum.Hire)
                {
                    request.Status = status;
                    db.SaveChanges(ModelState);
                }
                else
                {
                    model.ValidateNewHire(ModelState);
                    if (ModelState.IsValid)
                    {
                        request.Status = status;
                        db.SaveChanges(ModelState);
                    }
                }
            }
            model = new Models.PositionRequest.RequestFormViewModel(request);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }


        #endregion

        #region Main Form

        [HttpGet]
        public PartialViewResult Panel(byte hrco, int requestId)
        {
            using var db = new VPContext();
            var request = db.HRPositionRequests.FirstOrDefault(f => f.HRCo == hrco && f.RequestId == requestId);
            var model = new Models.PositionRequest.RequestFormViewModel(request);
            return PartialView("Form/_Panel", model);
        }

        [HttpGet]
        public PartialViewResult Form(byte hrco, int requestId)
        {
            using var db = new VPContext();
            var request = db.HRPositionRequests.FirstOrDefault(f => f.HRCo == hrco && f.RequestId == requestId);
            var model = new Models.PositionRequest.RequestFormViewModel(request);
            return PartialView("Form/_Form", model);
        }

        [HttpGet]
        public PartialViewResult InfoForm(byte hrco, int requestId)
        {
            using var db = new VPContext();
            var request = db.HRPositionRequests.FirstOrDefault(f => f.HRCo == hrco && f.RequestId == requestId);
            var model = new Models.PositionRequest.RequestViewModel(request);
            return PartialView("Form/Info/_Form", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(Models.PositionRequest.RequestViewModel model)
        {
            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult Validate(Models.PositionRequest.RequestViewModel model)
        {
            using var db = new VPContext();
            var request = db.HRPositionRequests.FirstOrDefault(f => f.HRCo == model.HRCo && f.RequestId == model.RequestId);
            var form = new Models.PositionRequest.RequestViewModel(request);
            this.ValidateModel(form);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region HR/PR Info Update


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PRInfoUpdate(Models.PositionRequest.PayroleViewModel model)
        {
            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult PRInfoValidate(Models.PositionRequest.PayroleViewModel model)
        {
            using var db = new VPContext();
            var request = db.HRPositionRequests.FirstOrDefault(f => f.HRCo == model.HRCo && f.RequestId == model.RequestId);
            var form = new Models.PositionRequest.PayroleViewModel(request);
            this.ValidateModel(form);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Applicant List
        [HttpGet]
        public ActionResult ApplicantTable(byte hrco, int requestId)
        {
            using var db = new VPContext();
            var request = db.HRPositionRequests.FirstOrDefault(f => f.HRCo == hrco && f.RequestId == requestId);
            var result = new Models.PositionRequest.ApplicationListViewModel(request);

            return PartialView("Form/Applicants/_Table", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApplicantUpdate(Models.PositionRequest.ApplicationViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult ApplicantValidate(Models.PositionRequest.ApplicationViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}