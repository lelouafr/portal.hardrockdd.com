using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.CreditCard.Batch;
using portal.Repository.VP.AP;
using portal.Repository.VP.HQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers
{
    [ControllerAuthorize]
    public class APCreditCardBatchController : BaseController
    {

        [HttpGet]
        [Route("AP/Credit/Batches")]
        public ActionResult Index()
        {
            using var db = new VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);
            var cld = db.Calendars.FirstOrDefault(f => f.Date.Year == DateTime.Now.Year && f.Date.Month == DateTime.Now.Month && f.Date.Day == 1);
            var mth = cld.Date;

            var model = new FormViewModel(company, mth, db);

            return View("../AP/CreditCard/Batch/Index", model);
        }


        [HttpGet]
        public ActionResult TransTable(byte co, DateTime mth)
        {
            using var db = new VPContext();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == co);

            var model = new BatchListViewModel(company, mth, db);

            return PartialView("../AP/CreditCard/Batch/List/Table", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateBatch(byte co, DateTime mth, string   source)
        {
            using var db = new VPContext();
            //using var dbAttch = new DB.Infrastructure.VPAttachmentDB.Data.VPAttachmentsContext();

            //var files = new List<HQAttachmentFile>();
            //var attchments = new List<Attachment>();
            var batch = APBatchSeqRepository.AddCCTransToBatch(co, mth, source, db);

            //var attachmentId = HQAttachmentFile.GetNextAttachmentId();
            //files.ForEach(f => f.AttachmentId += attachmentId);
            //attchments.ForEach(f => f.AttachmentID += attachmentId);

            //db.HQAttachmentFiles.AddRange(files);
            //dbAttch.Attachments.AddRange(attchments);
            try
            {
                db.BulkSaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                throw;
            }
            if (ModelState.IsValid && batch != null)
            {
                batch.InUseBy = null;
                db.BulkSaveChanges();
                //dbAttch.BulkSaveChanges();
            }
            return Json(new { success = ModelState.IsValidJson(), ModelState = ModelState.ModelErrors() });
        }
    }
}