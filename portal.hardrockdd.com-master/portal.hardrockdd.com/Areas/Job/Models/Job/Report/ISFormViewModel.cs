using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Areas.Job.Models.Job.Report
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
            Revenue = new Revenue.ListViewModel(job);
            Cost = new Cost.ListViewModel(job);
        }

        public ISFormViewModel(DB.Infrastructure.ViewPointDB.Data.Job job, DateTime mth)
        {
            if (job == null || mth == null)
                return;

            JCCo = job.JCCo;
            JobId = job.JobId;
            Mth = mth;

            Revenue = new Revenue.ListViewModel(job);
            Cost = new Cost.ListViewModel(job);
        }

        [Key]
        public byte JCCo { get; set; }

        [Key]
        public string JobId { get; set; }

        public DateTime? Mth { get; set; }

        public Revenue.ListViewModel Revenue { get; set; }
        
        public Cost.ListViewModel Cost { get; set; }
    }
}