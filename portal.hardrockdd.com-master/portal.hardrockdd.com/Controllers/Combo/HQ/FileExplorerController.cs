//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.Attachment;
//using portal.Repository.VP.HQ;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.IO.Compression;
//using System.Linq;
//using System.Runtime.Caching;
//using System.Web;
//using System.Web.Mvc;

//namespace portal.Controllers.VP.HQ
//{
//    [Authorize]
//    public class FileExplorerController : BaseController
//    {
//        [HttpGet]
//        public ActionResult FileExplorerPanel(Guid uniqueAttchID)
//        {
//            using var db = new VPContext();
//            var attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == uniqueAttchID);
//            var folder = attachment.GetRootFolder();
//            attachment.AssignFilesToRoot();

//            var model = new Models.Views.HQ.Attachment.FileExplorerListViewModel(folder);

//            return PartialView("../HQ/Attachment/Explorer/Panel", model);
//        }

//        [HttpGet]
//        public ActionResult FileExplorerForm(Guid uniqueAttchID, int folderId, DB.ExplorerViewTypeEnum viewType)
//        {
//            using var db = new VPContext();
//            var attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == uniqueAttchID);
//            var folder = attachment.Folders.FirstOrDefault(f => f.FolderId == folderId);
//            var model = new Models.Views.HQ.Attachment.FileExplorerListViewModel(folder, viewType);

//            return PartialView("../HQ/Attachment/Explorer/Folder", model);
//        }

//        [HttpGet]
//        public ActionResult FileExplorerFolder(Guid uniqueAttchID, int folderId, DB.ExplorerViewTypeEnum viewType)
//        {
//            using var db = new VPContext();
//            var attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == uniqueAttchID);
//            var folder = attachment.Folders.FirstOrDefault(f => f.FolderId == folderId);
//            var model = new Models.Views.HQ.Attachment.FileExplorerListViewModel(folder, viewType);

//            return PartialView("../HQ/Attachment/Explorer/Folder", model);
//        }




//        [HttpPost]
//        public ActionResult Download(List<Models.Views.HQ.Attachment.FileExplorerViewModel> List)
//        {
//            if (List == null)
//            {
//                Response.StatusCode = 404;
//                return PartialView();
//            }


//            var memKey = "DownloadList_" + StaticFunctions.GetUserId();
//            ObjectCache systemCache = MemoryCache.Default;
//            CacheItemPolicy policy = new CacheItemPolicy
//            {
//                AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(5)
//            };
//            systemCache.Set(memKey, List, policy);

//            return Json(new { success = true });
//        }

//        [HttpGet]
//        public ActionResult Download()
//        {
//            var memKey = "DownloadList_" + StaticFunctions.GetUserId();
//            if (!(MemoryCache.Default[memKey] is List<Models.Views.HQ.Attachment.FileExplorerViewModel> list))
//            {
//                Response.StatusCode = 404;
//                return PartialView();
//            }

//            using var db = new VPContext();
//            if (list.Count == 1 && list.Any(f => f.ObjectType == DB.ExplorerObjectTypeEnum.File))
//            {
//                var attachmentId = list.FirstOrDefault().AttachmentId;
//                var file = db.HQAttachmentFiles.FirstOrDefault(f => f.AttachmentId == attachmentId);
//                var cd = new System.Net.Mime.ContentDisposition
//                {
//                    FileName = file.OrigFileName,
//                    Inline = true,
//                };
//                Response.AppendHeader("Content-Disposition", cd.ToString());
//                return File(file.GetMemoryStream(), System.Net.Mime.MediaTypeNames.Application.Octet);
//            }

//            var ms = new MemoryStream();
//            using ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Create, true);
//            foreach (var item in list)
//            {
//                var attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == item.UniqueAttchId);

//                if (item.ObjectType == DB.ExplorerObjectTypeEnum.File)
//                {
//                    var file = attachment.Files.FirstOrDefault(f => f.AttachmentId == item.AttachmentId);
//                    zip.AddDBAttachment(file);
//                }
//                else if (item.ObjectType == DB.ExplorerObjectTypeEnum.Folder)
//                {
//                    var folder = attachment.Folders.FirstOrDefault(f => f.FolderId == item.FolderId);
//                    zip.AddDBFolder(folder);
//                }
//            }
//            zip.Dispose();
//            ms.Position = 0;
//            var oResult = new FileStreamResult(ms, "application/zip")
//            {
//                FileDownloadName = "download.zip"
//            };
//            return oResult;
//        }

