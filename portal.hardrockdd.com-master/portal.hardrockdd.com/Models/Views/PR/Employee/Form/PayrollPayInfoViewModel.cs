using portal.Code.Data.VP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.PR.Employee.Form
{
    public class PayrollPayInfoViewModel
    {
        public PayrollPayInfoViewModel()
        {

        }

        public PayrollPayInfoViewModel(HRResource resource)
        {
            if (resource == null) throw new System.ArgumentNullException(nameof(resource));
            var employee = resource.PREmployee;
            if (employee == null)
                return;

            PRCo = employee.PRCo;
            EmployeeId = employee.EmployeeId;

            PaycomNumber = employee.PaycomNumber;
            EarnCodeId = employee.EarnCodeId;

            HrlyRate = employee.HrlyRate;
            SalaryAmt = employee.SalaryAmt;
            DailyRate = employee.udDailyRate;
            PerDiemRate = employee.PerDiemRate;

            SalaryHistory = new HR.Resource.Form.ResourceSalaryHistoryListViewModel(resource);
        }

        public PayrollPayInfoViewModel(Code.Data.VP.Employee employee)
        {
            if (employee == null) throw new System.ArgumentNullException(nameof(employee));
            var resource = employee.Resource.FirstOrDefault();
            PRCo = employee.PRCo;
            EmployeeId = employee.EmployeeId;

            PaycomNumber = employee.PaycomNumber;
            EarnCodeId = employee.EarnCodeId;

            HrlyRate = employee.HrlyRate;
            SalaryAmt = employee.SalaryAmt;
            DailyRate = employee.udDailyRate;
            PerDiemRate = employee.PerDiemRate;

            SalaryHistory = new HR.Resource.Form.ResourceSalaryHistoryListViewModel(resource);
        }

        [Key]
        [HiddenInput]
        public byte PRCo { get; set; }

        [Key]
        [HiddenInput]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int EmployeeId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Paycom Number")]
        [Field(LabelSize = 6, TextSize = 6)]
        public string PaycomNumber { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 6, TextSize = 6, ComboUrl = "/PRCombo/EarnCodeCombo", ComboForeignKeys = "PRCo")]
        [Display(Name = "Earn Code")]
        public short EarnCodeId { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Hourly Rate")]
        [Field(LabelSize = 6, TextSize = 6)]
        public decimal HrlyRate { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Salary")]
        [Field(LabelSize = 6, TextSize = 6)]
        public decimal SalaryAmt { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Daily Rate")]
        [Field(LabelSize = 6, TextSize = 6)]
        public decimal? DailyRate { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Per Diem Rate")]
        [Field(LabelSize = 6, TextSize = 6)]
        public decimal? PerDiemRate { get; set; }

        public HR.Resource.Form.ResourceSalaryHistoryListViewModel SalaryHistory { get; set; }



        internal PayrollPayInfoViewModel ProcessUpdate(ModelStateDictionary modelState, VPEntities db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            var updObj = db.Employees.Where(f => f.PRCo == this.PRCo && f.EmployeeId == this.EmployeeId).FirstOrDefault();

            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.PaycomNumber = this.PaycomNumber;
                updObj.EarnCodeId = this.EarnCodeId;

                updObj.HrlyRate = this.HrlyRate;
                updObj.SalaryAmt = this.SalaryAmt;
                updObj.udDailyRate = this.DailyRate;
                updObj.PerDiemRate = (byte)this.PerDiemRate;

                try
                {
                    db.SaveChanges(modelState);
                    return new PayrollPayInfoViewModel(updObj);
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