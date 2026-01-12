using Newtonsoft.Json;
using portal.Code.Data.VP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;

namespace portal.Controllers.View.HR.Request.Term
{
    public class HRTermRequestFormController : BaseController
    {
        #region Create Request
        [HttpGet]
        public JsonResult ValidateCreate(Models.Views.HR.Request.Term.CreateTermRequestViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult Create()
        {
            using var db = new VPEntities();
            var results = new Models.Views.HR.Request.Term.CreateTermRequestViewModel(db);
            return PartialView("../HR/Request/Term/Create/Index", results);
        }
        [HttpGet]
        public PartialViewResult CreateForm()
        {
            using var db = new VPEntities();
            var results = new Models.Views.HR.Request.Term.CreateTermRequestViewModel(db);
            return PartialView("../HR/Request/Term/Create/Form", results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(Models.Views.HR.Request.Term.CreateTermRequestViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);
            if (ModelState.IsValid)
            {
                using var db = new VPEntities();
                var request = db.GetCurrentCompany().HRCompanyParm.AddTermRequest(model);
                request.HRRef = model.HRRef;
                request.Comments = model.Comments;
                request.Status = HRTermRequestStatusEnum.Submitted;

                db.SaveChanges(ModelState);

                return Json(new { success = ModelState.IsValidJson() });
            }

            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region List
        [HttpGet]
        [Route("HR/Terms")]
        public ViewResult AllIndex()
        {
            using var db = new VPEntities();
            var results = new Models.Views.HR.Request.Term.TermRequestListViewModel(db);

            return View("../HR/Request/Term/Reports/All/Index", results);
        }

        [HttpGet]
        public PartialViewResult AllTable()
        {
            using var db = new VPEntities();
            var results = new Models.Views.HR.Request.Term.TermRequestListViewModel(db);

            return PartialView("../HR/Request/Term/Reports/All/Table", results);
        }


        [HttpGet]
        [Route("HR/Term/Assigned")]
        public ViewResult UserIndex()
        {
            using var db = new VPEntities();
            var userId = db.CurrentUserId;
            var request = db.HRTermRequests.Where(f => f.WorkFlow.Sequances.Any(seq => seq.Active && seq.AssignedUsers.Any(f => f.AssignedTo == userId))).ToList();
            var results = new Models.Views.HR.Request.Term.TermRequestListViewModel(request);

            return View("../HR/Request/Term/Reports/User/Index", results);
        }

        [HttpGet]
        [Route("HR/Term/Supervisor")]
        public ViewResult SupervisorIndex()
        {
            using var db = new VPEntities();
            var results = new Models.Views.HR.Request.Term.TermRequestListViewModel(db.GetCurrentHREmployee());

            return View("../HR/Request/Term/Reports/Supervisor/Index", results);
        }

        [HttpGet]
        [Route("HR/Term/Open")]
        public ViewResult OpenIndex()
        {
            using var db = new VPEntities();
            var curEmp = StaticFunctions.GetCurrentHREmployee();
            var emp = db.HRResources.FirstOrDefault(f => f.HRCo == curEmp.HRCo && f.HRRef == curEmp.HRRef);
            var results = new Models.Views.HR.Request.Term.TermRequestListViewModel(emp);

            return View("../HR/Request/Term/Reports/Open/Index", results);
        }
        #endregion

        #region Main form Index
        [HttpGet]
        [Route("HR/Term/Form/{requestId}-{hrco}")]
        public ViewResult Index(byte hrco, int requestId)
        {
            using var db = new VPEntities();
            var request = db.HRTermRequests.FirstOrDefault(f => f.HRCo == hrco && f.RequestId == requestId);
            var model = new Models.Views.HR.Request.Term.TermRequestFormViewModel(request);

            return View("../HR/Request/Term/Form/Index", model);
        }

        [HttpGet]
        [Route("HR/Term/Popup/{requestId}-{hrco}")]
        public ActionResult PopupForm(byte hrco, int requestId)
        {
            using var db = new VPEntities();
            var request = db.HRTermRequests.FirstOrDefault(f => f.HRCo == hrco && f.RequestId == requestId);
            var model = new Models.Views.HR.Request.Term.TermRequestFormViewModel(request);

            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";
            return PartialView("../HR/Request/Term/Form/Index", model);
        }
        #endregion

        #region Request Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(byte hrco, int requestId, int statusId)
        {
            using var db = new Code.Data.VP.VPEntities();
            var request = db.HRTermRequests.FirstOrDefault(f => f.HRCo == hrco && f.RequestId == requestId);

            if (request != null)
            {
                request.Status = (HRTermRequestStatusEnum)statusId;
                db.SaveChanges(ModelState);
            }

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }


        #endregion

        #region Main Form

        [HttpGet]
        public PartialViewResult TermPanel(byte hrco, int requestId)
        {
            using var db = new VPEntities();
            var request = db.HRTermRequests.FirstOrDefault(f => f.HRCo == hrco && f.RequestId == requestId);
            var model = new Models.Views.HR.Request.Term.TermRequestFormViewModel(request);
            return PartialView("../HR/Request/Term/Form/Panel", model);
        }

        [HttpGet]
        public PartialViewResult TermForm(byte hrco, int requestId)
        {
            using var db = new VPEntities();
            var request = db.HRTermRequests.FirstOrDefault(f => f.HRCo == hrco && f.RequestId == requestId);
            var model = new Models.Views.HR.Request.Term.TermRequestFormViewModel(request);
            return PartialView("../HR/Request/Term/Form/Form", model);
        }

        [HttpGet]
        public PartialViewResult TermInfoForm(byte hrco, int requestId)
        {
            using var db = new VPEntities();
            var request = db.HRTermRequests.FirstOrDefault(f => f.HRCo == hrco && f.RequestId == requestId);
            var model = new Models.Views.HR.Request.Term.TermRequestViewModel(request);
            return PartialView("../HR/Request/Term/Form/Info/Form", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TermUpdate(Models.Views.HR.Request.Term.TermRequestViewModel model)
        {
            using var db = new VPEntities();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult TermValidate(Models.Views.HR.Request.Term.TermRequestViewModel model)
        {
            using var db = new VPEntities();
            var request = db.HRTermRequests.FirstOrDefault(f => f.HRCo == model.HRCo && f.RequestId == model.RequestId);
            var form = new Models.Views.HR.Request.Term.TermRequestViewModel(request);
            this.ValidateModel(form);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}