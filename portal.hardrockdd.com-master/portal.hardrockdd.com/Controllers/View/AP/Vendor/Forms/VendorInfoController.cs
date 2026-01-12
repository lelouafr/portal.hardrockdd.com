using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.Vendor.Forms;
using portal.Repository.VP.AP;
using System.Web.Mvc;

namespace portal.Controllers.VP.AP.Vendor.Merchant
{
    [ControllerAuthorize]
    public class VendorInfoController : BaseController
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(VendorInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                using var db = new VPContext();
                model = VendorRepository.ProcessUpdate(model, db);
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Validate(VendorInfoViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
    }
}