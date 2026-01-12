using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.Vendor.Merchant.Forms;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.AP.Vendor.Merchant
{
    [ControllerAuthorize]
    public class MerchantCategoryFormController : BaseController
    {


        [HttpGet]
        public ActionResult Panel(byte vendGroupId, string categoryGroup, int categoryCodeId)
        {
            using var db = new VPContext();
            var obj = db.CreditMerchantCategories.FirstOrDefault(f => f.VendGroupId == vendGroupId &&
                                                                      f.CategoryGroup == categoryGroup &&
                                                                      f.CategoryCodeId == categoryCodeId);

            var results = new MerchantCategoryFormViewModel(obj);
            return PartialView("../AP/Vendor/MerchantCategory/Forms/Panel", results);
        }

        [HttpGet]
        public ActionResult Form(byte vendGroupId, string categoryGroup, int categoryCodeId)
        {
            using var db = new VPContext();
            var obj = db.CreditMerchantCategories.FirstOrDefault(f => f.VendGroupId == vendGroupId &&
                                                                      f.CategoryGroup == categoryGroup &&
                                                                      f.CategoryCodeId == categoryCodeId);

            var results = new MerchantCategoryFormViewModel(obj);
            return PartialView("../AP/Vendor/MerchantCategory/Forms/Form", results);
        }
    }
}