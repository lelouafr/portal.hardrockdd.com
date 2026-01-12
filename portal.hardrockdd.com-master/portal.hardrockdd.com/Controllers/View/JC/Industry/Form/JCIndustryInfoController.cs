using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.JC.Industry.Form;
using System.Web.Mvc;


namespace portal.Controllers.View.JC.Forms
{
    [ControllerAuthorize]
    public class JCIndustryInfoController : BaseController
    {

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(IndustryInfoViewModel model)
        {
            if (model == null) throw new System.ArgumentNullException(nameof(model));

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult Validate(IndustryInfoViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
    }
}