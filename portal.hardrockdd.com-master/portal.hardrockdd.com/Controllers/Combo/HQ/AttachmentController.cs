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
//    public class AttachmentController : BaseController
//    {
       

//        #region Original View
//        [HttpGet]
//        public ActionResult Panel(byte hqco, string tableName, long keyId, Guid? uniqueAttchID)
//        {
//            using var db = new VPContext();
//            var attachment = HQAttachment.FindCreate(hqco, uniqueAttchID, tableName, keyId, db);
//            uniqueAttchID = attachment.UniqueAttchID;
//            var model = new AttachmentListViewModel(hqco, tableName, keyId, uniqueAttchID);

//            return PartialView(model);
//        }

//        [HttpGet]
//        public ActionResult Table(byte hqco, string tableName, long keyId, Guid? uniqueAttchID)
//        {
//            using var db = new VPContext();
//            var attachment = HQAttachment.FindCreate(hqco, uniqueAttchID, tableName, keyId, db);
//            uniqueAttchID = attachment.UniqueAttchID;
//            var model = new AttachmentListViewModel(hqco, tableName, keyId, uniqueAttchID);

//            return PartialView(model);
//        }


//        [HttpGet]
//        public PartialViewResult TableRows(byte hqco, string tableName, long keyId, Guid? uniqueAttchID)
//        {
//            var model = new AttachmentListViewModel(hqco, tableName, keyId, uniqueAttchID);

//            return PartialView(model);
//        }

//        [HttpGet]
//        [AllowAnonymous]
//        public ActionResult Open(int? attachmentId)
//        {
//            try
//            {
//                if (attachmentId == 0 || attachmentId == null)
//                {
//                    attachmentId = 256253; //Image not found Id 
//                }
//                using var db = new VPContext();
//                var file = db.HQAttachmentFiles.FirstOrDefault(f => f.AttachmentId == attachmentId);
//                var ext = Path.GetExtension(file.OrigFileName);
//                var name = Path.GetFileNameWithoutExtension(file.OrigFileName);
//                var ms = file.GetMemoryStream();
//                string mimeType = MimeMapping.GetMimeMapping(file.OrigFileName);
//                var cd = new System.Net.Mime.ContentDisposition
//                {
//                    FileName = file.OrigFileName,
//                    Inline = true,
//                };
//                Response.AppendHeader("Content-Disposition", cd.ToString());
//                return new FileStreamResult(ms, contentType: mimeType);
//            }
//            catch (Exception)
//            {
//                Response.StatusCode = 404;
//                return Json(attachmentId, JsonRequestBehavior.AllowGet);
//                //return PartialView();
//            }
//        }

//        [HttpPost]
//        public ActionResult DownloadFolder(List<AttachmentViewModel> List)
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

//            //var ms = new MemoryStream();
//            //using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
//            //{
//            //    var rootFolders = List.Where(f => !List.Any(a => a.FolderId == f.ParentId && a.AttachmentId == 0) && f.AttachmentId == 0).ToList();
//            //    foreach (var folder in rootFolders)
//            //    {
//            //        ConvertListToZip(zip, folder, List, folder.Description);
//            //    }
//            //}
//            //ms.Position = 0;
//            //var oResult = new FileStreamResult(ms, "application/zip")
//            //{
//            //    FileDownloadName = "download.zip"
//            //};
//            //return oResult;
//        }


//        [HttpGet]
//        public ActionResult DownloadFolder()
//        {
//            var memKey = "DownloadList_" + StaticFunctions.GetUserId();
//            if (!(MemoryCache.Default[memKey] is List<AttachmentViewModel> List))
//            {
//                Response.StatusCode = 404;
//                return PartialView();
//            }

//            if (List.Count == 1)
//            {
//                using var db = new VPContext();
//                using var dbAttch = new DB.Infrastructure.VPAttachmentDB.Data.VPAttachmentsContext();
//                var item = List.FirstOrDefault();

//                var file = db.HQAttachmentFiles.FirstOrDefault(f => f.AttachmentId == item.AttachmentId);
//                //var attachment = dbAttch.Attachments.FirstOrDefault(f => f.AttachmentID == file.AttachmentId);
//                //var data = attachment.AttachmentData;

