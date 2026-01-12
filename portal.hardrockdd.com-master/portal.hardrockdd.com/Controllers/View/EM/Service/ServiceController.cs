using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Equipment.Category;
using portal.Models.Views.Equipment.Service;
using portal.Repository.VP.EM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace portal.Controllers.VP.EM
{
    public class EquipmentServiceController : BaseController
    {
        #region Service Item Master
        [HttpGet]
        [Route("Equipment/Service/List")]
        public ActionResult ServiceItemIndex()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);

            var results = new EquipmentServiceListViewModel(company);
            return View("../EM/Service/Summary/Index", results);
        }

        [HttpGet]
        public PartialViewResult ServiceItemTable()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);

            var results = new EquipmentServiceListViewModel(company);
            return PartialView("../EM/Service/Summary/List/Table", results);
        }


        [HttpGet]
        public PartialViewResult ServiceItemAdd(byte co)
        {
            using var db = new VPContext();
            var newObj = new EMServiceItem
            {
                EMCo = co,
                ServiceItemId = db.EMServiceItems.DefaultIfEmpty().Max(max => max == null ? 0 : max.ServiceItemId) + 1,
                Description = "New"
            };

            db.EMServiceItems.Add(newObj);
            db.SaveChanges(ModelState);

            var result = new EquipmentServiceViewModel(newObj);
            ViewBag.tableRow = "True";
            return PartialView("../EM/Service/Summary/List/TableRow", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ServiceItemDelete(byte co, int serviceItemId)
        {
            using var db = new VPContext();
            var entity = db.EMServiceItems.FirstOrDefault(f => f.EMCo == co && f.ServiceItemId == serviceItemId);
            if (entity != null)
            {
                db.EMServiceItems.Remove(entity);
                db.SaveChanges(ModelState);
            }

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }
        #endregion

        #region Service Item Form

        [HttpGet]
        public ViewResult ServiceItemIndex(byte co, int serviceItemId)
        {
            using var db = new VPContext();
            var entity = db.EMServiceItems.FirstOrDefault(f => f.EMCo == co && f.ServiceItemId == serviceItemId);

            var result = new portal.Models.Views.Equipment.Service.Forms.FormViewModel(entity);
            return View("../EM/Service/Forms/Index", result);
        }

        [HttpGet]
        public PartialViewResult ServiceItemPanel(byte co, int serviceItemId)
        {
            using var db = new VPContext();
            var entity = db.EMServiceItems.FirstOrDefault(f => f.EMCo == co && f.ServiceItemId == serviceItemId);

            var result = new portal.Models.Views.Equipment.Service.Forms.FormViewModel(entity);
            return PartialView("../EM/Service/Forms/Panel", result);
        }

        [HttpGet]
        public PartialViewResult ServiceItemForm(byte co, int serviceItemId)
        {
            using var db = new VPContext();
            var entity = db.EMServiceItems.FirstOrDefault(f => f.EMCo == co && f.ServiceItemId == serviceItemId);

            var result = new portal.Models.Views.Equipment.Service.Forms.FormViewModel(entity);
            return PartialView("../EM/Service/Forms/Form", result);
        }

        #endregion

        #region Service Item Info Form
        [HttpGet]
        public PartialViewResult ServiceItemInfoForm(byte co, int serviceItemId)
        {
            using var db = new VPContext();
            var entity = db.EMServiceItems.FirstOrDefault(f => f.EMCo == co && f.ServiceItemId == serviceItemId);

            var result = new portal.Models.Views.Equipment.Service.Forms.InfoViewModel(entity);
            return PartialView("../EM/Service/Forms/Info/Form", result);
        }

        [HttpGet]
        public PartialViewResult ServiceItemInfoPanel(byte co, int serviceItemId)
        {
            using var db = new VPContext();
            var entity = db.EMServiceItems.FirstOrDefault(f => f.EMCo == co && f.ServiceItemId == serviceItemId);

            var result = new portal.Models.Views.Equipment.Service.Forms.InfoViewModel(entity);
            return PartialView("../EM/Service/Forms/Info/Panel", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ServiceItemInfoUpdate(portal.Models.Views.Equipment.Service.Forms.InfoViewModel model)
        {
            using var db = new VPContext();
            var entity = db.EMServiceItems.FirstOrDefault(f => f.EMCo == model.EMCo && f.ServiceItemId == model.ServiceItemId);
            if (entity != null)
            {
                entity.Description = model.Description;
                entity.ParentServiceItemId = model.ParentServiceItemId;
                db.SaveChanges(ModelState);
            }

            var result = new portal.Models.Views.Equipment.Service.Forms.InfoViewModel(entity);
            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult ServiceItemInfoValidate(portal.Models.Views.Equipment.Service.Forms.InfoViewModel model)
        {
            //ModelState.Clear();

            //using var db = new VPContext();
            //var entity = db.EMServiceItems.FirstOrDefault(f => f.EMCo == model.Co && f.ServiceItemId == model.ServiceItemId);
            //model = new portal.Models.Views.Equipment.Service.Forms.InfoViewModel(entity);
            //TryValidateModel(model);
            //model.Validate(ModelState);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Service Item Trigger Form
        [HttpGet]
        public PartialViewResult ServiceItemTriggerForm(byte co, int serviceItemId)
        {
            using var db = new VPContext();
            var entity = db.EMServiceItems.FirstOrDefault(f => f.EMCo == co && f.ServiceItemId == serviceItemId);

            var result = new portal.Models.Views.Equipment.Service.Forms.TriggerViewModel(entity);
            return PartialView("../EM/Service/Forms/Trigger/Form", result);
        }

        [HttpGet]
        public PartialViewResult ServiceItemTriggerPanel(byte co, int serviceItemId)
        {
            using var db = new VPContext();
            var entity = db.EMServiceItems.FirstOrDefault(f => f.EMCo == co && f.ServiceItemId == serviceItemId);

            var result = new portal.Models.Views.Equipment.Service.Forms.TriggerViewModel(entity);
            return PartialView("../EM/Service/Forms/Trigger/Panel", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ServiceItemTriggerUpdate(portal.Models.Views.Equipment.Service.Forms.TriggerViewModel model)
        {
            using var db = new VPContext();
            var entity = db.EMServiceItems.FirstOrDefault(f => f.EMCo == model.Co && f.ServiceItemId == model.ServiceItemId);
            if (entity != null)
            {
                entity.DateIntervalTypeId = (byte?)model.DateIntervalTypeId;
                entity.DateInterval = model.DateInterval;
                entity.OdoInterval = model.OdoInterval;
                entity.HourInterval = model.HourInterval;
                db.SaveChanges(ModelState);
            }

            var result = new portal.Models.Views.Equipment.Service.Forms.InfoViewModel(entity);
            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult ServiceItemTriggerValidate(portal.Models.Views.Equipment.Service.Forms.TriggerViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Service Item Category List
        [HttpGet]
        public PartialViewResult ServiceItemCategoryTable(byte co, int serviceItemId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var Category = db.EMServiceItems.FirstOrDefault(f => f.EMCo == co && f.ServiceItemId == serviceItemId);

            var result = new Models.Views.Equipment.Service.Forms.CategoryLinkListViewModel(Category);
            return PartialView("../EM/Service/Forms/Category/Table", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ServiceItemCategoryUpdate(portal.Models.Views.Equipment.Service.Forms.CategoryLinkViewModel model)
        {
            if (ModelState.IsValid && model != null)
            {
                using var db = new VPContext();
                var entity = db.EMServiceCategoryLinks.FirstOrDefault(f => f.EMCo == model.EMCo && f.LinkId == model.LinkId);
                if (entity != null)
                {
                    var updateEqpList = false;
                    if (entity.CategoryId != model.CategoryId)
                    {
                        entity.CategoryId = model.CategoryId;
                        entity.Category = null;
                        updateEqpList = true;
                    }
                    if (entity.CategoryId != null && entity.Category == null)
                    {
                        var Category = db.EquipmentCategories.FirstOrDefault(f => f.EMCo == entity.EMCo && f.CategoryId == entity.CategoryId);
                        entity.Category = Category;
                    }
                    if (entity.Category != null && updateEqpList)
                    {
                        var linkId = db.EMServiceEquipmentLinks.Where(f=> f.EMCo == entity.EMCo).DefaultIfEmpty().Max(max => max == null ? 0 : max.LinkId) + 1;
                        foreach (var equipment in entity.Category.ActiveEquipments)
                        {
                            var eqpLink = entity.ServiceItem.EquipmentLinks.FirstOrDefault(f => f.EquipmentId == equipment.EquipmentId);
                            if (eqpLink == null)
                            {
                                eqpLink = new EMServiceEquipmentLink
                                {
                                    EMCo = entity.EMCo,
                                    LinkId = linkId,
                                    ServiceItemId = entity.ServiceItemId,
                                    ServiceItem = entity.ServiceItem,
                                    EquipmentId = equipment.EquipmentId,
                                    Equipment = equipment
                                };
                                linkId++;
                                entity.ServiceItem.EquipmentLinks.Add(eqpLink);
                            }
                        }
                    }
                }
                db.SaveChanges(ModelState);
                model = new Models.Views.Equipment.Service.Forms.CategoryLinkViewModel(entity);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public PartialViewResult ServiceItemCategoryAdd(byte co, int serviceItemId)
        {
            using var db = new VPContext();
            var serviceItem = db.EMServiceItems.FirstOrDefault(f => f.EMCo == co && f.ServiceItemId == serviceItemId);
            var newObj = new EMServiceCategoryLink
            {
                EMCo = serviceItem.EMCo,
                LinkId = db.EMServiceCategoryLinks.DefaultIfEmpty().Max(max => max == null ? 0 : max.LinkId) + 1,
                ServiceItemId = serviceItem.ServiceItemId,
                ServiceItem = serviceItem,
            };

            db.EMServiceCategoryLinks.Add(newObj);
            db.SaveChanges(ModelState);

            var result = new Models.Views.Equipment.Service.Forms.CategoryLinkViewModel(newObj);
            ViewBag.tableRow = "True";
            return PartialView("../EM/Service/Forms/Category/TableRow", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ServiceItemCategoryDelete(byte co, int linkId)
        {
            using var db = new VPContext();

            var entity = db.EMServiceCategoryLinks.FirstOrDefault(f => f.EMCo == co && f.LinkId == linkId);
            if (entity != null)
            {
                foreach (var equipment in entity.Category.Equipments)
                {
                    var delObjs = entity.ServiceItem.EquipmentLinks.Where(f => f.EquipmentId == equipment.EquipmentId).ToList();
                    db.EMServiceEquipmentLinks.RemoveRange(delObjs);
                }
                
                db.EMServiceCategoryLinks.Remove(entity);


                db.SaveChanges(ModelState);
            }

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }
        #endregion

        #region Service Item Equipment List
        [HttpGet]
        public PartialViewResult ServiceItemEquipmentTable(byte co, int serviceItemId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var equipment = db.EMServiceItems.FirstOrDefault(f => f.EMCo == co && f.ServiceItemId == serviceItemId);

            var result = new Models.Views.Equipment.Service.Forms.EquipmentLinkListViewModel(equipment);
            return PartialView("../EM/Service/Forms/Equipment/Table", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ServiceItemEquipmentUpdate(portal.Models.Views.Equipment.Service.Forms.EquipmentLinkViewModel model)
        {
            if (ModelState.IsValid && model != null)
            {
                using var db = new VPContext();
                var entity = db.EMServiceEquipmentLinks.FirstOrDefault(f => f.EMCo == model.EMCo && f.LinkId == model.LinkId);
                if (entity != null)
                {
                    if (entity.EquipmentId != model.EquipmentId)
                    {
                        entity.EquipmentId = model.EquipmentId;
                        entity.Equipment = null;
                        entity.IsOverride = false;

                        model.DateIntervalTypeId = null;
                        model.DateInterval = null;
                        model.OdoInterval = null;
                        model.HourInterval = null;
                    }
                    if (entity.EquipmentId != null && entity.Equipment == null)
                    {
                        var equipment = db.Equipments.FirstOrDefault(f => f.EMCo == entity.EMCo && f.EquipmentId == entity.EquipmentId);
                        entity.Equipment = equipment;
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
                model = new Models.Views.Equipment.Service.Forms.EquipmentLinkViewModel(entity);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public PartialViewResult ServiceItemEquipmentAdd(byte co, int serviceItemId)
        {
            using var db = new VPContext();
            var serviceItem = db.EMServiceItems.FirstOrDefault(f => f.EMCo == co && f.ServiceItemId == serviceItemId);
            var newObj = new EMServiceEquipmentLink
            {
                EMCo = serviceItem.EMCo,
                LinkId = db.EMServiceEquipmentLinks.DefaultIfEmpty().Max(max => max == null ? 0 : max.LinkId) + 1,
                ServiceItemId = serviceItem.ServiceItemId,
                ServiceItem = serviceItem,
            };

            db.EMServiceEquipmentLinks.Add(newObj);
            db.SaveChanges(ModelState);

            var result = new Models.Views.Equipment.Service.Forms.EquipmentLinkViewModel(newObj);
            ViewBag.tableRow = "True";
            return PartialView("../EM/Service/Forms/Equipment/TableRow", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ServiceItemEquipmentDelete(byte co, int linkId)
        {
            using var db = new VPContext();

            var entity = db.EMServiceEquipmentLinks.FirstOrDefault(f => f.EMCo == co && f.LinkId == linkId);
            if (entity != null)
            {
                db.EMServiceEquipmentLinks.Remove(entity);
                db.SaveChanges(ModelState);
            }

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }
        #endregion

        #region Search
        [HttpGet]
        public PartialViewResult Search(byte co)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == co);

            var results = new EquipmentServiceListViewModel(company);

            return PartialView("../EM/Category/Search/Panel", results);
        }

        [HttpGet]
        public PartialViewResult SearchTable(byte co)
        {
            using var db = new VPContext();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == co);

            var results = new EquipmentServiceListViewModel(company);

            return PartialView("../EM/Category/Search/Table", results);
        }

        [HttpPost]
        public JsonResult SearchReturn(EquipmentServiceViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Json(new { success = "true", value = model.ServiceItemId, errorModel = ModelState.ModelErrors() });
        }
        #endregion
    }
}