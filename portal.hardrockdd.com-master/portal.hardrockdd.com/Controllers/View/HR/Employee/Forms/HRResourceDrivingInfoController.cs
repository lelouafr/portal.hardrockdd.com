using portal.Code.Data.VP;
using portal.Models.Views.HR.Resource.Form;
using portal.Repository.VP.HR;
using System.Web.Mvc;


namespace portal.Controllers.View.HR.Forms
{
    [ControllerAuthorize]
    public class HRResourceDrivingInfoController : BaseController
    {

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(ResourceDrivingInfoViewModel model)
        {
            if (model != null)
            {
                using var db = new VPEntities();
                model = model.ProcessUpdate(ModelState, db);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult Validate(ResourceDrivingInfoViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
    }
}