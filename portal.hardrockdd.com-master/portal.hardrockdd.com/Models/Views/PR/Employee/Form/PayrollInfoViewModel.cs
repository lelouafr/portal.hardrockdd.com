using portal.Code.Data.VP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.PR.Employee.Form
{
    public class PayrollInfoViewModel
    {
        public PayrollInfoViewModel()
        {

        }

        public PayrollInfoViewModel(HRResource resource)
        {
            if (resource == null) throw new System.ArgumentNullException(nameof(resource));

            PRCo = resource.HRCo;
            ResourceId = resource.HRRef;
            
            PositionId = resource.PositionCode;
            SSN = resource.SSN;

            Status = resource.EmployeeStatus();
            EarnCodeId = resource.PREmployee.EarnCodeId;
            PRDept = resource.PREmployee.PRDept;

            StdInsCode = resource.StdInsCode;
            StdTaxState = resource.StdTaxState;
            StdUnempState = resource.StdUnempState;
            StdInsState = resource.StdInsState;
        }

        [Key]
        [HiddenInput]
        public byte PRCo { get; set; }

        [Key]
        [HiddenInput]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int ResourceId { get; set; }

        [UIHint("SSNBox")]
        [Display(Name = "SSN")]
        public string SSN { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/HRCombo/PositionCodeCombo", ComboForeignKeys = "PRCo")]
        [Display(Name = "Position")]
        public string PositionId { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public HRActiveStatusEnum Status { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PRCombo/PREmployeeEarnCodeCombo", ComboForeignKeys = "PRCo")]
        [Display(Name = "Earn Code")]
        public short EarnCodeId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PRCombo/PRDepartmentCombo", ComboForeignKeys = "PRCo")]
        [Display(Name = "Department")]
        public string PRDept { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PRCombo/PRInsuranceCodeCombo", ComboForeignKeys = "PRCo")]
        [Display(Name = "Ins Code")]
        public string StdInsCode { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/HQCombo/StateCombo", ComboForeignKeys = "HQCo=PRCo")]
        [Display(Name = "Tax State")]
        public string StdTaxState { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/HQCombo/StateCombo", ComboForeignKeys = "HQCo=PRCo")]
        [Display(Name = "Unemp State")]
        public string StdUnempState { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/HQCombo/StateCombo", ComboForeignKeys = "HQCo=PRCo")]
        [Display(Name = "Ins State")]
        public string StdInsState { get; set; }


    }
}