using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.AR.Customer.Forms
{
    public class CustomerJobListViewModel
    {
        public CustomerJobListViewModel()
        {

        }

        public CustomerJobListViewModel(DB.Infrastructure.ViewPointDB.Data.Customer customer)
        {
            if (customer == null) throw new System.ArgumentNullException(nameof(customer));
            CustGroupId = customer.CustGroupId;
            CustomerId = customer.CustomerId;

            var jobs = customer.JobContracts
                .Where(f => f.ARTrans.Any(f => f.TransDate.Year >= 2017))
                .SelectMany(f => f.Jobs)
                .Select(s => s)
                .ToList();

            //var jobs = customer.JobContracts.ARTrans.Where(f => f.TransDate.Year >= 2017)
            //    .SelectMany(s => s.Jobs)
            //    .Select(s => s)
            //    .OrderByDescending(j => j.JobId)
            //    .ToList();
            List = jobs.Select(s => new CustomerJobViewModel(s)).ToList();
        }
        [HiddenInput]
        [Key]
        public byte CustGroupId { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Number")]
        public int CustomerId { get; set; }

        public List<CustomerJobViewModel> List { get; }
    }
    public class CustomerJobViewModel: portal.Areas.Project.Models.Job.JobViewModel
    {
        public CustomerJobViewModel()
        {

        }

        public CustomerJobViewModel(DB.Infrastructure.ViewPointDB.Data.Job job): base (job)
        {
            if (job == null) throw new System.ArgumentNullException(nameof(job));

            var costs = job.JobCosts.Where(w => w.JCTransType != "PO" && w.JCTransType != "OE").Select(s => s).ToList();
            Cost = costs.Sum(sum => sum == null ? 0 : sum.ActualCost);
            Revenue = job.Contract.ARTrans.Where(f => f.ARTransType == "I").Sum(sum => sum == null ? 0 : sum.Invoiced);
            Paid = job.Contract.ARTrans.Where(f => f.ARTransType == "I").Sum(sum => sum == null ? 0 : sum.Paid);
            Balance = Revenue - Paid;
            GrossProfit = Revenue - Cost;
            if (Revenue != 0)
            {
                GrossMargin = Math.Round((Revenue - Cost) / Revenue, 4);
            }
        }
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

        [UIHint("CurrencyBox")]
        [Display(Name = "Profit")]
        public decimal GrossProfit { get; set; }

        [UIHint("PercentBox")]
        [Display(Name = "GM")]
        public decimal GrossMargin { get; set; }




    }
}