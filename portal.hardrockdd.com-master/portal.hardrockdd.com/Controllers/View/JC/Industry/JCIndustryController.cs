using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.JC.Industry;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View.JC
{
    [ControllerAccess]
    [ControllerAuthorize]
    public class JCIndustryController : BaseController
    {
        [HttpGet]
        [Route("JC/Industries")]
        public ActionResult Index()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);

            var results = new IndustryListViewModel(company);
            ViewBag.DataController = "JCIndustry";
            return View("../JC/Industry/List/Index", results);
        }

        [HttpGet]
        public ActionResult Table()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);

            var results = new IndustryListViewModel(company);

            ViewBag.DataController = "JCIndustry";
            return PartialView("../JC/Industry/List/Table", results);
        }

        [HttpGet]
        public PartialViewResult Add(byte jcco)
        {
            using var db = new VPContext();
            var entity = new JCIndustry()
            {
                JCCo = jcco,
                IndustryId = db.JCIndustries
                                .Where(f => f.JCCo == jcco)
                                .DefaultIfEmpty()
                                .Max(f => f == null ? 0 : f.IndustryId) + 1,
                Description = "NEW",
                ActiveYN = "Y"
            };

            db.JCIndustries.Add(entity);
            db.SaveChanges(ModelState);
            var result = new IndustryViewModel(entity);

            return PartialView("../JC/Industry/List/TableRow", result);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Update(IndustryViewModel model)
        {
            if (model == null) throw new System.ArgumentNullException(nameof(model));

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Delete(byte jcco, int industryId)
        {
            using var db = new VPContext();
            var delObj = db.JCIndustries.FirstOrDefault(f => f.JCCo == jcco && f.IndustryId == industryId);
            var model = new IndustryViewModel(delObj);
            if (delObj != null)
            {
                db.JCIndustries.Remove(delObj);
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult Validate(IndustryViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
    }
}