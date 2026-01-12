using System.ComponentModel.DataAnnotations;

namespace portal.Areas.Applicant.Models
{
    public class FormViewModel
    {
        public FormViewModel()
        {

        }

        public FormViewModel(DB.Infrastructure.ViewPointDB.Data.WebApplicant applicant)
        {
            if (applicant == null)
                return;
            ApplicantId = applicant.ApplicantId;

            Info = new ApplicantInfoViewModel(applicant);
            EmergencyInfo = new EmergencyContactViewModel(applicant);
            LicenseInfo = new LicenseInfoViewModel(applicant);
            CurrentApplication = new ApplicationFormViewModel(applicant.CurrentApplication());
            Applications = new ApplicationListViewModel(applicant);
            HRNotes = new NotesViewModel(applicant);
            Forum = new portal.Models.Views.Forums.ForumLineListViewModel(applicant.CurrentApplication().GetForum());
        }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "#")]
        public int ApplicantId { get; set; }

        public ApplicantInfoViewModel Info { get; set; }

        public EmergencyContactViewModel EmergencyInfo { get; set; }

        public LicenseInfoViewModel LicenseInfo { get; set; }

        public ApplicationFormViewModel CurrentApplication { get; set; }

        public ApplicationListViewModel Applications { get; set; }

        public portal.Models.Views.Forums.ForumLineListViewModel Forum { get; set; }

        public NotesViewModel HRNotes { get; set; }

        internal void Validate(System.Web.Mvc.ModelStateDictionary modelState)
        {

            Info.Validate(modelState);
            EmergencyInfo.Validate(modelState);
            LicenseInfo.Validate(modelState);
            CurrentApplication.Validate(modelState);

            if (CurrentApplication.Info.Status == DB.WAApplicationStatusEnum.Approved)
            {

            }
        }
    }

}