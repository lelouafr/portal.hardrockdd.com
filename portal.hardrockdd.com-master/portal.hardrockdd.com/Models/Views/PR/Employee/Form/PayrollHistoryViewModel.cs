using portal.Code.Data.VP;
using portal.Models.Views.HR.Resource.Form;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.PR.Employee.Form
{
    public class PayrollHistoryViewModel
    {
        public PayrollHistoryViewModel()
        {

        }

        public PayrollHistoryViewModel(HRResource resource)
        {
            if (resource == null) throw new System.ArgumentNullException(nameof(resource));
            var employee = resource.PREmployee;
            if (employee == null)
                return;

            PRCo = employee.PRCo;
            EmployeeId = employee.EmployeeId;


            HireDate = employee.HireDate;
            RecentRehireDate = employee.RecentRehireDate;
            TermDate = employee.TermDate;
            RecentSeparationDate = employee.RecentSeparationDate;

            EmploymentHistory = new ResourceEmploymentHistoryListViewModel(resource);
        }

        public PayrollHistoryViewModel(Code.Data.VP.Employee employee)
        {
            if (employee == null) throw new System.ArgumentNullException(nameof(employee));
            var resource = employee.Resource.FirstOrDefault();

            PRCo = employee.PRCo;
            EmployeeId = employee.EmployeeId;

            HireDate = employee.HireDate;
            RecentRehireDate = employee.RecentRehireDate;
            TermDate = employee.TermDate;
            RecentSeparationDate = employee.RecentSeparationDate;

            EmploymentHistory = new ResourceEmploymentHistoryListViewModel(resource);
        }

        [Key]
        [HiddenInput]
        public byte PRCo { get; set; }

        [Key]
        [HiddenInput]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int EmployeeId { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Hire Date")]
        [Field(LabelSize = 7, TextSize = 5)]
        public DateTime? HireDate { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Recent Rehire Date")]
        [Field(LabelSize = 7, TextSize = 5)]
        public DateTime? RecentRehireDate { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Term Date")]
        [Field(LabelSize = 7, TextSize = 5)]
        public DateTime? TermDate { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Recent Separation Date")]
        [Field(LabelSize = 7, TextSize = 5)]
        public DateTime? RecentSeparationDate { get; set; }

        public ResourceEmploymentHistoryListViewModel EmploymentHistory { get; set; }

        internal PayrollHistoryViewModel ProcessUpdate(ModelStateDictionary modelState, VPEntities db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            var updObj = db.Employees.Where(f => f.PRCo == this.PRCo && f.EmployeeId == this.EmployeeId).FirstOrDefault();

            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.HireDate = this.HireDate;
                updObj.RecentRehireDate = this.RecentRehireDate;
                updObj.TermDate = this.TermDate;
                updObj.RecentSeparationDate = this.RecentSeparationDate;
                try
                {
                    db.SaveChanges(modelState);
                    return new PayrollHistoryViewModel(updObj);
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