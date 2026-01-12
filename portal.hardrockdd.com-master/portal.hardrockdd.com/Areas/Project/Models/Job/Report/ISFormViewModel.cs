using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Areas.Project.Models.Job.Report
{
    public class ISFormViewModel
    {
        public ISFormViewModel()
        {

        }

        public ISFormViewModel(DB.Infrastructure.ViewPointDB.Data.Job job)
        {
            if (job == null)
                return;

            JCCo = job.JCCo;
            JobId = job.JobId;
            Revenue = new Contract.RevenueListViewModel(job);
            Cost = new CostListViewModel(job);
        }

        public ISFormViewModel(DB.Infrastructure.ViewPointDB.Data.Job job, DateTime mth)
        {
            if (job == null || mth == null)
                return;

            JCCo = job.JCCo;
            JobId = job.JobId;
            Mth = mth;

            Revenue = new Contract.RevenueListViewModel(job);
            Cost = new CostListViewModel(job);
        }

        [Key]
        public byte JCCo { get; set; }

        [Key]
        public string JobId { get; set; }

        public DateTime? Mth { get; set; }

        public Contract.RevenueListViewModel Revenue { get; set; }
        
        public CostListViewModel Cost { get; set; }
    }
}