using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.Vendor.Merchant.Forms;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.AP.Vendor.Merchant
{
    [ControllerAuthorize]
    public class MerchantFormController : BaseController
    {

        [HttpGet]
        [Route("Merchant/{co}-{merchantId}")]
        public ActionResult Index(byte vendGroupId, string merchantId)
        {
            using var db = new VPContext();
            var obj = db.CreditMerchants.FirstOrDefault(f => f.VendGroupId == vendGroupId && f.MerchantId == merchantId);

            var results = new MerchantFormViewModel(obj);
            return View("../AP/Vendor/Merchant/Forms/Index", results);
        }

        [HttpGet]
        public ActionResult PartialIndex(byte vendGroupId, string merchantId)
        {
            using var db = new VPContext();
            var obj = db.CreditMerchants.FirstOrDefault(f => f.VendGroupId == vendGroupId && f.MerchantId == merchantId);

            var results = new MerchantFormViewModel(obj);
            return PartialView("../AP/Vendor/Merchant/Forms/PartialIndex", results);
        }


        [HttpGet]
        public ActionResult Panel(byte vendGroupId, string merchantId)
        {
            using var db = new VPContext();
            var obj = db.CreditMerchants.FirstOrDefault(f => f.VendGroupId == vendGroupId && f.MerchantId == merchantId);

            var results = new MerchantFormViewModel(obj);
            return PartialView("../AP/Vendor/Merchant/Forms/Panel", results);
        }


        [HttpGet]
        public ActionResult Form(byte vendGroupId, string merchantId)
        {
            using var db = new VPContext();
            var obj = db.CreditMerchants.FirstOrDefault(f => f.VendGroupId == vendGroupId && f.MerchantId == merchantId);

            var results = new MerchantFormViewModel(obj);
            return PartialView("../AP/Vendor/Merchant/Forms/Form", results);
        }
    }
}