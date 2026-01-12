using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace portal.Models.Views.PR.Report
{
    public class PayrateListViewModel
    {
        public PayrateListViewModel()
        {
            List = new List<PayrateViewModel>();
        }


        public PayrateListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company, int year)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));

            PRCo = company.HQCo;
            Year = year;

            List = company.PRCompanyParm.PREmployees.Where(f => f.EmployeeId < 900000 && f.ActiveYN == "Y").Select(s => new PayrateViewModel(s)).ToList();
        }


        [Key]
        public byte PRCo { get; set; }

        [Key]
        public int Year { get; set; }

        public List<PayrateViewModel> List { get;  }
    }

    public class PayrateViewModel
    {
        public PayrateViewModel()
        {

        }
        
        public PayrateViewModel(DB.Infrastructure.ViewPointDB.Data.Employee prEmployee)
        {
            if (prEmployee == null) throw new System.ArgumentNullException(nameof(prEmployee));
            var resource = prEmployee.Resource.FirstOrDefault();

            PRCo = prEmployee.PRCo;
            EmployeeId = prEmployee.EmployeeId;

            Name = prEmployee.FullName();
            Status = resource?.EmployeeStatus() ?? DB.HRActiveStatusEnum.Inactive;
            PositionId = resource?.PositionCode;
            Position = resource?.Position?.Description;
            Division = prEmployee.Division?.Description;

            HourlyRate = prEmployee.HrlyRate;
            SalaryRate = prEmployee.SalaryAmt;
            PerDiemRate = (decimal)(prEmployee.PerDiemRate ?? 0);
            DailyRate = prEmployee.udDailyRate ?? 0;

            StatusString = Status.ToString();
        }

        [Key]
        [HiddenInput]
        public byte PRCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int EmployeeId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/HRCombo/PositionCodeCombo", ComboForeignKeys = "HRCo")]
        [Display(Name = "Position")]
        public string PositionId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Position")]
        public string Position { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "Division")]
        public string Division { get; set; }



        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.HRActiveStatusEnum Status { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public string StatusString { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Hourly Rate")]
        public decimal HourlyRate { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Salary Rate")]
        public decimal SalaryRate { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "PerDiem Rate")]
        public decimal PerDiemRate { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Daily Rate")]
        public decimal DailyRate { get; set; }

        public decimal TotalHours { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal PerDiemDays { get; set; }

        public decimal CabinDays { get; set; }

        public decimal DailyRateDays { get; set; }
    }
}