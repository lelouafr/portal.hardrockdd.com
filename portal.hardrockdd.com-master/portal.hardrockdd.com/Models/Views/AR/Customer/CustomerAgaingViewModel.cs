using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.AR.Customer.Aging

{
    public class CustomerAgaingListViewModel
    {
        public CustomerAgaingListViewModel()
        {
            List = new List<CustomerAgaingViewModel>();
        }

        public CustomerAgaingListViewModel(DB.Infrastructure.ViewPointDB.Data.VPContext db)
        {
            var list = db.udARTH_Aging(30, 1, DateTime.Now, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), DateTime.Now, DateTime.Now).ToList();
            DivisionId = 1;
            list = list.OrderBy(o => o.Name).ToList();
            List = list.Select(s => new CustomerAgaingViewModel(s)).ToList();
        }

        public CustomerAgaingListViewModel(DB.Infrastructure.ViewPointDB.Data.CompanyDivision division)
        {
            var list = division.db.udARTH_AgingByDivision(30, 1, division.DivisionId, DateTime.Now, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), DateTime.Now, DateTime.Now).ToList();

            list = list.OrderBy(o => o.Name).ToList();
            DivisionId = division.DivisionId;

            List = list.Select(s => new CustomerAgaingViewModel(s)).ToList();
        }

        [Key]
        [UIHint("DropdownBox")]
        [Display(Name = "Division")]
        [Field(LabelSize = 1, TextSize = 2, ComboUrl = "/WPCombo/WPDivisionCombo", ComboForeignKeys = "")]
        public int DivisionId { get; set; }

        public List<CustomerAgaingViewModel> List { get; }


    }

    public class CustomerAgaingViewModel
    {
        public CustomerAgaingViewModel()
        {

        }

        public CustomerAgaingViewModel(DB.Infrastructure.ViewPointDB.Data.udARTH_Aging_Result trans)
        {
            if (trans == null)
                return;

            #region mapping
            CustGroupId = (byte)trans.CustGroup;
            CustomerId = (int)trans.Customer;
            CustomerName = trans.Name;
            CustomerAvgDays = trans.DaysOutstanding ?? 0;
            CompanyDaysOutstanding = trans.CompanyDaysOutstanding ?? 0;
            LastPayDate = trans.LastPaidDate;

            Job = trans.Job;
            JobDescription = string.Format("{0}: {1}", trans.Job, trans.Job_Description);
            Invoice = trans.Invoice;
            InvoiceDescription = trans.Invoice_Description;

            InvoiceDate = trans.InvoiceDate;
            Current = trans.Current ?? 0;
            Bucket1 = trans.Bucket_1 ?? 0;
            Bucket2 = trans.Bucket_2 ?? 0;
            Bucket3 = trans.Bucket_3 ?? 0;
            Bucket4 = trans.Bucket_4 ?? 0;
            Bucket5 = trans.Bucket_5 ?? 0;
            Total = trans.Total ?? 0;
            #endregion
        }


        public CustomerAgaingViewModel(DB.Infrastructure.ViewPointDB.Data.udARTH_AgingByDivision_Result trans)
        {
            if (trans == null)
                return;

            #region mapping
            CustGroupId = (byte)trans.CustGroup;
            CustomerId = (int)trans.Customer;
            CustomerName = trans.Name;
            CustomerAvgDays = trans.DaysOutstanding ?? 0;
            CompanyDaysOutstanding = trans.CompanyDaysOutstanding ?? 0;
            LastPayDate = trans.LastPaidDate;

            Job = trans.Job;
            JobDescription = string.Format("{0}: {1}", trans.Job, trans.Job_Description);
            Invoice = trans.Invoice;
            InvoiceDescription = trans.Invoice_Description;

            InvoiceDate = trans.InvoiceDate;
            Current = trans.Current ?? 0;
            Bucket1 = trans.Bucket_1 ?? 0;
            Bucket2 = trans.Bucket_2 ?? 0;
            Bucket3 = trans.Bucket_3 ?? 0;
            Bucket4 = trans.Bucket_4 ?? 0;
            Bucket5 = trans.Bucket_5 ?? 0;
            Total = trans.Total ?? 0;
            #endregion
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Vendor Group")]
        public byte CustGroupId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Id")]
        public int CustomerId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Name")]
        public string CustomerName { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Avg Days")]
        public int? CustomerAvgDays { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Avg Days")]
        public int? CompanyDaysOutstanding { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Job")]
        public string Job { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Job Description")]
        public string JobDescription { get; set; }
        

        [UIHint("TextBox")]
        [Display(Name = "Invoice")]
        public string Invoice { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Description")]
        public string InvoiceDescription { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Inv. Date")]
        public DateTime? InvoiceDate { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Last Pay Date")]
        public DateTime? LastPayDate { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Future")]
        public decimal? Current { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Current")]
        public decimal? Bucket1 { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "31-60")]
        public decimal? Bucket2 { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "61-90")]
        public decimal? Bucket3 { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "91-120")]
        public decimal? Bucket4 { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "120+")]
        public decimal? Bucket5 { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Total")]
        public decimal? Total { get; set; }

        [UIHint("TextAreaBox")]
        [Display(Name = "Comments")]
        public string Comments { get; set; }

    }
}
