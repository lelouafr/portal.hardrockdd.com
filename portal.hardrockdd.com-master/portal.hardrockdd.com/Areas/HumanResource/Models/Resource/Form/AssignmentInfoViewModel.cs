using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.HumanResource.Models.Resource.Form
{
    public class AssignmentViewModel
    {
        public AssignmentViewModel()
        {

        }

        public AssignmentViewModel(HRResource resource)
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
            OfficeId = employee.OfficeId;
        }

        public AssignmentViewModel(Employee employee)
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

            OfficeId = employee.OfficeId;
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
        [Field(ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo", InfoUrl = "/HumanResource/Resource/PopupForm", InfoForeignKeys = "HRCo=PRCo, EmployeeId=SupervisorId")]
        public int? SupervisorId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/JCCombo/Combo", ComboForeignKeys = "JCCo")]
        [Display(Name = "Job")]
        public string JobId { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Division")]
        [Field(ComboUrl = "/WPCombo/WPDivisionCombo")]
        public int? DivisionId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/HQCombo/OfficeCombo", ComboForeignKeys = "")]
        [Display(Name = "Main Office")]
        public int? OfficeId { get; set; }

        internal AssignmentViewModel ProcessUpdate(ModelStateDictionary modelState,  VPContext db)
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
                updObj.DivisionId = this.DivisionId ?? 1;
                resource.PositionCode = this.PositionId;
                updObj.OfficeId = this.OfficeId;
                try
                {
                    db.SaveChanges(modelState);
                    return new AssignmentViewModel(updObj);
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