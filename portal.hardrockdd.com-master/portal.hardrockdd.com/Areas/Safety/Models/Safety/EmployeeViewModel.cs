using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DB.Infrastructure.ViewPointDB.Data;
using System.Linq;
using System.Web;

namespace portal.Areas.Safety.Models.Safety
{

    public class EmployeeListViewModel
    {
        public EmployeeListViewModel()
        {
            List = new List<EmployeeViewModel>();
        }

        public EmployeeListViewModel(VPContext db)
        {
            List = new List<EmployeeViewModel>();
            if (db == null)
                return;

            List = db.HRResources.Where(f => f.ActiveYN == "Y").ToList().Select(s => new EmployeeViewModel(s)).ToList();
        }

        public List<EmployeeViewModel> List { get; }
    }
    public class EmployeeViewModel
    {
        public EmployeeViewModel()
        {

        }

        public EmployeeViewModel (DB.Infrastructure.ViewPointDB.Data.HRResource employee)
        {
            if (employee == null)
                return;

            HRCo = employee.HRCo;
            HRRef = employee.HRRef;
            Name = employee.FullName();
            Position = employee.Position.Description;
            Crew = employee.PREmployee.Crew?.Description;
            Supervisor = employee.Supervisor?.FullName();

        }


        [Key]
        public byte HRCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int HRRef { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Position")]
        public string Position { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Crew")]
        public string Crew { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Supervisor")]
        public string Supervisor { get; set; }
    }
}