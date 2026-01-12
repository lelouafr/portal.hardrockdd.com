using portal.Code.Data.VP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.WA.Application
{
    public class WorkHistoryListViewModel
    {
        public WorkHistoryListViewModel()
        {

        }

        public WorkHistoryListViewModel(Code.Data.VP.WebApplication application)
        {
            if (application == null)
                return;

            ApplicantId = application.ApplicantId;
            ApplicationId = application.ApplicationId;

            List = application.WorkExperiences.Select(s => new WorkHistoryViewModel(s)).ToList();
        }

        [Key]
        public int ApplicantId { get; set; }
        [Key]
        public int ApplicationId { get; set; }


        public List<WorkHistoryViewModel> List { get; }
    }

    public class WorkHistoryViewModel
    {

        public WorkHistoryViewModel()
        {

        }

        public WorkHistoryViewModel(Code.Data.VP.WAWorkExperience experience)
        {
            if (experience == null)
                return;

            ApplicantId = experience.ApplicantId;
            ApplicationId = experience.ApplicationId;
            SeqId = experience.SeqId;
            Employer = experience.Employer;
            AddressCity = experience.AddressCity;
            AddressState = experience.AddressState;
            AddressStreet = experience.AddressStreet;
            AddressZip = experience.AddressZip;
            JobDuties = experience.JobDuties;
            Position = experience.Position;
            ReasonForLeaving = experience.ReasonForLeaving;
            Salary = experience.Salary;
            SupervisorName = experience.SupervisorName;
            WorkFromDate = experience.WorkFromDate;
            WorkToDate = experience.WorkToDate;
            WorkPhone = experience.WorkPhone;
        }

        [Key]
        public int ApplicantId { get; set; }
        [Key]
        public int ApplicationId { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "#")]
        public int SeqId { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Employer")]
        public string Employer { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "City")]
        public string AddressCity { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "State")]
        public string AddressState { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Street")]
        public string AddressStreet { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Zip")]
        public string AddressZip { get; set; }

        [Required]
        [UIHint("TextAreaBox")]
        [Display(Name = "Job Duties")]
        public string JobDuties { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Position")]
        public string Position { get; set; }

        [UIHint("TextAreaBox")]
        [Display(Name = "Reason For Leaving")]
        public string ReasonForLeaving { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Salary")]
        public decimal? Salary { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Supervisor")]
        public string SupervisorName { get; set; }

        [Required]
        [UIHint("DateBox")]
        [Display(Name = "Start Date")]
        public DateTime? WorkFromDate { get; set; }

        [Required]
        [UIHint("DateBox")]
        [Display(Name = "End Date")]
        public DateTime? WorkToDate { get; set; }

        [Required]
        [UIHint("PhoneBox")]
        [Display(Name = "Work Phone")]
        public string WorkPhone { get; set; }

        internal void Validate(System.Web.Mvc.ModelStateDictionary modelState)
        {
            if (string.IsNullOrEmpty(Employer)) modelState.AddModelError("Employer", "Employer may not be blank");
            if (string.IsNullOrEmpty(AddressCity)) modelState.AddModelError("AddressCity", "City may not be blank");
            if (string.IsNullOrEmpty(AddressState)) modelState.AddModelError("AddressState", "State may not be blank");
            if (string.IsNullOrEmpty(AddressStreet)) modelState.AddModelError("AddressStreet", "Street may not be blank");
            if (string.IsNullOrEmpty(AddressZip)) modelState.AddModelError("AddressZip", "Zip Ne may not be blank");
            if (string.IsNullOrEmpty(JobDuties)) modelState.AddModelError("JobDuties", "Job Duties may not be blank");
            if (string.IsNullOrEmpty(Position)) modelState.AddModelError("Position", "Position may not be blank");
            if (string.IsNullOrEmpty(ReasonForLeaving)) modelState.AddModelError("ReasonForLeaving", "Reason For Leaving may not be blank");
            if (string.IsNullOrEmpty(SupervisorName)) modelState.AddModelError("SupervisorName", "Supervisor Name may not be blank");
            if (WorkFromDate == null) modelState.AddModelError("WorkFromDate", "From Date may not be blank");
            if (WorkToDate == null) modelState.AddModelError("WorkToDate", "To Date may not be blank");
            if (string.IsNullOrEmpty(WorkPhone)) modelState.AddModelError("WorkPhone", "Work Phone may not be blank");

        }

        internal WorkHistoryViewModel ProcessUpdate(VPEntities db, ModelStateDictionary modelState)
        {
            //throw new NotImplementedException();
            var updObj = db.WAWorkExperiences.FirstOrDefault(f => f.ApplicantId == this.ApplicantId &&
                                                                  f.ApplicationId == this.ApplicationId &&
                                                                  f.SeqId == this.SeqId);

            if (updObj != null)
            {
                this.Validate(modelState);

                updObj.Employer = this.Employer;
                updObj.AddressCity = this.AddressCity;
                updObj.AddressState = this.AddressState;
                updObj.AddressStreet = this.AddressStreet;
                updObj.AddressZip = this.AddressZip;
                updObj.JobDuties = this.JobDuties;
                updObj.Position = this.Position;
                updObj.ReasonForLeaving = this.ReasonForLeaving;
                updObj.Salary = this.Salary;
                updObj.SupervisorName = this.SupervisorName;
                updObj.WorkFromDate = this.WorkFromDate;
                updObj.WorkToDate = this.WorkToDate;
                updObj.WorkPhone = this.WorkPhone;
                try
                {
                    db.BulkSaveChanges();
                    return new WorkHistoryViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            else
            {
                modelState.AddModelError("", "Object Doesn't Exist For Update!");
                return this;
            }
        }
    }
}