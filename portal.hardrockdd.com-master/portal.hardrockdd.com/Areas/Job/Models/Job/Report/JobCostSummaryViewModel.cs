using DB.Infrastructure.ViewPointDB.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace portal.Areas.Job.Models.Job.Report
{
    public class JobCostSummaryListViewModel
    {
        public JobCostSummaryListViewModel()
        {

        }

        public JobCostSummaryListViewModel(DB.Infrastructure.ViewPointDB.Data.Job job)
        {
            if (job == null)
                return;

            Co = job.JCCo;
            JobId = job.JobId;

            var jobCosts = job.ActualJobCosts();
            DetailedList = jobCosts.Select(s => new Cost.ViewModel(s)).ToList();

            var jobUnPostCosts = job.UnPostedJobCosts();
            DetailedList.AddRange(jobUnPostCosts.Select(s => new Cost.ViewModel(s, true)).ToList());
        }

        
        public List<Cost.ViewModel> DetailedList { get; }

        [Key]
        public byte Co { get; set; }

        [Key]
        public string JobId { get; set; }
    }

}