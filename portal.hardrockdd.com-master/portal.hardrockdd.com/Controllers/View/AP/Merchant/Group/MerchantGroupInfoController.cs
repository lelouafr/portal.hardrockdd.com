using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.Vendor.Merchant;
using portal.Models.Views.AP.Vendor.Merchant.Forms;
using portal.Repository.VP.AP.Merchant;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.AP.Vendor.Merchant
{
    [ControllerAuthorize]
    public class MerchantGroupInfoController : BaseController
    {
        [HttpGet]
        public ActionResult Form(byte vendGroupId, string categoryGroup)
        {
            using var db = new VPContext();
            var obj = db.CreditMerchantGroups.FirstOrDefault(f => f.VendGroupId == vendGroupId && f.CategoryGroup == categoryGroup);

            var results = new MerchantGroupInfoViewModel(obj);
            return PartialView("../AP/Vendor/MerchantGroup/Forms/Info/Form", results);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(MerchantGroupInfoViewModel model)
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
        public JsonResult Validate(MerchantGroupInfoViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
    }
}