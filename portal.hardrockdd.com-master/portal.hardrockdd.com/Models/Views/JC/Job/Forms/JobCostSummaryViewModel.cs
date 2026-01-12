using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.JC.Job.Cost;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace portal.Models.Views.JC.Job.Forms
{
    public class JobCostSummaryListViewModel
    {
        public JobCostSummaryListViewModel()
        {

        }

        public JobCostSummaryListViewModel(DB.Infrastructure.ViewPointDB.Data.Job job)
        {
            Co = job.JCCo;
            JobId = job.JobId;

            var jobCosts = job.ActualJobCosts();
            DetailedList = jobCosts.Select(s => new JobCostViewModel(s)).ToList();

            var jobUnPostCosts = job.UnPostedJobCosts();
            DetailedList.AddRange(jobUnPostCosts.Select(s => new JobCostViewModel(s, true)).ToList());
        }

        
        public List<JobCostViewModel> DetailedList { get; }

        [Key]
        public byte Co { get; set; }

        [Key]
        public string JobId { get; set; }
    }

}