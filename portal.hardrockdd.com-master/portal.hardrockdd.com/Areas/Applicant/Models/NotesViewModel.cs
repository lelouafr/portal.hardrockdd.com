using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Applicant.Models
{
    public class NotesViewModel
    {
        public NotesViewModel()
        {

        }

        public NotesViewModel(DB.Infrastructure.ViewPointDB.Data.WebApplicant applicant)
        {
            if (applicant == null)
                return;

            ApplicantId = applicant.ApplicantId;
            Notes = applicant.Notes;

        }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "#")]
        public int ApplicantId { get; set; }

        [UIHint("TextAreaBox")]
        [Display(Name = "Notes")]
        [Field(TextSize = 12, LabelSize = 0)]
        public string Notes { get; set; }

        internal void Validate(System.Web.Mvc.ModelStateDictionary modelState)
        {
            
        }

        internal NotesViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.WebApplicants.FirstOrDefault(f => f.ApplicantId == this.ApplicantId);

            if (updObj != null)
            {
                updObj.Notes = this.Notes;

                try
                {
                    db.BulkSaveChanges();
                    return new NotesViewModel(updObj);
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