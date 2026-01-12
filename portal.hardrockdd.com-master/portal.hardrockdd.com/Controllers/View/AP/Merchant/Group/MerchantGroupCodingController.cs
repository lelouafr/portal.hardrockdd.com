using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.Vendor.Merchant.Forms;
using portal.Repository.VP.AP.Merchant;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.AP.Vendor.Merchant
{
    [ControllerAuthorize]
    public class MerchantGroupCodingController : BaseController
    {

        [HttpGet]
        public ActionResult Form(byte vendGroupId, string categoryGroup)
        {
            using var db = new VPContext();
            var obj = db.CreditMerchantGroups.FirstOrDefault(f => f.VendGroupId == vendGroupId && f.CategoryGroup == categoryGroup);

            var results = new MerchantGroupCodingViewModel(obj);
            return PartialView("../AP/Vendor/MerchantGroup/Forms/Coding/Form", results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(MerchantGroupCodingViewModel model)
        {
            if (ModelState.IsValid)
            {
                using var db = new VPContext();
                model = CreditMerchantGroupRepository.ProcessUpdate(model, db);
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Validate(MerchantGroupCodingViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
    }
}