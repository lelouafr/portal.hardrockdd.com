using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.Document;
using portal.Repository.VP.AP;
using portal.Repository.VP.WP;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using portal.Code;

namespace portal.Controllers
{
    //[AuthorizePosition(PositionCodes = "CFO,COO,PRES,IT-DIR,FIN-AP,FIN-APMGR,FIN-CTRL,OF-GA")]
    [ControllerAuthorize]
    public class APDocumentController : BaseController
    {

        #region Document List

        [HttpGet]
        [Route("AP/Documents")]
        public ActionResult Index(int status)
        {
           //ExchEmail.OpenEmail();
            using var db = new VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            var model = new vDocumentListViewModel(comp.HQCo, "udAPDH", (DB.APDocumentStatusEnum)status);

            ViewBag.Partial = false;
            ViewBag.DataAction = "Data";
            ViewBag.DataController = "APDocument";
            return ((DB.APDocumentStatusEnum)status) switch
            {
                DB.APDocumentStatusEnum.New => RedirectToAction("NewIndex", "APDocument"),
                DB.APDocumentStatusEnum.Filed => RedirectToAction("FiledIndex", "APDocument"),
                DB.APDocumentStatusEnum.RequestedInfo => RedirectToAction("RequestedInfoIndex", "APDocument"),
                DB.APDocumentStatusEnum.All => RedirectToAction("AllInfoIndex", "APDocument"),
                _ => RedirectToAction("NewIndex", "APDocument"),
            };
        }

        [HttpGet]
        [Route("AP/Documents/New")]
        public ActionResult NewIndex()
        {
            //ExchEmail.OpenEmail();
            var comp = StaticFunctions.GetCurrentCompany();
            var model = new vDocumentListViewModel(comp.HQCo, "udAPDH", DB.APDocumentStatusEnum.New);

            ViewBag.Partial = false;
            ViewBag.DataAction = "Data";
            ViewBag.DataController = "APDocument";
            return View("../AP/Document/New/Index", model);
        }

        [HttpGet]
        [Route("AP/Documents/Filed")]
        public ActionResult FiledIndex()
        {
           //ExchEmail.OpenEmail();
            var comp = StaticFunctions.GetCurrentCompany();
            var model = new vDocumentListViewModel(comp.HQCo, "udAPDH", DB.APDocumentStatusEnum.Filed);

            ViewBag.Partial = false;
            ViewBag.DataAction = "Data";
            ViewBag.DataController = "APDocument";
            return View("../AP/Document/Filed/Index", model);
        }

        [HttpGet]
        [Route("AP/Documents/RequestedInfo")]
        public ActionResult RequestedInfoIndex()
        {
           //ExchEmail.OpenEmail();
            var comp = StaticFunctions.GetCurrentCompany();
            var model = new vDocumentListViewModel(comp.HQCo, "udAPDH", DB.APDocumentStatusEnum.RequestedInfo);

            ViewBag.Partial = false;
            ViewBag.DataAction = "Data";
            ViewBag.DataController = "APDocument";
            return View("../AP/Document/Filed/Index", model);
        }

        [HttpGet]
        [Route("AP/Documents/All")]
        public ActionResult AllIndex()
        {
           //ExchEmail.OpenEmail();
            var comp = StaticFunctions.GetCurrentCompany();
            var model = new vDocumentListViewModel(comp.HQCo, "udAPDH", DB.APDocumentStatusEnum.All);

            ViewBag.Partial = false;
            ViewBag.DataAction = "Data";
            ViewBag.DataController = "APDocument";
            return View("../AP/Document/All/Index", model);
        }

        [HttpGet]
        public ActionResult PartialIndex(int status)
        {
            var comp = StaticFunctions.GetCurrentCompany();
            var model = new DocumentListViewModel(comp.HQCo, "udAPDH", (DB.APDocumentStatusEnum)status);
            ViewBag.Partial = true;
            return ((DB.APDocumentStatusEnum)status) switch
            {
                DB.APDocumentStatusEnum.New => PartialView("../AP/Document/New/PartialIndex", model),
                DB.APDocumentStatusEnum.Filed => PartialView("../AP/Document/Filed/PartialIndex", model),
                DB.APDocumentStatusEnum.RequestedInfo => View("../AP/Document/Filed/Index", model),
                DB.APDocumentStatusEnum.All => PartialView("../AP/Document/All/PartialIndex", model),
                _ => PartialView("../AP/Document/New/PartialIndex", model),
            };
        }

