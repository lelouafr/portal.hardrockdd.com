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
    public class TermRequestController : portal.Controllers.BaseController
    {
        #region Create Request
        [HttpGet]
        public JsonResult ValidateCreate(Models.TermRequest.CreateTermRequestViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult Create()
        {
            using var db = new VPContext();
            var results = new Models.TermRequest.CreateTermRequestViewModel(db);
            return PartialView("Create/Index", results);
        }
        [HttpGet]
        public PartialViewResult CreateForm()
        {
            using var db = new VPContext();
            var results = new Models.TermRequest.CreateTermRequestViewModel(db);
            return PartialView("Create/_Form", results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(Models.TermRequest.CreateTermRequestViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);
            if (ModelState.IsValid)
            {
                using var db = new VPContext();
                var request = db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo).HRCompanyParm.AddTermRequest(model.ToDBObject());
                request.HRRef = model.HRRef;
                request.Comments = model.Comments;
                request.Status = DB.HRTermRequestStatusEnum.Submitted;

                db.SaveChanges(ModelState);

                return Json(new { success = ModelState.IsValidJson() });
            }

            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region List
        [HttpGet]
        [Route("Terms")]
        public ViewResult AllIndex()
        {
            using var db = new VPContext();
            var results = new Models.TermRequest.TermRequestListViewModel(db);

            return View("List/All/Index", results);
        }

        [HttpGet]
        public PartialViewResult AllTable()
        {
            using var db = new VPContext();
            var results = new Models.TermRequest.TermRequestListViewModel(db);

            return PartialView("List/All/_Table", results);
        }


        [HttpGet]
        [Route("Term/Assigned")]
        public ViewResult UserIndex()
        {
            using var db = new VPContext();
            var userId = db.CurrentUserId;
            var request = db.HRTermRequests.Where(f => f.WorkFlow.Sequences.Any(seq => seq.Active && seq.AssignedUsers.Any(f => f.AssignedTo == userId))).ToList();
            var results = new Models.TermRequest.TermRequestListViewModel(request);

            return View("List/User/Index", results);
        }

        [HttpGet]
        [Route("Term/Supervisor")]
        public ViewResult SupervisorIndex()
        {
            using var db = new VPContext();
            var results = new Models.TermRequest.TermRequestListViewModel(db.GetCurrentHREmployee());

            return View("List/Supervisor/Index", results);
        }

        [HttpGet]
        [Route("Term/Open")]
        public ViewResult OpenIndex()
        {
            using var db = new VPContext();
            var curEmp = StaticFunctions.GetCurrentHREmployee();
            var emp = db.HRResources.FirstOrDefault(f => f.HRCo == curEmp.HRCo && f.HRRef == curEmp.HRRef);
            var results = new Models.TermRequest.TermRequestListViewModel(emp);

            return View("List/Open/Index", results);
        }
        #endregion

        #region Main form Index
        [HttpGet]
        [Route("HR/Term/_Form/{requestId}-{hrco}")]
        public ViewResult Index(byte hrco, int requestId)
        {
            using var db = new VPContext();
            var request = db.HRTermRequests.FirstOrDefault(f => f.HRCo == hrco && f.RequestId == requestId);
            var model = new Models.TermRequest.TermRequestFormViewModel(request);

            return View("Form/Index", model);
        }

        [HttpGet]
        [Route("HR/Term/Popup/{requestId}-{hrco}")]
        public ActionResult PopupForm(byte hrco, int requestId)
        {
            using var db = new VPContext();
            var request = db.HRTermRequests.FirstOrDefault(f => f.HRCo == hrco && f.RequestId == requestId);
            var model = new Models.TermRequest.TermRequestFormViewModel(request);

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
            var request = db.HRTermRequests.FirstOrDefault(f => f.HRCo == hrco && f.RequestId == requestId);

            if (request != null)
            {
                request.Status = (DB.HRTermRequestStatusEnum)statusId;
                db.SaveChanges(ModelState);
            }

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }


        #endregion

        #region Main Form

        [HttpGet]
        public PartialViewResult TermPanel(byte hrco, int requestId)
        {
            using var db = new VPContext();
            var request = db.HRTermRequests.FirstOrDefault(f => f.HRCo == hrco && f.RequestId == requestId);
            var model = new Models.TermRequest.TermRequestFormViewModel(request);
            return PartialView("Form/_Panel", model);
        }

        [HttpGet]
        public PartialViewResult TermForm(byte hrco, int requestId)
        {
            using var db = new VPContext();
            var request = db.HRTermRequests.FirstOrDefault(f => f.HRCo == hrco && f.RequestId == requestId);
            var model = new Models.TermRequest.TermRequestFormViewModel(request);
            return PartialView("Form/_Form", model);
        }

        [HttpGet]
        public PartialViewResult TermInfoForm(byte hrco, int requestId)
        {
            using var db = new VPContext();
            var request = db.HRTermRequests.FirstOrDefault(f => f.HRCo == hrco && f.RequestId == requestId);
            var model = new Models.TermRequest.TermRequestViewModel(request);
            return PartialView("Form/Info/_Form", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TermUpdate(Models.TermRequest.TermRequestViewModel model)
        {
            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult TermValidate(Models.TermRequest.TermRequestViewModel model)
        {
            using var db = new VPContext();
            var request = db.HRTermRequests.FirstOrDefault(f => f.HRCo == model.HRCo && f.RequestId == model.RequestId);
            var form = new Models.TermRequest.TermRequestViewModel(request);
            this.ValidateModel(form);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}