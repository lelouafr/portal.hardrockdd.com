using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace portal.Models.Views.PM.Project.Form
{
    public class JobPhaseSummaryListViewModel
    {
        public JobPhaseSummaryListViewModel()
        {

        }

        public JobPhaseSummaryListViewModel(DB.Infrastructure.ViewPointDB.Data.Job project)
        {
            if (project == null)
                return;
            
            JCCo = project.JCCo;
            ProjectId = project.JobId;

            if (!project.SubJobs.Any())
                return;
            var cnt = project.SubJobs.Max(max => max.JobPhaseTracks.Where(f => f.JobPhase.Description.Contains("Ream")).DefaultIfEmpty().Max(subMax => subMax == null ? 0 :  subMax.PassId));
            ReamPassCount = cnt;
            List = project.SubJobs.Select(s => new JobPhaseSummaryViewModel(s)).ToList();
        }

        [Key]
        public byte JCCo { get; set; }

        [Key]
        public string ProjectId { get; set; }

        public int ReamPassCount { get; set; }

        public List<JobPhaseSummaryViewModel> List { get;  }
    }

    public class JobPhaseSummaryViewModel: JobViewModel
    {
        public JobPhaseSummaryViewModel()
        {

        }

        public JobPhaseSummaryViewModel(DB.Infrastructure.ViewPointDB.Data.Job job) : base(job)
        {
            if (job == null) 
                return;

            JCCo = job.JCCo;
            JobId = job.JobId;

            IsComplete = job.Status == DB.JCJobStatusEnum.Completed;

            IsRigUpComplete = job.IsRigUpComplete;
            IsPilotComplete = job.IsPilotComplete;
            IsReam1Complete = job.IsReam1Complete;
            IsReam2Complete = job.IsReam2Complete;
            IsReam3Complete = job.IsReam3Complete;
            IsReam4Complete = job.IsReam4Complete;
            IsReam5Complete = job.IsReam5Complete;
            IsPullPipeComplete = job.IsPullPipeComplete;
            IsRigDownComplete = job.IsRigDownComplete;

            PilotPercentComplete = job.PilotPercentComplete;
            Ream1PercentComplete = job.Ream1PercentComplete;
            Ream2PercentComplete = job.Ream2PercentComplete;
            Ream3PercentComplete = job.Ream3PercentComplete;
            Ream4PercentComplete = job.Ream4PercentComplete;
            Ream5PercentComplete = job.Ream5PercentComplete;
            PullPipePercentComplete = job.PullPipePercentComplete;


        }

        [UIHint("SmallSwitchBox")]
        public bool IsComplete { get; set; }

        public decimal PercentComplete { get; set; }

        [Display(Name = "Rig Up")]
        [UIHint("SmallSwitchBox")]
        public bool IsRigUpComplete { get; set; }

        [Display(Name = "Pilot")]
        [UIHint("SmallSwitchBox")]
        public bool IsPilotComplete { get; set; }

        [Display(Name = "Ream")]
        [UIHint("SmallSwitchBox")]
        public bool IsReam1Complete { get; set; }

        [Display(Name = "Ream 2")]
        [UIHint("SmallSwitchBox")]
        public bool IsReam2Complete { get; set; }

        [Display(Name = "Ream 3")]
        [UIHint("SmallSwitchBox")]
        public bool IsReam3Complete { get; set; }

        [Display(Name = "Ream 4")]
        [UIHint("SmallSwitchBox")]
        public bool IsReam4Complete { get; set; }

        [Display(Name = "Ream 5")]
        [UIHint("SmallSwitchBox")]
        public bool IsReam5Complete { get; set; }

        [Display(Name = "Pulled")]
        [UIHint("SmallSwitchBox")]
        public bool IsPullPipeComplete { get; set; }

        [Display(Name = "Rig Down")]
        [UIHint("SmallSwitchBox")]
        public bool IsRigDownComplete { get; set; }


        [UIHint("PercentBox")]
        public decimal PilotPercentComplete { get; set; }

        [UIHint("PercentBox")]
        public decimal Ream1PercentComplete { get; set; }

        [UIHint("PercentBox")]
        public decimal Ream2PercentComplete { get; set; }

        [UIHint("PercentBox")]
        public decimal Ream3PercentComplete { get; set; }

        [UIHint("PercentBox")]
        public decimal Ream4PercentComplete { get; set; }

        [UIHint("PercentBox")]
        public decimal Ream5PercentComplete { get; set; }

        [UIHint("PercentBox")]
        public decimal PullPipePercentComplete { get; set; }


        public decimal PilotUnitsComplete { get; set; }

        public decimal Ream1UnitsComplete { get; set; }

        public decimal Ream2UnitsComplete { get; set; }

        public decimal Ream3UnitsComplete { get; set; }

        public decimal Ream4UnitsComplete { get; set; }

        public decimal Ream5UnitsComplete { get; set; }

        public decimal PullPipeUnitsComplete { get; set; }



    }
}