using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Models.Views.PM.Project.Form
{
    public class FormViewModel : AuditBaseViewModel
    {
        public FormViewModel()
        {

        }

        public FormViewModel(DB.Infrastructure.ViewPointDB.Data.Job project) : base(project)
        {
            if (project == null)
                throw new System.ArgumentNullException(nameof(project));

            Co = project.JCCo;
            JobId = project.JobId;

            ProjectInfo = new ProjectInfoViewModel(project);
            Crews = new ProjectCrewListViewModel(project);
            JobPhases = new JobPhaseSummaryListViewModel(project);
            Jobs = new JobListViewModel(project);
        }

        [Key]
        [HiddenInput]
        public byte Co { get; set; }

        [Key]
        [HiddenInput]
        public string JobId { get; set; }

        public ProjectInfoViewModel ProjectInfo { get; set; }

        public ProjectCrewListViewModel Crews { get; set; }

        public JobListViewModel Jobs { get; set; }

        public JobPhaseSummaryListViewModel JobPhases { get; set; }

    }
}