        [HttpGet]
        public ActionResult Table(int status)
        {
            using var db = new VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            var model = new vDocumentListViewModel(comp.HQCo, "udAPDH", (DB.APDocumentStatusEnum)status);
            ViewBag.DataAction = "Data";
            ViewBag.DataController = "APDocument";

            return ((DB.APDocumentStatusEnum)status) switch
            {
                DB.APDocumentStatusEnum.New => PartialView("../AP/Document/New/Table", model),
                DB.APDocumentStatusEnum.Filed => PartialView("../AP/Document/Filed/Table", model),
                DB.APDocumentStatusEnum.RequestedInfo => PartialView("../AP/Document/Filed/Table", model),
                DB.APDocumentStatusEnum.All => PartialView("../AP/Document/All/Table", model),
                _ => PartialView("../AP/Document/New/Table", model),
            };
        }

        [HttpGet]
        public ActionResult Data(int status)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var emp = StaticFunctions.GetCurrentEmployee();
            var comp = StaticFunctions.GetCurrentCompany();
            var results = new vDocumentListViewModel(comp.HQCo, "udAPDH", (DB.APDocumentStatusEnum)status);

            JsonResult result = Json(new
            {
                data = results.List.ToArray()
            }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }
        #endregion

        [HttpGet]
        public ActionResult Form(byte apco, int docId)
        {
            using var db = new VPContext();
            var results = db.APDocuments.FirstOrDefault(f => f.APCo == apco && f.DocId == docId);

            if (results.Forum == null)
            {
                results.Forum = results.AddForum();
                db.SaveChanges(ModelState);
            }

            var model = new DocumentFormViewModel(results);
            ViewBag.Partial = true;
            ViewBag.ViewOnly = model.Action.Access == DB.SessionAccess.Edit ? "False" : "True";
            return PartialView("../AP/Document/Form/Form", model);
        }

