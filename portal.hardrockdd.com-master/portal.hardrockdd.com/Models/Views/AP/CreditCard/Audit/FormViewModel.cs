using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.CreditCard.Administration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.AP.CreditCard.Audit
{
    public class FormViewModel
    {
        public FormViewModel()
        {

        }

        public FormViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company, DateTime mth)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));

            GLCo = (byte)company.GLCo;
            CCCo = (byte)company.PRCo;
            Mth = mth.Date.ToShortDateString();

            Transactions = new AuditListViewModel(company, mth);
        }

        [Key]
        public byte CCCo { get; set; }

        [Key]
        [UIHint("DropDownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/GLAccount/APAllMthCombo", ComboForeignKeys = "GLCo")]
        [Display(Name = "Statement Month")]
        public string Mth { get; set; }

        public byte GLCo { get; set; }

        public AuditListViewModel Transactions { get; set; }
        
    }
}