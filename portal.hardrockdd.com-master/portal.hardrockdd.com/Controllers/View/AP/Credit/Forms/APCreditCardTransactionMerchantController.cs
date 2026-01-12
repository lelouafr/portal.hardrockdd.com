using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.CreditCard.Form;
using portal.Repository.VP.AP.CreditCard;
using System.Web.Mvc;

namespace portal.Controllers
{
    [ControllerAuthorize]
    public class APCreditCardTransactionMerchantController : BaseController
    {        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(MerchantInfoViewModel model)
        {
            using var db = new VPContext();
            CreditTransactionRepository.ProcessUpdate(model, db);
            db.SaveChanges(ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult ValidateHeader(MerchantInfoViewModel model)
        {
            //ModelState.Clear();
            //model.Validate(ModelState);

            if (!ModelState.IsValid)
            {
                var errorModel = ModelState.ModelErrors();
                return Json(new { success = ModelState.IsValidJson(), errorModel }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        }

    }
}