using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.AP.CreditCard.Supervisor
{
    public class FormViewModel
    {
        public FormViewModel()
        {

        }

        public FormViewModel(DB.Infrastructure.ViewPointDB.Data.Employee employee, DateTime mth)
        {
            if (employee == null) throw new System.ArgumentNullException(nameof(employee));

            PRCo = employee.PRCo;
            SupervisorId = employee.EmployeeId;
            Mth = mth.Date.ToShortDateString();

            Transactions = new TransactionListViewModel(employee, mth);
        }

        [Key]
        public byte PRCo { get; set; }

        [Key]
        public int SupervisorId { get; set; }

        [Key]
        [UIHint("DropDownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/GLAccount/APAllMthCombo", ComboForeignKeys = "GLCo=PRCo")]
        [Display(Name = "Statement Month")]
        public string Mth { get; set; }


        public TransactionListViewModel Transactions { get; set; }
        
    }
}