using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.HumanResource.Models.Resource.Form
{
    public class PersonalInfoViewModel
    {
        public PersonalInfoViewModel()
        {

        }

        public PersonalInfoViewModel(HRResource resource)
        {
            if (resource == null) throw new System.ArgumentNullException(nameof(resource));
            var employee = resource.PREmployee;
            if (employee == null)
                return;

            PRCo = employee.PRCo;
            EmployeeId = employee.EmployeeId;

            Email = employee.Email;
            Phone = employee.CellPhone;
            BirthDate = employee.BirthDate;

        }

        public PersonalInfoViewModel(Employee employee)
        {
            if (employee == null) throw new System.ArgumentNullException(nameof(employee));
            var resource = employee.Resource.FirstOrDefault();
            PRCo = employee.PRCo;
            EmployeeId = employee.EmployeeId;

            Email = employee.Email;
            Phone = employee.CellPhone;
            BirthDate = employee.BirthDate;
        }

        [Key]
        [HiddenInput]
        public byte PRCo { get; set; }

        [Key]
        [HiddenInput]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int EmployeeId { get; set; }

        [UIHint("EmailBox")]
        [Display(Name = "Email")]
        [Field(LabelSize = 6, TextSize = 6)]
        public string Email { get; set; }

        [UIHint("PhoneBox")]
        [Display(Name = "Phone")]
        [Field(LabelSize = 6, TextSize = 6)]
        public string Phone { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Birth Date")]
        [Field(LabelSize = 6, TextSize = 6)]
        public DateTime? BirthDate { get; set; }

        internal PersonalInfoViewModel ProcessUpdate(ModelStateDictionary modelState, VPContext db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            var updObj = db.Employees.Where(f => f.PRCo == this.PRCo && f.EmployeeId == this.EmployeeId).FirstOrDefault();

            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.Email = this.Email;
                updObj.Phone = this.Phone;

                updObj.BirthDate = this.BirthDate;

                try
                {
                    db.SaveChanges(modelState);
                    return new PersonalInfoViewModel(updObj);
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