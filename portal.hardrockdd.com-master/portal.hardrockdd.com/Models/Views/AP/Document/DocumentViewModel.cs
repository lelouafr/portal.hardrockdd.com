using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AP.Document
{
    public class DocumentListViewModel
    {
        public DocumentListViewModel()
        {

        }

        public DocumentListViewModel(byte apco, string tableName, DB.APDocumentStatusEnum status )
        {
            using var db = new VPContext();
            var divisions = db.CompanyDivisions.Where(f => f.HQCompany.APCo == apco).Select(s => s.DivisionId).ToList();

            #region mapping
            APCo = apco;
            TableName = tableName;
            Status = status;
            #endregion
            if (status == DB.APDocumentStatusEnum.All)
            {
                var files = db.APDocuments.Where(f => divisions.Contains(f.DivisionId ?? 0)).ToList();
                List = files.Select(s => new DocumentViewModel(s, db)).ToList();

            }
            else
            {
                var files = db.APDocuments.Where(f => f.tStatusId == (int)status && divisions.Contains(f.DivisionId ?? 0))
                                            .ToList();
                List = files.Select(s => new DocumentViewModel(s, db)).ToList();
            }
        }


        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte APCo { get; set; }

        [Key]
        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Table Name")]
        public string TableName { get; set; }

        [Key]
        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.APDocumentStatusEnum Status { get; set; }

        public List<DocumentViewModel> List { get; }
    }

    public class DocumentViewModel
    {
        public DocumentViewModel()
        {

        }

        public DocumentViewModel(DB.Infrastructure.ViewPointDB.Data.APDocument apDocument, VPContext db)
        {
            if (apDocument == null) throw new System.ArgumentNullException(nameof(apDocument));
            if (db == null) throw new System.ArgumentNullException(nameof(db));


            #region mapping
            APCo = apDocument.APCo;
            DocId = apDocument.DocId;
            Source = apDocument.Source;
            DocumentName = apDocument.DocumentName;
            UniqueAttchID = apDocument.UniqueAttchID;
            DocumentType = apDocument.DocumentType;
            Mime = MimeMapping.GetMimeMapping(apDocument.DocumentName);
            Status = apDocument.Status;
            CreatedByUser = new WebUserViewModel(apDocument.CreatedUser);
            //CreatedOn = Attachment.CreatedOn;
            #endregion
            APEntry = new DocumentSeqViewModel(apDocument);
            Action = new ActionViewModel(apDocument);

            ErrorMsg = string.Empty;
            if (APEntry.APReference != null && Status <= DB.APDocumentStatusEnum.Filed)
            {
                if (db.APTrans.Any(f => f.VendorId == APEntry.VendorId && f.APRef == APEntry.APReference))
                {
                    ContainsErrors = true;
                    if (!string.IsNullOrEmpty(ErrorMsg))
                    {
                        ErrorMsg += "\r";
                    }
                    ErrorMsg += "AP Reference already Exists IN VP";
                }
                if (db.APDocuments.Any(f => f.VendorId == APEntry.VendorId && f.DocId != APEntry.DocId && f.APRef == APEntry.APReference && f.tStatusId != (int)DB.APDocumentStatusEnum.Duplicate && f.tStatusId != (int)DB.APDocumentStatusEnum.Canceled) )
                {
                    ContainsErrors = true;
                    if (!string.IsNullOrEmpty(ErrorMsg))
                    {
                        ErrorMsg += "\r";
                    }
                    ErrorMsg += "Duplicate Filled Document";
                }
            }
            if (APEntry.VendorId == 999999)
            {
                ContainsErrors = true;
                if (!string.IsNullOrEmpty(ErrorMsg))
                {
                    ErrorMsg += "\r";
                }
                ErrorMsg += "New Vendor Selected";
            }
        }

        public DocumentViewModel(DB.Infrastructure.ViewPointDB.Data.APDocument apDocument)
        {
            if (apDocument == null)
            {
                throw new ArgumentNullException(nameof(apDocument));
            }

            #region mapping
            APCo = apDocument.APCo;
            DocId = apDocument.DocId;
            Source = apDocument.Source;
            DocumentName = apDocument.DocumentName;
            UniqueAttchID = apDocument.UniqueAttchID;
            DocumentType = apDocument.DocumentType;
            Mime = MimeMapping.GetMimeMapping(apDocument.DocumentName);
            Status = apDocument.Status;
            CreatedByUser = new WebUserViewModel(apDocument.CreatedUser);
            //CreatedOn = Attachment.CreatedOn;
            #endregion
            APEntry = new DocumentSeqViewModel(apDocument);
            Action = new ActionViewModel(apDocument);

        }

        public bool ContainsErrors { get; set; }

        public string ErrorMsg { get; set; }
        [Key]
        //[Required]
        [HiddenInput]
        [Display(Name = "Company")]
        public byte APCo { get; set; }

        [Key]
        //[Required]
        [HiddenInput]
        [Display(Name = "Attachment Number")]
        public long DocId { get; set; }

        [HiddenInput]
        //[Required]
        [Display(Name = "UniqueAttchID")]
        public Guid? UniqueAttchID { get; set; }


        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.APDocumentStatusEnum Status { get; set; }

        //[Required]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Info", FormGroupRow = 1)]
        [Display(Name = "File Name")]
        public string DocumentName { get; set; }

        //[Required]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Info", FormGroupRow = 1)]
        [Display(Name = "Source")]
        public string Source { get; set; }

        // [Required]
        [HiddenInput]
        [Display(Name = "Type")]
        public int? DocumentType { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Info", FormGroupRow = 1)]
        [Display(Name = "File Mime")]
        public string Mime { get; set; }

        [UIHint("WebUserBox")]
        [Display(Name = "Added User")]
        public WebUserViewModel CreatedByUser { get; set; }

        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Added On")]
        public DateTime CreatedOn { get; set; }

        public ActionViewModel Action { get; set; }

        public DocumentSeqViewModel APEntry { get; set; }
    }
}