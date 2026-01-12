using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Areas.AccountsPayable.Models.Document
{
   
    public class DocumentInfoViewModel
    {
        public DocumentInfoViewModel()
        {

        }

        public DocumentInfoViewModel(DB.Infrastructure.ViewPointDB.Data.APDocument document)
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
            AllMth = document.Mth.ToShortDateString();
            Mth = document.Mth.ToShortDateString();
            MthDate = document.Mth;
            #endregion

            Action = new ActionViewModel(document);
            //Lines = new DocumentLineListViewModel(document);
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

        //public DocumentLineListViewModel Lines { get; set; }

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


        internal DocumentInfoViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.APDocuments.FirstOrDefault(f => f.APCo == this.APCo && f.DocId == this.DocId);

            if (updObj != null)
            {
                var mthDate = DateTime.TryParse(Mth, out DateTime mthDateOut) ? mthDateOut : updObj.Mth;
                if (updObj.VendorId != VendorId)
                    updObj.VendorId = VendorId;
                else if (updObj.PO != PO)
                    updObj.PO = PO;
                else
                {
                    updObj.APRef = APReference?.ToUpper();
                    updObj.Description = Description;
                    updObj.DivisionId = DivisionId;
                    if (updObj.InvDate != InvoiceDate)
                    {
                        updObj.InvDate = InvoiceDate;
                        DueDate = updObj.DueDate;
                    }
                    if (updObj.DueDate != DueDate)
                        updObj.DueDate = DueDate;
                    updObj.InvTotal = Amount;
                    updObj.Mth = mthDate;
                }

                try
                {
                    db.BulkSaveChanges();
                    return new DocumentInfoViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            else
            {
                modelState.AddModelError("", "Object Doesn't Exist For Update!");
                return this;
            }
        }
    }
}