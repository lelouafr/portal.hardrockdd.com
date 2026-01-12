using DB.Infrastructure.ViewPointDB.Data;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Areas.HumanResource.Models.Resource.Form
{
    public class ActionViewModel
    {
        public ActionViewModel()
        {


        }

        public ActionViewModel(HRResource entity)
        {
            if (entity == null) throw new System.ArgumentNullException(nameof(entity));

            HRCo = entity.HRCo;
            EmployeeId = entity.HRRef;
            CanRequestTermination = false;

            if (entity.IsSupervisor(StaticFunctions.GetCurrentEmployee().EmployeeId))
            {
                CanRequestTermination = true;
            }

            

        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Co")]
        public byte HRCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Id")]
        public int EmployeeId { get; set; }

        public DB.SessionAccess Access { get; set; }

        public bool CanRequestTermination { get; set; }

    }


}