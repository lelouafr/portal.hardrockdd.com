using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Areas.Applicant.Models
{
    public class DrugTestListViewModel
    {
        public DrugTestListViewModel()
        {

        }

        public DrugTestListViewModel(DB.Infrastructure.ViewPointDB.Data.WebApplication application)
        {
            if (application == null)
                return;

            ApplicantId = application.ApplicantId;
            ApplicationId = application.ApplicationId;

            List = application.DrugTests.Select(s => new DrugTestViewModel(s)).ToList();
        }

        [Key]
        public int ApplicantId { get; set; }
        [Key]
        public int ApplicationId { get; set; }

        public List<DrugTestViewModel> List { get; }
    }

    public class DrugTestViewModel
    {
        public DrugTestViewModel()
        {

        }

        public DrugTestViewModel(DB.Infrastructure.ViewPointDB.Data.WAApplicantDrugTest drugTest)
        {
            if (drugTest == null)
                return;


            ApplicantId = drugTest.ApplicantId;
            ApplicationId = drugTest.ApplicationId;
            SeqId = drugTest.SeqId;
            TestDate = drugTest.TestDate;
            TestType = drugTest.TestType;
            TestStatusId = drugTest.TestStatusId;
            Results = drugTest.Results;
            Location = drugTest.Location;
            ActionTaken = drugTest.ActionTaken;
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
        [UIHint("Datebox")]
        [Display(Name = "Date")]
        public DateTime? TestDate { get; set; }

        [Required]
        [UIHint("DropDownBox")]
        [Display(Name = "Type")]
        [Field(ComboUrl = "/HRCombo/HRTestCodesTypeCombo", ComboForeignKeys = "HRCo=1,type=D")]
        public string TestType { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Location")]
        public string Location { get; set; }

        [Required]
        [UIHint("DropDownBox")]
        [Display(Name = "Status")]
        [Field(ComboUrl = "/HRCombo/HRTestCodesTypeCombo", ComboForeignKeys = "HRCo=1,type=U")]
        public string TestStatusId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Results")]
        public string Results { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Action Taken")]
        public string ActionTaken { get; set; }

        internal DrugTestViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.WAApplicantDrugTests.FirstOrDefault(f => f.ApplicantId == this.ApplicantId &&
                                                                     f.ApplicationId == this.ApplicationId &&
                                                                     f.SeqId == this.SeqId);

            if (updObj != null)
            {
                updObj.TestDate = this.TestDate;
                updObj.TestType = this.TestType;
                updObj.TestStatusId = this.TestStatusId;
                updObj.Results = this.Results;
                updObj.Location = this.Location;
                updObj.ActionTaken = this.ActionTaken;

                try
                {
                    db.BulkSaveChanges();
                    return new DrugTestViewModel(updObj);
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