using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.JC.Market;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View.JC
{
    [ControllerAccess]
    [ControllerAuthorize]
    public class JCMarketController : BaseController
    {
        [HttpGet]
        [Route("JC/Markets")]
        public ActionResult Index()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);

            var results = new MarketListViewModel(company);
            ViewBag.DataController = "JCMarket";
            return View("../JC/Market/List/Index", results);
        }

        [HttpGet]
        public ActionResult Table()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);

            var results = new MarketListViewModel(company);

            ViewBag.DataController = "JCMarket";
            return PartialView("../JC/Market/List/Table", results);
        }

        [HttpGet]
        public PartialViewResult Add(byte jcco)
        {
            using var db = new VPContext();
            var entity = new JCMarket()
            {
                JCCo = jcco,
                MarketId = db.JCMarkets
                                .Where(f => f.JCCo == jcco)
                                .DefaultIfEmpty()
                                .Max(f => f == null ? 0 : f.MarketId) + 1,
                Description = "NEW",
                ActiveYN = "Y"
            };

            db.JCMarkets.Add(entity);
            db.SaveChanges(ModelState);
            var result = new MarketViewModel(entity);

            return PartialView("../JC/Market/List/TableRow", result);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Update(MarketViewModel model)
        {
            if (model == null) throw new System.ArgumentNullException(nameof(model));

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Delete(byte jcco, int marketId)
        {
            using var db = new VPContext();
            var entity = db.JCMarkets.FirstOrDefault(f => f.JCCo == jcco && f.MarketId == marketId);
            var model = new MarketViewModel(entity);
            if (entity != null)
            {
                db.JCMarkets.Remove(entity);
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult Validate(MarketViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
    }
}