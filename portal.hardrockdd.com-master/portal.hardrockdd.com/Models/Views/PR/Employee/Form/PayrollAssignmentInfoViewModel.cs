using portal.Code.Data.VP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.PR.Employee.Form
{
    public class PayrollAssignmentViewModel
    {
        public PayrollAssignmentViewModel()
        {

        }

        public PayrollAssignmentViewModel(HRResource resource)
        {
            if (resource == null) throw new System.ArgumentNullException(nameof(resource));
            var employee = resource.PREmployee;
            if (employee == null)
                return;

            PRCo = employee.PRCo;
            HRCo = resource.HRCo;
            EmployeeId = employee.EmployeeId;

            CrewId = employee.CrewId;
            SupervisorId = employee.ReportsToId;

            JCCo = employee.JCCo;
            JobId = employee.JobId;
            PositionId = resource.PositionCode;
            DivisionId = employee.DivisionId;
        }

        public PayrollAssignmentViewModel(Code.Data.VP.Employee employee)
        {
            if (employee == null) throw new System.ArgumentNullException(nameof(employee));
            var resource = employee.Resource.FirstOrDefault();
            PRCo = employee.PRCo;
            HRCo = resource.HRCo;
            EmployeeId = employee.EmployeeId;

            JCCo = employee.JCCo;
            CrewId = employee.CrewId;
            SupervisorId = employee.ReportsToId;
            JobId = employee.JobId;
            PositionId = resource.PositionCode;

            DivisionId = employee.DivisionId;
        }

        [Key]
        [HiddenInput]
        public byte PRCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int EmployeeId { get; set; }

        [HiddenInput]
        public byte? JCCo { get; set; }
        [HiddenInput]
        public byte? HRCo { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/HRCombo/PositionCodeCombo", ComboForeignKeys = "HRCo")]
        [Display(Name = "Position")]
        public string PositionId { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Crew")]
        [Field(ComboUrl = "/PRCombo/CrewCombo", ComboForeignKeys = "PRCo")]
        public string CrewId { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Supervisor")]
        [Field(ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo", InfoUrl = "/HRResourceForm/PopupForm", InfoForeignKeys = "HRCo=PRCo, EmployeeId=SupervisorId")]
        public int? SupervisorId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/JCCombo/Combo", ComboForeignKeys = "JCCo")]
        [Display(Name = "Job")]
        public string JobId { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Division")]
        [Field(ComboUrl = "/WPCombo/WPDivisionCombo")]
        public int? DivisionId { get; set; }

        internal PayrollAssignmentViewModel ProcessUpdate(ModelStateDictionary modelState,  VPEntities db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            var updObj = db.Employees.Where(f => f.PRCo == this.PRCo && f.EmployeeId == this.EmployeeId).FirstOrDefault();

            if (updObj != null)
            {
                var resource = updObj.Resource.FirstOrDefault();

                /****Write the changes to object****/
                updObj.CrewId = this.CrewId;
                updObj.ReportsToId = this.SupervisorId;
                updObj.JobId = this.JobId;
                updObj.DivisionId = this.DivisionId;
                resource.PositionCode = this.PositionId;

                try
                {
                    db.SaveChanges(modelState);
                    return new PayrollAssignmentViewModel(updObj);
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