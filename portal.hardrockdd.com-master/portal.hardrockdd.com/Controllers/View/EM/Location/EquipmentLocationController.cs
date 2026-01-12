using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Equipment.Location;
using portal.Repository.VP.EM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace portal.Controllers.VP.EM
{
    public class EquipmentLocationController : BaseController
    {
        
        [HttpGet]
        public JsonResult Combo(byte emco)
        {
            using var db = new VPContext();
            var list = db.EMLocations.Where(f => f.EMCo == emco).ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Location"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.LocationId.ToString(AppCultureInfo.CInfo()),
                Text = s.Description
            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ViewResult Index()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var emp = user.Employee.FirstOrDefault();
            var company = db.EMCompanyParms.FirstOrDefault(f => f.EMCo == emp.HRCo);

            var results = new EquipmentLocationListViewModel(company);
            return View("../EM/Location/Summary/Index", results);
        }

        [HttpGet]
        public PartialViewResult Table()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var emp = user.Employee.FirstOrDefault();
            var company = db.EMCompanyParms.FirstOrDefault(f => f.EMCo == emp.HRCo);

            var results = new EquipmentLocationListViewModel(company);
            return PartialView("../EM/Location/Summary/List/Table", results);
        }
        [HttpGet]
        public ViewResult TableRow(byte emco, string locationId)
        {
            using var db = new VPContext();
            var loc = db.EMLocations.FirstOrDefault(f => f.EMCo == emco && f.LocationId == locationId);
            var result = new EquipmentLocationViewModel(loc);
            return View("../EM/Location/Summary/List/Table", result);
        }

        [HttpGet]
        public PartialViewResult Form(byte emco, string locationId)
        {
            using var db = new VPContext();
            var loc = db.EMLocations.FirstOrDefault(f => f.EMCo == emco && f.LocationId == locationId);
            var result = new EquipmentLocationViewModel(loc);

            return PartialView("../EM/Location/Form/Panel", result);
        }

        [HttpGet]
        public PartialViewResult Search(byte emco)
        {
            using var db = new VPContext();
            var comp = db.EMCompanyParms.FirstOrDefault(f => f.EMCo == emco);
            var result = new EquipmentLocationListViewModel(comp);

            return PartialView("../EM/Location/Search/Panel", result);
        }

        [HttpGet]
        public PartialViewResult SearchTable(byte emco)
        {
            using var db = new VPContext();
            var comp = db.EMCompanyParms.FirstOrDefault(f => f.EMCo == emco);
            var result = new EquipmentLocationListViewModel(comp);

            return PartialView("../EM/Location/Search/Table", result);
        }

        [HttpPost]
        public JsonResult SearchReturn(EquipmentLocationViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Json(new { success = "true", value = model.LocationId, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public ActionResult Create()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var emp = user.Employee.FirstOrDefault();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emp.HRCo);


            var results = new EquipmentLocationCreateViewModel(company);
            return PartialView("../EM/Location/Create/Index", results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EquipmentLocationCreateViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            ModelState.Clear();
            TryValidateModel(model);
            if (ModelState.IsValid)
            {
                using var db = new VPContext();
                //var audit = EquipmentAuditRepository.Create(model, db);
                var loc = new EMLocation();
                loc.EMCo = model.Co;
                loc.LocationId = model.LocationId;
                loc.Description = model.Description;
                loc.Active = "Y";
                db.EMLocations.Add(loc);
                db.SaveChanges(ModelState);
                var url = Url.Action("Index", "EquipmentLocation", new { Area = "",emco = loc.EMCo, locationId = loc.LocationId });

                return Json(new { success = ModelState.IsValidJson(), url });
            }

            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Update(EquipmentLocationViewModel model)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            using var db = new VPContext();
            var updObj = db.EMLocations.FirstOrDefault(f => f.EMCo == model.EMCo && f.LocationId == model.LocationId);
            if (updObj != null)
            {
                updObj.Description = model.Description;
                db.SaveChanges(ModelState);
            }

            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Delete(byte emco, string categoryId)
        {
            using var db = new VPContext();
            var updObj = db.EMLocations.FirstOrDefault(f => f.EMCo == emco && f.LocationId == categoryId);
            var model = new EquipmentLocationViewModel(updObj);
            if (updObj != null)
            {
                db.EMLocations.Remove(updObj);
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson(), model });
        }

        [HttpGet]
        public JsonResult Validate(EquipmentLocationViewModel model)
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