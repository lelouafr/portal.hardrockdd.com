using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Areas.HumanResource.Models.Resource.Form
{
    public class BenefitListViewModel
    {
        public BenefitListViewModel()
        {

        }
        public BenefitListViewModel(HRResource resource)
        {
            if (resource == null)
                return;

            HRCo = resource.HRCo;
            ResourceId = resource.HRRef;
        }

        [Key]
        public byte HRCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int ResourceId { get; set; }

    }
    public class BenefitViewModel
    {
        public BenefitViewModel()
        {

        }

        public BenefitViewModel(HRResourceBenefit benefit)
        {
            if (benefit == null)
                return;
        }
        [Key]
        [UIHint("LongBox")]
        public byte HRCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        public int HRRef { get; set; }

        [Key]
        [UIHint("DropdownBox")]
        [Display(Name = "Requestor")]
        [Field(ComboUrl = "/HRCombo/BenefitCodeCombo")]
        public string BenefitCodeId { get; set; }

        [Key]
        [UIHint("LongBox")]
        public int DependentSeq { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Elgibility Date")]
        public DateTime? EligDate { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Effect Date")]
        public DateTime? EffectDate { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }
    }
}