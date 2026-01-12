using Newtonsoft.Json;
using portal.Code.Data.VP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;

namespace portal.Controllers.View.HR.Request.Term
{
    public class HRPositionController : BaseController
    {
        #region Create Request
        [HttpGet]
        public JsonResult ValidateCreate(Models.Views.HR.Position.CreatePositionViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult Create()
        {
            using var db = new VPEntities();
            var results = new Models.Views.HR.Position.CreatePositionViewModel(db);
            return PartialView("../HR/Position/Create/Index", results);
        }
        [HttpGet]
        public PartialViewResult CreateForm()
        {
            using var db = new VPEntities();
            var results = new Models.Views.HR.Position.CreatePositionViewModel(db);
            return PartialView("../HR/Position/Create/Form", results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(Models.Views.HR.Position.CreatePositionViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" }, JsonRequestBehavior.AllowGet);

            ModelState.Clear();
            TryValidateModel(model);
            if (ModelState.IsValid)
            {
                using var db = new VPEntities();
                var position = model.Create(db, ModelState);

                return Json(new { success = ModelState.IsValidJson() });
            }

            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region List
        [HttpGet]
        [Route("HR/Positions")]
        public ViewResult PositionListIndex()
        {
            using var db = new VPEntities();
            var hrComp = db.GetCurrentCompany().HRCompanyParm;
            var results = new Models.Views.HR.Position.PositionListViewModel(hrComp);

            return View("../HR/Position/Reports/Index", results);
        }

        [HttpGet]
        public PartialViewResult PositionListTable()
        {
            using var db = new VPEntities();
            var hrComp = db.GetCurrentCompany().HRCompanyParm;
            var results = new Models.Views.HR.Position.PositionListViewModel(hrComp);

            return PartialView("../HR/Position/Reports/Table", results);
        }


        #endregion

        #region Main Form Index
        [HttpGet]
        [Route("HR/Position/Form/{positionCodeId}-{hrco}")]
        public ViewResult Index(byte hrco, string positionCodeId)
        {
            using var db = new VPEntities();
            var position = db.HRPositions.FirstOrDefault(f => f.HRCo == hrco && f.PositionCodeId == positionCodeId);
            var model = new Models.Views.HR.Position.PositionFormViewModel(position);

            return View("../HR/Position/Form/Index", model);
        }

        [HttpGet]
        [Route("HR/Position/Popup/{positionCodeId}-{hrco}")]
        public ActionResult PopupForm(byte hrco, string positionCodeId)
        {
            using var db = new VPEntities();
            var position = db.HRPositions.FirstOrDefault(f => f.HRCo == hrco && f.PositionCodeId == positionCodeId);
            var model = new Models.Views.HR.Position.PositionFormViewModel(position);

            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";
            return View("../HR/Position/Form/Index", model);
        }
        #endregion

        #region Main Form

        [HttpGet]
        public PartialViewResult PositionPanel(byte hrco, string positionCodeId)
        {
            using var db = new VPEntities();
            var position = db.HRPositions.FirstOrDefault(f => f.HRCo == hrco && f.PositionCodeId == positionCodeId);
            var model = new Models.Views.HR.Position.PositionFormViewModel(position);
            return PartialView("../HR/Position/Form/Panel", model);
        }

        [HttpGet]
        public PartialViewResult PositionForm(byte hrco, string positionCodeId)
        {
            using var db = new VPEntities();
            var position = db.HRPositions.FirstOrDefault(f => f.HRCo == hrco && f.PositionCodeId == positionCodeId);
            var model = new Models.Views.HR.Position.PositionFormViewModel(position);
            return PartialView("../HR/Position/Form/Form", model);
        }

        [HttpGet]
        public PartialViewResult PositionInfoForm(byte hrco, string positionCodeId)
        {
            using var db = new VPEntities();
            var position = db.HRPositions.FirstOrDefault(f => f.HRCo == hrco && f.PositionCodeId == positionCodeId);
            var model = new Models.Views.HR.Position.PositionViewModel(position);
            return PartialView("../HR/Position/Form/Info/Form", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PositionUpdate(Models.Views.HR.Position.PositionViewModel model)
        {
            if (model == null)
                return Json(new { success = "false", errorModel = ModelState.ModelErrors() });

            using var db = new VPEntities();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult PositionValidate(Models.Views.HR.Position.PositionViewModel model)
        {
            using var db = new VPEntities();
            var position = db.HRPositions.FirstOrDefault(f => f.HRCo == model.HRCo && f.PositionCodeId == model.PositionCodeId);
            var form = new Models.Views.HR.Position.PositionViewModel(position);
            this.ValidateModel(form);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region New Hire Tasks
        [HttpGet]
        public ActionResult PositioNewHireTaskTable(byte hrco, string positionCodeId)
        {
            using var db = new VPEntities();
            var position = db.HRPositions.FirstOrDefault(f => f.HRCo == hrco && f.PositionCodeId == positionCodeId);
            var result = new Models.Views.HR.Position.PositionHireTaskListViewModel(position);

            return PartialView("../HR/Position/Form/NewHireTask/Table", result);
        }

        [HttpGet]
        public ActionResult PositioNewHireTaskAdd(byte hrco, string positionCodeId)
        {
            using var db = new VPEntities();
            var position = db.HRPositions.FirstOrDefault(f => f.HRCo == hrco && f.PositionCodeId == positionCodeId);
            var result = new Models.Views.HR.Position.PositionHireTaskViewModel(position.AddHireTask());
            db.BulkSaveChanges();

            return PartialView("../HR/Position/Form/NewHireTask/TableRow", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PositioNewHireTaskDelete(byte hrco, string positionCodeId, int seqId)
        {
            using var db = new VPEntities();
            var delObj = db.HRPositionTasks.FirstOrDefault(s => s.HRCo == hrco && s.PositionCodeId == positionCodeId && s.SeqId == seqId);
            if (delObj != null)
            {
                try
                {
                    db.HRPositionTasks.Remove(delObj);
                    db.BulkSaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Object could not be removed!");
                }
            }
            else
            {
                ModelState.AddModelError("", "Object doesn't exisit!");
            }
;
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PositioNewHireTaskUpdate(Models.Views.HR.Position.PositionHireTaskViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            using var db = new VPEntities();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult PositioNewHireTaskValidate(Models.Views.HR.Position.PositionHireTaskViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Employee List

        #endregion
    }
}