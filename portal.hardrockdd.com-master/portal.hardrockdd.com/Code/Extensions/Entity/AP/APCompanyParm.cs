using System;
using System.Collections.Generic;
using System.Linq;
//using System.Web;

namespace portal.Code.Data.VP
{
    public partial class APCompanyParm
    {
        private VPEntities _db;

        public VPEntities db
        {
            set
            {
                _db = value;
            }
            get
            {
                if (_db == null)
                {
                    _db = VPEntities.GetDbContextFromEntity(this);

                    if (_db == null)
                        _db = this.HQCompanyParm.db;

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }

        public APDocument AddDocument(Microsoft.Exchange.WebServices.Data.EmailMessage item, Microsoft.Exchange.WebServices.Data.FileAttachment file)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (file == null)
                throw new ArgumentNullException(nameof(file));


            var userId = item.From.Address;
            var user = db.WebUsers.FirstOrDefault(f => f.Email == userId);
            if (user != null)
                userId = user.Id;
            else
                userId = "System";
            file.Load();
            var fileData = file.Content;

            var document = new APDocument
            {
                APCo = this.APCo,
                DocId = db.GetNextId("budAPDH"),
                DocumentName = file.Name,
                DocData = fileData,
                tStatusId = 0,
                CreatedBy = userId,
                Source = "Email",

                APCompanyParm = this,
                CreatedUser = user,
                db = this.db,
            };
            document.GenerateStatusLog();
            document.AddWorkFlow();
            document.WorkFlow.CreateSequance(document.StatusId);
            document.AddWorkFlowAssignments();
            
            if (!document.DocumentSeqs.Any())
                document.AddSequance();
            APDocuments.Add(document);

            return document;

        }

        public APDocument AddDocument(System.Web.HttpPostedFileBase uploadFile)
        {
            if (uploadFile == null)
                return null;

            using var binaryReader = new System.IO.BinaryReader(uploadFile.InputStream);
            var fileData = binaryReader.ReadBytes(uploadFile.ContentLength);
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);

            var document = db.APDocuments.FirstOrDefault(f => f.APCo == APCo && f.DocumentName == uploadFile.FileName && f.tStatusId == (int)DB.APDocumentStatusEnum.New);
            if (document == null)
            {
                document = new APDocument
                {
                    APCo = APCo,
                    DocId = db.GetNextId("budAPDH"),

                    DocumentName = uploadFile.FileName,
                    DocData = fileData,
                    tStatusId = 0,
                    CreatedBy = userId,
                    Source = "Upload",

                    CreatedUser = user,
                    APCompanyParm = this,
                    db = db,
                };

                document.GenerateStatusLog();
                document.AddWorkFlow();
                document.WorkFlow.CreateSequance(document.StatusId);
                document.AddWorkFlowAssignments();

                db.APDocuments.Add(document);
            }

            if (!document.DocumentSeqs.Any())
                document.AddSequance();

            return document;
        }

    }
}