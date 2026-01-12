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
    public class DocumentListViewModel
    {
        public DocumentListViewModel()
        {
            List = new List<DocumentViewModel>();
        }

        public DocumentListViewModel(List<APDocument> list, bool checkErrors = false)
        {
            List = list.Select(s => new DocumentViewModel(s, checkErrors)).ToList();
        }

        public DocumentListViewModel(byte apco, DB.APDocumentStatusEnum status )
        {
            using var db = new VPContext();
            var divisions = db.CompanyDivisions.Where(f => f.HQCompany.APCo == apco).Select(s => s.DivisionId).ToList();

            APCo = apco;
            Status = status;

            if (status == DB.APDocumentStatusEnum.All)
            {
                var files = db.APDocuments.Where(f => divisions.Contains(f.DivisionId ?? 0)).ToList();
                List = files.Select(s => new DocumentViewModel(s)).ToList();

            }   
            else
            {
                var files = db.APDocuments.Where(f => f.tStatusId == (int)status &&  divisions.Contains(f.DivisionId ?? 0))
                                            .ToList();
                List = files.Select(s => new DocumentViewModel(s)).ToList();
            }
        }


        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte APCo { get; set; }

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

        public DocumentViewModel(APDocument document, bool checkErrors = false)
        {
            if (document == null) throw new System.ArgumentNullException(nameof(document));

            #region mapping
            APCo = document.APCo;
            DocId = document.DocId;
            Source = document.Source;
            Mth = document.Mth.ToShortDateString();
            DocumentName = document.DocumentName;
            UniqueAttchID = document.UniqueAttchID;
            DocumentType = document.DocumentType;
            Mime = MimeMapping.GetMimeMapping(document.DocumentName);
            Status = document.Status;
            CreatedByUser = document.CreatedUser.FullName();
            StatusString = Status.ToString();
            Amount = document.InvTotal;
            APReference = document.APRef;
            VendorDescription = document.Vendor?.LongName;
            CreatedOn = document.CreatedOn ?? DateTime.Now;
            InvoiceDate = document.InvDate;
            #endregion

            ErrorMsg = string.Empty;
            LastPostedCommentUser = string.Empty;

            if (checkErrors)
            { 
                var parms = document.db.GLParameters.FirstOrDefault(f => f.GLCo == document.APCompanyParm.GLCo);
                if (parms.LastMthAPClsd >= document.Mth)
				{
					ContainsErrors = true;
					if (!string.IsNullOrEmpty(ErrorMsg))
						ErrorMsg += "\r";
					ErrorMsg += "Batch month is closed!";
				}

				if (document.APRef != null && document.Status <= DB.APDocumentStatusEnum.Filed)
                {
                    if (document.db.APTrans.Any(f => f.VendorGroupId == document.VendorGroupId && 
                                                     f.VendorId == document.VendorId && 
                                                     f.APRef.ToUpper() == document.APRef.ToUpper()))
                    {
                        ContainsErrors = true;
                        if (!string.IsNullOrEmpty(ErrorMsg))
                            ErrorMsg += "\r";
                        ErrorMsg += "AP Reference already Exists IN VP";
                    }
                    if (document.db.APDocuments.Any(f => f.VendorGroupId == document.VendorGroupId &&
                                                         f.tVendorId == document.VendorId &&
                                                         f.DocId != document.DocId && 
                                                         f.tAPRef.ToUpper() == document.APRef.ToUpper() && 
                                                         f.tStatusId != (int)DB.APDocumentStatusEnum.Duplicate && 
                                                         f.tStatusId != (int)DB.APDocumentStatusEnum.Canceled))
                    {
                        ContainsErrors = true;
                        if (!string.IsNullOrEmpty(ErrorMsg))
                            ErrorMsg += "\r";
                        ErrorMsg += "Duplicate Filled Document";
                    }
                }
                if (document.VendorId == 999999)
                {
                    ContainsErrors = true;
                    if (!string.IsNullOrEmpty(ErrorMsg))
                        ErrorMsg += "\r";
                    ErrorMsg += "New Vendor Selected";
                }
                if (Status == DB.APDocumentStatusEnum.Error)
                {
                    ContainsErrors = true;
                    if (!string.IsNullOrEmpty(ErrorMsg))
                        ErrorMsg += "\r";
                    ErrorMsg += "Error on Posting";
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
                
                var commentUser = document.Forum?.Lines?.LastOrDefault()?.CreatedUser;
                if (commentUser != null)
                {
                    LastPostedCommentUser = commentUser.FullName();
                }
            }
        }
        public bool ContainsErrors { get; set; }

        public string ErrorMsg { get; set; }

        public string LastPostedCommentUser { get; set; }

        [Key]
        [Display(Name = "Company")]
        public byte APCo { get; set; }

        [Key]
        [Display(Name = "Attachment Number")]
        public long DocId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/GLAccount/APMthCombo", ComboForeignKeys = "GLCo=APCo")]
        [Display(Name = "Batch Month")]
        public string Mth { get; set; }

        
        [Display(Name = "UniqueAttchID")]
        public Guid? UniqueAttchID { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.APDocumentStatusEnum Status { get; set; }

        [Display(Name = "Status")]
        public string StatusString { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Info", FormGroupRow = 1)]
        [Display(Name = "File Name")]
        public string DocumentName { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Info", FormGroupRow = 1)]
        [Display(Name = "Source")]
        public string Source { get; set; }

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
        [Display(Name = "Added On")]
        public DateTime CreatedOn { get; set; }



        [UIHint("CurrencyBox")]
        [Display(Name = "Amount")]
        public decimal? Amount { get; set; }


        [UIHint("DateBox")]
        [Display(Name = "Invoice Date")]
        public DateTime? InvoiceDate { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "AP Reference")]
        public string APReference { get; set; }

        [UIHint("TextBox")]
        [Field()]
        [Display(Name = "Vendor")]
        public string VendorDescription { get; set; }
    }
}