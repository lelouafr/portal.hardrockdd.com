using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Equipment.Category;
using portal.Repository.VP.EM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace portal.Controllers.VP.EM
{
    public class EquipmentCategoryController : BaseController
    {
        [HttpGet]
        public ViewResult Index()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var emp = user.Employee.FirstOrDefault();
            var company = db.EMCompanyParms.FirstOrDefault(f => f.EMCo == emp.HRCo);

            var results = new EquipmentCategoryListViewModel(company);
            return View("../EM/Category/Summary/Index", results);
        }

        [HttpGet]
        public PartialViewResult Table()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var emp = user.Employee.FirstOrDefault();
            var company = db.EMCompanyParms.FirstOrDefault(f => f.EMCo == emp.HRCo);

            var results = new EquipmentCategoryListViewModel(company);
            return PartialView("../EM/Category/Summary/Index", results);
        }

        [HttpGet]
        public PartialViewResult Form(byte co, string categoryId)
        {
            using var db = new VPContext();
            var cat = db.EquipmentCategories.FirstOrDefault(f => f.EMCo == co && f.CategoryId == categoryId);
            var result = new EquipmentCategoryViewModel(cat);

            return PartialView("../EM/Category/Forms/Panel", result);
        }

        [HttpGet]
        public ActionResult Create()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var emp = user.Employee.FirstOrDefault();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emp.HRCo);


            var results = new EquipmentCategoryCreateViewModel(company);
            return PartialView("../EM/Category/Create/Index", results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EquipmentCategoryCreateViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            ModelState.Clear();
            TryValidateModel(model);
            if (ModelState.IsValid)
            {
                using var db = new VPContext();
                //var audit = EquipmentAuditRepository.Create(model, db);
                var loc = new EquipmentCategory();
                loc.EMCo = model.Co;
                loc.CategoryId = model.CategoryId;
                loc.Description = model.Description;
                db.EquipmentCategories.Add(loc);
                db.SaveChanges(ModelState);
                var url = Url.Action("Index", "EquipmentCategory", new { Area = "", co = loc.EMCo, locationId = loc.CategoryId });

                return Json(new { success = ModelState.IsValidJson(), url });
            }

            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult Combo(byte co)
        {
            using var db = new VPContext();
            var list = db.EquipmentCategories.Where(f => f.EMCo == co).ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Category"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.CategoryId.ToString(AppCultureInfo.CInfo()),
                Text = s.Description
            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult Search(byte co)
        {
            using var db = new VPContext();
            var comp = db.EMCompanyParms.FirstOrDefault(f => f.EMCo == co);
            var result = new EquipmentCategoryListViewModel(comp);

            return PartialView("../EM/Category/Search/Panel", result);
        }

        [HttpGet]
        public PartialViewResult SearchTable(byte co)
        {
            using var db = new VPContext();
            var comp = db.EMCompanyParms.FirstOrDefault(f => f.EMCo == co);
            var result = new EquipmentCategoryListViewModel(comp);

            return PartialView("../EM/Category/Search/Table", result);
        }

        [HttpPost]
        public JsonResult SearchReturn(EquipmentCategoryViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Json(new { success = "true", value = model.CategoryId, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public PartialViewResult Add(byte co)
        {
            var result = new EquipmentCategoryViewModel
            {
                Co = co
            };

            return PartialView("../EM/Category/Add/Form", result);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult Create(EquipmentCategoryViewModel model)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }

            using var db = new VPContext();
            var result = new EquipmentCategory
            {
                EMCo = model.Co,
                CategoryId = model.CategoryId,
                Description = model.Description
            };
            if (!db.EquipmentCategories.Any(f => f.EMCo == model.Co && f.CategoryId == model.CategoryId))
            {
                db.EquipmentCategories.Add(result);
                db.SaveChanges(ModelState);
            }

            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Json(new { success = ModelState.IsValidJson(), value = result.CategoryId, errorModel = ModelState.ModelErrors() });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Update(EquipmentCategoryViewModel model)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            using var db = new VPContext();
            var updObj = db.EquipmentCategories.FirstOrDefault(f => f.EMCo == model.Co && f.CategoryId == model.CategoryId);
            if (updObj != null)
            {
                updObj.Description = model.Description;
            }
            db.SaveChanges(ModelState);

            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Delete(byte co, string categoryId)
        {
            using var db = new VPContext();
            var updObj = db.EquipmentCategories.FirstOrDefault(f => f.EMCo == co && f.CategoryId == categoryId);
            if (updObj != null)
            {
                db.EquipmentCategories.Remove(updObj);
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson() });
        }

        [HttpGet]
        public JsonResult Validate(EquipmentCategoryViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);

            if (!ModelState.IsValid)
            {
                var errorModel = ModelState.ModelErrors();
                    return Json(new { success = ModelState.IsValidJson(), errorModel }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        }

    }
}