using portal.Models.Views.PM.Project.Form;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Models.Views.PM.Project.Schedule
{
    public class FormViewModel : AuditBaseViewModel
    {
        public FormViewModel()
        {

        }

        public FormViewModel(DB.Infrastructure.ViewPointDB.Data.Job job) : base(job)
        {
            if (job == null)
                throw new System.ArgumentNullException(nameof(job));

            JCCo = job.JCCo;
            JobId = job.JobId;

            if (job.BidBoreLine != null)
            {
                BDCo = job.BidBoreLine.BDCo;
                BidId = job.BidBoreLine.BidId;
                PackageId = (int)job.BidBoreLine.PackageId;
            }

            Crews = new ProjectCrewListViewModel(job);
            ProjectInfo = new ProjectInfoViewModel(job);
           // TimeLine = new TimeLineListViewModel(job);
            Jobs = new JobPhaseSummaryListViewModel(job);
        }

        public FormViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackage package) : base(package)
        {
            if (package == null)
                throw new System.ArgumentNullException(nameof(package));

            BDCo = package.BDCo;
            BidId = package.BidId;
            PackageId = package.PackageId;

            //Crews = new BidPackageCrewListViewModel(package);
            //ProjectInfo = new ProjectInfoViewModel(package);
        }

        [Key]
        [HiddenInput]
        public byte JCCo { get; set; }

        [Key]
        [HiddenInput]
        public string JobId { get; set; }


        [Key]
        [HiddenInput]
        public byte BDCo { get; set; }

        [Key]
        [HiddenInput]
        public int BidId { get; set; }


        [Key]
        [HiddenInput]
        public int PackageId { get; set; }

        public ProjectInfoViewModel ProjectInfo { get; set; }

        public ProjectCrewListViewModel Crews { get; set; }



        //public TimeLineListViewModel TimeLine { get; set; }

        public JobPhaseSummaryListViewModel Jobs { get; set; }


    }
}