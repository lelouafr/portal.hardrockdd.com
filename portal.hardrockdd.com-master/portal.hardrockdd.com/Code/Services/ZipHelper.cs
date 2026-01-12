//using portal;
//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.IO.Compression;
//using System.Linq;
//using System.Web;

//namespace System.IO.Compression
//{
//    public static class ZipArchiveHelper
//    {

//        public static void AddDBAttachment(this ZipArchive archive, DB.Infrastructure.ViewPointDB.Data.HQAttachmentFile file)
//        {
//            if (archive == null)
//                return;
//            if (file == null)
//                return;

//            using var db = new VPContext();
//            using var dbAttch = new DB.Infrastructure.VPAttachmentDB.Data.VPAttachmentsContext();

//            var attachment = dbAttch.Attachments.FirstOrDefault(f => f.AttachmentID == file.AttachmentId);
//            if (attachment == null)
//            {
//                return;
//            }

//            var data = attachment.AttachmentData;
//            var ms = new MemoryStream(data);

//            var filePath = file.OrigFileName;
//            if (!string.IsNullOrEmpty(file.FolderPath))
//            {
//                filePath = string.Format(AppCultureInfo.CInfo(), "{0}/{1}", file.FolderPath, file.OrigFileName);
//            }

//            var entry = archive.CreateEntry(filePath, CompressionLevel.Fastest);
//            using (var entryStream = entry.Open())
//            {
//                ms.CopyTo(entryStream);
//            }
//            ms.Dispose();
//        }

//        public static void AddDBFolder(this ZipArchive archive, HQAttachmentFolder mainFolder)
//        {
//            if (mainFolder == null)
//                return;

//            foreach (var folder in mainFolder.SubFolders)
//            {
//                archive.AddDBFolder(folder);
//            }
//            foreach (var file in mainFolder.Files)
//            {
//                archive.AddDBAttachment(file);
//            }
//        }

//    }
//}