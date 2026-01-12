using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Employee;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.Payroll.Review
{
    public class PayrollEmployeeReviewFormViewModel
    {
        public PayrollEmployeeReviewFormViewModel()
        {

        }

        public PayrollEmployeeReviewFormViewModel(DB.Infrastructure.ViewPointDB.Data.Employee employee, int weekId, VPContext db)
        {
            if (employee == null) throw new System.ArgumentNullException(nameof(employee));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            WeekId = weekId;
            PRCo = employee.PRCo;
            EmployeeId = employee.EmployeeId;

            Employee = new EmployeeViewModel(employee);
            Tickets = new PayrollEmployeeTicketListViewModel(employee, weekId, db);
            Perdiems = new PayrollEmployeePerdiemListViewModel(employee, weekId, db);
            Hours = new PayrollEmployeeHourListViewModel(employee, weekId, db);
        }

        [Key]
        [HiddenInput]
        public byte PRCo { get; set; }

        [Key]
        [HiddenInput]
        public int WeekId { get; set; }

        [Key]
        [HiddenInput]
        public int EmployeeId { get; set; }

        public PayrollEmployeeTicketListViewModel Tickets { get; set; }

        public PayrollEmployeePerdiemListViewModel Perdiems { get; set; }
        
        public PayrollEmployeeHourListViewModel Hours { get; set; }

        public EmployeeViewModel Employee { get; set; }
    }
}