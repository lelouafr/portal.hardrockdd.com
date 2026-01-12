using portal.Code.Data.VP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.WA.Applicant
{
    public class LicenseInfoViewModel
    {
        public LicenseInfoViewModel()
        {

        }

        public LicenseInfoViewModel(Code.Data.VP.WebApplicant applicant)
        {
            if (applicant == null)
                return;

            ApplicantId = applicant.ApplicantId;
            LicNumber = applicant.LicNumber;
            LicType = applicant.LicType;
            LicState = applicant.LicState;
            LicExpDate = applicant.LicExpDate;
            LicClass = applicant.LicClass;
            LicCountry = applicant.LicCountry;
            //LicDenied = applicant.LicDenied;
            //LicRevoked = applicant.LicRevoked;
            //LicDeniedRevokedReason = applicant.LicDeniedRevokedReason;

        }

        [Key]
        public int ApplicantId { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Number")]
        public string LicNumber { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Type")]
        public string LicType { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "Country")]
        public string LicCountry { get; set; }

        [Required]
        [UIHint("DropDownBox")]
        [Display(Name = "State")]
        public string LicState { get; set; }

        [Required]
        [UIHint("DateBox")]
        [Display(Name = "Exp Date")]
        public DateTime? LicExpDate { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Class")]
        public string LicClass { get; set; }

        [Required]
        [UIHint("SwitchBox")]
        [Display(Name = "Denied")]
        public bool LicDenied { get; set; }

        [Required]
        [UIHint("SwitchBox")]
        [Display(Name = "Revoked")]
        public bool LicRevoked { get; set; }

        [UIHint("TextAreaBox")]
        [Display(Name = "Denied/Revoked Reason")]
        public string LicDeniedRevokedReason { get; set; }

        internal void Validate(System.Web.Mvc.ModelStateDictionary modelState)
        {

            if (string.IsNullOrEmpty(LicNumber))
                modelState.AddModelError("LicNumber", "License Number may not be blank");

            if (string.IsNullOrEmpty(LicState))
                modelState.AddModelError("LicState", "License State may not be blank");

            if (LicExpDate == null)
                modelState.AddModelError("LicExpDate", "License Expire Date may not be blank");

            if (string.IsNullOrEmpty(LicType))
                modelState.AddModelError("LicType", "License Type may not be blank");


        }

        internal LicenseInfoViewModel ProcessUpdate(VPEntities db, ModelStateDictionary modelState)
        {
            var updObj = db.WebApplicants.FirstOrDefault(f => f.ApplicantId == this.ApplicantId);

            if (updObj != null)
            {
                this.Validate(modelState);

                updObj.LicNumber = this.LicNumber;
                updObj.LicType = this.LicType;
                updObj.LicState = this.LicState;
                updObj.LicExpDate = this.LicExpDate;
                updObj.LicClass = this.LicClass;
                updObj.LicCountry = this.LicCountry;
                try
                {
                    db.BulkSaveChanges();
                    return new LicenseInfoViewModel(updObj);
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