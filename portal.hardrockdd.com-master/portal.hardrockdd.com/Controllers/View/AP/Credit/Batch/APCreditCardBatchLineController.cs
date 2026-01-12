using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.CreditCard.Batch.Lines;
using System;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers
{
    [ControllerAuthorize]
    public class APCreditCardBatchLineController : BaseController
    {

        [HttpGet]
        public ActionResult Panel(byte co, DateTime mth, int batchId, string source)
        {
            using var db = new VPContext();

            var model = new FormViewModel(co, mth, batchId, source, db);

            return PartialView("../AP/CreditCard/Batch/Lines/Panel", model);
        }

        [HttpGet]
        public ActionResult Form(byte co, DateTime mth, int batchId, string source)
        {
            using var db = new VPContext();

            var model = new FormViewModel(co, mth, batchId, source, db);

            return PartialView("../AP/CreditCard/Batch/Lines/Form", model);
        }


        [HttpGet]
        public ActionResult TransTable(byte co, DateTime mth, int batchId, string source)
        {
            using var db = new VPContext();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == co);

            var model = new BatchTransactionListViewModel(co, mth, batchId, source, db);

            return PartialView("../AP/CreditCard/Batch/Lines/List/Table", model);
        }
    }
}