//        [HttpPost]
//        public ActionResult Add(Guid uniqueAttchID, int folderId, string folderPath, bool IsThumbnail)
//        {
//            using var db = new VPContext();

//            var attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == uniqueAttchID);
//            var folder = new HQAttachmentFolder();
//            foreach (string requestFileName in Request.Files)
//            {
//                HttpPostedFileBase fileUpload = Request.Files[requestFileName];
//                var fileName = fileUpload.FileName;
//                var dstFolder = folderPath.Replace(fileName, "");
//                if (dstFolder == "undefined")
//                {
//                    dstFolder = "";
//                }
//                folder = attachment.BuildFolderTree(folderId, dstFolder);

//                var file = folder.Files.FirstOrDefault(f => f.OrigFileName == fileName);
//                if (file == null)
//                {
//                    file = folder.AddFile(fileUpload);                    
//                }
//                else
//                {
//                    file.SetData(fileUpload);
//                }
//            }

//            db.SaveChanges(ModelState);

//            var model = new Models.Views.HQ.Attachment.FileExplorerListViewModel(folder);

//            return Json(new { Message = "Uploaded", uniqueAttchID, model });
//        }


//        [HttpGet]
//        public ActionResult AddFolder(Guid uniqueAttchID, int folderId)
//        {
//            using var db = new VPContext();

//            var attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == uniqueAttchID);
//            var folder = attachment.Folders.FirstOrDefault(f => f.FolderId == folderId);
//            if (folder == null)
//                folder.Attachment.GetRootFolder();

//            var newFolder = folder.AddFolder("New Folder");
//            db.BulkSaveChanges();

//            var model = new Models.Views.HQ.Attachment.FileExplorerViewModel(newFolder);
//            model.IsNew = true;

//            return PartialView("../HQ/Attachment/Explorer/Views/List/TableRow", model);
//        }

//        //ItemUpdate

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult ItemUpdate(Models.Views.HQ.Attachment.FileExplorerViewModel model)
//        {
//            using var db = new VPContext();
//            if (model.ObjectType == DB.ExplorerObjectTypeEnum.File)
//            {
//                var attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == model.UniqueAttchId);
//                var file = attachment.Files.FirstOrDefault(f => f.AttachmentId == model.AttachmentId);
//                if (file != null)
//                {
//                    if (file.Description != model.Title)
//                    {
//                        var newName = Path.GetFileNameWithoutExtension(model.Title);
//                        newName = string.Format("{0}{1}", newName, Path.GetExtension(file.OrigFileName));

//                        file.Description = newName;

//                        model.Title = newName;
//                    }
                    
//                }

//                db.SaveChanges(ModelState);
//            }
//            else
//            {
//                var attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == model.UniqueAttchId);
//                var folder = attachment.Folders.FirstOrDefault(f => f.FolderId == model.FolderId);
//                if (folder != null)
//                {
//                    folder.tDescription = model.Title;
//                }

//                db.SaveChanges(ModelState);
//            }


//            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
//        }

//        [HttpPost]
//        //[ValidateAntiForgeryToken]
//        public ActionResult DeleteFile(Guid uniqueAttchID, int attachmentId)
//        {
//            using var db = new VPContext();
//            var attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == uniqueAttchID);
//            var file = attachment.Files.FirstOrDefault(f => f.AttachmentId == attachmentId);

//            if (file != null)
//            {
//                db.HQAttachmentFiles.Remove(file);
//                db.SaveChanges(ModelState);
//            }

//            return Json(new { success = ModelState.IsValidJson() });
//        }


//        [HttpPost]
//        //[ValidateAntiForgeryToken]
//        public ActionResult ItemDelete(Models.Views.HQ.Attachment.FileExplorerViewModel model)
//        {
//            using var db = new VPContext();
//            if (model.ObjectType == DB.ExplorerObjectTypeEnum.File)
//            {
//                var attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == model.UniqueAttchId);
//                var file = attachment.Files.FirstOrDefault(f => f.AttachmentId == model.AttachmentId);

//                if (file != null)
//                {
//                    file.DeleteData();
//                    db.HQAttachmentFiles.Remove(file);
//                    db.SaveChanges(ModelState);
//                }

//            }
//            else
//            {
//                var attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == model.UniqueAttchId);
//                var folder = attachment.Folders.FirstOrDefault(f => f.FolderId == model.FolderId);
//                folder.DeleteSubItems();
//                attachment.Folders.Remove(folder);

//                db.SaveChanges(ModelState);
//            }

//            return Json(new { success = ModelState.IsValidJson() });
//        }
//    }
//}