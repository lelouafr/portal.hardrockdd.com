using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.DailyTicket
{
    public class DailyFormViewModel
    {
        public DailyFormViewModel()
        {

        }

        public DailyFormViewModel(DailyForm form)
        {
            if (form == null)
            {
                throw new ArgumentNullException(nameof(form));
            }
            FormId = form.FormId;
            Description = form.Description;
            Active = form.Active;
            ControllerName = form.ControllerName;
        }

        [Key]
        [Required]
        [Display(Name = "Form Id")]
        public int FormId { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Active")]
        public string Active { get; set; }

        [Display(Name = "Controller Name")]
        public string ControllerName { get; set; }

        internal DailyFormViewModel ProcessUpdate(DB.Infrastructure.ViewPointDB.Data.VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.DailyForms.FirstOrDefault(f => f.FormId == FormId);

            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.Description = Description;
                updObj.Active = Active;
                updObj.ControllerName = ControllerName;
                try
                {
                    db.BulkSaveChanges();
                    return new DailyFormViewModel(updObj);
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