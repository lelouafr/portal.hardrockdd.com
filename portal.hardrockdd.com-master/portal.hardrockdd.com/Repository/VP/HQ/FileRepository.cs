using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.HQ
{ 
    public partial class FileRepository : IDisposable
    {
        private VPContext db = new VPContext();
        
        public FileRepository()
        {

        }

        public static HQAttachmentFile Init()
        {
            var model = new HQAttachmentFile
            {

            };

            return model;
        }

        public static HQAttachmentFile Init(HRResource resource)
        {
            if (resource == null)
            {
                throw new System.ArgumentNullException(nameof(resource));
            }

            var model = new HQAttachmentFile
            {
                HQCo = resource.HRCo,
                FormName = "HRResourceMaster",
                KeyField = string.Concat("KeyID=", resource.KeyID.ToString(AppCultureInfo.CInfo())),
                AddedBy = "WebPortal",
                AddDate = DateTime.Now,
                DocName = "Database",
                TableName = "HRRM",
                UniqueAttchID = resource.UniqueAttchID,
                DocAttchYN = "N",
                CurrentState = "A",
                IsEmail = "N"
            };

            return model;
        }
        public static HQAttachmentFile Init(string tableName, Guid UniqueAttchID, long KeyID)
        {
            using var db = new VPContext();
            
            //var object_context = VPEntities.GetDbContextFromEntity(entity);
            //string tableName = object_context.GetTableName<Company>();
            //tableName = tableName.Remove(0, 1);
            var form = db.vFormHeaders.FirstOrDefault(f => f.ViewName == tableName);
            var keyField = string.Format(AppCultureInfo.CInfo(), "KeyID={0}", KeyID);
            var userId = StaticFunctions.GetUserId();
            var usr = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var emp = usr.Employee.FirstOrDefault().PREmployee;
            var usrProf = emp.UserProfile.FirstOrDefault();
            if (form == null)
            {

            }

            var model = new HQAttachmentFile
            {
                FormName = form?.Form ?? tableName,
                KeyField = keyField,
                AddedBy = usrProf == null ? usr.Email : usrProf.VPUserName,
                AddDate = DateTime.Now,
                DocName = "Database",
                TableName = tableName,
                UniqueAttchID = UniqueAttchID,
                DocAttchYN ="Y",
                CurrentState = "A",
                IsEmail = "N"
            };
            return model;
        }

        public static HQAttachmentFile DuplicateFile(HQAttachmentFile srcFile)
        {
            var file = new HQAttachmentFile
            {                
                FormName = srcFile.TableName,
                KeyField = srcFile.KeyField,
                AddedBy = srcFile.AddedBy,
                AddDate = srcFile.AddDate,
                DocName = srcFile.DocName,
                TableName = srcFile.TableName,
                UniqueAttchID = srcFile.UniqueAttchID,
                DocAttchYN = srcFile.DocAttchYN,
                CurrentState = srcFile.CurrentState,
                IsEmail = srcFile.IsEmail,
                Description = srcFile.Description,
                OrigFileName = srcFile.OrigFileName,
                AttachmentTypeID = srcFile.AttachmentTypeID,
                HQCo = srcFile.HQCo,
            };

            return file;
        }

        public static int NextId(HQCompanyParm model)
        {
            return HQAttachmentFile.GetNextAttachmentId();
        }
        public static int NextId()
        {
            using var db = new VPContext();
            using var dbAttch = new DB.Infrastructure.VPAttachmentDB.Data.VPAttachmentsContext();
            var attachId = db.HQAttachmentFiles
                            .DefaultIfEmpty()
                            .Max(f => f == null ? 0 : f.AttachmentId) + 1;

            while (dbAttch.Attachments.Any(f => f.AttachmentID == attachId))
            {
                attachId++;
            }

            return attachId;
        }
        public List<HQAttachmentFile> GetFiles(Guid UniqueAttchID)
        {
            var qry = db.HQAttachmentFiles
                        .Where(f => f.UniqueAttchID == UniqueAttchID)
                        .ToList();

            return qry;
        }

        public HQAttachmentFile GetFile(int AttachmentID)
        {
            var qry = db.HQAttachmentFiles.FirstOrDefault(f => f.AttachmentId == AttachmentID);

            return qry;
        }

        public HQAttachmentFile ProcessUpdate(HQAttachmentFile model, ModelStateDictionary modelState)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            var updObj = GetFile(model.AttachmentId);
            if (updObj != null)
            {
                updObj.FormName = model.FormName;
                updObj.KeyField = model.KeyField;
                updObj.Description = model.Description;
                updObj.AddedBy = model.AddedBy;
                updObj.AddDate = model.AddDate;
                updObj.DocName = model.DocName;
                updObj.TableName = model.TableName;
                updObj.OrigFileName = model.OrigFileName;
                updObj.DocAttchYN = model.DocAttchYN;
                updObj.CurrentState = model.CurrentState;
                updObj.AttachmentTypeID = model.AttachmentTypeID;
                updObj.IsEmail = model.IsEmail;
                db.SaveChanges(modelState);
            }
            return updObj;
        }

      
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~FileRepository()
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