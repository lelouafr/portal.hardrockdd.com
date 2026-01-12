using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.Invoice;
using System;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.PurchasOrders
{
    //[AuthorizePosition(PositionCodes = "CFO,COO,PRES,IT-DIR,FIN-AP,FIN-APMGR,FIN-AR,FIN-ARMGR,FIN-CTRL,HR-MGR,OF-GA,OP-DM,OP-ENGD,OP-EQADM,OP-EQMGR,OP-GM,OP-PM,OP-SF,OP-SFMGR,OP-SLS,OP-SLSMGR,OP-SUP,SHP-MGR,SHP-SUP")]
    public class APInvoiceController : BaseController
    {
        #region Invoice List
        [HttpGet]
        [Route("AP/Invoices/{status}")]
        public ActionResult Index(int status)
        {
            var timeSelection = DB.TimeSelectionEnum.LastThreeMonths;
            var results = new InvoiceSummaryListViewModel((DB.APStatusEnum)status, timeSelection);
            results.List.Add(new InvoiceSummaryViewModel());

            ViewBag.Controller = "APInvoice";

            return View("../AP/Invoice/Index", results);
        }

        [HttpGet]
        public ActionResult Table(DB.APStatusEnum status, DB.TimeSelectionEnum timeSelection)
        {
            var results = new InvoiceSummaryListViewModel(status, timeSelection);
            results.List.Add(new InvoiceSummaryViewModel());

            ViewBag.Controller = "APInvoice";

            return PartialView("../AP/Invoice/Table", results);
        }

        [HttpGet]
        public ActionResult Data(DB.APStatusEnum status, DB.TimeSelectionEnum timeSelection)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);
            var results = new InvoiceSummaryListViewModel(company, status, timeSelection);

            JsonResult result = Json(new
            {
                data = results.List.ToArray()
            }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }
        #endregion

        #region Invoice Form
        [HttpGet]
        public ActionResult HeaderForm(byte apco, string mth, int aPTransId)
        {
            using var db = new VPContext();
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateout) ? mthDateout : DateTime.Now;
            var result = db.APTrans.FirstOrDefault(f => f.APCo == apco && f.Mth == mthDate && f.APTransId == aPTransId);
            var model = new InvoiceViewModel(result);
            ViewBag.ViewOnly = true;
            return PartialView("../AP/Invoice/Header/Form", model);
        }

        [HttpGet]
        public ActionResult Form(byte apco, string mth, int aPTransId)
        {
            using var db = new VPContext();
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateout) ? mthDateout : DateTime.Now;
            var result = db.APTrans.FirstOrDefault(f => f.APCo == apco && f.Mth == mthDate && f.APTransId == aPTransId);
            var model = new InvoiceFormViewModel(result);
            ViewBag.ViewOnly = true;
            return PartialView("../AP/Invoice/Form/PartialIndex", model);
        }

        [HttpGet]
        public ActionResult PopupForm(byte apco, string mth, int aPTransId)
        {
            using var db = new VPContext();
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateout) ? mthDateout : DateTime.Now;
            var result = db.APTrans.FirstOrDefault(f => f.APCo == apco && f.Mth == mthDate && f.APTransId == aPTransId);
            var model = new InvoiceFormViewModel(result);
            ViewBag.ViewOnly = true;
            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";
            return View("../AP/Invoice/Form/Index", model);
        }

        //
        [HttpGet]
        public ActionResult LineForm(byte apco, DateTime mth, int aPTransId, int lineId)
        {
            using var db = new VPContext();
            var result = db.APTransLines.FirstOrDefault(f => f.APCo == apco && f.Mth == mth && f.APTransId == aPTransId && f.APLineId == lineId);
            var model = new InvoiceLineViewModel(result);
            ViewBag.ViewOnly = true;
            return PartialView("../AP/Invoice/Lines/Form/Form", model);
        }
        [HttpGet]
        public ActionResult LinePanel(byte apco, DateTime mth, int aPTransId, int lineId)
        {
            using var db = new VPContext();
            var result = db.APTransLines.FirstOrDefault(f => f.APCo == apco && f.Mth == mth && f.APTransId == aPTransId && f.APLineId == lineId);
            var model = new InvoiceLineViewModel(result);
            ViewBag.ViewOnly = true;
            return PartialView("../AP/Invoice/Lines/Form/Panel", model);
        }
        #endregion

        #region AP Batch Form
        [HttpGet]
        public ActionResult APBatchPanel(byte co, string mth, int batchId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;
            var controller = (Controller)this;
            using var db = new VPContext();
            var result = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId);
            var model = new Models.Views.HQ.Batch.AP.APFormViewModel(result, controller, db);
            return PartialView("../HQ/Batch/Detail/AP/Panel", model);
        }

        [HttpGet]
        public ActionResult APBatchForm(byte co, string mth, int batchId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;
            var controller = (Controller)this;
            using var db = new VPContext();
            var result = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId);
            var model = new Models.Views.HQ.Batch.AP.APFormViewModel(result, controller, db);
            return PartialView("../HQ/Batch/Detail/AP/Form", model);
        }
        #endregion

        #region AP Unposted List
        #region Batch
        #region AP Unposted Header
        #region List 
        [HttpGet]
        public ActionResult APBatchListPanel(byte co, string mth, int batchId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var result = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId);
            var model = new Models.Views.HQ.Batch.AP.APBatchTransListViewModel(result);
            return PartialView("../HQ/Batch/Detail/AP/Batch/Transaction/List/Panel", model);
        }

        [HttpGet]
        public ActionResult APBatchListTable(byte co, string mth, int batchId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var result = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId);
            var model = new Models.Views.HQ.Batch.AP.APBatchTransListViewModel(result);
            return PartialView("../HQ/Batch/Detail/AP/Batch/Transaction/List/Table", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult APBatchTransactionDelete(byte co, string mth, int batchId, int batchSeq)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var result = db.APBatchHeaders.FirstOrDefault(f => f.APCo == co && f.Mth == mthDate && f.BatchId == batchId && f.BatchSeq == batchSeq);
            if (result != null)
            {
                result.Lines.ToList().ForEach(e => {
                    db.APBatchLines.Remove(e);
                });
                db.APBatchHeaders.Remove(result);
                db.SaveChanges(ModelState);
            }

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }


        #endregion region
        #region Form
        [HttpGet]
        public ActionResult APBatchTransactionPanel(byte co, string mth, int batchId, int batchSeq)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var result = db.APBatchHeaders.FirstOrDefault(f => f.APCo == co && f.Mth == mthDate && f.BatchId == batchId && f.BatchSeq == batchSeq);
            var model = new Models.Views.HQ.Batch.AP.APBatchTransViewModel(result);
            return PartialView("../HQ/Batch/Detail/AP/Batch/Transaction/Form/Panel", model);
        }


        [HttpGet]
        public ActionResult APBatchTransactionForm(byte co, string mth, int batchId, int batchSeq)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var result = db.APBatchHeaders.FirstOrDefault(f => f.APCo == co && f.Mth == mthDate && f.BatchId == batchId && f.BatchSeq == batchSeq);
            var model = new Models.Views.HQ.Batch.AP.APBatchTransViewModel(result);
            return PartialView("../HQ/Batch/Detail/AP/Batch/Transaction/Form/Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult APBatchTransactionUpdate(Models.Views.HQ.Batch.AP.APBatchTransViewModel model)
        {
            using var db = new VPContext();
            var updObj = Repository.VP.AP.APBatchSeqRepository.ProcessUpdate(model, db);
            db.SaveChanges(ModelState);

            var result = new Models.Views.HQ.Batch.AP.APBatchTransViewModel(updObj);
            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult APBatchTransactionValidate(Models.Views.HQ.Batch.AP.APBatchTransViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);
            model.Validate(ModelState);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion
        #region AP Unposted Line
        #region Line List
        [HttpGet]
        public ActionResult APBatchLineListPanel(byte co, string mth, int batchId, int batchSeq)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var result = db.APBatchHeaders.FirstOrDefault(f => f.APCo == co && f.Mth == mthDate && f.BatchId == batchId && f.BatchSeq == batchSeq);
            var model = new Models.Views.HQ.Batch.AP.APBatchLineListViewModel(result);
            return PartialView("../HQ/Batch/Detail/AP/Batch/Line/Panel", model);
        }

        [HttpGet]
        public ActionResult APBatchLineListTable(byte co, string mth, int batchId, int batchSeq)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var result = db.APBatchHeaders.FirstOrDefault(f => f.APCo == co && f.Mth == mthDate && f.BatchId == batchId && f.BatchSeq == batchSeq);
            var model = new Models.Views.HQ.Batch.AP.APBatchLineListViewModel(result);
            return PartialView("../HQ/Batch/Detail/AP/Batch/Line/List/Table", model);
        }
        #endregion
        #region line Form
        [HttpGet]
        public ActionResult APBatchLinePanel(byte co, string mth, int batchId, int batchSeq, int apLineId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var result = db.APBatchLines.FirstOrDefault(f => f.APCo == co && f.Mth == mthDate && f.BatchId == batchId && f.BatchSeq == batchSeq && f.APLineId == apLineId);
            var model = new Models.Views.HQ.Batch.AP.APBatchLineViewModel(result);
            return PartialView("../HQ/Batch/Detail/AP/Batch/Line/Form/Panel", model);
        }


        [HttpGet]
        public ActionResult APBatchLineForm(byte co, string mth, int batchId, int batchSeq, int apLineId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var result = db.APBatchLines.FirstOrDefault(f => f.APCo == co && f.Mth == mthDate && f.BatchId == batchId && f.BatchSeq == batchSeq && f.APLineId == apLineId);
            var model = new Models.Views.HQ.Batch.AP.APBatchLineViewModel(result);
            return PartialView("../HQ/Batch/Detail/AP/Batch/Line/Form/Form", model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult APBatchLineUpdate(Models.Views.HQ.Batch.AP.APBatchLineViewModel model)
        {
            model.MthDate = DateTime.TryParse(model.Mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;
            using var db = new VPContext();
            var updObj = Repository.VP.AP.APBatchItemRepository.ProcessUpdate(model, db);
            db.SaveChanges(ModelState);

            var result = new Models.Views.HQ.Batch.AP.APBatchLineViewModel(updObj);
            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult APBatchLineValidate(Models.Views.HQ.Batch.AP.APBatchLineViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);
            model.Validate(ModelState);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion
        #endregion
        #endregion

        #region AP Posted List
        #region Header
        #region Header List
        [HttpGet]
        public ActionResult APPostedListPanel(byte co, string mth, int batchId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var result = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId);
            var model = new Models.Views.HQ.Batch.AP.APPostedTransListViewModel(result);
            return PartialView("../HQ/Batch/Detail/AP/Posted/Transaction/List/Panel", model);
        }

        [HttpGet]
        public ActionResult APPostedListTable(byte co, string mth, int batchId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var result = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId);
            var model = new Models.Views.HQ.Batch.AP.APPostedTransListViewModel(result);
            return PartialView("../HQ/Batch/Detail/AP/Posted/Transaction/List/Table", model);
        }
        #endregion
        #region Header Form

        [HttpGet]
        public ActionResult APPostedTransactionPanel(byte co, string mth, int apTransId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var result = db.APTrans.FirstOrDefault(f => f.APCo == co && f.Mth == mthDate && f.APTransId == apTransId);
            var model = new Models.Views.HQ.Batch.AP.APPostedTransViewModel(result);
            return PartialView("../HQ/Batch/Detail/AP/Posted/Transaction/Form/Panel", model);
        }


        [HttpGet]
        public ActionResult APPostedTransactionForm(byte co, string mth, int apTransId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var result = db.APTrans.FirstOrDefault(f => f.APCo == co && f.Mth == mthDate && f.APTransId == apTransId);
            var model = new Models.Views.HQ.Batch.AP.APPostedTransViewModel(result);
            return PartialView("../HQ/Batch/Detail/AP/Posted/Transaction/Form/Form", model);
        }

        #endregion
        #endregion
        #region Line
        #region List
        [HttpGet]
        public ActionResult APPostedLineListPanel(byte co, string mth, int apTransId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var result = db.APTrans.FirstOrDefault(f => f.APCo == co && f.Mth == mthDate && f.APTransId == apTransId);
            var model = new Models.Views.HQ.Batch.AP.APPostedLineListViewModel(result);
            return PartialView("../HQ/Batch/Detail/AP/Posted/Line/Panel", model);
        }

        [HttpGet]
        public ActionResult APPostedLineListTable(byte co, string mth, int apTransId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var result = db.APTrans.FirstOrDefault(f => f.APCo == co && f.Mth == mthDate && f.APTransId == apTransId);
            var model = new Models.Views.HQ.Batch.AP.APPostedLineListViewModel(result);
            return PartialView("../HQ/Batch/Detail/AP/Posted/Line/List/Table", model);
        }
        #endregion
        #region Form
        [HttpGet]
        public ActionResult APPostedLinePanel(byte co, string mth, int apTransId, int apLineId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var result = db.APTransLines.FirstOrDefault(f => f.APCo == co && f.Mth == mthDate && f.APTransId == apTransId && f.APLineId == apLineId);
            var model = new Models.Views.HQ.Batch.AP.APPostedTransLineViewModel(result);
            return PartialView("../HQ/Batch/Detail/AP/Posted/Line/Form/Panel", model);
        }


        [HttpGet]
        public ActionResult APPostedLineForm(byte co, string mth, int apTransId, int apLineId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var result = db.APTransLines.FirstOrDefault(f => f.APCo == co && f.Mth == mthDate && f.APTransId == apTransId && f.APLineId == apLineId);
            var model = new Models.Views.HQ.Batch.AP.APPostedTransLineViewModel(result);
            return PartialView("../HQ/Batch/Detail/AP/Posted/Line/Form/Form", model);
        }
        #endregion
        #endregion
        #endregion

    }
}