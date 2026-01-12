//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.JC.Job.Cost;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Runtime.Caching;
//using System.Web.Mvc;

//namespace portal.Models.Views.JC.Job.Forms
//{
//    public class JobRevSummaryListViewModel
//    {
//        public JobRevSummaryListViewModel()
//        {

//        }

//        public JobRevSummaryListViewModel(DB.Infrastructure.ViewPointDB.Data.Job job)
//        {
//            //var grp = jobCosts.GroupBy(g => new { g.JCCo, g.JobId, g.CostTypeID, g.CostType.Description, g.JCTransType, g.Source })
//            //                 .Select(s => new JobCostSummaryViewModel {
//            //                     JCCo = s.Key.JCCo,
//            //                     JobId = s.Key.JobId,
//            //                     CostTypeID = s.Key.CostTypeID,
//            //                     CostTypeDesc= s.Key.Description,
//            //                     JCTransType = s.Key.JCTransType,
//            //                     Source = s.Key.Source,
//            //                     List = s.Select(c => new JobCostViewModel(c)).ToList()
//            //                 })
//            //                 .ToList();
//            Co = job.JCCo;
//            JobId = job.JobId;

//            var jobCosts = job.ActualJobCosts();
//            DetailedList = jobCosts.Select(s => new JobCostViewModel(s)).ToList();

//            var jobUnPostCosts = job.UnPostedJobCosts();
//            DetailedList.AddRange(jobUnPostCosts.Select(s => new JobCostViewModel(s, true)).ToList());
//        }


//        public JobRevSummaryListViewModel(DB.Infrastructure.ViewPointDB.Data.Job job, DateTime mth)
//        {
            
//            Co = job.JCCo;
//            JobId = job.JobId;

//            var jobCosts = job.ActualJobCosts(mth);
//            var jobUnPostCosts = job.UnPostedJobCosts(mth);

//            var allCost = jobCosts.Union(jobUnPostCosts).ToList();
//            DetailedList = jobCosts.Select(s => new JobCostViewModel(s)).ToList();

//            DetailedList.AddRange(jobUnPostCosts.Select(s => new JobCostViewModel(s, true)).ToList());

//        }

        
//        public List<JobCostViewModel> DetailedList { get; }

//        public byte Co { get; set; }

//        public string JobId { get; set; }
//    }

//}