//                //var ext = Path.GetExtension(file.OrigFileName);
//                //var name = Path.GetFileNameWithoutExtension(file.OrigFileName);
//                //string mimeType = MimeMapping.GetMimeMapping(file.OrigFileName);
//                var cd = new System.Net.Mime.ContentDisposition
//                {
//                    FileName = file.OrigFileName,
//                    Inline = true,
//                };
//                Response.AppendHeader("Content-Disposition", cd.ToString());
//                return File(file.GetMemoryStream(), System.Net.Mime.MediaTypeNames.Application.Octet);
//            }
//            var ms = new MemoryStream();
//            var downloadFileName = "";
//            using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
//            {
//                var rootFolders = List.Where(f => !List.Any(a => a.FolderId == f.ParentId && a.AttachmentId == 0) && f.AttachmentId == 0).ToList();
//                if (rootFolders.Count == 0)
//                {
//                    rootFolders = List.GroupBy(g => g.ParentId).Select(s => new AttachmentViewModel()
//                    {
//                        Description = "root",
//                        FolderId = s.Key,
//                        HQCo = s.FirstOrDefault().HQCo,

//                    }).ToList();
//                }
//                downloadFileName = string.Format("{0}_Download.zip", List.FirstOrDefault().UniqueAttchID);
//                foreach (var folder in rootFolders)
//                {
//                    ConvertListToZip(zip, folder, List, folder.Description);
//                }
//            }
//            ms.Position = 0;
//            var oResult = new FileStreamResult(ms, "application/zip")
//            {
//                FileDownloadName = downloadFileName// "download.zip"
//            };
//            return oResult;
//        }


//        [HttpGet]
//        public ActionResult GetDownloadFolder()
//        {
//            var ms = new MemoryStream();
//            using var db = new VPContext();
//            var job = db.Jobs.FirstOrDefault(f => f.JobId == "16-1052-19");
//            var attList = new AttachmentListViewModel(job);
//            var list = AttachmentListViewModel.FlattenTreeList(attList.Tree).Where(f => f.Description != "root").ToList();
//            using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
//            {
//                var rootFolders = list.Where(f => !list.Any(a => a.FolderId == f.ParentId && a.AttachmentId == 0) && f.AttachmentId == 0).ToList();
//                foreach (var folder in rootFolders)
//                {
//                    ConvertListToZip(zip, folder, list, folder.Description);
//                }
//            }
//            ms.Position = 0;
//            var oResult = new FileStreamResult(ms, "application/zip")
//            {
//                FileDownloadName = "download.zip"
//            };
//            return oResult;
//        }

//        public void ConvertListToZip(ZipArchive archive, AttachmentViewModel folder, List<AttachmentViewModel> List,string folderPath)
//        {
//            if (archive == null)
//            {
//                throw new System.ArgumentNullException(nameof(archive));
//            }
//            if (folder == null)
//            {
//                throw new System.ArgumentNullException(nameof(folder));
//            }
//            using var db = new VPContext();
//            var files = List.Where(f => (f.ParentId == folder.FolderId || f.VirtualFolderId == folder.FolderId) && f.AttachmentId != 0).ToList();

//            foreach (var item in files)
//            {                
//                var file = db.HQAttachmentFiles.FirstOrDefault(f => f.AttachmentId == item.AttachmentId);
//                var data = file.GetData();
//                var ms = new MemoryStream(data);
//                var filePath = string.Format(AppCultureInfo.CInfo(), "{0}/{1}", folderPath, item.FileName);
//                var entry = archive.CreateEntry(filePath, CompressionLevel.Fastest);
//                using (var entryStream = entry.Open())
//                {
//                    ms.CopyTo(entryStream);
//                }
//                ms.Dispose();
//            }
//            var subFolders = List.Where(f => f.ParentId == folder.FolderId && f.FolderId != folder.FolderId && f.AttachmentId == 0).ToList();
//            foreach (var subFolder in subFolders)
//            {
//                ConvertListToZip(archive, subFolder, List, string.Format(AppCultureInfo.CInfo(), "{0}/{1}", folder.Description, subFolder.Description));
//            }
//        }
        
//        [HttpPost]
//        public ActionResult Add(byte hqco, string tableName, long keyId, Guid? uniqueAttchID, string FullPath, string rootFolder)
//        {
//            if (FullPath == null)
//            {
//                throw new System.ArgumentNullException(nameof(FullPath));
//            }
//            using var db = new VPContext();

//            var attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == uniqueAttchID);            
//            var parentFolder = attachment.Folders.FirstOrDefault(f => f.Description == rootFolder);
//            if (parentFolder == null)
//                parentFolder = attachment.GetRootFolder();

//            foreach (string requestFileName in Request.Files)
//            {
//                HttpPostedFileBase fileUpload = Request.Files[requestFileName];
//                var fileName = fileUpload.FileName;
//                var dstFolder = FullPath.Replace(fileName, "");
//                if (dstFolder == "undefined")
//                {
//                    dstFolder = "";
//                }
//                var folder = attachment.BuildFolderTree(parentFolder.FolderId, dstFolder);

