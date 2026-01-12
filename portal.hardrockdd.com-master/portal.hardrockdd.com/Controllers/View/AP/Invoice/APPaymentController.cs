
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace portal.Controllers.View.AP.Invoice
{
    public class APPaymentController : BaseController
    {
        #region Batch List
        [HttpGet]
        public ActionResult BatchListIndex()
        {

            using var db = new VPContext();
            var company = db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo);
            var cld = db.Calendars.FirstOrDefault(f => f.Date.Year == DateTime.Now.Year && f.Date.Month == DateTime.Now.Month && f.Date.Day == 1);

            var mth = cld.Date;
            var model = new Models.Views.AP.Payment.PostedBatchListViewModel(company, mth);
            return View("../AP/Payment/Posted/Index", model);
        }

        [HttpGet]
        public ActionResult BatchListTable(byte apco, string mth)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;
            using var db = new VPContext();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == apco);
            var model = new Models.Views.AP.Payment.PostedBatchListViewModel(company, mthDate);

            return PartialView("../AP/Batch/Posted/Table", model);
        }


        [HttpGet]
        public ActionResult BatchListData(byte apco, string mth)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;
            using var db = new VPContext();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == apco);
            var model = new Models.Views.AP.Payment.PostedBatchListViewModel(company, mthDate);

            JsonResult result = Json(new { data = model.List.ToArray() }, JsonRequestBehavior.AllowGet);


            result.MaxJsonLength = int.MaxValue;
            return result;
        }

        #endregion

        #region Batch Payment List Form
        [HttpGet]
        public ActionResult BatchDetailPanel(byte apco, string mth, int batchId, int cmAcct)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;
            using var db = new VPContext();
            var batch = db.Batches.FirstOrDefault(f => f.Co == apco && f.Mth == mthDate && f.BatchId == batchId);
            var model = new Models.Views.AP.Payment.InvoicePaymentListViewModel(batch, cmAcct);

            return PartialView("../AP/Payment/Posted/Transactions/Panel", model);
        }

        [HttpGet]
        public ActionResult BatchDetailTable(byte apco, string mth, int batchId, int cmAcct)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;
            using var db = new VPContext();
            var batch = db.Batches.FirstOrDefault(f => f.Co == apco && f.Mth == mthDate && f.BatchId == batchId);
            var model = new Models.Views.AP.Payment.InvoicePaymentListViewModel(batch, cmAcct);

            return PartialView("../AP/Payment/Posted/Transactions/Table", model);
        }
        #endregion

        [HttpGet]
        public ActionResult DownloadZionCSV(byte apco, string mth, int batchId, int cmAcct)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;
            using var db = new VPContext();
            var batch = db.Batches.FirstOrDefault(f => f.Co == apco && f.Mth == mthDate && f.BatchId == batchId);
            var model = new Models.Views.AP.Payment.CSVZionCheckListModel(batch, cmAcct);

            using var ms = new MemoryStream();
            using TextWriter tw = new StreamWriter(ms);

            using CsvHelper.CsvWriter csv = new CsvHelper.CsvWriter(tw, System.Globalization.CultureInfo.CurrentCulture);
            csv.Configuration.HasHeaderRecord = false;
            csv.WriteRecords(model.List);

            tw.Flush(); 
            ms.Seek(0, SeekOrigin.Begin); 

            var fileName = string.Format("ZionExport_{0}_{1}.csv", mthDate.ToString("yyyy_MM_dd"), batchId);
            var file = File(ms.GetBuffer(), "text/csv", fileName);

            return file;
        }
    }

}