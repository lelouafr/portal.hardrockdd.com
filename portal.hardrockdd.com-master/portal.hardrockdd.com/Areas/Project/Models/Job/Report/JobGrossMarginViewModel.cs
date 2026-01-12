using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Areas.Project.Models.Job.Report
{
    public class GrossMarginListViewModel
    {
        public GrossMarginListViewModel()
        {

        }

        public GrossMarginListViewModel(DateTime mth)
        {
            using var db = new VPContext();

            var costJobList = db.JobCosts.Where(f => f.Mth == mth && f.ActualCost != 0).Select(s => s.Job).Distinct();

            costJobList = costJobList.Where(f => f.JobCosts.Sum(f => f.ActualCost) != 0);
            var revJobList = db.JCInvoiceDetails
                .Where(f => f.Mth == mth && f.BilledAmt != 0)
                .Select(s => s.JCContract)
                .SelectMany(s => s.Jobs)
                .Distinct();
            var jobList = costJobList.Union(revJobList).ToList();
            List = jobList.Select(s => new GrossMarginViewModel(s, mth)).ToList();
            Mth = mth.Date.ToShortDateString();

            GLCo = 1;
        }

        [Key]
        [UIHint("DropDownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/GLAccount/APAllMthCombo", ComboForeignKeys = "GLCo")]
        [Display(Name = "Statement Month")]
        public string Mth { get; set; }

        public byte GLCo { get; set; }

        public List<GrossMarginViewModel> List { get; }
    }

    public class GrossMarginViewModel
    {
        public GrossMarginViewModel()
        {

        }

        public GrossMarginViewModel(DB.Infrastructure.ViewPointDB.Data.Job job, DateTime mth)
        {
            if (job == null)
                return;

            Mth = mth;
            JCCo = job.JCCo;
            JobId = job.JobId;
            JobName = job.Description;
            Division = job.Division.WPDivision.Description;
            CrewName = job.Crew?.Description;
            
            Revenue = job.Contract.InvoiceDetails.Where(f => f.JCTransType != "OC").Sum(sum => sum.BilledAmt);
            Cost = job.ActualJobCosts().Where(f => (!(f.Job.JobType == DB.JCJobTypeEnum.Job || f.Job.JobType == DB.JCJobTypeEnum.Project) && f.Mth == mth) || (f.Job.JobType == DB.JCJobTypeEnum.Job || f.Job.JobType == DB.JCJobTypeEnum.Project)).Sum(sum => sum.ActualCost);
            GM = (Revenue ?? 0) - (Cost ?? 0);
            GMPercent = (Revenue ?? 0) == 0 ? 0 : GM / Revenue;

            RevenueCurrent = job.Contract.InvoiceDetails.Where(f => f.Mth == mth && f.JCTransType != "OC").Sum(sum => sum.BilledAmt);
            CostCurrent = job.ActualJobCosts(mth).Sum(sum => sum.ActualCost);
            GMCurrent = (RevenueCurrent ?? 0) - (CostCurrent ?? 0);
            GMPercentCurrent = (RevenueCurrent ?? 0) == 0 ? 0 : GM / RevenueCurrent;
        }

        [Key]
        public byte JCCo { get; set; }

        [Key]
        public string JobId { get; set; }

        [Key]
        public DateTime Mth { get; set; }

        public string JobName { get; set; }

        public string Division { get; set; }

        public string CrewName { get; set; }

        public decimal? Revenue { get; set; }

        public decimal? Cost { get; set; }

        public decimal? GM { get; set; }

        public decimal? GMPercent { get; set; }


        public decimal? RevenueCurrent { get; set; }

        public decimal? CostCurrent { get; set; }

        public decimal? GMCurrent { get; set; }

        public decimal? GMPercentCurrent { get; set; }


    }
}