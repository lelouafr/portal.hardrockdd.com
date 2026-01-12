using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.AP.CreditCard.Supervisor
{
    public class TransactionListViewModel
    {
        public TransactionListViewModel()
        {

        }

        public TransactionListViewModel(DB.Infrastructure.ViewPointDB.Data.Employee employee, DateTime mth )
        {
            if (employee == null) throw new System.ArgumentNullException(nameof(employee));

            PRCo = employee.PRCo;
            SupervisorId = employee.EmployeeId;
            Mth = mth;

            List = employee.CreditCardTransactions.Where(w => w.Mth == mth && w.EmployeeId != employee.EmployeeId).Select(s => new Employee.TransactionViewModel(s)).ToList();
            foreach (var emp in employee.DirectReports)
            {
                List.AddRange(emp.CreditCardTransactions.Where(w => w.Mth == mth).Select(s => new Employee.TransactionViewModel(s)).ToList());
                foreach (var emp2 in emp.DirectReports)
                {
                    List.AddRange(emp2.CreditCardTransactions.Where(w => w.Mth == mth).Select(s => new Employee.TransactionViewModel(s)).ToList());
                }
            }

            List = List.OrderBy(o => o.EmployeeName).ThenBy(o => o.TransDate).ToList();
        }

        [Key]

        public byte PRCo { get; set; }

        [Key]
        [UIHint("DropdownBox")]
        [Display(Name = "Supervisor")]
        [Field(ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo")]
        public int SupervisorId { get; set; }


        [Key]
        public DateTime Mth { get; set; }

        public List<Employee.TransactionViewModel> List { get; }

    }

}