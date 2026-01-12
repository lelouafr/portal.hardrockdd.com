using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.JC.Job.Forms
{
    public class JobISFormViewModel
    {
        public JobISFormViewModel()
        {

        }

        public JobISFormViewModel(DB.Infrastructure.ViewPointDB.Data.Job job)
        {
            if (job == null)
                return;

            Co = job.JCCo;
            JobId = job.JobId;
            Revenue = new Revenue.JobRevenueListViewModel(job);
            Cost = new Cost.JobCostListViewModel(job);
        }

        public JobISFormViewModel(DB.Infrastructure.ViewPointDB.Data.Job job, DateTime mth)
        {
            if (job == null || mth == null)
                return;

            Co = job.JCCo;
            JobId = job.JobId;
            Mth = mth;

            Revenue = new Revenue.JobRevenueListViewModel(job);
            Cost = new Cost.JobCostListViewModel(job);
        }


        [Key]
        public byte Co { get; set; }

        [Key]
        public string JobId { get; set; }

        public DateTime? Mth { get; set; }

        public Revenue.JobRevenueListViewModel Revenue { get; set; }
        
        public Cost.JobCostListViewModel Cost { get; set; }
    }
}