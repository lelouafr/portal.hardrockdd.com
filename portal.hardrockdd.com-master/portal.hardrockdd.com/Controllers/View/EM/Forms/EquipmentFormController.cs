using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Equipment;
using portal.Models.Views.Equipment.Forms;
using portal.Repository.VP.EM;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.Equipment
{
    public class EquipmentFormController : BaseController
    {
        #region Equipment Form
        [HttpGet]
        [Route("Equipment/Details/{emco-equipmentId}")]
        public ActionResult Index(byte emco, string equipmentId)
        {

            using var db = new VPContext();
            var entity = db.Equipments.FirstOrDefault(f => f.EMCo == emco && f.EquipmentId == equipmentId);
            var result = new EquipmentFormViewModel(entity);

            return View("../EM/Equipment/Forms/PartialIndex", result);
        }

        [HttpGet]
        public ActionResult PartialIndex(byte emco, string equipmentId)
        {

            using var db = new VPContext();
            var entity = db.Equipments.FirstOrDefault(f => f.EMCo == emco && f.EquipmentId == equipmentId);
            var result = new EquipmentFormViewModel(entity);

            return PartialView("../EM/Equipment/Forms/PartialIndex", result);
        }

        [HttpGet]
        public ActionResult Form(byte emco, string equipmentId)
        {
            using var db = new VPContext();
            var entity = db.Equipments.FirstOrDefault(f => f.EMCo == emco && f.EquipmentId == equipmentId);
            var result = new EquipmentFormViewModel(entity);

            return PartialView("../EM/Equipment/Forms/Form", result);
        }


        [HttpGet]
        public ActionResult PopupForm(byte emco, string equipmentId)
        {

            using var db = new VPContext();
            var entity = db.Equipments.FirstOrDefault(f => f.EMCo == emco && f.EquipmentId == equipmentId);
            var result = new EquipmentFormViewModel(entity);

            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";
            return View("../EM/Equipment/Forms/Index", result);
        }

        [HttpGet]
        public JsonResult Validate(EquipmentFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorModel = ModelState.ModelErrors();
                return Json(new { success = ModelState.IsValidJson(), errorModel }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Equipment Assignment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateAssignment(EquipmentAssignmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                using var db = new VPContext();
                model = EquipmentRepository.ProcessUpdate(model, db);
                db.SaveChanges(ModelState);
            }
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult ValidateAssignment(EquipmentAssignmentViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Equipment Info
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateInfo(EquipmentInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                using var db = new VPContext();
                model = EquipmentRepository.ProcessUpdate(model, db);
                db.SaveChanges(ModelState);
            }
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult ValidateInfo(EquipmentInfoViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Equipment License Info
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateLicInfo(EquipmentLicInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                using var db = new VPContext();
                model = EquipmentRepository.ProcessUpdate(model, db);
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult ValidateLicInfo(EquipmentLicInfoViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Equipment Meter
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateMeter(EquipmentMeterViewModel model)
        {
            if (ModelState.IsValid)
            {
                using var db = new VPContext();
                model = EquipmentRepository.ProcessUpdate(model, db);
                db.SaveChanges(ModelState);
            }
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult ValidateMeter(EquipmentViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Equipment Specifications
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateSpec(EquipmentSpecViewModel model)
        {
            if (ModelState.IsValid)
            {
                using var db = new VPContext();
                model = EquipmentRepository.ProcessUpdate(model, db);
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult ValidateSpec(EquipmentSpecViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Equipment Service Item List
        [HttpGet]
        public ViewResult EquipmentServiceItemPanel(byte emco, string equipmentId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var equipment = db.Equipments.FirstOrDefault(f => f.EMCo == emco && f.EquipmentId == equipmentId);

            var results = new Models.Views.Equipment.Forms.EquipmentServiceItemListViewModel(equipment);
            return View("../EM/Service/Item/Index", results);
        }
        [HttpGet]
        public PartialViewResult EquipmentServiceItemForm(byte emco, string equipmentId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var equipment = db.Equipments.FirstOrDefault(f => f.EMCo == emco && f.EquipmentId == equipmentId);

            var results = new Models.Views.Equipment.Forms.EquipmentServiceItemListViewModel(equipment);
            return PartialView("../EM/Service/Item/Index", results);
        }

        [HttpGet]
        public PartialViewResult EquipmentServiceItemTable(byte emco, string equipmentId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var equipment = db.Equipments.FirstOrDefault(f => f.EMCo == emco && f.EquipmentId == equipmentId);

            var result = new Models.Views.Equipment.Forms.EquipmentServiceItemListViewModel(equipment);
            return PartialView("../EM/Equipment/Forms/ServiceItem/Table", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EquipmentServiceItemUpdate(Models.Views.Equipment.Forms.EquipmentServiceItemViewModel model)
        {
            if (ModelState.IsValid && model != null)
            {
                using var db = new VPContext();
                var entity = db.EMServiceEquipmentLinks.FirstOrDefault(f => f.EMCo == model.EMCo && f.LinkId == model.LinkId);
                if (entity != null)
                {
                    if (entity.ServiceItemId != model.ServiceItemId)
                    {
                        entity.ServiceItemId = model.ServiceItemId;
                        entity.ServiceItem = null;
                        entity.IsOverride = false;

                        model.DateIntervalTypeId = null;
                        model.DateInterval = null;
                        model.OdoInterval = null;
                        model.HourInterval = null;
                    }
                    if (entity.ServiceItemId != null && entity.ServiceItem == null)
                    {
                        var serviceItem = db.EMServiceItems.FirstOrDefault(f => f.EMCo == entity.EMCo && f.ServiceItemId == entity.ServiceItemId);
                        entity.ServiceItem = serviceItem;
                    }
                    if (entity.ServiceItem != null)
                    {
                        if ((byte?)model.DateIntervalTypeId == entity.ServiceItem.DateIntervalTypeId)
                            model.DateIntervalTypeId = null;

                        if (model.DateInterval == entity.ServiceItem.DateInterval)
                            model.DateInterval = null;

                        if (model.OdoInterval == entity.ServiceItem.OdoInterval)
                            model.OdoInterval = null;

                        if (model.HourInterval == entity.ServiceItem.HourInterval)
                            model.HourInterval = null;
                    }

                    entity.IsActive = model.IsActive;

                    entity.OverrideDateIntervalTypeId = (byte?)model.DateIntervalTypeId;
                    entity.OverrideDateInterval = model.DateInterval;
                    entity.OverrideOdoInterval = model.OdoInterval;
                    entity.OverrideHourInterval = model.HourInterval;

                    if (entity.OverrideDateIntervalTypeId != null ||
                        entity.OverrideDateInterval != null ||
                        entity.OverrideOdoInterval != null ||
                        entity.OverrideHourInterval != null)
                    {
                        entity.IsOverride = true;
                    }


                }
                db.SaveChanges(ModelState);
                model = new Models.Views.Equipment.Forms.EquipmentServiceItemViewModel(entity);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public PartialViewResult EquipmentServiceItemAdd(byte emco, string equipmentId)
        {
            using var db = new VPContext();
            var equipment = db.Equipments.FirstOrDefault(f => f.EMCo == emco && f.EquipmentId == equipmentId);
            var newObj = new EMServiceEquipmentLink
            {
                EMCo = equipment.EMCo,
                LinkId = db.EMServiceEquipmentLinks.DefaultIfEmpty().Max(max => max == null ? 0 : max.LinkId) + 1,
                EquipmentId = equipment.EquipmentId,
                Equipment = equipment,
            };

            db.EMServiceEquipmentLinks.Add(newObj);
            db.SaveChanges(ModelState);

            var result = new Models.Views.Equipment.Forms.EquipmentServiceItemViewModel(newObj);
            ViewBag.tableRow = "True";
            return PartialView("../EM/Equipment/Forms/ServiceItem/TableRow", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EquipmentServiceItemDelete(byte emco, int linkId)
        {
            using var db = new VPContext();

            var entity = db.EMServiceEquipmentLinks.FirstOrDefault(f => f.EMCo == emco && f.LinkId == linkId);
            if (entity != null)
            {
                db.EMServiceEquipmentLinks.Remove(entity);
                db.SaveChanges(ModelState);
            }

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }
        #endregion
    }
}