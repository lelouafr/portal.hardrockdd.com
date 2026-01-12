using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;

namespace portal.Areas.Attachment.Controllers
{
    [RouteArea("Attachment")]
    public class ExplorerController : portal.Controllers.BaseController
    {
        [HttpGet]
        public ActionResult Panel(Guid uniqueAttchID)
        {
            using var db = new VPContext();
            var attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == uniqueAttchID);
            var folder = attachment.GetRootFolder();
            attachment.AssignFilesToRoot();
            attachment.SyncFromSource();
            db.BulkSaveChanges();
            var model = new Models.ExplorerListViewModel(folder);

            return PartialView("_Panel", model);
        }

        [HttpGet]
        public ActionResult PanelNoPreview(Guid uniqueAttchID)
        {
            using var db = new VPContext();
            var attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == uniqueAttchID);
            var folder = attachment.GetRootFolder();
            attachment.AssignFilesToRoot();
            attachment.SyncFromSource();
            db.BulkSaveChanges();
            var model = new Models.ExplorerListViewModel(folder);
            model.Preview = false;
            return PartialView("_Panel", model);
        }

        [HttpGet]
        public ActionResult Form(Guid uniqueAttchID, int folderId, DB.ExplorerViewTypeEnum viewType, bool? preview = true)
        {
            using var db = new VPContext();
            var attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == uniqueAttchID);
            var folder = attachment.Folders.FirstOrDefault(f => f.FolderId == folderId);
            if (folder == null)
                folder = attachment.GetRootFolder();
            folder.SyncFromSource();
            db.BulkSaveChanges();
            var model = new Models.ExplorerListViewModel(folder, viewType);
            model.Preview = preview ?? true;

            return PartialView("_Form", model);
        }

        [HttpGet]
        public ActionResult Folder(Guid uniqueAttchID, int folderId, DB.ExplorerViewTypeEnum viewType, bool? preview = true)
        {
            using var db = new VPContext();
            var attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == uniqueAttchID);
            var folder = attachment.Folders.FirstOrDefault(f => f.FolderId == folderId);
            if (folder == null)
                folder = attachment.GetRootFolder();
            folder.SyncFromSource();
            db.BulkSaveChanges();
            var model = new Models.ExplorerListViewModel(folder, viewType);
            model.Preview = preview ?? true;

            return PartialView("_Form", model);
        }

        [HttpGet]
        public ActionResult Preview(Guid uniqueAttchID)
        {
            using var db = new VPContext();
            var attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == uniqueAttchID);
            var folder = attachment.GetRootFolder();
            var model = new Models.ExplorerListViewModel(folder);

            return PartialView("_FilePreview", model);
        }

        [HttpPost]
        public ActionResult Download(List<Models.ExplorerViewModel> List)
        {
            if (List == null)
            {
                Response.StatusCode = 404;
                return PartialView();
            }


            var memKey = "DownloadList_" + StaticFunctions.GetUserId();
            ObjectCache systemCache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(5)
            };
            systemCache.Set(memKey, List, policy);

            return Json(new { success = true });
        }

        [HttpGet]
        public ActionResult Download()
        {
            var memKey = "DownloadList_" + StaticFunctions.GetUserId();
            if (!(MemoryCache.Default[memKey] is List<Models.ExplorerViewModel> list))
            {
                Response.StatusCode = 404;
                return PartialView();
            }

            using var db = new VPContext();
            if (list.Count == 1 && list.Any(f => f.ObjectType == DB.ExplorerObjectTypeEnum.File))
            {
                var attachmentId = list.FirstOrDefault().AttachmentId;
                var file = db.HQAttachmentFiles.FirstOrDefault(f => f.AttachmentId == attachmentId);
                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = file.OrigFileName,
                    Inline = true,
                };
                Response.AppendHeader("Content-Disposition", cd.ToString());
                return File(file.GetMemoryStream(), System.Net.Mime.MediaTypeNames.Application.Octet);
            }

            var ms = new MemoryStream();
            using ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Create, true);
            foreach (var item in list)
            {
                var attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == item.UniqueAttchId);

                if (item.ObjectType == DB.ExplorerObjectTypeEnum.File)
                {
                    var file = attachment.Files.FirstOrDefault(f => f.AttachmentId == item.AttachmentId);
                    zip.AddDBAttachment(file);
                }
                else if (item.ObjectType == DB.ExplorerObjectTypeEnum.Folder)
                {
                    var folder = attachment.Folders.FirstOrDefault(f => f.FolderId == item.FolderId);
                    zip.AddDBFolder(folder);
                }
            }
            zip.Dispose();
            ms.Position = 0;
            var oResult = new FileStreamResult(ms, "application/zip")
            {
                FileDownloadName = "download.zip"
            };
            return oResult;
        }

        [HttpPost]
       // [ValidateAntiForgeryToken]
        public ActionResult Add(Guid uniqueAttchID, string folderPath, bool IsThumbnail, int folderId = 0)
        {
            using var db = new VPContext();

            var attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == uniqueAttchID);
            var folder = attachment.AddFiles(Request.Files, folderId, folderPath);           
            var model = new Models.ExplorerListViewModel(folder);

            return Json(new { Message = "Uploaded", uniqueAttchID, model });
        }

        [HttpGet]
        public ActionResult AddFolder(Guid uniqueAttchID, int folderId)
        {
            using var db = new VPContext();
            var attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == uniqueAttchID);
            var folder = attachment.Folders.FirstOrDefault(f => f.FolderId == folderId);
            if (folder == null)
                folder = attachment.GetRootFolder();

            var newFolder = folder.AddFolder("New Folder");
            db.BulkSaveChanges();

            var model = new Models.ExplorerViewModel(newFolder)
            {
                IsNew = true
            };

            return PartialView("Views/List/_TableRow", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ItemUpdate(Models.ExplorerViewModel model)
        {
            if (model == null)
                return Json(new { success = "false", model, errorModel = ModelState.ModelErrors() });
            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ItemMove(Models.ExplorereMoveViewMode model)
        {
            if (model == null)
                return Json(new { success = "false", model, errorModel = ModelState.ModelErrors() });

            using var db = new VPContext();
            model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //public ActionResult DeleteFile(Guid uniqueAttchID, int attachmentId)
        //{
        //    using var db = new VPContext();
        //    var attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == uniqueAttchID);
        //    var file = attachment.Files.FirstOrDefault(f => f.AttachmentId == attachmentId);
            
        //    if (file != null)
        //    {
        //        db.HQAttachmentFiles.Remove(file);
        //        db.SaveChanges(ModelState);
        //    }

        //    return Json(new { success = ModelState.IsValidJson() });
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ItemDelete(Models.ExplorerViewModel model)
        {
            if (model == null)
                return Json(new { success = "false" });

            using var db = new VPContext();
            var attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == model.UniqueAttchId);
            if (model.ObjectType == DB.ExplorerObjectTypeEnum.File)
            {
                var file = attachment.Files.FirstOrDefault(f => f.AttachmentId == model.AttachmentId);

                if (file != null)
                {
                    file.Delete();
                    db.BulkSaveChanges();
                }
            }
            else if (model.ObjectType == DB.ExplorerObjectTypeEnum.Folder)
            {
                var folder = attachment.Folders.FirstOrDefault(f => f.FolderId == model.FolderId);
                if (folder != null)
                {
                    folder.Delete();
                    db.BulkSaveChanges();
                }
            }

            return Json(new { success = ModelState.IsValidJson() });
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult Open(int? attachmentId)
        {
            if (attachmentId == 0 || attachmentId == null)
                attachmentId = 256253; //Image not found Id 

            try
            {
                using var db = new VPContext();
                var file = db.HQAttachmentFiles.FirstOrDefault(f => f.AttachmentId == attachmentId);
                 var ms = file.GetMemoryStream();
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
                return Json(attachmentId, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpGet]
        public JsonResult FileData(int? attachmentId)
        {
            using var db = new VPContext();
            var attachment = db.HQAttachmentFiles.FirstOrDefault(f => f.AttachmentId == attachmentId);
            var results = new Models.ExplorerViewModel(attachment);

            JsonResult result = Json(new { data = results }, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;
            return result;
        }

    }
}