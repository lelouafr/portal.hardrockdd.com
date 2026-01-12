using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.Vendor.Merchant.Forms;
using portal.Repository.VP.AP.Merchant;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.AP.Vendor.Merchant
{
    [ControllerAuthorize]
    public class MerchantCodingController : BaseController
    {


        [HttpGet]
        public ActionResult Form(byte vendGroupId, string merchantId)
        {
            using var db = new VPContext();
            var obj = db.CreditMerchants.FirstOrDefault(f => f.VendGroupId == vendGroupId && f.MerchantId == merchantId);

            var results = new MerchantCodingViewModel(obj);
            return PartialView("../AP/Vendor/Merchant/Forms/Coding/Form", results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(MerchantCodingViewModel model)
        {
            if (ModelState.IsValid)
            {
                using var db = new VPContext();
                model = CreditMerchantRepository.ProcessUpdate(model, db);
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Validate(MerchantCodingViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
    }
}