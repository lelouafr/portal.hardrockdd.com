using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace portal.Areas.Applicant.Models
{
    public class ApplicantListViewModel
    {
        public ApplicantListViewModel()
        {

        }

        public ApplicantListViewModel(List<DB.Infrastructure.ViewPointDB.Data.WebApplicant> list)
        {
            List = list.Select(s => new ApplicantViewModel(s)).ToList();

        }
        public List<ApplicantViewModel> List { get; }
    }


    public class ApplicantViewModel
    {
        public ApplicantViewModel()
        {

        }

        public ApplicantViewModel(DB.Infrastructure.ViewPointDB.Data.WebApplicant applicant)
        {
            if (applicant == null)
                return;

            ApplicantId = applicant.ApplicantId;
            SSN = applicant.SSN;

            FirstName = applicant.FirstName;
            LastName = applicant.LastName;
            MiddleName = applicant.MiddleName;
            Nickname = applicant.Nickname;

            City = applicant.City;
            State = applicant.State;

            var app = applicant.CurrentApplication();
            if  (app != null)
            {
                AppliedDate = app.ApplicationDate;
                Status = app.Status.ToString();
                var pos = app.CurrentPosition();
                if (pos != null)
                {
                    Position = pos.HRPositionCode?.Description ?? "unknown";
                }
            }
        }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "#")]
        public int ApplicantId { get; set; }

        [UIHint("SSN")]
        [Display(Name = "SSN")]
        public string SSN { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "First")]
        public string FirstName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Last" )]
        public string LastName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Middle")]
        public string MiddleName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Nick name")]
        public string Nickname { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "Status")]
        public string Status { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Applied Date")]
        public DateTime? AppliedDate { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Position")]
        public string Position { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "City")]
        public string City { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "State")]
        public string State { get; set; }


    }
}