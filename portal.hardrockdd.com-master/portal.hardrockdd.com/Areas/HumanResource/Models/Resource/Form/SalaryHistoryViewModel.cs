using DB.Infrastructure.ViewPointDB.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.HumanResource.Models.Resource.Form
{
    public class SalaryHistoryListViewModel
    {
        public SalaryHistoryListViewModel()
        {
            List = new List<SalaryHistoryViewModel>();
        }


        public SalaryHistoryListViewModel(HRResource resource)
        {
            if (resource == null) throw new System.ArgumentNullException(nameof(resource));

            HRCo = resource.HRCo;
            ResourceId = resource.HRRef;

            List = resource.SaleryHistory.Select(s => new SalaryHistoryViewModel(s)).ToList();
        }

        [Key]
        public byte HRCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int ResourceId { get; set; }

        public List<SalaryHistoryViewModel> List { get;  }
    }

    public class SalaryHistoryViewModel
    {
        public SalaryHistoryViewModel()
        {

        }
        
        public SalaryHistoryViewModel(HRSalaryHistory salaryHistory)
        {
            if (salaryHistory == null) throw new System.ArgumentNullException(nameof(salaryHistory));

            HRCo = salaryHistory.HRCo;
            PRCo = (byte)salaryHistory.HREmployee.PRCo;

            ResourceId = salaryHistory.HRRef;

            EffectiveDate = salaryHistory.EffectiveDate;
            Type = salaryHistory.Type;
            OldSalary = salaryHistory.OldSalary;
            NewSalary = salaryHistory.NewSalary;
            NewPositionCode = salaryHistory.NewPositionCode;

        }

        [Key]
        [HiddenInput]
        public byte HRCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int ResourceId { get; set; }

        [Key]
        [UIHint("DateBox")]
        [Display(Name = "Effective Date")]
        public System.DateTime EffectiveDate { get; set; }

        public byte PRCo { get; set; }
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PRCombo/SalaryTypeCombo", ComboForeignKeys = "PRCo")]
        [Display(Name = "Type")]
        public string Type { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Old Salary")]
        public decimal? OldSalary { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "New Salary")]
        public decimal? NewSalary { get; set; }


        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/HRCombo/PositionCodeCombo", ComboForeignKeys = "HRCo")]
        [Display(Name = "New Position")]
        public string NewPositionCode { get; set; }

    }
}