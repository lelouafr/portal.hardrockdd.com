using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.DailyTicket;
using portal.Repository.VP.DT;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.DT
{
    [Authorize]
    public class DailyFormController : BaseController
    {

        [HttpGet]
        public ActionResult Form(int DailyFormId)
        {
            using var db = new VPContext();
            var result = db.DailyForms.Where(f => f.FormId == DailyFormId).FirstOrDefault();            
            var model = new DailyFormViewModel(result);
            return PartialView(model);
        }

        [HttpGet]
        public PartialViewResult Add()
        {
            var result = new DailyFormViewModel
            {
            };

            return PartialView("Form", result);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult Create(DailyFormViewModel model)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            using var db = new VPContext();
            var result = db.AddDailyForm();
            db.SaveChanges(ModelState);

            model.FormId = result.FormId;
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), value = result.FormId, errorModel = ModelState.ModelErrors() });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Update(DailyFormViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });
            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Delete(int formId)
        {
            using var db = new VPContext();
            var delObj = db.DailyForms.FirstOrDefault(f => f.FormId == formId);
            if (delObj != null)
            {
                db.DailyForms.Remove(delObj);
                db.SaveChanges(ModelState);
            }
            else
            {
                ModelState.AddModelError("", "Could not delete, form not found.");
            }
            return Json(new { success = ModelState.IsValidJson() });
        }

        [HttpGet]
        public JsonResult Validate(DailyFormViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Combo()
        {
            using var db = new VPContext();
            var list = db.DailyForms.Where(f => f.Active == "1").ToList();
            if (User.IsInRole("Payroll") || User.IsInRole("Admin"))
            {
                list = db.DailyForms.Where(f => f.Active == "1" || f.FormId == 11).ToList();
            }

            var selectList = list.Select(s => new SelectListItem
            {
                Value = s.FormId.ToString(AppCultureInfo.CInfo()),
                Text = s.Description
            }).ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select DailyForm"
                }
            };

            result.AddRange(selectList);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}