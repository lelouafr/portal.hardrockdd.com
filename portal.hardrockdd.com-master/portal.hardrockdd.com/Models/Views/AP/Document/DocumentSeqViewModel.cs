using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AP.Document
{
    public class DocumentSeqListViewModel: AuditBaseViewModel
    {
        public DocumentSeqListViewModel()
        {

        }

        public DocumentSeqListViewModel(APDocument document) : base(document)
        {
            if (document == null) throw new System.ArgumentNullException(nameof(document));

            using var db = new VPContext();
            #region mapping
            APCo = document.APCo;
            DocId = document.DocId;
            #endregion
            List = new List<DocumentSeqViewModel>();
            List.Add(new DocumentSeqViewModel(document));         
        }


        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte APCo { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "DocId")]
        public int DocId { get; set; }

        public List<DocumentSeqViewModel> List { get; }
    }

    public class DocumentSeqViewModel
    {
        public DocumentSeqViewModel()
        {

        }

        public DocumentSeqViewModel(DB.Infrastructure.ViewPointDB.Data.APDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            #region mapping
            Status = (DB.APDocumentStatusEnum)document.Status;
            APCo = document.APCo;
            DocId = document.DocId;
            VendorGroupId = document.VendorGroupId ?? document.APCompanyParm.HQCompanyParm.VendorGroupId;
            VendorId = document.VendorId;
            if (document.Vendor != null)
            {
                Vendor = string.Format(AppCultureInfo.CInfo(), "{0} - {1}", document.VendorId, document.Vendor.Name.ToCamelCase());
                VendorDescription = document.Vendor.Name.ToCamelCase();
                VendorIdDisplay = document.VendorId.ToString();
            }
            DivisionId = document.DivisionId;

            POCo = document.POCo ?? document.APCo;
            PO = document.PO;
            APReference = document.APRef;
            Description = document.Description;
            InvoiceDate = document.InvDate;
            DueDate = document.DueDate;
            Amount = document.InvTotal;
            AllMth = ((DateTime)document.Mth).ToShortDateString();
            Mth = ((DateTime)document.Mth).ToShortDateString();
            MthDate = (DateTime)document.Mth;
            #endregion

            Action = new ActionViewModel(document);
            Lines = new DocumentSeqLineListViewModel(document);
        }


        [HiddenInput]
        public byte? POCo { get; set; }
        [HiddenInput]
        public byte? VendorGroupId { get; set; }

        public DB.APDocumentStatusEnum Status { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Company")]
        public byte APCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Doc Id")]
        public int DocId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Seq Id")]
        public int SeqId { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/APCombo/VendorCombo", ComboForeignKeys = "VendGroupId=VendorGroupId", SearchUrl = "/APCombo/VendorSearch", SearchForeignKeys = "VendGroupId=VendorGroupId", InfoUrl = "/VendorForm/PopupForm", InfoForeignKeys = "VendGroupId=VendorGroupId,VendorId")]
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

        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/WPCombo/WPDivisionCombo", ComboForeignKeys = "")]
        [Display(Name = "Division")]
        public int? DivisionId { get; set; }

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

        [Required]
        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Invoice Date")]
        public DateTime? InvoiceDate { get; set; }

        [Required]
        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Due Date")]
        public DateTime? DueDate { get; set; }

        [Required]
        [UIHint("CurrencyBox")]
        [Display(Name = "Amount")]
        public decimal? Amount { get; set; }

        public DocumentSeqLineListViewModel Lines { get; set; }

        public bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new System.ArgumentNullException(nameof(modelState));
            }
            var ok = true;
            if (!string.IsNullOrEmpty(APReference))
            {
                if (APReference.Length > 15)
                {
                    modelState.AddModelError("APReference", "Max Charaters is 15");
                    ok = false;
                }

            }
            return ok;
        }
        public ActionViewModel Action { get; set; }
    }
}