using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.AP.CreditCard.Employee
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
            EmployeeId = employee.EmployeeId;
            Mth = mth.Date.ToShortDateString();
            GLCo = (byte)employee.PRCompanyParm.GLCo;
            Transactions = new TransactionListViewModel(employee, mth);
            Images = new ImageBankListViewModel(employee, mth);
        }

        [Key]
        public byte PRCo { get; set; }

        [Key]
        public int EmployeeId { get; set; }

        [Key]
        [UIHint("DropDownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/GLAccount/APAllMthCombo", ComboForeignKeys = "GLCo=PRCo")]
        [Display(Name = "Statement Month")]
        public string Mth { get; set; }

        public byte GLCo { get; set; }

        public TransactionListViewModel Transactions { get; set; }
        
        public ImageBankListViewModel Images { get; set; }
    }
}