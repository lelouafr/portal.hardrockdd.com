using Newtonsoft.Json;
using portal.Code.Data.VP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace portal.Models.Views.HR.Resource.Form
{
    public class ResourceSalaryHistoryListViewModel
    {
        public ResourceSalaryHistoryListViewModel()
        {
            List = new List<ResourceSalaryHistoryViewModel>();
        }


        public ResourceSalaryHistoryListViewModel(HRResource resource)
        {
            if (resource == null) throw new System.ArgumentNullException(nameof(resource));

            HRCo = resource.HRCo;
            ResourceId = resource.HRRef;

            List = resource.SaleryHistory.Select(s => new ResourceSalaryHistoryViewModel(s)).ToList();
        }

        [Key]
        public byte HRCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int ResourceId { get; set; }

        public List<ResourceSalaryHistoryViewModel> List { get;  }
    }

    public class ResourceSalaryHistoryViewModel
    {
        public ResourceSalaryHistoryViewModel()
        {

        }
        
        public ResourceSalaryHistoryViewModel(Code.Data.VP.HRSalaryHistory salaryHistory)
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