//                var file = folder.Files.FirstOrDefault(f => f.OrigFileName == fileName);
//                if (file == null)
//                {
//                    file = attachment.AddFile(fileUpload, folder);
//                }
//                else
//                {
//                    file.SetData(fileUpload);
//                }
//            }

//            db.SaveChanges(ModelState);

//            var model = new AttachmentListViewModel(hqco, tableName, keyId, uniqueAttchID);
//            return Json(new { Message = "Uploaded", uniqueAttchID, model });
//        }

//        [HttpPost]
//        //[ValidateAntiForgeryToken]
//        public ActionResult AddFolder(byte hqco, Guid? uniqueAttchID, int parentId, string description)
//        {
//            using var db = new VPContext();

//            var attachment = db.HQAttachments.FirstOrDefault(f => f.UniqueAttchID == uniqueAttchID);
//            var parentFolder = attachment.Folders.FirstOrDefault(f => f.FolderId == parentId);
//            if (parentFolder == null)
//                parentFolder = attachment.GetRootFolder();

//            var folder = parentFolder.AddFolder(description);
//            db.BulkSaveChanges();
//            var files = db.HQAttachmentFiles.Where(f => f.HQCo == hqco && f.FolderId == folder.FolderId).ToList();
//            var model = new AttachmentTreeViewModel(folder, files);


//            return Json(new { Message = "Created", uniqueAttchID, model });
//        }

//        [HttpPost]
//        //[ValidateAntiForgeryToken]
//        public ActionResult DeleteFolder(byte hqco, Guid? uniqueAttchID, int folderId)
//        {
//            using var db = new VPContext();
//            var folder = db.HQAttachmentFolders.FirstOrDefault(f => f.UniqueAttchID == uniqueAttchID && f.FolderId == folderId);
//            folder.DeleteSubItems();
//            db.BulkSaveChanges();
//            return Json(new { Message = "Delete", success = ModelState.IsValidJson() });
//        }

//        [HttpPost]
//        //[ValidateAntiForgeryToken]
//        public ActionResult Delete(byte hqco, int attachmentId)
//        {
//            using var db = new VPContext();
//            var file = db.HQAttachmentFiles.FirstOrDefault(f => f.HQCo == hqco && f.AttachmentId == attachmentId);
//            if (file != null)
//            {
//                file.DeleteData();
//                db.HQAttachmentFiles.Remove(file);
//            }

//            db.BulkSaveChanges();
//            return Json(new { success = ModelState.IsValidJson() });
//        }

//        [HttpPost]
//        //[ValidateAntiForgeryToken]
//        public ActionResult UpdateFolder(byte hqco, Guid? uniqueAttchID, int folderId, int parentId, string description)
//        {
//            using var db = new VPContext();
//            var folder = db.HQAttachmentFolders.FirstOrDefault(f => f.HQCo == hqco && f.UniqueAttchID == uniqueAttchID && f.FolderId == folderId);
//            folder.Description = description;
//            folder.ParentId = parentId;
//            db.BulkSaveChanges();

//            return Json(new { Message = "Created", success = ModelState.IsValidJson() });
//        }

//        [HttpPost]
//        //[ValidateAntiForgeryToken]
//        public ActionResult UpdateFile(byte hqco, int attachmentId, int folderId, int parentId, string description)
//        {
//            using var db = new VPContext();
//            var file = db.HQAttachmentFiles.FirstOrDefault(f => f.HQCo == hqco && f.AttachmentId == attachmentId);
//            var attachmentTypes = db.AttachmentTypes.Where(f => f.TableId == file.TableName).ToList();
//            var folder = db.HQAttachmentFolders.FirstOrDefault(f => f.HQCo == hqco && f.UniqueAttchID == file.UniqueAttchID && f.FolderId == parentId);
//            var ext = Path.GetExtension(description);
//            var name = Path.GetFileNameWithoutExtension(description);
//            if (string.IsNullOrEmpty(ext))
//            {
//                ext = Path.GetExtension(file.Description);
//                description += ext;
//            }
//            file.Description = description; 
//            file.FolderId = parentId;
//            if (folder != null)
//            {
//                file.AttachmentTypeID = attachmentTypes.FirstOrDefault(f => f.TableId == file.TableName & f.Description == folder.Description)?.AttachmentTypeID;
//            }
//            db.BulkSaveChanges();

//            return Json(new { Message = "Created", success = ModelState.IsValidJson() });
//        }

//        #endregion
//    }
//}