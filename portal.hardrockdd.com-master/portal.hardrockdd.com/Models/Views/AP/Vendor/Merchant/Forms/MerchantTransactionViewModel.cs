using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.CreditCard.Employee;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.AP.Vendor.Merchant.Forms
{
    public class MerchantTransactionViewModel
    {
        public MerchantTransactionViewModel()
        {

        }

        public MerchantTransactionViewModel(DB.Infrastructure.ViewPointDB.Data.CreditMerchant merchant, DateTime mth, VPContext db)
        {
            if (merchant == null) throw new System.ArgumentNullException(nameof(merchant));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            VendGroupId = merchant.VendGroupId;
            Mth = mth;
            List = merchant.Transactions.Select(s => new TransactionViewModel(s)).ToList();
            //List = company.PayrollEmployees.SelectMany(s => s.CreditCardTransactions.Where(f => f.Mth == mth)).Select(s => new Employee.TransactionViewModel(s)).ToList();
            //foreach (var emp in company.PayrollEmployees)
            //{
            //    var trans = emp.CreditCardTransactions.Where(w => w.Mth == mth).ToList();
            //    List.AddRange(trans.Select(s => new Employee.TransactionViewModel(s)).ToList());
            //}
        }

        [Key]

        public byte VendGroupId { get; set; }

        [Key]
        [UIHint("DropdownBox")]
        [Display(Name = "Supervisor")]
        [Field(ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo")]
        public int SupervisorId { get; set; }


        [Key]
        public DateTime Mth { get; set; }

        public List<TransactionViewModel> List { get; }

    }

}