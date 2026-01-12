using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.HumanResource.Controllers
{
    [RouteArea("HumanResource")]
    //[RoutePrefix("Position")]
    public class PositionController : portal.Controllers.BaseController
    {
        #region Create Request
        [HttpGet]
        public JsonResult ValidateCreate(Models.Position.CreateViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult Create()
        {
            using var db = new VPContext();
            var results = new Models.Position.CreateViewModel(db);
            return PartialView("Create/Index", results);
        }
        [HttpGet]
        public PartialViewResult CreateForm()
        {
            using var db = new VPContext();
            var results = new Models.Position.CreateViewModel(db);
            return PartialView("Create/_Form", results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(Models.Position.CreateViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" }, JsonRequestBehavior.AllowGet);

            ModelState.Clear();
            TryValidateModel(model);
            if (ModelState.IsValid)
            {
                using var db = new VPContext();
                var position = model.Create(db, ModelState);

                return Json(new { success = ModelState.IsValidJson() });
            }

            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region List are
        [HttpGet]
        [Route("Position")]
        public ViewResult PositionListIndex()
        {
            using var db = new VPContext();
            var hrComp = db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo).HRCompanyParm;
            var results = new Models.Position.PositionListViewModel(hrComp);

            return View("List/Index", results);
        }

        [HttpGet]
        public PartialViewResult PositionListTable()
        {
            using var db = new VPContext();
            var hrComp = db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo).HRCompanyParm;
            var results = new Models.Position.PositionListViewModel(hrComp);

            return PartialView("List/_Table", results);
        }


        #endregion

        #region Main Form Index
        [HttpGet]
        [Route("Form/{positionCodeId}-{hrco}")]
        public ViewResult Index(byte hrco, string positionCodeId)
        {
            using var db = new VPContext();
            var position = db.HRPositions.FirstOrDefault(f => f.HRCo == hrco && f.PositionCodeId == positionCodeId);
            var model = new Models.Position.FormViewModel(position);

            return View("Form/Index", model);
        }

        [HttpGet]
        [Route("Popup/{positionCodeId}-{hrco}")]
        public ActionResult PopupForm(byte hrco, string positionCodeId)
        {
            using var db = new VPContext();
            var position = db.HRPositions.FirstOrDefault(f => f.HRCo == hrco && f.PositionCodeId == positionCodeId);
            var model = new Models.Position.FormViewModel(position);

            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";
            return View("Form/Index", model);
        }
        #endregion

        #region Main Form
        [HttpGet]
        //[Route("Panel/{positionCodeId}-{hrco}")]
        public PartialViewResult PositionPanel(byte hrco, string positionCodeId)
        {
            using var db = new VPContext();
            var position = db.HRPositions.FirstOrDefault(f => f.HRCo == hrco && f.PositionCodeId == positionCodeId);
            var model = new Models.Position.FormViewModel(position);
            return PartialView("Form/_Panel", model);
        }

        [HttpGet]
        public PartialViewResult PositionForm(byte hrco, string positionCodeId)
        {
            using var db = new VPContext();
            var position = db.HRPositions.FirstOrDefault(f => f.HRCo == hrco && f.PositionCodeId == positionCodeId);
            var model = new Models.Position.FormViewModel(position);
            return PartialView("Form/_Form", model);
        }

        [HttpGet]
        public PartialViewResult PositionInfoForm(byte hrco, string positionCodeId)
        {
            using var db = new VPContext();
            var position = db.HRPositions.FirstOrDefault(f => f.HRCo == hrco && f.PositionCodeId == positionCodeId);
            var model = new Models.Position.PositionViewModel(position);
            return PartialView("Form/Info/_Form", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PositionUpdate(Models.Position.PositionViewModel model)
        {
            if (model == null)
                return Json(new { success = "false", errorModel = ModelState.ModelErrors() });

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult PositionValidate(Models.Position.PositionViewModel model)
        {
            using var db = new VPContext();
            var position = db.HRPositions.FirstOrDefault(f => f.HRCo == model.HRCo && f.PositionCodeId == model.PositionCodeId);
            var form = new Models.Position.PositionViewModel(position);
            this.ValidateModel(form);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region New Hire Tasks
        [HttpGet]
        public ActionResult PositioNewHireTaskTable(byte hrco, string positionCodeId)
        {
            using var db = new VPContext();
            var position = db.HRPositions.FirstOrDefault(f => f.HRCo == hrco && f.PositionCodeId == positionCodeId);
            var result = new Models.Position.HireTaskListViewModel(position);

            return PartialView("Form/NewHireTask/_Table", result);
        }

        [HttpGet]
        public ActionResult PositioNewHireTaskAdd(byte hrco, string positionCodeId)
        {
            using var db = new VPContext();
            var position = db.HRPositions.FirstOrDefault(f => f.HRCo == hrco && f.PositionCodeId == positionCodeId);
            var result = new Models.Position.HireTaskViewModel(position.AddHireTask());
            db.BulkSaveChanges();

            return PartialView("Form/NewHireTask/_TableRow", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PositioNewHireTaskDelete(byte hrco, string positionCodeId, int seqId)
        {
            using var db = new VPContext();
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
        public ActionResult PositioNewHireTaskUpdate(Models.Position.HireTaskViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult PositioNewHireTaskValidate(Models.Position.HireTaskViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Employee List

        #endregion
    }
}