using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace portal.Areas.HumanResource.Models.PositionRequest
{
    public class PayroleViewModel
    {
        public PayroleViewModel()
        {

        }

        public PayroleViewModel(HRPositionRequest request)
        {
            if (request == null)
                return;

            HRCo = request.HRCo;
            RequestId = request.RequestId;
            PositionCodeId = request.PositionCodeId;
            WAApplicantId = request.WAApplicantId;
            WAApplicationId = request.WAApplicationId;
            PRCo = request.PRCo ?? request.HRCo;

            if (request.WAApplication == null)
                return;

            PRDeptId = request.WAApplication.PRDeptId;
            PRGroupId = request.WAApplication.PRGroupId;
            PRInsCodeId = request.WAApplication.PRInsCodeId;
            PRHrlyRate = request.WAApplication.PRHrlyRate;
            PRSalaryAmt = request.WAApplication.PRSalaryAmt;
            PRPerDeimRate = request.WAApplication.PRPerDeimRate;
            PREarnCodeId = request.WAApplication.PREarnCodeId;
            PRReportsToId = request.WAApplication.PRReportsToId;
            WPDivisionId = request.WAApplication.WPDivisionId;
            HRDotStatus = request.WAApplication.HRDotStatus;

            var drugTest = request.WAApplication.DrugTests.FirstOrDefault();
            if (drugTest != null)
            {
                HRDrugTestDate = drugTest.TestDate;
                HRDrugTestType = drugTest.TestType;
                HRDrugTestStatus = drugTest.TestStatusId;
            }
        }

        [Key]
        public byte HRCo { get; set; }
        [Key]
        public int RequestId { get; set; }

        [UIHint("DropDownbox")]
        [Display(Name = "Position")]
        [Field(ComboUrl = "/HRCombo/PositionCodeCombo", ComboForeignKeys = "HRCo")]
        public string PositionCodeId { get; set; }

        [UIHint("DropDownbox")]
        [Display(Name = "Applicant")]
        [Field(ComboUrl = "/WACombo/WAApprovedAppicantCombo", ComboForeignKeys = "PositionCodeId")]
        public int? WAApplicantId { get; set; }

        public int? WAApplicationId { get; set; }

        public byte PRCo { get; set; }

        [Required]
        [UIHint("DropDownbox")]
        [Display(Name = "Department")]//
        [Field(ComboUrl = "/PRCombo/PRDepartmentCode", ComboForeignKeys = "PRCo")]
        public string PRDeptId { get; set; }

        public byte? PRGroupId { get; set; }

        [Required]
        [UIHint("DropDownbox")]
        [Display(Name = "Ins. Code")]
        [Field(ComboUrl = "/PRCombo/PRInsuranceCode", ComboForeignKeys = "PRCo")]
        public string PRInsCodeId { get; set; }


        [UIHint("CurrencyBox")]
        [Display(Name = "Hourly Rate")]
        public decimal? PRHrlyRate { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Salary Amt")]
        public decimal? PRSalaryAmt { get; set; }


        [UIHint("CurrencyBox")]
        [Display(Name = "PerDeim Amt")]
        public decimal? PRPerDeimRate { get; set; }


        [Required]
        [UIHint("DropDownbox")]
        [Display(Name = "Earn Code")]
        [Field(ComboUrl = "/PRCombo/DefaultEarnCodeCombo", ComboForeignKeys = "PRCo")]
        public short? PREarnCodeId { get; set; }

        [Required]
        [UIHint("DropDownbox")]
        [Display(Name = "Supervisor")]
        [Field(ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo")]
        public int? PRReportsToId { get; set; }

        [Required]
        [UIHint("DropDownbox")]
        [Display(Name = "Division")]
        [Field(ComboUrl = "/WPCombo/WPDivisionCombo")]
        public int? WPDivisionId { get; set; }

        [Required]
        [UIHint("DateBox")]
        [Display(Name = "Drug Test Date")]
        public DateTime? HRDrugTestDate { get; set; }

        [Required]
        [UIHint("DropDownbox")]
        [Display(Name = "Testing Status")]
        [Field(ComboUrl = "/HRCombo/HRTestCodesTypeCombo", ComboForeignKeys = "HRCo,type='U'")]
        public string HRDrugTestStatus { get; set; }

        [Required]
        [UIHint("DropDownbox")]
        [Display(Name = "Test Type")]
        [Field(ComboUrl = "/HRCombo/HRTestCodesTypeCombo", ComboForeignKeys = "HRCo,type='D'")]
        public string HRDrugTestType { get; set; }


        [UIHint("DropDownbox")]
        [Display(Name = "Dot Status")]
        [Field(ComboUrl = "/HRCombo/DotStatusCodesCombo")]
        public string HRDotStatus { get; set; }


        internal PayroleViewModel ProcessUpdate(VPContext db, System.Web.Mvc.ModelStateDictionary modelState)
        {
            var updObj = db.HRPositionRequests.FirstOrDefault(f => f.HRCo == this.HRCo && f.RequestId == this.RequestId);

            if (updObj != null)
            {
                updObj.HRCo = this.HRCo;
                updObj.RequestId = this.RequestId;
                updObj.PositionCodeId = this.PositionCodeId;
                if (updObj.WAApplicantId != this.WAApplicantId)
                {
                    updObj.WAApplicantId = this.WAApplicantId;
                }
                else
                {
                    updObj.PRCo = this.PRCo;
                    if (updObj.WAApplication != null)
                    {
                        updObj.WAApplication.PRDeptId = this.PRDeptId;
                        updObj.WAApplication.PRGroupId = this.PRGroupId;
                        updObj.WAApplication.PRInsCodeId = this.PRInsCodeId;
                        updObj.WAApplication.PRHrlyRate = this.PRHrlyRate;
                        updObj.WAApplication.PRSalaryAmt = this.PRSalaryAmt;
                        updObj.WAApplication.PRPerDeimRate = this.PRPerDeimRate;
                        updObj.WAApplication.PREarnCodeId = this.PREarnCodeId;
                        updObj.WAApplication.PRReportsToId = this.PRReportsToId;
                        updObj.WAApplication.WPDivisionId = this.WPDivisionId;
                        updObj.WAApplication.HRDotStatus = this.HRDotStatus;
                        if (this.HRDrugTestDate != null)
                        {
                            var test = updObj.WAApplication.AddDrugTest((DateTime)this.HRDrugTestDate);
                            test.TestStatusId = this.HRDrugTestStatus;
                            test.TestType = this.HRDrugTestType;
                        }
                    }
                }
                try
                {
                    db.BulkSaveChanges();
                    return new PayroleViewModel(updObj);
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