using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.HQ.Batch.AP
{
    public class APBatchTransListViewModel
    {
        public APBatchTransListViewModel()
        {

        }

        public APBatchTransListViewModel(DB.Infrastructure.ViewPointDB.Data.Batch batch)
        {
            if (batch == null)
                return;

            Co = batch.Co;
            Mth = batch.Mth;
            BatchId = batch.BatchId;

            List = batch.APBatches.Select(s => new APBatchTransViewModel(s)).ToList();
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Company")]
        public byte Co { get; set; }

        [Key]
        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/GLAccount/APAllMthCombo", ComboForeignKeys = "GLCo=Co")]
        [Display(Name = "Batch Month")]
        public DateTime Mth { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Batch Id")]
        public int BatchId { get; set; }

        public List<APBatchTransViewModel> List { get; set; }
    }

    public class APBatchTransViewModel
    {
        public APBatchTransViewModel()
        {

        }

        public APBatchTransViewModel(DB.Infrastructure.ViewPointDB.Data.APBatchHeader transaction) 
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            #region mapping
            Co = transaction.APCo;
            VendGroupId = transaction.VendorGroupId;
            Mth = transaction.Mth.ToShortDateString();
            BatchId = transaction.BatchId;
            BatchSeq = transaction.BatchSeq;
            APTransId = transaction.APTransId;
            VendorId = transaction.VendorId;

            CMCo = transaction.bAPCO.CMCo; 
            CMAcct = transaction.CMAcct;
            if (transaction.Vendor != null)
            {
                Vendor = string.Format(AppCultureInfo.CInfo(), "{0} - {1}", transaction.VendorId, transaction.Vendor.Name.ToCamelCase());
                VendorDescription = transaction.Vendor.Name.ToCamelCase();
                VendorIdDisplay = transaction.VendorId.ToString(AppCultureInfo.CInfo());
            }

            APReference = transaction.APRef;
            Description = transaction.Description;
            InvoiceDate = transaction.InvDate;
            DueDate = transaction.DueDate;
            Amount = transaction.InvTotal;
            AllMth = transaction.Mth.ToShortDateString();
            Mth = transaction.Mth.ToShortDateString();
            MthDate = transaction.Mth;
            #endregion 

        }

        public bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new System.ArgumentNullException(nameof(modelState));
            }
            var ok = true;

            //if (AwardStatus == DB.BidAwardStatusEnum.Awarded && CustomerId == null)
            //{
            //    modelState.AddModelError("CustomerId", "Customer Field is Required");
            //    ok &= false;
            //}
            return ok;
        }
        [HiddenInput]
        [Display(Name = "VendGroupId")]
        public byte VendGroupId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Company")]
        public byte Co { get; set; }

        [Key]
        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/GLAccount/APAllMthCombo", ComboForeignKeys = "GLCo=Co")]
        [Display(Name = "Batch Month")]
        public string Mth { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Batch Id")]
        public int BatchId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Seq Id")]
        public int BatchSeq { get; set; }

        [HiddenInput]
        [Display(Name = "Trans Id")]
        public int? APTransId { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/APCombo/VendorCombo", ComboForeignKeys = "VendGroupId", SearchUrl = "/APCombo/VendorSearch", SearchForeignKeys = "VendGroupId")]
        [Display(Name = "Vendor")]
        public int VendorId { get; set; }

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


        public byte? CMCo { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "CMAcct")]
        [Field(ComboUrl = "/CMCombo/CMAccountCombo", ComboForeignKeys = "CMCo")]
        public short? CMAcct { get; set; }

        //[UIHint("DropdownBox")]
        //[Field(LabelSize = 2, TextSize = 4, ComboUrl = "/POCombo/VendorCombo", ComboForeignKeys = "POCo=APCo, VendorId", SearchUrl = "/POCombo/Search", SearchForeignKeys = "vendGroupId, VendorId")]
        //[Display(Name = "PO")]
        //public string PO { get; set; }

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
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/GLAccount/APAllMthCombo", ComboForeignKeys = "GLCo=Co")]
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
        public DateTime InvoiceDate { get; set; }

        [Required]
        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }


        [UIHint("CurrencyBox")]
        [Display(Name = "Paid")]
        public decimal? PaidAmount { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Cleared Amount")]
        public decimal? ClearedAmount { get; set; }
    }
}