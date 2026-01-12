using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Areas.AccountsPayable.Models.Document
{
    public class vDocumentListViewModel
    {
        public vDocumentListViewModel()
        {

        }

        public vDocumentListViewModel(byte apco, string tableName, DB.APDocumentStatusEnum status)
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
                List = db.vAPDocuments.Where(f => f.APCo == apco).AsEnumerable().Select(s => new vDocumentViewModel(s)).ToList();
            }
            else if (status != DB.APDocumentStatusEnum.RequestedInfo)
            {
                // var files = db.vAPDocuments.Where(f => f.Co == co && f.Status == (int)status).ToList();
                List = db.vAPDocuments.Where(f => f.APCo == apco && f.Status == (int)status).AsEnumerable().Select(s => new vDocumentViewModel(s, db)).ToList();
                if (status == DB.APDocumentStatusEnum.Filed)
                {
                    List.AddRange(db.vAPDocuments.Where(f => f.APCo == apco && f.Status == (int)DB.APDocumentStatusEnum.RequestedInfo).AsEnumerable().Select(s => new vDocumentViewModel(s, db)).ToList());
                    List.AddRange(db.vAPDocuments.Where(f => f.APCo == apco && f.Status == (int)DB.APDocumentStatusEnum.Error).AsEnumerable().Select(s => new vDocumentViewModel(s, db)).ToList());
                }
            }
            else
            {
                var userId = StaticFunctions.GetUserId();
                List = db.vAPDocuments.Where(f => f.APCo == apco && f.Status == (int)status && f.AssignedToStr.Contains(userId)).AsEnumerable().Select(s => new vDocumentViewModel(s, db)).ToList();
            }
        }

        [Key]
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

        public List<vDocumentViewModel> List { get; }
    }

    public class vDocumentViewModel
    {
        public vDocumentViewModel()
        {

        }

        public vDocumentViewModel(vAPDocument document, VPContext db)
        {
            if (document == null) throw new System.ArgumentNullException(nameof(document));
            if (db == null) throw new System.ArgumentNullException(nameof(db));


            #region mapping
            APCo = document.APCo;
            DocId = document.DocId;
            Source = document.Source;
            DocumentName = document.DocumentName;
            UniqueAttchID = document.UniqueAttchID;
            DocumentType = document.DocumentType;
            Mime = MimeMapping.GetMimeMapping(document.DocumentName);
            Status = (DB.APDocumentStatusEnum)document.Status;
            CreatedByUser = document.CreatedUserName;
            SeqId = document.SeqId;
            APReference = document.APRef;
            Vendor = document.VendorName;
            VendorId = document.VendorId;
            CreatedByUser = document.CreatedUserName;
            AllMth = document.Mth.ToShortDateString();
            Mth = document.Mth.ToShortDateString();
            Vendor = document.VendorName;
            VendorDescription = document.VendorName;
            InvoiceDate = document.InvDate;
            Amount = document.InvTotal;

            BatchMth = document.Mth.ToString("MMMM", AppCultureInfo.CInfo());
            AmountString = document.InvTotal?.ToString("{C2}", AppCultureInfo.CInfo());
            InvoiceDate = document.InvDate;
            Amount = document.InvTotal;
            StatusString = Status.ToString();
            LastPostedCommentUser = document.LastCommentBy;
            #endregion
            //APEntry = new DocumentSeqViewModel(Attachment.DocumentSeqs.FirstOrDefault());
            //Action = new ActionViewModel(Attachment);

            ErrorMsg = string.Empty;
            if (APReference != null && (Status <= DB.APDocumentStatusEnum.Filed || Status == DB.APDocumentStatusEnum.RequestedInfo || Status == DB.APDocumentStatusEnum.Error))
            {
                if (db.APTrans.Any(f => f.VendorId == document.VendorId && f.APRef.Trim() == document.APRef.Trim()))
                {
                    ContainsErrors = true;
                    if (!string.IsNullOrEmpty(ErrorMsg))
                    {
                        ErrorMsg += "\r";
                    }
                    ErrorMsg += "AP Reference already Exists IN VP";
                }
                else if (db.APDocuments.Any(f => f.tVendorId == document.VendorId && f.DocId != document.DocId && f.tAPRef == document.APRef && f.tStatusId != (int)DB.APDocumentStatusEnum.Duplicate && f.tStatusId != (int)DB.APDocumentStatusEnum.Canceled) )
                {
                    ContainsErrors = true;
                    if (!string.IsNullOrEmpty(ErrorMsg))
                    {
                        ErrorMsg += "\r";
                    }
                    ErrorMsg += "Duplicate Filled Document";
                }
            }
            if (Status == DB.APDocumentStatusEnum.Error)
            {
                ContainsErrors = true;
                ErrorMsg += "Error on Posting";
            }
            if (document.VendorId == 999999)
            {
                ContainsErrors = true;
                if (!string.IsNullOrEmpty(ErrorMsg))
                {
                    ErrorMsg += "\r";
                }
                ErrorMsg += "New Vendor Selected";
            }
            if (Status == DB.APDocumentStatusEnum.RequestedInfo)
            {
                ContainsErrors = true;
                if (!string.IsNullOrEmpty(ErrorMsg))
                {
                    ErrorMsg += "\r";
                }
                ErrorMsg += "Requested More Information";
            }
        }

        public vDocumentViewModel(vAPDocument document)
        {
            if (document == null) throw new System.ArgumentNullException(nameof(document));

            #region mapping
            APCo = document.APCo;
            DocId = document.DocId;
            Source = document.Source;
            DocumentName = document.DocumentName;
            UniqueAttchID = document.UniqueAttchID;
            DocumentType = document.DocumentType;
            //Mime = MimeMapping.GetMimeMapping(Attachment.DocumentName);
            Status = (DB.APDocumentStatusEnum)document.Status;
            CreatedByUser = document.CreatedUserName;
            SeqId = document.SeqId;
            APReference = document.APRef;
            Vendor = document.VendorName;
            VendorId = document.VendorId;
            CreatedByUser = document.CreatedUserName;
            Mth = document.Mth.ToShortDateString();
            AllMth = document.Mth.ToShortDateString();
            BatchMth = document.Mth.ToString("MMMM", AppCultureInfo.CInfo());
            AmountString = string.Format(AppCultureInfo.CInfo(), "{0:C}", document.InvTotal);
            //AmountString = Attachment.InvTotal?.ToString("{C2}");
            InvoiceDate = document.InvDate;
            Amount = document.InvTotal;
            StatusString = Status.ToString();
            LastPostedCommentUser = document.LastCommentBy;



            POCo = document.POCo;
            VendorGroupId = document.VendorGroupId;
            //CreatedOn = Attachment.CreatedOn;
            #endregion
            //APEntry = new DocumentSeqViewModel() { 
            //    Co = Attachment.Co,
            //    DocId = Attachment.DocId,
            //    SeqId = Attachment.SeqId
            //};
            //Action = new ActionViewModel(Attachment);

        }

        public vDocumentViewModel(APDocument document)
        {
            if (document == null) throw new System.ArgumentNullException(nameof(document));

            var mth = (DateTime)document.Mth;
            #region mapping
            APCo = document.APCo;
            DocId = document.DocId;
            Source = document.Source;
            DocumentName = document.DocumentName;
            UniqueAttchID = document.UniqueAttchID;
            DocumentType = document.DocumentType;
            //Mime = MimeMapping.GetMimeMapping(Attachment.DocumentName);
            Status = (DB.APDocumentStatusEnum)document.Status;
            CreatedByUser = document.CreatedUser.FullName();
            SeqId = 1;
            APReference = document.APRef;
            Vendor = document.Vendor.Name;
            VendorId = document.VendorId;
            Mth = mth.ToShortDateString();
            AllMth = mth.ToShortDateString();
            BatchMth = mth.ToString("MMMM", AppCultureInfo.CInfo());
            AmountString = string.Format(AppCultureInfo.CInfo(), "{0:C}", document.InvTotal);
            //AmountString = Attachment.InvTotal?.ToString("{C2}");
            InvoiceDate = document.InvDate;
            Amount = document.InvTotal;
            StatusString = Status.ToString();
            POCo = document.POCo?? document.APCo;
            VendorGroupId = document.VendorGroupId;
            var commentUser = document.Forum.Lines.LastOrDefault().CreatedUser;
            if (commentUser != null)
            {
                LastPostedCommentUser = commentUser.FirstName + ' ' + commentUser.LastName;
            }
            //CreatedOn = Attachment.CreatedOn;
            #endregion
            //APEntry = new DocumentSeqViewModel() { 
            //    Co = Attachment.Co,
            //    DocId = Attachment.DocId,
            //    SeqId = Attachment.SeqId
            //};
            //Action = new ActionViewModel(Attachment);

        }

        [HiddenInput]
        public byte? POCo { get; set; }

        [HiddenInput]
        public byte? VendorGroupId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Seq Id")]
        public int SeqId { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/APCombo/VendorCombo", ComboForeignKeys = "VendGroupId=VendorGroupId", SearchUrl = "/APCombo/VendorSearch", SearchForeignKeys = "VendorGroupId")]
        [Display(Name = "Vendor")]
        public int? VendorId { get; set; }

        [UIHint("TextBox")]
        [Field()]
        [Display(Name = "Vendor")]
        public string Vendor { get; set; }

        [UIHint("TextBox")]
        [Field()]
        [Display(Name = "Vendor")]
        public string VendorDescription { get; set; }

        [UIHint("TextBox")]
        [Field()]
        [Display(Name = "Vendor")]
        public string VendorIdDisplay { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/POCombo/VendorCombo", ComboForeignKeys = "POCo, VendorId", SearchUrl = "/POCombo/Search", SearchForeignKeys = "VendGroupId=VendorGroupId, VendorId")]
        [Display(Name = "PO")]
        public string PO { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "AP Reference")]
        public string APReference { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/GLAccount/APMthCombo", ComboForeignKeys = "GLCo=APCo")]
        [Display(Name = "Batch Month")]
        public string Mth { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/GLAccount/APAllMthCombo", ComboForeignKeys = "GLCo=APCo")]
        [Display(Name = "Batch Month")]
        public string AllMth { get; set; }

        [Required]
        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Mth Date")]
        public DateTime MthDate { get; set; }

        public bool ContainsErrors { get; set; }

        public string ErrorMsg { get; set; }
        
        public string LastPostedCommentUser { get; set; }
        
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

        public string StatusString { get; set; }

        public string BatchMth { get; set; }

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

        [UIHint("TextBox")]
        [Display(Name = "Added User")]
        public string CreatedByUser { get; set; }

        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Added On")]
        public DateTime CreatedOn { get; set; }

        [Required]
        [UIHint("CurrencyBox")]
        [Display(Name = "Amount")]
        public decimal? Amount { get; set; }

        public string AmountString { get; set; }

        [Required]
        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Invoice Date")]
        public DateTime? InvoiceDate { get; set; }

        public string InvoiceDateString { get { return InvoiceDate?.ToShortDateString();  } }
    }
}