using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Areas.Admin.Controllers 
{
    [RouteArea("Admin")]
    public class OfficeController : portal.Controllers.BaseController
    {
        #region Index


        [HttpGet]
        public ActionResult Index()
        {
            using var db = new VPContext();
            var office = db.HQOffices.ToList();
            var model = new Models.Office.OfficeListViewModel(office);


            return View("List/Index", model);
        }

        [HttpGet]
        public ActionResult OfficeTable()
        {
            using var db = new VPContext();
            var office = db.HQOffices.ToList();
            var model = new Models.Office.OfficeListViewModel(office);

            return PartialView("List/_Table", model);
        }

        [HttpGet]
        public ActionResult OfficeData()
        {
            using var db = new VPContext();
            var office = db.HQOffices.ToList();
            var results = new Models.Office.OfficeListViewModel(office);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;
            return result;
        }

        [HttpGet]
        public JsonResult OfficeAdd()
        {
            using var db = new VPContext();
            var office = db.AddOffice();
            db.SaveChanges(ModelState);

            var model = new Models.Office.OfficeViewModel(office);
            
            JsonResult result = Json(new { data = model }, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;
            return result;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OfficeDelete(int officeId)
        {
            using var db = new VPContext();
            var delObj = db.HQOffices.FirstOrDefault(f => f.OfficeId == officeId);
            if (delObj != null)
            {
                if (delObj != null)
                {
                    db.HQOffices.Remove(delObj);
                    db.SaveChanges(ModelState);
                }
            }

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }
        #endregion

        #region Office Form
        [HttpGet]
        public ActionResult OfficePanel(int officeId)
        {
            using var db = new VPContext();
            var office = db.HQOffices.FirstOrDefault(f => f.OfficeId == officeId);
            var model = new Models.Office.OfficeFormViewModel(office);


            return PartialView("Forms/_Panel", model);
        }

        [HttpGet]
        public ActionResult OfficeForm(int officeId)
        {
            using var db = new VPContext();
            var office = db.HQOffices.FirstOrDefault(f => f.OfficeId == officeId);
            var model = new Models.Office.OfficeFormViewModel(office);

            return PartialView("Forms/_Form", model);
        }
        #endregion

        #region Info
        [HttpGet]
        public ActionResult OfficeInfoPanel(int officeId)
        {
            using var db = new VPContext();
            var office = db.HQOffices.FirstOrDefault(f => f.OfficeId == officeId);
            var model = new Models.Office.OfficeViewModel(office);


            return PartialView("Forms/Info/_Panel", model);
        }

        [HttpGet]
        public ActionResult OfficeInfoForm(int officeId)
        {
            using var db = new VPContext();
            var office = db.HQOffices.FirstOrDefault(f => f.OfficeId == officeId);
            var model = new Models.Office.OfficeViewModel(office);

            return PartialView("Forms/Info/_Form", model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OfficeUpdate(Models.Office.OfficeViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }


        [HttpGet]
        public JsonResult OfficeValidate(Models.Office.OfficeViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}