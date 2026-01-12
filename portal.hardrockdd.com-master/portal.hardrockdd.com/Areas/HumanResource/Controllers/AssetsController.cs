using DB.Infrastructure.ViewPointDB.Data;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.HumanResource.Controllers
{
    [RouteArea("HumanResource")]
    public class AssetsController : portal.Controllers.BaseController
    {
        // GET: HR/Assets
        [HttpGet]
        //[Route("Assets")]
        public ActionResult Index()
        {
            using var db = new VPContext();
            var result = new Models.Assets.AssetListViewModel(db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo).HRCompanyParm);
            //result.List = result.List.Take(100).ToList();
            return View("List/Index", result);
        }

        [HttpGet]
        public PartialViewResult Table()
        {
            using var db = new VPContext();
            var result = new Models.Assets.AssetListViewModel(db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo).HRCompanyParm);

            return PartialView("List/_Table", result);
        }

        [HttpGet]
        public PartialViewResult TableRow(byte hrco, string assetId)
        {
            using var db = new VPContext();
            var obj = db.HRCompanyAssets.FirstOrDefault(f => f.HRCo == hrco && f.AssetId == assetId);
            var result = new Models.Assets.AssetViewModel(obj);

            return PartialView("List/_TableRow", result);
        }

        [HttpGet]
        public PartialViewResult Create()
        {
            using var db = new VPContext();
            var model = new Models.Assets.CreateViewModel(db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo).HRCompanyParm);

            return PartialView("Create/_Model", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(Models.Assets.CreateViewModel model)
        {
            var url = "";
            using var db = new VPContext();
            if (ModelState.IsValid && model != null)
            {
                var obj = db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo).HRCompanyParm.AddAsset(model.AssetCategory);
                model.AssetId = obj.AssetId;
                //model = new AssetViewModel(obj);
                model = model.ProcessUpdate(db, ModelState);

                url = this.Url.Action("TableRow", new { Area = "",hrco = obj.HRCo, assetId = model.AssetId });
            } 
            return Json(new { success = ModelState.IsValidJson(), url }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult Panel(byte hrco, string assetId)
        {
            using var db = new VPContext();
            var obj = db.HRCompanyAssets.FirstOrDefault(f => f.HRCo == hrco && f.AssetId == assetId);
            var result = new Models.Assets.AssetViewModel(obj);

            return PartialView("Form/_Panel", result);
        }

        [HttpGet]
        public PartialViewResult Form(byte hrco, string assetId)
        {
            using var db = new VPContext();
            var obj = db.HRCompanyAssets.FirstOrDefault(f => f.HRCo == hrco && f.AssetId == assetId);
            var result = new Models.Assets.AssetViewModel(obj);

            return PartialView("Form/_Form", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Update(Models.Assets.AssetViewModel model)
        {
            if (model == null)
                ModelState.AddModelError("", "Empty Model!");
            else
            {
                using var db = new VPContext();
                model = model.ProcessUpdate(db, ModelState);
            }

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }
    }
}