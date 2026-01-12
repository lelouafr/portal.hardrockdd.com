using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Areas.AccountsReceivable.Models
{   
    public class InvoiceListViewModel
    {
        public InvoiceListViewModel()
        {

        }

        public InvoiceListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));

            CustGroupId = (byte)company.CustGroupId;

            List = company.ARTransactions.Select(s => new InvoiceViewModel(s)).ToList();
        }


        public InvoiceListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company, DB.TimeSelectionEnum timeSelection)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));
            var asofDate = timeSelection switch
            {
                DB.TimeSelectionEnum.LastMonth => DateTime.Now.AddMonths(-1),
                DB.TimeSelectionEnum.LastThreeMonths => DateTime.Now.AddMonths(-3),
                DB.TimeSelectionEnum.LastSixMonths => DateTime.Now.AddMonths(-6),
                DB.TimeSelectionEnum.LastYear => DateTime.Now.AddYears(-1),
                DB.TimeSelectionEnum.All => DateTime.Now.AddYears(-99),
                _ => DateTime.Now.AddYears(-99),
            };
            var mthFlt = new DateTime(asofDate.Year, asofDate.Month, 1);

            CustGroupId = (byte)company.CustGroupId;

            List = company.ARTransactions.Where(f => f.Mth > mthFlt).Select(s => new InvoiceViewModel(s)).ToList();
        }

        public InvoiceListViewModel(DB.Infrastructure.ViewPointDB.Data.Customer customer)
        {
            if (customer == null)
                throw new System.ArgumentNullException(nameof(customer));

            CustGroupId = customer.CustGroupId;
            var fldDate = DateTime.Now.AddYears(-10);
            fldDate = new DateTime(fldDate.Year, fldDate.Month, 1);
            var trans = customer.ARTrans.Where(f => f.Mth >= fldDate && f.ARTransType == "I").ToList();

            CustomerId = customer.CustomerId;
            List = trans.OrderByDescending(o => o.Mth).ThenByDescending(o => o.TransDate).Where(f => f.Mth >= fldDate).Select(s => new InvoiceViewModel(s)).ToList();
        }

        public InvoiceListViewModel(DB.Infrastructure.ViewPointDB.Data.Job job)
        {
            if (job == null)
                return;

            JCCo = job.JCCo;
            CustGroupId = (byte)job.Contract?.CustGroupId;
            JobId = job.JobId;

            var trans = job.Contract.ARTrans.Where(f => f.ARTransType == "I").ToList();

            List = trans.OrderByDescending(o => o.Mth).ThenByDescending(o => o.TransDate).Select(s => new InvoiceViewModel(s)).ToList();
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Co")]
        public byte CustGroupId { get; set; }

        [Key]
        [Required]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/CustomerCombo/Combo", ComboForeignKeys = "CustGroupId", SearchUrl = "/CustomerCombo/Search", SearchForeignKeys = "CustGroupId")]
        [Display(Name = "Customer")]
        public int? CustomerId { get; set; }

        public byte JCCo { get; set; }

        public string JobId { get; set; }

        public List<InvoiceViewModel> List { get; }
    }

    public class InvoiceViewModel 
    {
        public InvoiceViewModel()
        {

        }

        public InvoiceViewModel(DB.Infrastructure.ViewPointDB.Data.ARTran transaction) 
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            #region mapping
            ARCo = transaction.ARCo;
            Mth = transaction.Mth.ToShortDateString();
            ARTransId = transaction.ARTransId;
            CustGroupId = transaction.CustGroup;
            CustomerId = transaction.CustomerId;
            Invoice = transaction.Invoice;
            if (transaction.Customer != null)
            {
                var customer = transaction.Customer;
                Customer = string.Format(AppCultureInfo.CInfo(), "{0} - {1}", transaction.CustomerId, customer.Name.ToCamelCase());
                CustomerDescription = customer.Name.ToCamelCase();
            }

            CustRef = transaction.CustRef;
            Description = transaction.Description;
            InvoiceDate = transaction.TransDate;
            DueDate = transaction.DueDate;
            Amount = transaction.Invoiced;
            PaidAmount = transaction.Paid;
            AllMth = transaction.Mth.ToShortDateString();
            Mth = transaction.Mth.ToShortDateString();
            MthDate = transaction.Mth;
            JCCo = transaction.JCCo;
            ContractId = transaction.ContractId;

            var contract = transaction.Contract;
            if (contract != null)
            {
                var job = transaction.Contract.Jobs.FirstOrDefault();
                if (job != null)
                {
                    JobId = job.JobId;
                    JobDescription = string.Format("{0} - {1}", job.JobId, job.Description);
                }
            }
            #endregion

            Lines = new InvoiceLineListViewModel(transaction);
        }

        public byte? CustGroupId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Company")]
        public byte ARCo { get; set; }

        [Key]
        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/GLAccount/APAllMthCombo", ComboForeignKeys = "GLCo=ARCo")]
        [Display(Name = "Batch Month")]
        public string Mth { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Trans Id")]
        public int ARTransId { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/CustomerCombo/Combo", ComboForeignKeys = "CustGroupId", SearchUrl = "/CustomerCombo/Search", SearchForeignKeys = "CustGroupId")]
        [Display(Name = "Customer")]
        public int? CustomerId { get; set; }

        [UIHint("TextBox")]
        [Field()]
        [Display(Name = "Customer")]
        public string Customer { get; set; }

        [UIHint("TextBox")]
        [Field()]
        [Display(Name = "Customer")]
        public string CustomerDescription { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Cust Ref")]
        public string CustRef { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Invoice")]
        public string Invoice { get; set; }

        public byte? JCCo { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 10, ComboUrl = "/JCCombo/ContractCombo", ComboForeignKeys = "JCCo")]
        [Display(Name = "Contract")]
        public string ContractId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/JCCombo/Combo", ComboForeignKeys = "JCCo")]
        [Display(Name = "Job")]
        public string JobId { get; set; }

        [UIHint("Job Description")]
        [Field(LabelSize = 0, TextSize = 6)]
        [Display(Name = "Job")]
        public string JobDescription { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/GLAccount/APAllMthCombo", ComboForeignKeys = "GLCo=ARCo")]
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