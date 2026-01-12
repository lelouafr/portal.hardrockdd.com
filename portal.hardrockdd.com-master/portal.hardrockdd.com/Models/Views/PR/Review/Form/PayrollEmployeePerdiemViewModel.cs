using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.Payroll.Review
{
    public class PayrollEmployeePerdiemListViewModel
    {
        public PayrollEmployeePerdiemListViewModel()
        {

        }

        public PayrollEmployeePerdiemListViewModel(DB.Infrastructure.ViewPointDB.Data.Employee employee, int weekId, VPContext db)
        {
            if (employee == null) throw new System.ArgumentNullException(nameof(employee));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            var subList = db.DTPayrollPerdiems.Where(f => f.Calendar.Week == weekId && f.EmployeeId == employee.EmployeeId).ToList();

            List = subList.GroupBy(g => new { g.PRCo, g.EmployeeId, g.WorkDate })
                                           .Select(s => new PayrollEmployeePerdiemViewModel {
                                                PRCo = s.Key.PRCo,
                                                EmployeeId = s.Key.EmployeeId,
                                                WorkDate = s.Key.WorkDate,
                                                PerdiemId = (DB.PerdiemEnum)s.Max(max => max.PerdiemId)
                                           })
                                           .ToList();
            Calendar = db.Calendars.Where(f => f.Week == weekId).ToList()
                                   .Select(s => new PayrollCalendarViewModel(s)).ToList();
        }

        public List<PayrollEmployeePerdiemViewModel> List { get; }

        public List<PayrollCalendarViewModel> Calendar { get; }
    }

    public class PayrollEmployeePerdiemViewModel
    {
        public PayrollEmployeePerdiemViewModel()
        {

        }
       
        [Key]
        [HiddenInput]
        [Required]
        public byte PRCo { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        public int EmployeeId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        public DateTime WorkDate { get; set; }

        [Display(Name = "Perdiem")]
        [UIHint("EnumBox")]
        [Field(LabelSize = 0, TextSize = 12, FormGroup = "Perdiem", FormGroupRow = 1)]
        public DB.PerdiemEnum PerdiemId { get; set; }
    }
}