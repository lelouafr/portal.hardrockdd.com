using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.Vendor.Forms;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.AP.Vendor.Merchant
{
    [ControllerAuthorize]
    public class VendorFormController : BaseController
    {
        [HttpGet]
        public ActionResult Panel(byte vendGroupId, int vendorId)
        {
            using var db = new VPContext();
            var obj = db.APVendors.FirstOrDefault(f => f.VendorGroupId == vendGroupId && f.VendorId == vendorId);

            var results = new VendorFormViewModel(obj);
            return PartialView("../AP/Vendor/Forms/Panel", results);
        }

        [HttpGet]
        public ActionResult Form(byte vendGroupId, int vendorId)
        {
            using var db = new VPContext();
            var obj = db.APVendors.FirstOrDefault(f => f.VendorGroupId == vendGroupId && f.VendorId == vendorId);

            var results = new VendorFormViewModel(obj);
            return PartialView("../AP/Vendor/Forms/Form", results);
        }

        [HttpGet]
        public ActionResult PopupForm(byte vendGroupId, int vendorId)
        {
            using var db = new VPContext();
            var obj = db.APVendors.FirstOrDefault(f => f.VendorGroupId == vendGroupId && f.VendorId == vendorId);

            var results = new VendorFormViewModel(obj);
            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";
            return View("../AP/Vendor/Forms/Index", results);
        }

        [HttpGet]
        public ActionResult InfoForm(byte vendGroupId, int vendorId)
        {
            using var db = new VPContext();
            var obj = db.APVendors.FirstOrDefault(f => f.VendorGroupId == vendGroupId && f.VendorId == vendorId);

            var results = new VendorFormViewModel(obj);
            return PartialView("../AP/Vendor/Forms/PartialIndex", results);
        }

    }
}