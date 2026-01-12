using DB.Infrastructure.ViewPointDB.Data;
using DB.Infrastructure.VPAttachmentDB.Data;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Repository.VP.HQ
{
    public partial class AttachmentRepository : IDisposable
    {
        private VPAttachmentsContext db = new VPAttachmentsContext();
        
        public AttachmentRepository()
        {

        }

        public static Attachment Init()
        {
            var model = new Attachment
            {

            };

            return model;
        }
        
        public static Attachment DuplicateAttachment(Attachment src, DB.Infrastructure.ViewPointDB.Data.HQAttachmentFile dstFile)
        {
            var attachment = new Attachment();
            attachment.AttachmentID = dstFile.AttachmentId;
            attachment.AttachmentData = src.AttachmentData;
            attachment.AttachmentFileType = src.AttachmentFileType;
            attachment.SaveStamp = src.SaveStamp;

            return attachment;
        }

        public Attachment GetAttachment(int AttachmentID)
        {
            var qry = db.Attachments
                        .FirstOrDefault(f => f.AttachmentID == AttachmentID);

            return qry;
        }

        public Attachment ProcessUpdate(Attachment model, ModelStateDictionary modelState)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var updObj = GetAttachment(model.AttachmentID);
            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.AttachmentData = model.AttachmentData;
                updObj.AttachmentFileType = model.AttachmentFileType;

                db.SaveChanges(modelState);
            }
            return updObj;
        }

        public static DB.Infrastructure.ViewPointDB.Data.HQAttachmentFile AddFile(byte co, string tableName, long keyId, Guid? uniqueAttchID, string FullPath, string fileName, string rootFolder,  byte[] data, VPContext db, DB.Infrastructure.VPAttachmentDB.Data.VPAttachmentsContext dbAttch)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));
            if (dbAttch == null) throw new ArgumentNullException(nameof(dbAttch));

            var userId = StaticFunctions.GetUserId();
            var attachmentTypes = db.AttachmentTypes.Where(f => f.TableId == tableName).ToList();
            if (uniqueAttchID == null)
            {
                uniqueAttchID = Guid.NewGuid();
                db.Database.ExecuteSqlCommand(string.Format(AppCultureInfo.CInfo(), "UPDATE b{0} SET UniqueAttchID = '{1}' FROM b{0} WHERE KeyID = {2}", tableName, uniqueAttchID, keyId));
            }

            char delimiter = '/';
            var directories = FullPath.Split(delimiter);
            var attachmentFolders = db.HQAttachmentFolders.Where(f => f.UniqueAttchID == uniqueAttchID).ToList();
            var parentFolder = attachmentFolders.FirstOrDefault(f => f.tDescription == rootFolder);
            if (parentFolder == null)
            {
                parentFolder = new HQAttachmentFolder();
                parentFolder.HQCo = co;
                parentFolder.UniqueAttchID = (Guid)uniqueAttchID;
                parentFolder.tDescription = "root";
                parentFolder.FolderId = attachmentFolders.DefaultIfEmpty().Max(f => f == null ? 0 : f.FolderId) + 1;

                attachmentFolders.Add(parentFolder);
                db.HQAttachmentFolders.Add(parentFolder);
            }
            var folder = parentFolder;
            int? attachmentType = null;
            foreach (var dir in directories.Take(directories.Length).ToList())
            {
                folder = attachmentFolders.FirstOrDefault(f => f.tDescription == dir);
                if (attachmentType == null)
                {
                    var attachmentTypeObj = attachmentTypes.FirstOrDefault(f => f.TableId == tableName & f.Description == dir);
                    if (attachmentTypeObj != null && attachmentType == null)
                    {
                        attachmentType = attachmentTypeObj.AttachmentTypeID;
                    }
                }
                if (folder == null)
                {
                    folder = new HQAttachmentFolder();
                    folder.HQCo = co;
                    folder.UniqueAttchID = (Guid)uniqueAttchID;
                    folder.tDescription = dir;
                    folder.FolderId = attachmentFolders.DefaultIfEmpty().Max(f => f == null ? 0 : f.FolderId) + 1;
                    if (parentFolder != null)
                    {
                        folder.ParentId = parentFolder.FolderId;
                    }
                    attachmentFolders.Add(folder);
                    db.HQAttachmentFolders.Add(folder);
                }
                parentFolder = folder;
            }
            var file = db.HQAttachmentFiles.FirstOrDefault(f => f.OrigFileName == fileName && f.UniqueAttchID == uniqueAttchID);//&& f.FolderId == parentFolder.FolderId
            if (file == null)
            {
                var attachId = HQAttachmentFile.GetNextAttachmentId();
                file = FileRepository.Init(tableName, (Guid)uniqueAttchID, keyId);
                file.HQCo = co;
                file.AttachmentId = attachId;
                file.Description = fileName;
                file.OrigFileName = fileName;
                file.FolderId = parentFolder.FolderId;
                file.AttachmentTypeID = attachmentType;
                db.HQAttachmentFiles.Add(file);

                var attachment = new Attachment();
                attachment.AttachmentData = data;
                attachment.AttachmentFileType = System.IO.Path.GetExtension(fileName);
                attachment.SaveStamp = BitConverter.GetBytes(default(DateTime).Add(DateTime.Now.TimeOfDay).Ticks);
                attachment.AttachmentID = file.AttachmentId;

                dbAttch.Attachments.Add(attachment);
            }
            else
            {
                file.FolderId = parentFolder.FolderId;
                file.AttachmentTypeID = attachmentType;// attachmentTypes.FirstOrDefault(f => f.TableId == tableName & f.Description == parentFolder.Description)?.AttachmentTypeID;

                if (data != null)
                {
                    var attachment = dbAttch.Attachments.FirstOrDefault(f => f.AttachmentID == file.AttachmentId);
                    if (attachment.AttachmentData != data)
                    {
                        attachment.AttachmentData = data;
                    }
                }
            }
            return file;
        }


        public static DB.Infrastructure.ViewPointDB.Data.HQAttachmentFile AddFile(byte co, string tableName, long keyId, Guid? uniqueAttchID, int AttachmentTypeId, HttpPostedFileBase fileUpload, VPContext db, DB.Infrastructure.VPAttachmentDB.Data.VPAttachmentsContext dbAttch)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));
            if (dbAttch == null) throw new ArgumentNullException(nameof(dbAttch));
            if (fileUpload == null) throw new ArgumentNullException(nameof(fileUpload));

            var userId = StaticFunctions.GetUserId();
            var attachmentTypes = db.AttachmentTypes.Where(f => f.TableId == tableName).ToList();
            if (uniqueAttchID == null)
            {
                uniqueAttchID = Guid.NewGuid();
                db.Database.ExecuteSqlCommand(string.Format(AppCultureInfo.CInfo(), "UPDATE b{0} SET UniqueAttchID = '{1}' FROM b{0} WHERE KeyID = {2}", tableName, uniqueAttchID, keyId));
            }

            var folder = db.HQAttachmentFolders.FirstOrDefault(f => f.UniqueAttchID == (Guid)uniqueAttchID &&  f.tDescription == "root");
            if (folder == null)
            {
                folder = new HQAttachmentFolder();
                folder.HQCo = co;
                folder.UniqueAttchID = (Guid)uniqueAttchID;
                folder.tDescription = "root";
                folder.FolderId = 0;

                db.HQAttachmentFolders.Add(folder);
            }

            var attachId = HQAttachmentFile.GetNextAttachmentId();
            var file = FileRepository.Init(tableName, (Guid)uniqueAttchID, keyId);
            file.HQCo = co;
            file.AttachmentId = attachId;
            file.Description = fileUpload.FileName;
            file.OrigFileName = fileUpload.FileName;
            file.FolderId = folder.FolderId;
            file.AttachmentTypeID = AttachmentTypeId;
            db.HQAttachmentFiles.Add(file);
            using var binaryReader = new BinaryReader(fileUpload.InputStream);
            var fileData = binaryReader.ReadBytes(fileUpload.ContentLength);

            var attachment = new Attachment();
            attachment.AttachmentData = fileData;
            attachment.AttachmentFileType = System.IO.Path.GetExtension(file.OrigFileName);
            attachment.SaveStamp = BitConverter.GetBytes(default(DateTime).Add(DateTime.Now.TimeOfDay).Ticks);
            attachment.AttachmentID = file.AttachmentId;

            dbAttch.Attachments.Add(attachment);
            return file;
        }

        public Attachment Create(Attachment model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            model.SaveStamp = BitConverter.GetBytes(default(DateTime).Add(DateTime.Now.TimeOfDay).Ticks);

            db.Attachments.Add(model);
            db.SaveChanges(modelState);
            return model;
        }
        
        public bool Delete(Attachment model, ModelStateDictionary modelState = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            db.Attachments.Remove(model);
            return db.SaveChanges(modelState) == 0 ? false : true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AttachmentRepository()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }

            }
        }
    }
}