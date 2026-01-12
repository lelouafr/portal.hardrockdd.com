using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Areas.Applicant.Models
{
    public class ApplicationListViewModel
    {
        public ApplicationListViewModel()
        {

        }

        public ApplicationListViewModel(DB.Infrastructure.ViewPointDB.Data.WebApplicant applicant)
        {
            if (applicant == null)
                return;

            ApplicantId = applicant.ApplicantId;

            List = applicant.Applications.OrderByDescending(o => o.ApplicationDate).Select(s => new ApplicationViewModel(s)).ToList();
        }

        [Key]
        public int ApplicantId { get; set; }

        public List<ApplicationViewModel> List { get; }
    }

    public class ApplicationViewModel
    {
        public ApplicationViewModel()
        {

        }

        public ApplicationViewModel(DB.Infrastructure.ViewPointDB.Data.WebApplication application)
        {
            if (application == null)
                return;

            ApplicantId = application.ApplicantId;
            ApplicationId = application.ApplicationId;
            ApplicationDate = application.ApplicationDate;
            Status = application.Status;
            HireDate = application.HireDate;
            ExpireDate = application.ExpireDate;
            IsActive = application.ActiveYN == "Y" ? true : false;
            ExpireDate = application.ExpireDate;
            Accidents = application.Accidents;
            Citiations = application.Citiations;

            Actions = new ActionViewModel(application);
        }

        [Key]
        public int ApplicantId { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "#")]
        public int ApplicationId { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Application Date")]
        public DateTime ApplicationDate { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.WAApplicationStatusEnum Status { get; set; }


        [UIHint("DateBox")]
        [Display(Name = "Hire Date")]
        public DateTime? HireDate { get; set; }


        [UIHint("DateBox")]
        [Display(Name = "Expire Date")]
        public DateTime? ExpireDate { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Accidents")]
        public bool IsActive { get; set; }


        [UIHint("EnumBox")]
        [Display(Name = "Accidents")]
        public bool Accidents { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Citiations")]
        public bool Citiations { get; set; }

        public ActionViewModel Actions { get; set; }
    }
}