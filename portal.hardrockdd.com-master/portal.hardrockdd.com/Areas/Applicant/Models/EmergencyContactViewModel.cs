using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;



namespace portal.Areas.Applicant.Models
{
    public class EmergencyContactViewModel
    {
        public EmergencyContactViewModel()
        {

        }

        public EmergencyContactViewModel(DB.Infrastructure.ViewPointDB.Data.WebApplicant applicant)
        {
            if (applicant == null)
                return;

            ApplicantId = applicant.ApplicantId;
            EmergencyContact = applicant.EmergencyContact;
            EmegencyRelationship = applicant.EmegencyRelationship;
            ContactPhone = applicant.ContactPhone;
            EmegencyContactPhone = applicant.EmegencyContactPhone;
        }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "#")]
        public int ApplicantId { get; set; }
                
        [UIHint("TextBox")]
        [Display(Name = "Emergency Contact")]
        public string EmergencyContact { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Relationship")]
        public string EmegencyRelationship { get; set; }

        [UIHint("PhoneBox")]
        [Display(Name = "Contact Phone")]
        public string ContactPhone { get; set; }

        [UIHint("PhoneBox")]
        [Display(Name = "Contact Phone")]
        public string EmegencyContactPhone { get; set; }

        internal void Validate(System.Web.Mvc.ModelStateDictionary modelState)
        {
           


        }

        internal EmergencyContactViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.WebApplicants.FirstOrDefault(f => f.ApplicantId == this.ApplicantId);

            if (updObj != null)
            {
                updObj.EmergencyContact = this.EmergencyContact;
                updObj.EmegencyRelationship = this.EmegencyRelationship;
                updObj.ContactPhone = this.ContactPhone;
                updObj.EmegencyContactPhone = this.EmegencyContactPhone;

                try
                {
                    db.BulkSaveChanges();
                    return new EmergencyContactViewModel(updObj);
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