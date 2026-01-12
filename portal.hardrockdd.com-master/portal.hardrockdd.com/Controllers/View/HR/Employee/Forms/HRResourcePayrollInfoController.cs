using portal.Code.Data.VP;
using portal.Models.Views.PR.Employee.Form;
using portal.Repository.VP.HR;
using System.Web.Mvc;


namespace portal.Controllers.View.HR.Forms
{
    [ControllerAuthorize]
    public class HRResourcePayrollInfoController : BaseController
    {

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(PayrollPayInfoViewModel model)
        {
            if (model != null)
            {
                using var db = new VPEntities();
                model = model.ProcessUpdate(ModelState, db);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult Validate(PayrollPayInfoViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
    }
}