using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.AccountsPayable.Models
{
    public class InvoiceListViewModel
    {
        public InvoiceListViewModel()
        {

        }


        public InvoiceListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));

            Co = company.HQCo;

            //List = company.AP.Select(s => new InvoiceViewModel(s)).ToList();
        }

        public InvoiceListViewModel(DB.Infrastructure.ViewPointDB.Data.APVendor vendor)
        {
            if (vendor == null)
            {
                throw new System.ArgumentNullException(nameof(vendor));
            }
            Co = vendor.VendorGroupId;
            VendorGroupId = vendor.VendorGroupId;
            var fldDate = DateTime.Now.AddYears(-10);
            fldDate = new DateTime(fldDate.Year, fldDate.Month, 1);
            var trans = vendor.APTransactions.Where(f => f.Mth >= fldDate);

            while (trans.Count() >= 500 && fldDate < DateTime.Now)
            {
                if (fldDate.Year < DateTime.Now.Year)
                {
                    fldDate = fldDate.AddYears(1);
                }
                else
                {
                    fldDate = fldDate.AddMonths(1);
                }
                trans = trans.Where(f => f.Mth >= fldDate);
            }
            List = trans.OrderByDescending(o => o.Mth).ThenByDescending(o => o.InvDate).Where(f => f.Mth >= fldDate).Select(s => new InvoiceViewModel(s)).ToList();

            
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Co")]
        public byte Co { get; set; }

        public byte? VendorGroupId { get; set; }

        [Key]
        [Required]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/APCombo/VendorCombo", ComboForeignKeys = "VendGroupId=VendorGroupId", SearchUrl = "/APCombo/VendorSearch", SearchForeignKeys = "VendorGroupId")]
        [Display(Name = "Vendor")]
        public int? VendorId { get; set; }


        public List<InvoiceViewModel> List { get; }
    }

    public class InvoiceViewModel
    {
        public InvoiceViewModel()
        {

        }

        public InvoiceViewModel(DB.Infrastructure.ViewPointDB.Data.APTran document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            #region mapping
            APCo = document.APCo;
            GLCo = document.APCo;

            Mth = document.Mth.ToShortDateString();
            APTransId = document.APTransId;
            VendorGroupId = document.VendorGroupId;
            VendorId = document.VendorId;
            if (document.Vendor != null)
            {
                Vendor = string.Format(AppCultureInfo.CInfo(), "{0} - {1}", document.VendorId, document.Vendor.Name.ToCamelCase());
                VendorDescription = document.Vendor.Name.ToCamelCase();
                VendorIdDisplay = document.VendorId.ToString(AppCultureInfo.CInfo());
            }
            CMAcct = document.CMAcct;
            APReference = document.APRef;
            Description = document.Description;
            InvoiceDate = document.InvDate;
            DueDate = document.DueDate;
            Amount = document.InvTotal;
            PaidAmount = document.Lines.Sum(s => s.Payments.Where(f => f.PaidDate != null).Sum(sum => sum.Amount));
            AllMth = document.Mth.ToShortDateString();
            Mth = document.Mth.ToShortDateString();
            MthDate = document.Mth;
            #endregion

            Lines = new InvoiceLineListViewModel(document);
        }

        [HiddenInput]
        public byte? VendorGroupId { get; set; }

        [HiddenInput]
        public byte GLCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Company")]
        public byte APCo { get; set; }

        [Key]
        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/GLAccount/APAllMthCombo", ComboForeignKeys = "GLCo")]
        [Display(Name = "Batch Month")]
        public string Mth { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Trans Id")]
        public int APTransId { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/APCombo/VendorCombo", ComboForeignKeys = "VendGroupId=VendorGroupId", SearchUrl = "/APCombo/VendorSearch", SearchForeignKeys = "VendGroupId=VendorGroupId")]
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

        [UIHint("TextBox")]
        [Field()]
        [Display(Name = "CM Account")]
        public short? CMAcct { get; set; }

        //[UIHint("DropdownBox")]
        //[Field(LabelSize = 2, TextSize = 4, ComboUrl = "/POCombo/VendorCombo", ComboForeignKeys = "POCo, VendorId", SearchUrl = "/POCombo/Search", SearchForeignKeys = "POCo, VendorId")]
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
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/GLAccount/APAllMthCombo", ComboForeignKeys = "GLCo")]
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

        public InvoiceLineListViewModel Lines { get; set; }


    }
}