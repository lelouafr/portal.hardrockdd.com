using DB.Infrastructure.ViewPointDB.Data;
using portal.Areas.AccountsPayable.Models.Document;
using portal.Code.Services;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Areas.AccountsPayable.Controllers
{
    [RouteArea("AccountsPayable")]

    public class DocumentController : portal.Controllers.BaseController
    {
        #region Upload List
        [HttpGet]
        [Route("Documents/Upload")]
        public ActionResult UploadIndex()
        {
            ExchEmail.OpenEmail();
            var model = new DocumentListViewModel();

            return View("List/Upload/Index", model);
        }

        [HttpGet]
        public PartialViewResult UploadTable()
        {
            var model = new DocumentListViewModel();

            return PartialView("List/Upload/_Table", model);
        }

        [HttpGet]
        public ActionResult UploadData()
        {
            using var db = new VPContext();
            var comp = db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo);
            var list = db.APDocuments.Where(f => f.APCo == comp.APCo && f.tStatusId == (int)DB.APDocumentStatusEnum.New).ToList();
            var results = new DocumentListViewModel(list);

            JsonResult result = Json(new
            {
                data = results.List.ToArray()
            }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }

        #endregion

        #region Filed List
        [HttpGet]
        [Route("Documents/Filed")]
        public ActionResult FiledIndex()
        {
            ExchEmail.OpenEmail();
            var model = new DocumentListViewModel();

            return View("List/Filed/Index", model);
        }


        [HttpGet]
        public PartialViewResult FiledTable()
        {
            var model = new DocumentListViewModel();

            return PartialView("List/Filed/_Table", model);
        }

        [HttpGet]
        public ActionResult FiledData()
        {
            using var db = new VPContext();
            var comp = db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo);
            var list = db.APDocuments.Where(f => f.APCo == comp.APCo &&
                                                (f.tStatusId == (int)DB.APDocumentStatusEnum.Filed ||
                                                 f.tStatusId == (int)DB.APDocumentStatusEnum.Error ||
                                                 f.tStatusId == (int)DB.APDocumentStatusEnum.RequestedInfo)).ToList();
            var results = new DocumentListViewModel(list, true);

            JsonResult result = Json(new
            {
                data = results.List.ToArray()
            }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }

        #endregion

        #region Requested Info List
        [HttpGet]
        [Route("Documents/RequestedInfo")]
        public ActionResult RequestedInfoIndex()
        {
            var model = new DocumentListViewModel();

            return View("List/User/Index", model);
        }

        [HttpGet]
        public PartialViewResult RequestedInfoTable()
        {
            var model = new DocumentListViewModel();

            return PartialView("List/User/_Table", model);
        }

        [HttpGet]
        public ActionResult RequestedInfoData()
        {
            using var db = new VPContext();
            var user = db.GetCurrentUser();

            var workFlows = db.WorkFlowUsers
                            .Include("Sequence")
                            .Include("Sequence.WorkFlow")
                            .Include("Sequence.WorkFlow.APDocuments")
                            .Where(f => f.AssignedTo == user.Id && f.Sequence.Active && f.Sequence.WorkFlow.APDocuments.Any())
                            .Select(s => s.Sequence.WorkFlow)
                            .Distinct();
            var docs = workFlows.SelectMany(s => s.APDocuments).Distinct().ToList();

            var results = new DocumentListViewModel(docs, true);

            JsonResult result = Json(new
            {
                data = results.List.ToArray()
            }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }
        #endregion

        #region Full List
        [HttpGet]
        [Route("Documents")]
        public ActionResult AllIndex()
        {
            ExchEmail.OpenEmail();
            var model = new DocumentListViewModel();

            return View("List/All/Index", model);
        }


        [HttpGet]
        public PartialViewResult AllTable()
        {
            var model = new DocumentListViewModel();

            return PartialView("List/All/_Table", model);
        }

        [HttpGet]
        public ActionResult AllData()
        {
            using var db = new VPContext();
            var comp = db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo);
            var list = db.APDocuments.Where(f => f.APCo == comp.APCo).ToList();
            var results = new DocumentListViewModel(list);

            JsonResult result = Json(new
            {
                data = results.List.ToArray()
            }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }

        #endregion

        #region Document Status
        [HttpGet]
        public ActionResult ActionPanel(byte apco, int docId)
        {
            using var db = new VPContext();
            var entity = db.APDocuments.FirstOrDefault(f => f.APCo == apco && f.DocId == docId);
            var model = new portal.Areas.AccountsPayable.Models.Document.WizardFormViewModel(entity, this);

            ViewBag.ViewOnly = !entity.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);

            return PartialView("Forms/Action/_Panel", model);
        }

        [HttpGet]
        public ActionResult ActionForm(byte apco, int docId)
        {
            using var db = new VPContext();
            var entity = db.APDocuments.FirstOrDefault(f => f.APCo == apco && f.DocId == docId);
            var model = new portal.Areas.AccountsPayable.Models.Document.WizardFormViewModel(entity, this);

            ViewBag.ViewOnly = !entity.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);

            return PartialView("Forms/Action/_Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(byte apco, int docId, byte status)
        {
            using var db = new VPContext();
            var document = db.APDocuments
                .Include("WorkFlow")
                .Include("WorkFlow.Sequences")
                .Include("WorkFlow.Sequences.AssignedUsers")
                .Include("StatusLogs")
                .Include("Lines")
                .Include("Vendor")
                .Include("APCompanyParm")
                .Include("APCompanyParm.HQCompanyParm")
                .FirstOrDefault(f => f.APCo == apco && f.DocId == docId);
            if (document == null)
                return Json(new { success = "false" });

            var model = new DocumentViewModel(document);

            ModelState.Clear();
            if (status != 6 && status != 5)
            {
                TryValidateModelRecursive(model);
            }
            if (model.ContainsErrors)
            {
                ModelState.AddModelError("", "Review Errors");
            }

            if (ModelState.IsValid)
            {
                document.StatusId = status;
                try
                {
                    db.BulkSaveChanges();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.InnerException.Message);
                }
                if (document.Status == DB.APDocumentStatusEnum.Processed && document?.Batch?.InUseBy != null && ModelState.IsValid)
                {
                    document.Batch.InUseBy = null;
                    db.BulkSaveChanges();
                }
            }
            var isInWorkFlow = document.WorkFlow.IsUserInWorkFlow(db.CurrentUserId);
            model = new DocumentViewModel(document);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors(), isInWorkFlow });
        }

        [HttpGet]
        public ActionResult RequestInfoModel(byte apco, int docId)
        {
            using var db = new VPContext();
            var results = db.APDocuments.FirstOrDefault(f => f.APCo == apco && f.DocId == docId);

            var model = new DocumentRequestInfoViewModel(results);
            return PartialView("Forms/RequestInfo/_Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RequestInfoModel(DocumentRequestInfoViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            using var db = new VPContext();
            model.ProcessUpdate(db, ModelState, ControllerContext);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        #endregion

        #region Document List Shared
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult DocumentAdd()
        {
            using var db = new VPContext();
            var apCompany = db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo).APCompanyParm;

            foreach (string fileName in Request.Files)
            {
                var fileUpload = Request.Files[fileName];
                var document = apCompany.AddDocument(fileUpload);
                db.SaveChanges(ModelState);
            }

            return Json(new {  });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DocumentDelete(byte apco, int docId)
        {
            using var db = new VPContext();

            var document = db.APDocuments.FirstOrDefault(f => f.APCo == apco && f.DocId == docId);

            if (document != null)
            {
                document.Attachment.Files.ToList().ForEach(e => e.Delete());
                db.APDocuments.Remove(document);
                db.SaveChanges(ModelState);
            }

            return Json(new { success = ModelState.IsValidJson() });
        }
        #endregion

        #region Document Form
        [HttpGet]
        public ActionResult DocumentPanel(byte apco, int docId)
        {
            using var db = new VPContext();
            var document = db.APDocuments.FirstOrDefault(f => f.APCo == apco && f.DocId == docId);
            var controller = (Controller)this;
            if (document.Forum == null)
                document.GetForum();

            var model = new DocumentFormViewModel(document, controller);

            return PartialView("Forms/_Panel", model);
        }

        [HttpGet]
        public ActionResult DocumentForm(byte apco, int docId)
        {
            using var db = new VPContext();
            var document = db.APDocuments.FirstOrDefault(f => f.APCo == apco && f.DocId == docId);
            var controller = (Controller)this;

            var model = new DocumentFormViewModel(document, controller);

            return PartialView("Forms/_Form", model);
        }

        #endregion

        #region Document Info Form
        [HttpGet]
        public ActionResult InfoForm(byte apco, int docId)
        {
            using var db = new VPContext();
            var document = db.APDocuments.FirstOrDefault(f => f.APCo == apco && f.DocId == docId);

            var model = new DocumentInfoViewModel(document);
            return PartialView("Forms/Info/_Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult InfoUpdate(DocumentInfoViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });
            ModelState.Clear();
            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InfoValidate(DocumentInfoViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            ModelState.Clear();
            TryValidateModel(model);
            model.Validate(ModelState);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Document Lines
        [HttpGet]
        public ActionResult LineListPanel(byte apco, int docId)
        {
            using var db = new VPContext();
            var document = db.APDocuments.FirstOrDefault(f => f.APCo == apco && f.DocId == docId);
            var model = new DocumentLineListViewModel(document);
            return PartialView("Forms/Lines/_Panel", model);
        }

        [HttpGet]
        public ActionResult LineListTable(byte apco, int docId)
        {
            using var db = new VPContext();
            var document = db.APDocuments.FirstOrDefault(f => f.APCo == apco && f.DocId == docId );
            var model = new DocumentLineListViewModel(document);
            return PartialView("Forms/Lines/_Table", model);
        }

        [HttpGet]
        public PartialViewResult LineListAdd(byte apco, int docId)
        {
            using var db = new VPContext();
            var document = db.APDocuments.FirstOrDefault(f => f.APCo == apco && f.DocId == docId);
            var newObj = document.AddLine();
            db.SaveChanges(ModelState);

            var result = new DocumentLineViewModel(newObj);
            ViewBag.tableRow = "True";
            return PartialView("Forms/Lines/_TableRow", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportPOLines(byte apco, int docId)
        {
            using var db = new VPContext();
            var document = db.APDocuments.FirstOrDefault(f => f.APCo == apco && f.DocId == docId);

            if (document.PurchaseOrder != null)
            {
                document.Lines.ToList().ForEach(line => document.Lines.Remove(line));
                document.PurchaseOrder.Items.ToList().ForEach(item => document.AddLine(item));
                db.SaveChanges(ModelState);
            }

            return Json(new { success = ModelState.IsValidJson() });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LineListDelete(byte apco, int docId, int lineId)
        {
            using var db = new VPContext();
            var document = db.APDocuments.FirstOrDefault(f => f.APCo == apco && f.DocId == docId);
            var line = document.Lines.FirstOrDefault(f => f.LineId == lineId);
            if (line != null)
            {
                document.Lines.Remove(line);
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }
        #endregion

        #region Document Summary
        [HttpGet]
        public ActionResult SummaryPanel(byte apco, int docId)
        {
            using var db = new VPContext();
            var document = db.APDocuments.FirstOrDefault(f => f.APCo == apco && f.DocId == docId);
            var model = new DocumentFormViewModel(document);

            return PartialView("Forms/Summary/_Panel", model);
        }

        [HttpGet]
        public ActionResult SummaryForm(byte apco, int docId)
        {
            using var db = new VPContext();
            var document = db.APDocuments.FirstOrDefault(f => f.APCo == apco && f.DocId == docId);
            var model = new DocumentFormViewModel(document);

            return PartialView("Forms/Summary/_Form", model);
        }
        #endregion

        #region Line Forms
        [HttpGet]
        public ActionResult LinePanel(byte apco, int docId, int lineId)
        {
            using var db = new VPContext();
            var entity = db.APDocumentLines.FirstOrDefault(f => f.APCo == apco && f.DocId == docId && f.LineId == lineId);
            var model = new DocumentLineViewModel(entity);

            //var userId = db.CurrentUserId;
            //ViewBag.ViewOnly = entity.WorkFlows.Any(w => w.AssignedTo == userId && w.Bid.Status == w.Status && w.Active == "Y") ? "False" : "True";

            return PartialView("Forms/Lines/Info/_Panel", model);
        }

        [HttpGet]
        public ActionResult LineForm(byte apco, int docId, int lineId)
        {
            using var db = new VPContext();
            var entity = db.APDocumentLines.FirstOrDefault(f => f.APCo == apco && f.DocId == docId && f.LineId == lineId);
            var model = new DocumentLineViewModel(entity);

            //var userId = db.CurrentUserId;
            //ViewBag.ViewOnly = entity.WorkFlows.Any(w => w.AssignedTo == userId && w.Bid.Status == w.Status && w.Active == "Y") ? "False" : "True";

            return PartialView("Forms/Lines/Info/_Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LineUpdate(DocumentLineViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult LineValidate(DocumentLineViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            ModelState.Clear();
            TryValidateModel(model);
            //model.Validate(ModelState);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion


    }
}