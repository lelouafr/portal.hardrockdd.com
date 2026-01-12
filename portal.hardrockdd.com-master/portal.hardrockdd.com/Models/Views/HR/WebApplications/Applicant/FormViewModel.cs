using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.WA.Applicant
{
    public class FormViewModel
    {
        public FormViewModel()
        {

        }

        public FormViewModel(Code.Data.VP.WebApplicant applicant)
        {
            if (applicant == null)
                return;
            ApplicantId = applicant.ApplicantId;

            Info = new ApplicantInfoViewModel(applicant);
            EmergencyInfo = new EmergencyContactViewModel(applicant);
            LicenseInfo = new LicenseInfoViewModel(applicant);
            CurrentApplication = new Application.FormViewModel(applicant.CurrentApplication());
            Applications = new Application.ApplicationListViewModel(applicant);

            Forum = new Forums.ForumLineListViewModel(applicant.CurrentApplication().GetForum());
        }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "#")]
        public int ApplicantId { get; set; }

        public ApplicantInfoViewModel Info { get; set; }

        public EmergencyContactViewModel EmergencyInfo { get; set; }

        public LicenseInfoViewModel LicenseInfo { get; set; }

        public Application.FormViewModel CurrentApplication { get; set; }

        public Application.ApplicationListViewModel Applications { get; set; }

        public Forums.ForumLineListViewModel Forum { get; set; }

        internal void Validate(System.Web.Mvc.ModelStateDictionary modelState)
        {

            Info.Validate(modelState);
            EmergencyInfo.Validate(modelState);
            LicenseInfo.Validate(modelState);
            CurrentApplication.Validate(modelState);

            if (CurrentApplication.Info.Status == WAApplicationStatusEnum.Approved)
            {

            }
        }
    }

}