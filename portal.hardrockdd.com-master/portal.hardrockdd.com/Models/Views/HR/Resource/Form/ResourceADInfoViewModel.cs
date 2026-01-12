using portal.Code.Data.VP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.HR.Resource.Form
{
    public class ResourceADInfoViewModel
    {
        public ResourceADInfoViewModel()
        {

        }

        public ResourceADInfoViewModel(HRResource resource)
        {
            if (resource == null) throw new System.ArgumentNullException(nameof(resource));

            HRCo = resource.HRCo;
            EmployeeId = resource.HRRef;

            IsDomainAccount = resource.AddActiveDirectory == "Y";
            IsAccountActive = resource.ADActive == "Y";

            CompanyEmail = resource.CompanyEmail;
            ADPassword = resource.ADPassword;

            HasEmail = resource.HasCompanyEmail == "Y";

            PortalActive = resource.PortalAccountActive == "Y";
            if (resource.Position != null)
            {
                CanChangePassword = resource.Position.AutoAssignOrg == "Y" || resource.WebId == StaticFunctions.GetUserId();

            }
            else
            {
                CanChangePassword = true;
            }
        }

        [Key]
        [HiddenInput]
        public byte HRCo { get; set; }

        [Key]
        [HiddenInput]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int EmployeeId { get; set; }

        [UIHint("SwitchBox")]
        [Display(Name = "Domain Account")]
        public bool IsDomainAccount { get; set; }

        [UIHint("SwitchBox")]
        [Display(Name = "Active")]
        public bool IsAccountActive { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Password Reset")]
        public string ADPassword { get; set; }

        public bool CanChangePassword { get; set; }

        [UIHint("EmailBox")]
        [Display(Name = "User Name")]
        public string CompanyEmail { get; set; }

        [UIHint("SwitchBox")]
        [Display(Name = "Has Email")]
        public bool HasEmail { get; set; }

        [UIHint("SwitchBox")]
        [Display(Name = "Portal Active")]
        public bool PortalActive { get; set; }

        internal ResourceADInfoViewModel ProcessUpdate(ModelStateDictionary modelState, VPEntities db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            var updObj = db.HRResources.Where(f => f.PRCo == this.HRCo && f.HRRef == this.EmployeeId).FirstOrDefault();

            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.HasCompanyEmail = this.HasEmail ? "Y" : "N";
                updObj.AddActiveDirectory = this.IsDomainAccount ? "Y" : "N";
                updObj.ADActive = this.IsAccountActive ? "Y" : "N";
                updObj.PortalAccountActive = this.PortalActive ? "Y" : "N";

                updObj.CompanyEmail = this.CompanyEmail;
                updObj.ADPassword = this.ADPassword;
                try
                {
                    db.SaveChanges(modelState);
                    return new ResourceADInfoViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            modelState.AddModelError("", "Object Doesn't Exist For Update!");
            return this;
        }
    }
}