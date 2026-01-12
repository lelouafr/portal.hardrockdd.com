using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Firm;
using portal.Repository.VP.PM;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.PM
{
    [Authorize]
    public class FirmController : BaseController
    {
        [HttpGet]
        public ActionResult Form(byte vendGroupId, int firmNumber)
        {
            using var db = new VPContext();
            var result = db.Firms.Where(f => f.VendorGroupId == vendGroupId && f.FirmNumber == firmNumber).FirstOrDefault();
            using var repo = new FirmRepository();
            var model = new FirmViewModel(result);
            return PartialView("../PM/Firm/Form", model);
        }

        [HttpGet]
        public PartialViewResult Add(byte vendGroupId, string firmType)
        {
           var result = new FirmViewModel
            {
               VendorGroupId = vendGroupId,
                FirmTypeId = firmType
            };

            return PartialView("../PM/Firm/Form", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(FirmViewModel model)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            using var repo = new FirmRepository();
            model.FirmTypeId = "OWNER";
            var result = repo.Create(model, ModelState);

            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Json(new { success = ModelState.IsValidJson(), value = result.FirmNumber, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(FirmViewModel model)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }

            using var repo = new FirmRepository();
            var result = repo.ProcessUpdate(model, ModelState);
            //var result = repo.GetFirm(model.VendorGroup, model.FirmNumber);

            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(byte vendGroupId, int firmNumber)
        {
            using var repo = new FirmRepository();
            var delObj = repo.GetFirm(vendGroupId, firmNumber);
            var model = new FirmViewModel(delObj);
            if (delObj != null)
            {
                repo.Delete(delObj);
            }
            return Json(new { success = ModelState.IsValidJson(), model });
        }

        [HttpGet]
        public JsonResult Validate(FirmViewModel model)
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
        
        [HttpGet]
        public JsonResult Combo(byte vendGroupId, string selected)
        {
            using var repo = new FirmRepository();
            var list = repo.GetFirms(vendGroupId);

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Firm"
                }
            };
            result.AddRange(FirmRepository.GetSelectList(list, selected));

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult OwnerCombo(byte vendGroupId, string selected)
        {
            using var repo = new FirmRepository();
            var list = repo.GetFirms(vendGroupId, "OWNER");

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Owner"
                }
            };
            result.AddRange(FirmRepository.GetSelectList(list, selected));

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}