        [HttpGet]
        public ActionResult RequestInfoStatus(byte apco, int docId)
        {
            using var db = new VPContext();
            var results = db.APDocuments.FirstOrDefault(f => f.APCo == apco && f.DocId == docId);

            var model = new DocumentPendingInfoViewModel(results);
            return PartialView("../AP/Document/RequestInfo/Index", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RequestInfoStatus(DocumentPendingInfoViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            using var db = new VPContext();
            model.ProcessUpdate(db, ModelState, ControllerContext);
            //using var empRepo = new WebUserRepository();
            //var result = db.APDocuments.FirstOrDefault(f => f.APCo == model.APCo && f.DocId == model.DocId);
            
            //result.RequestedUserList = model.SendTo;
            //result.Status = DB.APDocumentStatusEnum.RequestedInfo;

            //db.SaveChanges(ModelState);
            //if (ModelState.IsValid)
            //{
            //    var line = result.Forum.AddLine();
            //    line.CreatedBy = StaticFunctions.GetUserId();
            //    line.CreatedOn = DateTime.Now;
            //    line.Comment = model.Comments;
            //    line.HtmlComment = model.Comments;
            //    var userList = model.SendTo.Split('|');
            //    foreach (var userid in userList)
            //    {
            //        var user = empRepo.GetUser(userid);
            //        line.Comment += string.Format(AppCultureInfo.CInfo(), "{2} Sent To: {0} {1}", user.FirstName, user.LastName, System.Environment.NewLine);
            //        line.HtmlComment += string.Format(AppCultureInfo.CInfo(), @"<br> <small>Sent To: {0} {1}</small>", user.FirstName, user.LastName);
            //        var form = db.WebControllers.FirstOrDefault(f => f.ControllerName == "APDocument" && f.ActionName == "RequestedInfoIndex");
            //        if (!form.Users.Any(f => f.UserId == user.Id))
            //        {
            //            var userAccess = new WebUserAccess
            //            {
            //                ControllerActionId = form.Id,
            //                UserId = user.Id,
            //                AccessLevel = (byte)DB.AccessLevelEnum.Write
            //            };

            //            form.Users.Add(userAccess);
            //        }
            //    }
            //    result.Forum.Lines.Add(line);
            //    db.SaveChanges(ModelState);
            //    EmailRequestInfo(model);
            //}
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        //private void EmailRequestInfo(DocumentPendingInfoViewModel request)
        //{
        //    if (request == null)
        //    {
        //        throw new ArgumentNullException(nameof(request));
        //    }
        //    using var db = new VPContext();
        //    var result = db.APDocuments.FirstOrDefault(f => f.APCo == request.APCo && f.DocId == request.DocId);
        //    var model = new DocumentFormViewModel(result);
        //    try
        //    {
        //        using MailMessage msg = new MailMessage()
        //        {
        //            Body = EmailHelper.RenderViewToString(ControllerContext, "../AP/Document/Email/EmailInvoiceRequestedInfo", model, false),
        //            IsBodyHtml = true,
        //            Subject = string.Format(AppCultureInfo.CInfo(), "Requested Invoice Info {0}", model.Document.APEntry.APReference),
        //        };
        //        foreach (var workflow in result.WorkFlow.CurrentSequence().AssignedUsers)
        //        {
        //            msg.To.Add(new MailAddress(workflow.AssignedUser.Email));
        //        }
        //        EmailHelper.Send(msg);
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", ex.GetBaseException().ToString());
        //    }
        //}
        

        #region Document File Management

        [HttpPost]
        public ActionResult Add(byte co, string tableName, string FullPath, string rootFolder)
        {
            using var db = new VPContext();
            //db.Database.CommandTimeout = 600;
            
            var APCo = StaticFunctions.GetCurrentCompany().APCo;
            var apParms = db.APCompanyParms.FirstOrDefault(f => f.APCo == APCo);

            var msg = "";
            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase fileUpload = Request.Files[fileName];

                var document = apParms.AddDocument(fileUpload);// APDocument.AddDocument(apParms, fileUpload);
            }
            db.SaveChanges(ModelState);

            var model = new vDocumentListViewModel((byte)APCo, tableName, DB.APDocumentStatusEnum.New);
            return Json(new { Message = msg, model });
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(byte apco, int docId)
        {
            using var db = new VPContext();

            var file = db.APDocuments.FirstOrDefault(f => f.APCo == apco && f.DocId == docId);

            if (file != null)
            {
                db.APDocuments.Remove(file);
                db.SaveChanges(ModelState);
            }

            return Json(new { success = ModelState.IsValidJson() });
        }

        [HttpGet]
        public ActionResult Open(byte apco, int docId)
        {
            try
            {
                using var db = new VPContext();

                var document = db.APDocuments.FirstOrDefault(f => f.APCo == apco && f.DocId == docId);
                if (document == null)
                {
                    Response.StatusCode = 404;
                    return PartialView();
                }
                var file = document.Attachment.Files.FirstOrDefault();
                if (file == null)
                    throw new Exception();
                var data = file.GetData();

                var name = Path.GetFileNameWithoutExtension(file.OrigFileName);
                var ms = new MemoryStream(data);

                string mimeType = MimeMapping.GetMimeMapping(file.OrigFileName);
                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = file.OrigFileName,
                    Inline = true,
                };
                Response.AppendHeader("Content-Disposition", cd.ToString());
                return new FileStreamResult(ms, contentType: mimeType);
            }
            catch (Exception)
            {
                Response.StatusCode = 404;
                return PartialView();
            }
        }

        #endregion

        #region Documnet Sequence (Header)

        [HttpGet]
        public ActionResult HeaderForm(byte apco, int docId)
        {
            using var db = new VPContext();
            var results = db.APDocuments.FirstOrDefault(f => f.APCo == apco && f.DocId == docId);

            var model = new DocumentFormViewModel(results);
            ViewBag.ViewOnly = model.Action.Access == DB.SessionAccess.Edit ? "False" : "True";

            return PartialView("../AP/Document/Header/Form", model.Sequences.List.FirstOrDefault());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateHeader(DocumentSeqViewModel model)
        {
            if (model == null)
                return Json(new { success = "false", model, errorModel = ModelState.ModelErrors() });

            model.Validate(ModelState);
            model = APDocumentSeqRepository.ProcessUpdate(model, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(byte apco, int docId, byte status)
        {
            using var db = new VPContext();
            var result = db.APDocuments
                .Include("WorkFlow")
                .Include("WorkFlow.Sequences")
                .Include("WorkFlow.Sequences.AssignedUsers")
                .Include("StatusLogs")
                .Include("Lines")
                .Include("Vendor")
                .Include("APCompanyParm")
                .Include("APCompanyParm.HQCompanyParm")
                //.Include("APCompanyParm.HQCompanyParm.Batches")
                .FirstOrDefault(f => f.APCo == apco && f.DocId == docId);
            var model = new DocumentViewModel(result);

            ModelState.Clear();
            if (status != 6 && status != 5)
            {
                TryValidateModelRecursive(model);
            }
            if (ModelState.IsValid)
            {
                result.StatusId = status;
                
                //db.SaveChanges(ModelState);
                db.BulkSaveChanges();
                if (result.Status == DB.APDocumentStatusEnum.Processed && result?.Batch?.InUseBy != null && ModelState.IsValid)
                {
                    result.Batch.InUseBy = null;
                    //db.SaveChanges(ModelState);
                    db.BulkSaveChanges();
                }
            }
            var isInWorkFlow = result.WorkFlow.IsUserInWorkFlow(StaticFunctions.GetUserId());
            model = new DocumentViewModel(result);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() , isInWorkFlow });
        }

        [HttpGet]
        public JsonResult ValidateHeader(DocumentSeqViewModel model)
        {
            //ModelState.Clear();
            model.Validate(ModelState);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        
        #region Document Line


        [HttpGet]
        public ActionResult LineTablePanel(byte apco, int docId)
        {
            using var db = new VPContext();
            var results = db.APDocuments.FirstOrDefault(f => f.APCo == apco && f.DocId == docId);
            var model = new DocumentFormViewModel(results);
            var res = model.Sequences.List.FirstOrDefault();
            ViewBag.ViewOnly = model.Action.Access == DB.SessionAccess.Edit ? "False" : "True";

            return PartialView("../AP/Document/Lines/Panel", res.Lines);
        }

        [HttpGet]
        public ActionResult LinePanel(byte apco, int docId, int lineId)
        {
            using var db = new VPContext();
            //var result = db.APDocumentLines.FirstOrDefault(f => f.APCo == co && f.DocId == docId && f.SeqId == seqId && f.LineId == lineId);
            //var model = new DocumentSeqLineViewModel(result);

            var results = db.APDocuments.FirstOrDefault(f => f.APCo == apco && f.DocId == docId);
            var model = new DocumentFormViewModel(results);
            var res = model.Sequences.List.FirstOrDefault().Lines.List.FirstOrDefault(f => f.LineId == lineId);
            ViewBag.ViewOnly = model.Action.Access == DB.SessionAccess.Edit ? "False" : "True";

            return PartialView("../AP/Document/Lines/Form/Panel", res);
        }

        [HttpGet]
        public ActionResult LineFrom(byte apco, int docId, int lineId)
        {
            using var db = new VPContext();
            //var result = db.APDocumentLines.FirstOrDefault(f => f.APCo == co && f.DocId == docId && f.SeqId == seqId && f.LineId == lineId);
            //var model = new DocumentSeqLineViewModel(result);

            var results = db.APDocuments.FirstOrDefault(f => f.APCo == apco && f.DocId == docId);
            var model = new DocumentFormViewModel(results);
            var res = model.Sequences.List.FirstOrDefault().Lines.List.FirstOrDefault(f => f.LineId == lineId);
            ViewBag.ViewOnly = model.Action.Access == DB.SessionAccess.Edit ? "False" : "True";
            return PartialView("../AP/Document/Lines/Form/Form", res);
        }

        [HttpGet]
        public ActionResult LineTable(byte apco, int docId)
        {
            using var db = new VPContext();
            //var result = db.APDocumentSeqs.FirstOrDefault(f => f.APCo == co && f.DocId == docId && f.SeqId == seqId);
            //var model = new DocumentSeqLineListViewModel(result);
            var results = db.APDocuments.FirstOrDefault(f => f.APCo == apco && f.DocId == docId);
            var model = new DocumentFormViewModel(results);
            var res = model.Sequences.List.FirstOrDefault();
            ViewBag.ViewOnly = model.Action.Access == DB.SessionAccess.Edit ? "False" : "True";

            return PartialView("../AP/Document/Lines/Table", res.Lines);
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

        [HttpGet]
        public PartialViewResult LineAdd(byte apco, int docId)
        {
            using var db = new VPContext();

            var document = db.APDocuments.FirstOrDefault(f => f.APCo == apco && f.DocId == docId);
            var result = document.AddLine();
            db.SaveChanges(ModelState);

            var model = new DocumentSeqLineViewModel(result);

            return PartialView("../AP/Document/Lines/TableRow", model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LineDelete(byte apco, int docId, int lineId)
        {
            using var db = new VPContext();
            var model = db.APDocumentLines.FirstOrDefault(f => f.APCo == apco && f.DocId == docId && f.LineId == lineId);
            if (model != null)
            {
                db.APDocumentLines.Remove(model);
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LineUpdate(DocumentSeqLineViewModel model)
        {
            using var db = new VPContext();
            var result = APDocumentSeqLineRepository.ProcessUpdate(model, db);
            db.SaveChanges(ModelState);


            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult LineValidate(DocumentSeqLineViewModel model)
        {
            ModelState.Clear();

            if (!ModelState.IsValid)
            {
                var errorModel = ModelState.ModelErrors();
                return Json(new { success = ModelState.IsValidJson(), errorModel }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Document User Review List
        [HttpGet]
        public ActionResult ReviewPanel(byte apco, int docId)
        {
            using var db = new VPContext();
            var results = db.APDocuments.FirstOrDefault(f => f.APCo == apco && f.DocId == docId);
            var model = new DocumentFormViewModel(results);
            ViewBag.ViewOnly = model.Action.Access == DB.SessionAccess.Edit ? "False" : "True";

            return PartialView("../AP/Document/Review/Panel", model);
        }

        [HttpGet]
        public ActionResult ReviewFrom(byte apco, int docId)
        {
            using var db = new VPContext();
            var results = db.APDocuments.FirstOrDefault(f => f.APCo == apco && f.DocId == docId);
            var model = new DocumentFormViewModel(results);

            ViewBag.ViewOnly = model.Action.Access == DB.SessionAccess.Edit ? "False" : "True";

            return PartialView("../AP/Document/Review/Form", model);
        }
        #endregion
    }
}