using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Models.Views.PR.Import
{
    public class BenefitsCSVViewModel
    {

        [Name("EE Code")]
        public string EECode { get; set; }
        [Name("EE Name")]
        public string EEName { get; set; }
        [Name("Benefits Eligibility Profile")]
        public string BenefitsEligibilityProfile { get; set; }
        [Name("Home Allocation")]
        public string HomeAllocation { get; set; }
        [Name("Plan Year(s)")]
        public string PlanYear { get; set; }
        [Name("Plan Name")]
        public string PlanName { get; set; }
        [Name("Coverage Level")]
        public string CoverageLevel { get; set; }
        [Name("Benefit Status")]
        public string BenefitStatus { get; set; }
        [Name("Coverage Start Date")]
        public Nullable<System.DateTime> CoverageStartDate { get; set; }
        [Name("Coverage End Date")]
        public Nullable<System.DateTime> CoverageEndDate { get; set; }
        [Name("Monthly Premium")]
        public string MonthlyPremium { get; set; }
        [Name("Employer Cost Per Pay Period")]
        public string EmployerCostPerPayPeriod { get; set; }
        [Name("Employee Cost Per Pay Period")]
        public string EmployeeCostPerPayPeriod { get; set; }
        [Name("Deduction Code")]
        public string DeductionCode { get; set; }
        [Name("Tax Treatment")]
        public string TaxTreatment { get; set; }
        [Name("Current Payroll Deduction")]
        public string CurrentPayrollDeduction { get; set; }
    }
}