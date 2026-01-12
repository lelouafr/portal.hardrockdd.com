using Newtonsoft.Json;
using portal.Code.Data.VP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;

namespace portal.Controllers.View.HR.Request.Position
{
    public class HRPositionRequestFormController : BaseController
    {
        #region Create Request
        [HttpGet]
        public JsonResult ValidateCreate(Models.Views.HR.Request.Position.CreateRequestViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult Create()
        {
            using var db = new VPEntities();
            var results = new Models.Views.HR.Request.Position.CreateRequestViewModel(db);
            return PartialView("../HR/Request/Position/Create/Index", results);
        }
        [HttpGet]
        public PartialViewResult CreateForm()
        {
            using var db = new VPEntities();
            var results = new Models.Views.HR.Request.Position.CreateRequestViewModel(db);
            return PartialView("../HR/Request/Position/Create/Form", results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(Models.Views.HR.Request.Position.CreateRequestViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);
            if (ModelState.IsValid)
            {
                using var db = new VPEntities();
                var request = db.GetCurrentCompany().HRCompanyParm.AddPositionRequest(model);
                //request.HRRef = model.HRRef;
                //request.Comments = model.Comments;
                request.Status = HRPositionRequestStatusEnum.Submitted;

                db.SaveChanges(ModelState);

                return Json(new { success = ModelState.IsValidJson() });
            }

            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region List
        [HttpGet]
        [Route("HR/Position/Requests")]
        public ViewResult AllIndex()
        {
            using var db = new VPEntities();
            var results = new Models.Views.HR.Request.Position.RequestListViewModel(db);

            return View("../HR/Request/Position/Reports/All/Index", results);
        }

        [HttpGet]
        public PartialViewResult AllTable()
        {
            using var db = new VPEntities();
            var results = new Models.Views.HR.Request.Position.RequestListViewModel(db);

            return PartialView("../HR/Request/Position/Reports/All/Table", results);
        }


        [HttpGet]
        [Route("HR/Positions/Request/Open")]
        public ViewResult OpenIndex()
        {
            using var db = new VPEntities();
            var list = db.HRPositionRequests.Where(f => f.tStatusId == (int)HRPositionRequestStatusEnum.Submitted ||
                                                        f.tStatusId == (int)HRPositionRequestStatusEnum.HRApproved ||
                                                        f.tStatusId == (int)HRPositionRequestStatusEnum.ManagementReviewed ||
                                                        f.tStatusId == (int)HRPositionRequestStatusEnum.HRReview 
                                                        ).ToList();
            var results = new Models.Views.HR.Request.Position.RequestListViewModel(list);

            return View("../HR/Request/Position/Reports/Open/Index", results);
        }

        [HttpGet]
        public PartialViewResult OpenTable()
        {
            using var db = new VPEntities();
            var list = db.HRPositionRequests.Where(f => f.tStatusId == (int)HRPositionRequestStatusEnum.Submitted ||
                                                        f.tStatusId == (int)HRPositionRequestStatusEnum.HRApproved ||
                                                        f.tStatusId == (int)HRPositionRequestStatusEnum.ManagementReviewed ||
                                                        f.tStatusId == (int)HRPositionRequestStatusEnum.HRReview
                                                        ).ToList();
            var results = new Models.Views.HR.Request.Position.RequestListViewModel(list);

            return PartialView("../HR/Request/Position/Reports/Open/Table", results);
        }


        [HttpGet]
        public ViewResult UserIndex()
        {
            using var db = new VPEntities();
            var userId = db.CurrentUserId;
            var request = db.HRPositionRequests.Where(f => f.WorkFlow.Sequances.Any(seq => seq.Active && seq.AssignedUsers.Any(f => f.AssignedTo == userId))).ToList();
            var results = new Models.Views.HR.Request.Position.RequestListViewModel(request);

            return View("../HR/Request/Position/Reports/User/Index", results);
        }

        [HttpGet]
        public ViewResult UserTable()
        {
            using var db = new VPEntities();
            var userId = db.CurrentUserId;
            var request = db.HRPositionRequests.Where(f => f.WorkFlow.Sequances.Any(seq => seq.Active && seq.AssignedUsers.Any(f => f.AssignedTo == userId))).ToList();
            var results = new Models.Views.HR.Request.Position.RequestListViewModel(request);

            return View("../HR/Request/Position/Reports/User/Table", results);
        }
        #endregion

        #region Main form Index
        [HttpGet]
        [Route("HR/Position/Request/Form/{requestId}-{hrco}")]
        public ViewResult Index(byte hrco, int requestId)
        {
            using var db = new VPEntities();
            var request = db.HRPositionRequests.FirstOrDefault(f => f.HRCo == hrco && f.RequestId == requestId);
            var model = new Models.Views.HR.Request.Position.RequestFormViewModel(request);

            return View("../HR/Request/Position/Form/Index", model);
        }

        [HttpGet]
        [Route("HR/Position/Request/Popup/{requestId}-{hrco}")]
        public ActionResult PopupForm(byte hrco, int requestId)
        {
            using var db = new VPEntities();
            var request = db.HRPositionRequests.FirstOrDefault(f => f.HRCo == hrco && f.RequestId == requestId);
            var model = new Models.Views.HR.Request.Position.RequestFormViewModel(request);

            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";
            return PartialView("../HR/Request/Position/Form/Index", model);
        }

        #endregion

        #region Request Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(byte hrco, int requestId, int statusId)
        {
            using var db = new Code.Data.VP.VPEntities();
            var request = db.HRPositionRequests.FirstOrDefault(f => f.HRCo == hrco && f.RequestId == requestId);

            if (request != null)
            {
                request.Status = (HRPositionRequestStatusEnum)statusId;
                if (request.Status != HRPositionRequestStatusEnum.Hire)
                {
                    db.SaveChanges(ModelState);

                }
                else
                {
                    db.SaveChanges(ModelState);
                }
            }

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }


        #endregion

        #region Main Form

        [HttpGet]
        public PartialViewResult PositionPanel(byte hrco, int requestId)
        {
            using var db = new VPEntities();
            var request = db.HRPositionRequests.FirstOrDefault(f => f.HRCo == hrco && f.RequestId == requestId);
            var model = new Models.Views.HR.Request.Position.RequestFormViewModel(request);
            return PartialView("../HR/Request/Position/Form/Panel", model);
        }

        [HttpGet]
        public PartialViewResult PositionForm(byte hrco, int requestId)
        {
            using var db = new VPEntities();
            var request = db.HRPositionRequests.FirstOrDefault(f => f.HRCo == hrco && f.RequestId == requestId);
            var model = new Models.Views.HR.Request.Position.RequestFormViewModel(request);
            return PartialView("../HR/Request/Position/Form/Form", model);
        }

        [HttpGet]
        public PartialViewResult PositionInfoForm(byte hrco, int requestId)
        {
            using var db = new VPEntities();
            var request = db.HRPositionRequests.FirstOrDefault(f => f.HRCo == hrco && f.RequestId == requestId);
            var model = new Models.Views.HR.Request.Position.RequestViewModel(request);
            return PartialView("../HR/Request/Position/Form/Info/Form", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PositionUpdate(Models.Views.HR.Request.Position.RequestViewModel model)
        {
            using var db = new VPEntities();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult PositionValidate(Models.Views.HR.Request.Position.RequestViewModel model)
        {
            using var db = new VPEntities();
            var request = db.HRPositionRequests.FirstOrDefault(f => f.HRCo == model.HRCo && f.RequestId == model.RequestId);
            var form = new Models.Views.HR.Request.Position.RequestViewModel(request);
            this.ValidateModel(form);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }


        #endregion


        #region HR/PR Info Update


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PRInfoUpdate(Models.Views.HR.Request.Position.PayroleViewModel model)
        {
            using var db = new VPEntities();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult PRInfoValidate(Models.Views.HR.Request.Position.PayroleViewModel model)
        {
            using var db = new VPEntities();
            var request = db.HRPositionRequests.FirstOrDefault(f => f.HRCo == model.HRCo && f.RequestId == model.RequestId);
            var form = new Models.Views.HR.Request.Position.PayroleViewModel(request);
            this.ValidateModel(form);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Applicant List
        [HttpGet]
        public ActionResult ApplicantTable(byte hrco, int requestId)
        {
            using var db = new VPEntities();
            var request = db.HRPositionRequests.FirstOrDefault(f => f.HRCo == hrco && f.RequestId == requestId);
            var result = new Models.Views.HR.Request.Position.ApplicationListViewModel(request);

            return PartialView("../HR/Applicant/Form/Application/Form/Applicants/Table", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApplicantUpdate(Models.Views.HR.Request.Position.ApplicationViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            using var db = new VPEntities();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult ApplicantValidate(Models.Views.HR.Request.Position.ApplicationViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}