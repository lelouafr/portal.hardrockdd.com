using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.HQ.Batch.AP
{
    public class APPostedTransListViewModel
    {

        public APPostedTransListViewModel()
        {

        }

        public APPostedTransListViewModel(DB.Infrastructure.ViewPointDB.Data.Batch batch)
        {
            if (batch == null)
                return;


            Co = batch.Co;
            Mth = batch.Mth;
            BatchId = batch.BatchId;

            List = batch.APTransactions.Select(s => new APPostedTransViewModel(s)).ToList();
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Company")]
        public byte Co { get; set; }

        [Key]
        [Required]
        [UIHint("DateBox")]
        [Display(Name = "Batch Month")]
        public DateTime Mth { get; set; }

        [Required]
        [HiddenInput]
        [Display(Name = "Batch Id")]
        public int BatchId { get; set; }

        public List<APPostedTransViewModel> List { get; set; }
    }

    public class APPostedTransViewModel
    {
        public APPostedTransViewModel()
        {

        }

        public APPostedTransViewModel(DB.Infrastructure.ViewPointDB.Data.APTran transaction) 
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            #region mapping
            Co = transaction.APCo;
            POCo = transaction.APCo;
            Mth = transaction.Mth.ToShortDateString();
            APTransId = transaction.APTransId;
            BatchId = transaction.BatchId;
            VendorGroupId = transaction.VendorGroupId;
            VendorId = transaction.VendorId;
            if (transaction.Vendor != null)
            {
                Vendor = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", transaction.VendorId, transaction.Vendor.Name.ToCamelCase());
                VendorDescription = transaction.Vendor.Name.ToCamelCase();
                VendorIdDisplay = transaction.VendorId.ToString(AppCultureInfo.CInfo());
            }

            APReference = transaction.APRef;
            Description = transaction.Description;
            InvoiceDate = transaction.InvDate;
            DueDate = transaction.DueDate;
            Amount = transaction.InvTotal;
            PaidAmount = transaction.Lines.Sum(s => s.Payments.Where(f => f.PaidDate != null).Sum(sum => sum.Amount));
            AllMth = transaction.Mth.ToShortDateString();
            Mth = transaction.Mth.ToShortDateString();
            MthDate = transaction.Mth;
            #endregion

        }


        [HiddenInput]
        public byte? GLCo { get; set; }
        [HiddenInput]
        public byte? PRCo { get; set; }
        [HiddenInput]
        public byte? EMCo { get; set; }
        [HiddenInput]
        public byte? JCCo { get; set; }
        [HiddenInput]
        public byte? EMGroupId { get; set; }
        [HiddenInput]
        public byte? PhaseGroupId { get; set; }
        [HiddenInput]
        public byte? VendorGroupId { get; set; }


        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Company")]
        public byte Co { get; set; }
        public byte POCo { get; private set; }
        [Key]
        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/GLAccount/APAllMthCombo", ComboForeignKeys = "GLCo=Co")]
        [Display(Name = "Batch Month")]
        public string Mth { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Trans Id")]
        public int APTransId { get; set; }

        [Required]
        [HiddenInput]
        [Display(Name = "Batch Id")]
        public int BatchId { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/APCombo/VendorCombo", ComboForeignKeys = "POCo", SearchUrl = "/APCombo/VendorSearch", SearchForeignKeys = "VendGroupId=VendorGroupId")]
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
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/POCombo/VendorCombo", ComboForeignKeys = "POCo=APCo, VendorId", SearchUrl = "/POCombo/Search", SearchForeignKeys = "VendGroupId=VendorGroupId, VendorId")]
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
        public DateTime? InvoiceDate { get; set; }

        [Required]
        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Due Date")]
        public DateTime? DueDate { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Amount")]
        public decimal? Amount { get; set; }


        [UIHint("CurrencyBox")]
        [Display(Name = "Paid")]
        public decimal? PaidAmount { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Cleared Amount")]
        public decimal? ClearedAmount { get; set; }
    }
}