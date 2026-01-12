using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.CreditCard.Form;
using portal.Repository.VP.AP;
using portal.Repository.VP.AP.CreditCard;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers
{
    //[AuthorizePosition(PositionCodes = "CFO,COO,PRES,IT-DIR,FIN-AP,FIN-APMGR,FIN-CTRL,OF-GA")]
    [ControllerAuthorize]
    public class APCreditCardTransactionCodingController : BaseController
    {

        [HttpGet]
        public ActionResult Panel(byte ccco, int transId)
        {
            using var db = new VPContext();
            var results = db.CreditTransactions.FirstOrDefault(f => f.CCCo == ccco && f.TransId == transId);
            var model = new CodingInfoListViewModel(results);

            return PartialView("../AP/CreditCard/Form/Coding/Panel", model);
        }

        [HttpGet]
        public ActionResult Form(byte ccco, int transId)
        {
            using var db = new VPContext();
            var results = db.CreditTransactions.FirstOrDefault(f => f.CCCo == ccco && f.TransId == transId);
            var model = new CodingInfoListViewModel(results);

            return PartialView("../AP/CreditCard/Form/Coding/Form", model);
        }

        [HttpGet]
        public ActionResult LineTable(byte ccco, int transId)
        {
            using var db = new VPContext();
            var results = db.CreditTransactions.FirstOrDefault(f => f.CCCo == ccco && f.TransId == transId);
            var model = new CodingInfoListViewModel(results);

            return PartialView("../AP/CreditCard/Form/Coding/List/Table", model);
        }

        [HttpGet]
        public ActionResult LinePanel(byte ccco, int transId, int seqId)
        {
            using var db = new VPContext();
            var results = db.CreditTransactionCodes.FirstOrDefault(f => f.CCCo == ccco && f.TransId == transId && f.SeqId == seqId);
            var model = new CodingInfoViewModel(results);

            return PartialView("../AP/CreditCard/Form/Coding/Form/Panel", model);
        }

        [HttpGet]
        public ActionResult LineFrom(byte ccco, int transId, int seqId)
        {
            using var db = new VPContext();
            var results = db.CreditTransactionCodes.FirstOrDefault(f => f.CCCo == ccco && f.TransId == transId && f.SeqId == seqId);
            var model = new CodingInfoViewModel(results);

            return PartialView("../AP/CreditCard/Form/Coding/Form/Form", model);
        }

        [HttpGet]
        public PartialViewResult LineAdd(byte ccco, int transId)
        {
            using var db = new VPContext();

            var trans = db.CreditTransactions.FirstOrDefault(f => f.CCCo == ccco && f.TransId == transId);
            var model = new CodingInfoViewModel();
            var line = trans.Coding.FirstOrDefault();
            if (line != null)
            {
                var result = CreditCardTransactionCodingRepository.Init(line, trans);
                trans.Coding.Add(result);
                model = new CodingInfoViewModel(result);
            }
            else
            {

                var result = CreditCardTransactionCodingRepository.Init(trans);
                trans.Coding.Add(result);
                model = new CodingInfoViewModel(result);
            }
            db.SaveChanges(ModelState);

            return PartialView("../AP/CreditCard/Form/Coding/List/TableRow", model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LineDelete(byte ccco, int transId, int seqId)
        {
            using var db = new VPContext();
            var model = db.CreditTransactionCodes.FirstOrDefault(f => f.CCCo == ccco && f.TransId == transId && f.SeqId == seqId);
            if (model != null)
            {
                db.CreditTransactionCodes.Remove(model);
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson()});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LineUpdate(CodingInfoViewModel model)
        {
            using var db = new VPContext();
            var updObj = CreditCardTransactionCodingRepository.ProcessUpdate(model, db);

            db.SaveChanges(ModelState);
            var result = new CodingInfoViewModel(updObj);

            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult LineValidate(CodingInfoViewModel model)
        {
            ModelState.Clear();

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }
    }
}