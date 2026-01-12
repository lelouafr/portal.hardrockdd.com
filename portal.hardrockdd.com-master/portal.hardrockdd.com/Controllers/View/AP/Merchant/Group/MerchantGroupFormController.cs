using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.Vendor.Merchant.Forms;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.AP.Vendor.Merchant
{
    [ControllerAuthorize]
    public class MerchantGroupFormController : BaseController
    {

        [HttpGet]
        public ActionResult Panel(byte vendGroupId, string categoryGroup)
        {
            using var db = new VPContext();
            var obj = db.CreditMerchantGroups.FirstOrDefault(f => f.VendGroupId == vendGroupId && f.CategoryGroup == categoryGroup);

            var results = new MerchantGroupFormViewModel(obj);
            return PartialView("../AP/Vendor/MerchantGroup/Forms/Panel", results);
        }
        [HttpGet]
        public ActionResult Form(byte vendGroupId, string categoryGroup)
        {
            using var db = new VPContext();
            var obj = db.CreditMerchantGroups.FirstOrDefault(f => f.VendGroupId == vendGroupId && f.CategoryGroup == categoryGroup);

            var results = new MerchantGroupFormViewModel(obj);
            return PartialView("../AP/Vendor/MerchantGroup/Forms/Form", results);
        }
    }
}