using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.ComponentModel.DataAnnotations;

namespace portal.Models.Views.AP.CreditCard.Administration
{
    public class FormViewModel
    {
        public FormViewModel()
        {

        }

        public FormViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company, DateTime mth, VPContext db)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            CCCo = company.HQCo;
            GLCo = (byte)company.GLCo;
            Mth = mth.Date.ToShortDateString();

            Transactions = new TransactionListViewModel(company, mth, db);
        }

        public FormViewModel(DB.Infrastructure.ViewPointDB.Data.CompanyDivision company, DateTime mth, VPContext db)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));
            if (db == null) throw new System.ArgumentNullException(nameof(db));
            DivisionId = company.DivisionId;
            CCCo = company.HQCompany.HQCo;
            GLCo = (byte)company.HQCompany.GLCo;
            Mth = mth.Date.ToShortDateString();

            Transactions = new TransactionListViewModel(company, mth, db);
        }

        public byte GLCo { get; set; }


        [Key]
        public int DivisionId { get; set; }

        [Key]
        public byte CCCo { get; set; }

        [Key]
        [UIHint("DropDownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/GLAccount/APAllMthCombo", ComboForeignKeys = "GLCo")]
        [Display(Name = "Statement Month")]
        public string Mth { get; set; }


        public TransactionListViewModel Transactions { get; set; }
        
    }
}