using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.Vendor.Merchant.Forms;
using portal.Repository.VP.AP.Merchant;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.AP.Vendor.Merchant
{
    [ControllerAuthorize]
    public class MerchantCategoryCodingController : BaseController
    {

        [HttpGet]
        public ActionResult Form(byte vendGroupId, string categoryGroup, int categoryCodeId)
        {
            using var db = new VPContext();
            var obj = db.CreditMerchantCategories.FirstOrDefault(f => f.VendGroupId == vendGroupId &&
                                                                      f.CategoryGroup == categoryGroup &&
                                                                      f.CategoryCodeId == categoryCodeId);

            var results = new MerchantCategoryCodingViewModel(obj);
            return PartialView("../AP/Vendor/MerchantCategory/Forms/Coding/Form", results);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(MerchantCategoryCodingViewModel model)
        {
            if (ModelState.IsValid)
            {
                using var db = new VPContext();
                model = CreditMerchantCategoryRepository.ProcessUpdate(model, db);
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Validate(MerchantCategoryCodingViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
    }
}