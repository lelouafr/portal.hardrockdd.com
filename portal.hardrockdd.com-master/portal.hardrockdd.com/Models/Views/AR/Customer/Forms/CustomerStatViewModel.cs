using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.AR.Customer.Forms
{
    public class CustomerStatListViewModel
    {
        const int StartingYear = 2017;
        public CustomerStatListViewModel()
        {

        }

        public CustomerStatListViewModel(DB.Infrastructure.ViewPointDB.Data.Customer customer)
        {
            if (customer == null) throw new System.ArgumentNullException(nameof(customer));

            var years = customer.ARTrans.Where(f => f.TransDate.Year >= StartingYear).GroupBy(g => g.TransDate.Year).Select(s => s.Key).OrderByDescending(j => j).ToList();

            List = years.Select(s => new CustomerStatViewModel(s, customer)).ToList();
        }
        [HiddenInput]
        [Key]
        public byte Co { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Number")]
        public int CustomerId { get; set; }

        public List<CustomerStatViewModel> List { get; set; }
    }

    public class CustomerStatViewModel
    {
        const int StartingYear = 2017;
        public CustomerStatViewModel()
        {

        }

        public CustomerStatViewModel(int year, DB.Infrastructure.ViewPointDB.Data.Customer customer)
        {
            if (customer == null) throw new System.ArgumentNullException(nameof(customer));
            CustGroupId = customer.CustGroupId;
            CustomerId = customer.CustomerId;
            Year = year;

            var jobs = customer.JobContracts
                .Where(f => f.ARTrans.Any() && f.ARTrans.Max(max => (max == null ? 0 : max.TransDate.Year)) == year)
                .SelectMany(f => f.Jobs)
                .Select(s => s)
                .ToList();
            var costs = jobs.SelectMany(s => s.JobCosts).Where(w => w.JCTransType != "PO" && w.JCTransType != "OE").Select(s => s).ToList();
            Cost = costs.Sum(sum => sum == null ? 0 : sum.ActualCost);

            Revenue = customer.ARTrans.Where(f => f.ARTransType == "I" && f.TransDate.Year == year).Sum(sum => sum == null ? 0 : sum.Invoiced);
            Paid = customer.ARTrans.Where(f => f.ARTransType == "I" && f.TransDate.Year == year).Sum(sum => sum == null ? 0 : sum.Paid);
            Balance = Revenue - Paid;
            GrossProfit = Revenue - Cost;
            if (Revenue != 0)
            {
                GrossMargin = Math.Round((Revenue - Cost) / Revenue, 4);
            }

            AvgInvoiceDays = customer.ARTrans.Where(f => f.ARTransType == "I" && f.PayFullDate != null && f.TransDate.Year == year).Average(avg => (avg.PayFullDate - avg.TransDate)?.TotalDays);
            AvgInvoiceDays = Math.Round(AvgInvoiceDays ?? 0, 0);
            //AvgInvoiceDays = customer.ARTrans.Where(f => f.ARTransType == "I").Average(avg => SqlFunctions.DateDiff("day", avg.DueDate, (avg.PayFullDate == null ? SqlFunctions.GetDate() : avg.PayFullDate)));
        }


        public CustomerStatViewModel(DB.Infrastructure.ViewPointDB.Data.Customer customer)
        {
            if (customer == null) throw new System.ArgumentNullException(nameof(customer));
            CustGroupId = customer.CustGroupId;
            CustomerId = customer.CustomerId;

            var jobs = customer.JobContracts
                .Where(f => f.ARTrans.Any(f => f.TransDate.Year >= StartingYear))
                .SelectMany(f => f.Jobs)
                .Select(s => s)
                .ToList();
            var costs = jobs.SelectMany(s => s.JobCosts).Where(w => w.JCTransType != "PO" && w.JCTransType != "OE").Select(s => s).ToList();

            Cost = costs.Sum(sum => sum == null ? 0 : sum.ActualCost);
            Revenue = customer.ARTrans.Where(f => f.ARTransType == "I" && f.TransDate.Year >= StartingYear).Sum(sum => sum == null ? 0 : sum.Invoiced);
            Paid = customer.ARTrans.Where(f => f.ARTransType == "I" && f.TransDate.Year >= StartingYear).Sum(sum => sum == null ? 0 : sum.Paid);
            Balance = Revenue - Paid;
            GrossProfit = Revenue - Cost;
            if (Revenue != 0)
            {
                GrossMargin = Math.Round((Revenue - Cost) / Revenue, 4);
            }

            AvgInvoiceDays = customer.ARTrans.Where(f => f.ARTransType == "I" && f.PayFullDate != null && f.TransDate.Year >= StartingYear).Average(avg => (avg.PayFullDate - avg.TransDate)?.TotalDays);
            AvgInvoiceDays = Math.Round(AvgInvoiceDays ?? 0, 0);
            //AvgInvoiceDays = customer.ARTrans.Where(f => f.ARTransType == "I").Average(avg => SqlFunctions.DateDiff("day", avg.DueDate, (avg.PayFullDate == null ? SqlFunctions.GetDate() : avg.PayFullDate)));
        }

        [HiddenInput]
        [Key]
        public byte CustGroupId { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Number")]
        public int CustomerId { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Year")]
        public int Year { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Revenue")]
        public decimal Revenue { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Cost")]
        public decimal Cost { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Balance")]
        public decimal Balance { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Paid")]
        public decimal Paid { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Avg. Pay Days")]
        public double? AvgInvoiceDays { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Profit")]
        public decimal GrossProfit { get; set; }

        [UIHint("PercentBox")]
        [Display(Name = "GM")]
        public decimal GrossMargin { get; set; }




    }
}