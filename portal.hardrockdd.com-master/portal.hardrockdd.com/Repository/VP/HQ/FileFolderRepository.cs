using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.HQ
{ 
    public partial class HQAttachmentFolderRepository : IDisposable
    {
        private VPContext db = new VPContext();
        public HQAttachmentFolderRepository()
        {

        }

        public static HQAttachmentFolder Init()
        {
            var model = new HQAttachmentFolder
            {

            };

            return model;
        }

        public static HQAttachmentFolder Init(string tableName, Guid UniqueAttchID, long KeyID)
        {
            using var db = new VPContext();
            
            var model = new HQAttachmentFolder
            {
               
            };
            return model;
        }


        //public static int NextId(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm model)
        //{
        //    using var db = new VPContext();
        //    using var dbAttch = new DB.Infrastructure.VPAttachmentDB.Data.VPAttachmentsContext();
        //    var attachId = db.HQAttachmentFolders
        //                    .Where(f => f.HQCo == model.HQCo)
        //                    .DefaultIfEmpty()
        //                    .Max(f => f == null ? 0 : f.AttachmentId) + 1;

        //    while (dbAttch.Attachments.Any(f => f.AttachmentID == attachId))
        //    {
        //        attachId++;
        //    }

        //    return attachId;
        //}

        public HQAttachmentFolder ProcessUpdate(HQAttachmentFolder model, ModelStateDictionary modelState)
        {
            using var db = new VPContext();

            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            var updObj = db.HQAttachmentFolders.FirstOrDefault(f => f.HQCo == model.HQCo && f.UniqueAttchID == model.UniqueAttchID && f.FolderId == model.FolderId);
            if (updObj != null)
            {
                updObj.tDescription = model.tDescription;
                updObj.ParentId = model.ParentId;
            }
            return updObj;
        }

        public static bool Delete(HQAttachmentFolder model, List<HQAttachmentFile> Files, ModelStateDictionary modelState)
        {
            using var db = new VPContext();
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            if (Files == null)
            {
                throw new ArgumentNullException(nameof(Files));
            }
            foreach (var subFolder in model.SubFolders.ToList())
            {
                Delete(subFolder, Files, modelState); 
                var delObj = db.HQAttachmentFolders.FirstOrDefault(f => f.HQCo == subFolder.HQCo && f.UniqueAttchID == subFolder.UniqueAttchID && f.FolderId == subFolder.FolderId);
                if (delObj != null)
                {
                    db.HQAttachmentFolders.Remove(delObj);
                }
            }
            foreach (var file in Files.Where(f => f.FolderId == model.FolderId).ToList())
            {
                var delObj = db.HQAttachmentFiles.FirstOrDefault(f => f.HQCo == file.HQCo && f.AttachmentId == file.AttachmentId);
                if (delObj != null)
                {
                    db.HQAttachmentFiles.Remove(delObj);
                }
            }
            var folder = db.HQAttachmentFolders.FirstOrDefault(f => f.HQCo == model.HQCo && f.UniqueAttchID == model.UniqueAttchID && f.FolderId == model.FolderId);
            if (folder != null)
            {
                db.HQAttachmentFolders.Remove(folder);
            }
            db.SaveChanges(modelState);
            return true;            
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~HQAttachmentFolderRepository()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                db = null;
            }
        }
    }
}