using System;
using System.Collections.Generic;
using System.Linq;
//using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class APCompanyParm
    {
        private VPContext _db;

        public VPContext db
        {
            set
            {
                _db = value;
            }
            get
            {
                if (_db == null)
                {
                    _db = VPContext.GetDbContextFromEntity(this);

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

            var emp = db.GetCurrentEmployee();
            var document = new APDocument
            {
                APCompanyParm = this,
                CreatedUser = user,
                db = this.db,

                APCo = this.APCo,
                DocId = db.GetNextId("budAPDH"),
                DocumentName = file.Name,
                tStatusId = 0,
                CreatedBy = userId,
                Source = "Email",

                Mth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                DivisionId = emp.DivisionId,
                Division = emp.Division,

            };
            document.Attachment.GetRootFolder().AddFile(file.Name, fileData);

            document.GenerateStatusLog();
            document.AddWorkFlow();
            document.WorkFlow.CreateSequence(document.StatusId);
            document.AddWorkFlowAssignments();
            
            //if (!document.DocumentSeqs.Any())
            //    document.AddSequence();
            APDocuments.Add(document);

            return document;

        }

        public APDocument AddDocument(System.Web.HttpPostedFileBase uploadFile)
        {
            if (uploadFile == null)
                return null;

            var document = db.APDocuments.FirstOrDefault(f => f.APCo == APCo && f.DocumentName == uploadFile.FileName && f.tStatusId == (int)APDocumentStatusEnum.New);
            if (document == null)
            {
                var emp = db.GetCurrentEmployee();
                document = new APDocument
                {
                    CreatedUser = db.GetCurrentUser(),
                    APCompanyParm = this,
                    db = db,

                    APCo = APCo,
                    DocId = db.GetNextId("budAPDH"),

                    DocumentName = uploadFile.FileName,
                    tStatusId = 0,
                    CreatedBy = db.CurrentUserId,
                    Source = "Upload",

                    Mth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                    DivisionId = emp.DivisionId,
                    Division = emp.Division,
                };
                document.Attachment.GetRootFolder().AddFile(uploadFile);

                document.GenerateStatusLog();
                document.AddWorkFlow();
                document.WorkFlow.CreateSequence(document.StatusId);
                document.AddWorkFlowAssignments();

                db.APDocuments.Add(document);
            }

            //if (!document.DocumentSeqs.Any())
            //    document.AddSequence();

            return document;
        }

    }
}