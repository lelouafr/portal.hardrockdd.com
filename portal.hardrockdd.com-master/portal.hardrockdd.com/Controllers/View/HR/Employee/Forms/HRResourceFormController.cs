using portal.Code.Data.VP;
using portal.Models.Views.HR.Resource.Form;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.HR.Forms
{
    [ControllerAuthorize]
    public class HRResourceFormController : BaseController
    {
        [HttpGet]
        //[Route("HR/Resource/Details/{hrco-employeeId}")]
        public ActionResult Index(byte hrco, int employeeId)
        {

            using var db = new VPEntities();
            var entity = db.HRResources.FirstOrDefault(f => f.HRCo == hrco && f.HRRef == employeeId);
            var result = new ResourceFormViewModel(entity);

            return View("../HR/Employee/Forms/Index", result);
        }

        [HttpGet]
        public ActionResult PartialIndex(byte hrco, int employeeId)
        {

            using var db = new VPEntities();
            var entity = db.HRResources.FirstOrDefault(f => f.HRCo == hrco && f.HRRef == employeeId);
            var result = new ResourceFormViewModel(entity);

            return PartialView("../HR/Employee/Forms/PartialIndex", result);
        }

        [HttpGet]
        public ActionResult Form(byte hrco, int employeeId)
        {
            using var db = new VPEntities();
            var entity = db.HRResources.FirstOrDefault(f => f.HRCo == hrco && f.HRRef == employeeId);
            var result = new ResourceFormViewModel(entity);

            return PartialView("../HR/Employee/Forms/Form", result);
        }


        [HttpGet]
        public ActionResult PopupForm(byte hrco, int employeeId)
        {

            using var db = new VPEntities();
            var entity = db.HRResources.FirstOrDefault(f => f.HRCo == hrco && f.HRRef == employeeId);
            var result = new ResourceFormViewModel(entity);

            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";
            return View("../HR/Employee/Forms/Index", result);
        }

        [HttpGet]
        public JsonResult Validate(ResourceFormViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
    }
}