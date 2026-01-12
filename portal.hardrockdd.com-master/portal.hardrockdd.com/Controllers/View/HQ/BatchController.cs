using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.HQ.Batch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Controllers.View.HQ
{
    public class BatchController: BaseController
    {
        #region Batch Actions

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ClearBatch(byte co, string mth, int batchId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var batch = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId);
            //var action = batch.Actions.FirstOrDefault(f => f.SeqId == seqId);
            var errorMsg = batch.ClearBatch();

            if (!string.IsNullOrEmpty(errorMsg))
            {
                ModelState.AddModelError("", errorMsg);
            }
            
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ValidateBatch(byte co, string mth, int batchId, int seqId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;
            
            using var db = new VPContext();
            var batch = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId);
            var action = batch.Actions.FirstOrDefault(f => f.SeqId == seqId);
            if (action.SubBatchMth != batch.Mth || action.SubBatchId != batch.BatchId)
            {
                batch = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == action.SubBatchMth && f.BatchId == action.SubBatchId);
            }
            if (action.IsActive)
            {
                var errorMsg = batch.ValidateBatch();
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    ModelState.AddModelError("", errorMsg);
                    batch.InUseBy = null;
                    batch.StatusEnum = DB.BatchStatusEnum.Open;
                    action.Status = (int)DB.BatchStatusEnum.ValidatedErros;
                    action.IsActive = true;
                    db.SaveChanges(ModelState);
                }
                else
                {
                    action.IsActive = false;
                    action.Status = (int)DB.BatchStatusEnum.ValidationOK;
                    db.SaveChanges();
                }
            }
            //ModelState.AddModelError("", "TEST");

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult PostBatch(byte co, string mth, int batchId, int seqId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;
            
            using var db = new VPContext();
            var batch = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId);

            var action = batch.Actions.FirstOrDefault(f => f.SeqId == seqId);
            if (action.SubBatchMth != batch.Mth || action.SubBatchId != batch.BatchId)
            {
                batch = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == action.SubBatchMth && f.BatchId == action.SubBatchId);
            }

            if (batch.StatusEnum == DB.BatchStatusEnum.Open)
            {
                action.IsActive = true;
            }
            if (action.IsActive)
            {
                if (batch.StatusEnum == DB.BatchStatusEnum.Open)
                {
                    batch.ValidateBatch();
                }

                var errorMsg = batch.PostBatch();
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    ModelState.AddModelError("", errorMsg);
                    batch.InUseBy = null;
                    batch.StatusEnum = DB.BatchStatusEnum.Open;
                    action.Status = (int)DB.BatchStatusEnum.ValidatedErros;
                    action.IsActive = true;
                    db.SaveChanges(ModelState);
                }
                else
                {
                    action.IsActive = false;
                    action.Status = (int)DB.BatchStatusEnum.PostingSuccessful;
                    db.SaveChanges();
                }
            }

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public ActionResult ProcessBatchPopup(byte co, string mth, int batchId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;
            var controller = (Controller)this;

            using var db = new VPContext();
            var entity = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId);
            var result = new Models.Views.HQ.Batch.ActionListViewModel(entity, controller, db);

            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";
            return View("../HQ/Batch/Action/Index", result);
        }

        [HttpGet]
        public ActionResult ProcessBatchModal(byte co, string mth, int batchId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;
            var controller = (Controller)this;

            using var db = new VPContext();
            var entity = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId);
            var result = new Models.Views.HQ.Batch.ActionListViewModel(entity, controller, db);

            return PartialView("../HQ/Batch/Action/Panel", result);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ProcessActions(byte co, string mth, int batchId, int seqId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var batch = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId);
            var action = batch.Actions.FirstOrDefault(f => f.SeqId == seqId);
            if (action.SubBatchMth != batch.Mth && action.SubBatchId != batch.BatchId)
            {
                batch = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == action.SubBatchMth && f.BatchId == action.SubBatchId);
            }
            if (action.IsActive)
            {
                var errorMsg = batch.PostBatch();
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    ModelState.AddModelError("", errorMsg);
                    batch.InUseBy = null;
                    db.SaveChanges(ModelState);
                }
                else
                {
                    action.IsActive = false;
                    db.SaveChanges();
                }
            }

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        #endregion

        #region Batch List
        [HttpGet]
        [Route("Batch/List")]
        public ActionResult BatchListIndex()
        {
            using var db = new VPContext();
            var tmpCompany = StaticFunctions.GetCurrentCompany();
            var obj = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == tmpCompany.HQCo);

            var cld = db.Calendars.FirstOrDefault(f => f.Date.Year == DateTime.Now.Year && f.Date.Month == DateTime.Now.Month && f.Date.Day == 1);
            //if (DateTime.Now.Date.Day <= 3)
            //{
            //    cld.Date = cld.Date.AddMonths(-1);
            //}
            var mth = cld.Date;

            var model = new Models.Views.HQ.Batch.BatchListViewModel(obj, mth);

            return View("../HQ/Batch/List/Index", model);
        }

        [HttpGet]
        public ActionResult BatchListTable(DateTime mth)
        {
            using var db = new VPContext();
            var tmpCompany = StaticFunctions.GetCurrentCompany();
            var obj = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == tmpCompany.HQCo);
            var model = new Models.Views.HQ.Batch.BatchListViewModel(obj, mth);
            return PartialView("../HQ/Batch/List/Table", model);
        }

        [HttpGet]
        public ActionResult BatchListData(DateTime mth)
        {
            using var db = new VPContext();
            var tmpCompany = StaticFunctions.GetCurrentCompany();
            var obj = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == tmpCompany.HQCo);
            var results = new Models.Views.HQ.Batch.BatchListViewModel(obj, mth);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);


            result.MaxJsonLength = int.MaxValue;
            return result;
        }

        [HttpGet]
        public ActionResult BatchDetailPanel(byte co, string mth, int batchId)
        {
            using var db = new VPContext();
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;
            var result = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId);
            return result.Source.Trim() switch
            {
                "AP Entry" => RedirectToAction("APPanel", "Batch", new { Area = "",co, mth, batchId }),
                "PR Entry" => RedirectToAction("PRPanel", "Batch", new { Area = "",co, mth, batchId }),
                _ => RedirectToAction("NotMapped", "Batch", new { Area = "",co, mth, batchId }),
            };
        }

        [HttpGet]
        public ActionResult BatchDetailForm(byte co, string mth, int batchId)
        {
            using var db = new VPContext();
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;
            var result = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId);
            return result.Source.Trim() switch
            {
                "AP Entry" => RedirectToAction("APForm", "Batch", new { Area = "",co, mth, batchId }),
                "PR Entry" => RedirectToAction("PRForm", "Batch", new { Area = "",co, mth, batchId }),
                _ => RedirectToAction("NotMapped", "Batch", new { Area = "",co, mth, batchId }),
            };
        }

        #endregion

        #region Batch Source List
        [HttpGet]
        [Route("Batch/List/{source}")]
        public ActionResult BatchSourceListIndex(string source)
        {
            using var db = new VPContext();
            var tmpCompany = StaticFunctions.GetCurrentCompany();
            var obj = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == tmpCompany.HQCo);

            var cld = db.Calendars.FirstOrDefault(f => f.Date.Year == DateTime.Now.Year && f.Date.Month == DateTime.Now.Month && f.Date.Day == 1);
            var mth = cld.Date;

            var model = new Models.Views.HQ.Batch.BatchListViewModel(obj, mth, source);

            return View("../HQ/Batch/SourceList/Index", model);
        }

        [HttpGet]
        public ActionResult BatchSourceListTable(DateTime mth, string source)
        {
            using var db = new VPContext();
            var tmpCompany = StaticFunctions.GetCurrentCompany();
            var obj = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == tmpCompany.HQCo);
            var model = new Models.Views.HQ.Batch.BatchListViewModel(obj, mth, source);
            return PartialView("../HQ/Batch/SourceList/Table", model);
        }

        [HttpGet]
        public ActionResult BatchSourceListData(DateTime mth, string source)
        {
            using var db = new VPContext();
            var tmpCompany = StaticFunctions.GetCurrentCompany();
            var obj = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == tmpCompany.HQCo);
            var results = new Models.Views.HQ.Batch.BatchListViewModel(obj, mth, source);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);


            result.MaxJsonLength = int.MaxValue;
            return result;
        }

        #endregion

        #region Batch Details
        [HttpGet]
        public ActionResult NotMapped(byte co, string mth, int batchId)
        {
            return PartialView("../HQ/Batch/Detail/NotMapped");
        }
        #endregion

        #region Batchh Errors


        [HttpGet]
        public ActionResult BatchErrorPanel(byte co, string mth, int batchId, int seqId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var entity = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId);
            var model = new Models.Views.HQ.Batch.BatchErrorListViewModel(entity);
            return PartialView("../HQ/Batch/Error/Panel", model);
        }

        [HttpGet]
        public ActionResult BatchErrorListTable(byte co, string mth, int batchId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var entity = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId);
            var model = new Models.Views.HQ.Batch.BatchErrorListViewModel(entity);
            return PartialView("../HQ/Batch/Error/Table", model);
        }

        #endregion

        #region AP

        #region AP Panel
        [HttpGet]
        public ActionResult APPanel(byte co, string mth, int batchId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;
            var controller = (Controller)this;
            using var db = new VPContext();
            var result = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId);
            var model = new Models.Views.HQ.Batch.AP.APFormViewModel(result, controller, db);
            return PartialView("../HQ/Batch/Detail/AP/Panel", model);
        }
        
        [HttpGet]
        public ActionResult APForm(byte co, string mth, int batchId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;
            var controller = (Controller)this;
            using var db = new VPContext();
            var result = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId);
            var model = new Models.Views.HQ.Batch.AP.APFormViewModel(result, controller, db);
            return PartialView("../HQ/Batch/Detail/AP/Form", model);
        }
        #endregion

        #region AP Batch Transactions
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

        #region AP Batch Line
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

        #region AP Posted Transactions
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

        #region AP Posted Line
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
            var result = db.APTrans.FirstOrDefault(f => f.APCo == co && f.Mth == mthDate  && f.APTransId == apTransId);
            var model = new Models.Views.HQ.Batch.AP.APPostedLineListViewModel(result);
            return PartialView("../HQ/Batch/Detail/AP/Posted/Line/List/Table", model);
        }

        [HttpGet]
        public ActionResult APPostedLinePanel(byte co, string mth, int apTransId, int apLineId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var result = db.APTransLines.FirstOrDefault(f => f.APCo == co && f.Mth == mthDate  && f.APTransId == apTransId && f.APLineId == apLineId);
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

        #region PR

        #region PR Actions
        [HttpGet]
        public ActionResult PRCreateOTBatch(byte co, string mth, int batchId, int seqId)
        {
            var model = new List<ActionViewModel>();
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var batch = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId);
            var result = batch.Actions.FirstOrDefault(f => f.SeqId == seqId);
            if (result.IsActive)
            {
                var subBatch = db.Batches.FirstOrDefault(f => f.Co == result.Co && f.Mth == result.SubBatchMth && f.BatchId == result.SubBatchId);
                if (subBatch == null)
                {
                    subBatch = Repository.VP.HQ.BatchRepository.CreatePR(batch.TableName, batch.Source, (int)batch.PRGroup, (DateTime)batch.PREndDate, db);
                    subBatch.InUseBy = "WebPortalUser";
                    db.BulkSaveChanges();
                    var addedYN = new System.Data.Entity.Core.Objects.ObjectParameter("PRTBAddedYN", typeof(string));
                    var msgParm = new System.Data.Entity.Core.Objects.ObjectParameter("msg", typeof(string));
                    var error = db.bspPRAutoOT(subBatch.Co, subBatch.Mth, subBatch.BatchId, null, null, addedYN, msgParm);

                    result.SubBatchId = subBatch.BatchId;
                    result.SubBatchMth = subBatch.Mth;
                    result.IsActive = false;

                    subBatch.InUseBy = null;
                    db.BulkSaveChanges();
                    var retroDate = ((DateTime)batch.PREndDate).AddDays(-6);

					var retroEntries = db.PRBatchTimeEntries.Where(f => f.PostDate < retroDate && f.Co == subBatch.Co && f.BatchId == subBatch.BatchId).ToList();
					foreach (var entry in retroEntries)
					{
						//entry.EarnCodeId = entry.EarnCode.udRetroEarnCode ?? entry.EarnCodeId;
					}
					db.BulkSaveChanges();

					db.Entry(subBatch).Reload();
                }
                result.Status = 5;
                result.IsActive = false;

                var timeEntries = db.PRBatchTimeEntries.Where(f => f.Co == subBatch.Co && f.Mth == subBatch.Mth && f.BatchId == subBatch.BatchId).ToList();
                if (timeEntries.Any())
                {
					var subBatchActions = batch.Actions.Where(f => f.SubBatchId == subBatch.BatchId && f.SeqId != result.SeqId);
                    if (!subBatchActions.Any())
                    {
                        var action = new BatchAction()
                        {
                            Co = batch.Co,
                            Mth = batch.Mth,
                            BatchId = batch.BatchId,
                            SeqId = batch.Actions.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
                            Title = "Validate OT Batch",
                            ActionUrl = Url.Action("ValidateBatch", "Batch"),
                            ActionType = "post",
                            IsActive = true,
                            SubBatchId = subBatch.BatchId,
                            SubBatchMth = subBatch.Mth
                        };
                        batch.Actions.Add(action);
                        model.Add(new ActionViewModel(action));

                        action = new BatchAction()
                        {
                            Co = batch.Co,
                            Mth = batch.Mth,
                            BatchId = batch.BatchId,
                            SeqId = batch.Actions.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
                            Title = "Post OT Batch",
                            ActionUrl = Url.Action("PostBatch", "Batch"),
                            ActionType = "post",
                            IsActive = true,
                            SubBatchId = subBatch.BatchId,
                            SubBatchMth = subBatch.Mth
                        };
                        batch.Actions.Add(action);
                        model.Add(new ActionViewModel(action));
                    }
                }
                else
                {
                    subBatch.StatusEnum = DB.BatchStatusEnum.Canceled;
                }
                db.BulkSaveChanges();
            }

            return PartialView("../HQ/Batch/Action/List/AddTableRows", model);
        }

        [HttpGet]
        public ActionResult PRCreateSalaryDistroBatch(byte co, string mth, int batchId, int seqId)
        {
            var model = new List<ActionViewModel>();
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var batch = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId);
            var result = batch.Actions.FirstOrDefault(f => f.SeqId == seqId);
            if (result.IsActive)
            {
                //DB.Infrastructure.ViewPointDB.Data.Batch subBatch;
                var subBatch = db.Batches.FirstOrDefault(f => f.Co == result.Co && f.Mth == result.SubBatchMth && f.BatchId == result.SubBatchId);
                if (subBatch == null)
                {
                    subBatch = Repository.VP.HQ.BatchRepository.CreatePR(batch.TableName, batch.Source, (int)batch.PRGroup, (DateTime)batch.PREndDate, db);
                    subBatch.InUseBy = "WebPortalUser";
                    db.SaveChanges();
                    var addedYN = new System.Data.Entity.Core.Objects.ObjectParameter("PRTBAlteredYN", typeof(string));
                    var msgParm = new System.Data.Entity.Core.Objects.ObjectParameter("msg", typeof(string));
                    var error = db.bspPRSalaryDistrib(subBatch.Co, subBatch.Mth, subBatch.BatchId, null, null, addedYN, msgParm);

                    result.SubBatchId = subBatch.BatchId;
                    result.SubBatchMth = subBatch.Mth;
                    result.IsActive = false;

                    subBatch.InUseBy = null;
                    db.SaveChanges();

                    db.Entry(subBatch).Reload();
                }

                result.Status = 5;
                result.IsActive = false;

                var timeEntries = db.PRBatchTimeEntries.Where(f => f.Co == subBatch.Co && f.Mth == subBatch.Mth && f.BatchId == subBatch.BatchId).ToList();
                if (timeEntries.Any())
                {
                    var subBatchActions = batch.Actions.Where(f => f.SubBatchId == subBatch.BatchId && f.SeqId != result.SeqId);
                    if (!subBatchActions.Any())
                    {
                        var action = new BatchAction()
                        {
                            Co = batch.Co,
                            Mth = batch.Mth,
                            BatchId = batch.BatchId,
                            SeqId = batch.Actions.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
                            Title = "Validate Salary Batch",
                            ActionUrl = Url.Action("ValidateBatch", "Batch"),
                            ActionType = "post",
                            IsActive = true,
                            SubBatchId = subBatch.BatchId,
                            SubBatchMth = subBatch.Mth
                        };
                        batch.Actions.Add(action);
                        model.Add(new ActionViewModel(action));

                        action = new BatchAction()
                        {
                            Co = batch.Co,
                            Mth = batch.Mth,
                            BatchId = batch.BatchId,
                            SeqId = batch.Actions.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
                            Title = "Post Salary Batch",
                            ActionUrl = Url.Action("PostBatch", "Batch"),
                            ActionType = "post",
                            IsActive = true,
                            SubBatchId = subBatch.BatchId,
                            SubBatchMth = subBatch.Mth
                        };
                        batch.Actions.Add(action);
                        model.Add(new ActionViewModel(action));
                    }
                }
                else
                {
                    subBatch.StatusEnum = DB.BatchStatusEnum.Canceled;
                }
                db.SaveChanges();
            }

            return PartialView("../HQ/Batch/Action/List/AddTableRows", model);
        }

        #endregion

        #region PR Panel
        [HttpGet]
        public ActionResult PRPanel(byte co, string mth, int batchId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;
            var controller = (Controller)this;
            using var db = new VPContext();
            var result = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId);
            var model = new Models.Views.HQ.Batch.PR.PRFormViewModel(result, controller);
            return PartialView("../HQ/Batch/Detail/PR/Panel", model);
        }

        [HttpGet]
        public ActionResult PRForm(byte co, string mth, int batchId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;
            var controller = (Controller)this;
            using var db = new VPContext();
            var result = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId);
            var model = new Models.Views.HQ.Batch.PR.PRFormViewModel(result, controller);
            return PartialView("../HQ/Batch/Detail/PR/Form", model);
        }
        #endregion

        #region PR Batch Transactions
        [HttpGet]
        public ActionResult PRBatchListPanel(byte co, string mth, int batchId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var result = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId);
            var model = new Models.Views.HQ.Batch.PR.PRBatchTransListViewModel(result);
            return PartialView("../HQ/Batch/Detail/PR/Batch/Transaction/List/Panel", model);
        }

        [HttpGet]
        public ActionResult PRBatchListTable(byte co, string mth, int batchId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var result = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId);
            var model = new Models.Views.HQ.Batch.PR.PRBatchTransListViewModel(result);
            return PartialView("../HQ/Batch/Detail/PR/Batch/Transaction/List/Table", model);
        }

        [HttpGet]
        public ActionResult PRBatchTransactionPanel(byte co, string mth, int batchId, int batchSeq)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var result = db.PRBatchTimeEntries.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId && f.BatchSeq == batchSeq);
            var model = new Models.Views.HQ.Batch.PR.PRBatchTransViewModel(result);
            return PartialView("../HQ/Batch/Detail/PR/Batch/Transaction/Form/Panel", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PRBatchTransactionDelete(byte co, string mth, int batchId, int batchSeq)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var delObj = db.PRBatchTimeEntries.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId && f.BatchSeq == batchSeq);
            if (delObj != null)
            {

                db.PRBatchTimeEntries.Remove(delObj);
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }



        [HttpGet]
        public ActionResult PRBatchTransactionForm(byte co, string mth, int batchId, int batchSeq)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var result = db.PRBatchTimeEntries.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId && f.BatchSeq == batchSeq);
            var model = new Models.Views.HQ.Batch.PR.PRBatchTransViewModel(result);
            return PartialView("../HQ/Batch/Detail/PR/Batch/Transaction/Form/Form", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PRBatchTransactionUpdate(Models.Views.HQ.Batch.PR.PRBatchTransViewModel model)
        {
            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult PRBatchTransactionValidate(Models.Views.HQ.Batch.PR.PRBatchTransViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);
            //model.Validate(ModelState);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region PR Posted Transactions
        [HttpGet]
        public ActionResult PRPostedListPanel(byte co, string mth, int batchId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var result = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId);
            var model = new Models.Views.HQ.Batch.PR.PRPostedTransListViewModel(result);
            return PartialView("../HQ/Batch/Detail/PR/Posted/Transaction/List/Panel", model);
        }

        [HttpGet]
        public ActionResult PRPostedListTable(byte co, string mth, int batchId)
        {
            var mthDate = DateTime.TryParse(mth, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var result = db.Batches.FirstOrDefault(f => f.Co == co && f.Mth == mthDate && f.BatchId == batchId);
            var model = new Models.Views.HQ.Batch.PR.PRPostedTransListViewModel(result);
            return PartialView("../HQ/Batch/Detail/PR/Posted/Transaction/List/Table", model);
        }

        [HttpGet]
        public ActionResult PRPostedTransactionPanel(byte co, byte prGroup, string prEndDate, int employeeId, int paySeq, int postSeq)
        {
            var mthDate = DateTime.TryParse(prEndDate, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var result = db.PayrollEntries.FirstOrDefault(f => f.PRCo == co && 
                                                                f.PRGroup == prGroup && 
                                                                f.PREndDate == mthDate &&
                                                                f.EmployeeId == employeeId &&
                                                                f.PaySeq == paySeq &&
                                                                f.PostSeq == postSeq
                                                                );
            var model = new Models.Views.HQ.Batch.PR.PRPostedTransViewModel(result);
            return PartialView("../HQ/Batch/Detail/PR/Posted/Transaction/Form/Panel", model);
        }


        [HttpGet]
        public ActionResult PRPostedTransactionForm(byte co, byte prGroup, string prEndDate, int employeeId, int paySeq, int postSeq)
        {
            var mthDate = DateTime.TryParse(prEndDate, out DateTime mthDateOut) ? mthDateOut : DateTime.Now;

            using var db = new VPContext();
            var result = db.PayrollEntries.FirstOrDefault(f => f.PRCo == co &&
                                                                f.PRGroup == prGroup &&
                                                                f.PREndDate == mthDate &&
                                                                f.EmployeeId == employeeId &&
                                                                f.PaySeq == paySeq &&
                                                                f.PostSeq == postSeq
                                                                );
            var model = new Models.Views.HQ.Batch.PR.PRPostedTransViewModel(result);
            return PartialView("../HQ/Batch/Detail/PR/Posted/Transaction/Form/Form", model);
        }


        #endregion

        #endregion



    }
}