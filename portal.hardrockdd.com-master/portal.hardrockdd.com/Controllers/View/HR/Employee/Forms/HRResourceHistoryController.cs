using portal.Code.Data.VP;
using portal.Models.Views.PR.Employee.Form;
using portal.Repository.VP.HR;
using System.Web.Mvc;


namespace portal.Controllers.View.HR.Forms
{
    [ControllerAuthorize]
    public class HRResourceHistoryController : BaseController
    {

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(PayrollHistoryViewModel model)
        {
            if (model != null)
            {
                using var db = new VPEntities();
                model = model.ProcessUpdate(ModelState, db);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult Validate(PayrollHistoryViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
    }
}