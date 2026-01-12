using portal.Code.Data.VP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.PR.Employee.Form
{
    public class PayrollTerminationViewModel
    {
        public PayrollTerminationViewModel()
        {

        }

        public PayrollTerminationViewModel(HRResource resource)
        {
            if (resource == null) throw new System.ArgumentNullException(nameof(resource));
            var employee = resource.PREmployee;
            if (employee == null)
                return;

            Co = employee.PRCo;
            EmployeeId = employee.EmployeeId;
            Date = DateTime.Now.Date;

            EmployeeName = employee.FullName();
            RequestedUser = StaticFunctions.GetLoggedInUser().FullName;
            RequestedUserEmail = StaticFunctions.GetLoggedInUser().Email;
        }

        public PayrollTerminationViewModel(Code.Data.VP.Employee employee)
        {
            if (employee == null) throw new System.ArgumentNullException(nameof(employee));
            //var resource = employee.Resource.FirstOrDefault();

            Co = employee.PRCo;
            EmployeeId = employee.EmployeeId;
            Date = DateTime.Now.Date;

            EmployeeName = employee.FullName();
            RequestedUser = StaticFunctions.GetLoggedInUser().FullName;
            RequestedUserEmail = StaticFunctions.GetLoggedInUser().Email;
        }

        [Key]
        [Required]
        [HiddenInput]
        public byte Co { get; set; }

        [Key]
        [Required]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int EmployeeId { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Employee")]
        public string EmployeeName { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        public string RequestedUser { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        public string RequestedUserEmail { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/HRCombo/SupervisorTermReasonCodeCombo", ComboForeignKeys = "HRCo")]
        [Display(Name = "Reason")]
        public string TermReason { get; set; }
        
        [Required]
        [UIHint("DateBox")]
        [Display(Name = "Date")]
        [Field(LabelSize = 2, TextSize = 4)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        
        [Required]
        [UIHint("TextAreaBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Notes")]
        public string Notes { get; set; }

        public string ReasonDescription()
        {
            using var db = new VPEntities();

            return db.HRCodes.FirstOrDefault(f => f.HRCo == Co && f.CodeId == TermReason)?.Description;
        }
    }


}