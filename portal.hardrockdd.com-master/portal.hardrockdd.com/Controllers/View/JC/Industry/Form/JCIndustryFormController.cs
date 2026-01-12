using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.JC.Industry.Form;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.JC.Forms
{
    [ControllerAuthorize]
    public class JCIndustryFormController : BaseController
    {
        [HttpGet]
        public ActionResult Index(byte jcco, int industryId)
        {

            using var db = new VPContext();
            var entity = db.JCIndustries.FirstOrDefault(f => f.JCCo == jcco && f.IndustryId == industryId);
            var result = new IndustryFormViewModel(entity);

            return View("../JC/Industry/Forms/Index", result);
        }

        [HttpGet]
        public ActionResult PartialIndex(byte jcco, int industryId)
        {

            using var db = new VPContext();
            var entity = db.JCIndustries.FirstOrDefault(f => f.JCCo == jcco && f.IndustryId == industryId);
            var result = new IndustryFormViewModel(entity);

            return PartialView("../JC/Industry/Forms/PartialIndex", result);
        }

        [HttpGet]
        public ActionResult Form(byte jcco, int industryId)
        {
            using var db = new VPContext();
            var entity = db.JCIndustries.FirstOrDefault(f => f.JCCo == jcco && f.IndustryId == industryId);
            var result = new IndustryFormViewModel(entity);

            return PartialView("../JC/Industry/Forms/Form", result);
        }


        [HttpGet]
        public ActionResult PopupForm(byte jcco, int industryId)
        {

            using var db = new VPContext();
            var entity = db.JCIndustries.FirstOrDefault(f => f.JCCo == jcco && f.IndustryId == industryId);
            var result = new IndustryFormViewModel(entity);

            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";
            return View("../JC/Industry/Forms/Index", result);
        }

        [HttpGet]
        public JsonResult Validate(IndustryFormViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
    }
}