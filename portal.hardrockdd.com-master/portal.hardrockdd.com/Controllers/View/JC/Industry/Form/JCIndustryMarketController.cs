using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.JC.Industry.Form;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.JC.Forms
{
    [ControllerAuthorize]
    public class JCIndustryMarketController : BaseController
    {
        [HttpGet]
        public PartialViewResult Add(byte jcco, int industryId)
        {
            using var db = new VPContext();
            var entity = new JCIndustryMarket()
            {
                JCCo = jcco,
                IndustryId = industryId,
                SeqId = db.JCIndustryMarkets
                                .Where(f => f.JCCo == jcco && f.IndustryId == industryId)
                                .DefaultIfEmpty()
                                .Max(f => f == null ? 0 : f.SeqId) + 1,
                ActiveYN = "Y"
            };

            db.JCIndustryMarkets.Add(entity);
            db.SaveChanges(ModelState);
            var result = new IndustryAssignedMarketViewModel(entity);

            return PartialView("../JC/Industry/Forms/Market/TableRow", result);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Delete(byte jcco, int industryId, int seqId)
        {
            using var db = new VPContext();
            var delObj = db.JCIndustryMarkets.FirstOrDefault(f => f.JCCo == jcco && f.IndustryId == industryId && f.SeqId == seqId);
            var model = new IndustryAssignedMarketViewModel(delObj);
            if (delObj != null)
            {
                db.JCIndustryMarkets.Remove(delObj);
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(IndustryAssignedMarketViewModel model)
        {
            if (model == null) throw new System.ArgumentNullException(nameof(model));
            if (ModelState.IsValid)
            {
                using var db = new VPContext();
                var updObj = db.JCIndustryMarkets.FirstOrDefault(f => f.JCCo == model.JCCo && f.IndustryId == model.IndustryId && f.SeqId == model.SeqId);
                if (updObj != null)
                {
                    /****Write the changes to object****/
                    updObj.MarketId = model.MarketId;
                }
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult Validate(IndustryAssignedMarketViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
    }
}