using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.AP.CreditCard.Administration
{
    public class TransactionListViewModel
    {
        public TransactionListViewModel()
        {

        }




        public TransactionListViewModel(DB.Infrastructure.ViewPointDB.Data.CompanyDivision company, DateTime mth, VPContext db)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            CCCo = (byte)company.PRCo;
            Mth = mth;
            DivisionId = company.DivisionId;

            var divisionList = company.SubDivisions.Select(s => s.DivisionId).ToList();
            divisionList.Add(company.DivisionId);

            List = db.CreditTransactions
                .Include("Employee")
                .Where(f => f.CCCo == company.PRCo && f.Mth == mth && divisionList.Contains(f.Employee.tDivisionId ?? 0))
                .ToList()
                .Select(s => new Employee.TransactionViewModel(s)).ToList();
        }


        public TransactionListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company, DateTime mth, VPContext db)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            CCCo = (byte)company.PRCo;
            Mth = mth;
            List = db.CreditTransactions.Where(f => f.CCCo == company.PRCo && f.Mth == mth).ToList().Select(s => new Employee.TransactionViewModel(s)).ToList();
            //List = company.PayrollEmployees.SelectMany(s => s.CreditCardTransactions.Where(f => f.Mth == mth)).Select(s => new Employee.TransactionViewModel(s)).ToList();
            //foreach (var emp in company.PayrollEmployees)
            //{
            //    var trans = emp.CreditCardTransactions.Where(w => w.Mth == mth).ToList();
            //    List.AddRange(trans.Select(s => new Employee.TransactionViewModel(s)).ToList());
            //}
        }

        [Key]

        public int DivisionId { get; set; }

        [Key]

        public byte CCCo { get; set; }

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