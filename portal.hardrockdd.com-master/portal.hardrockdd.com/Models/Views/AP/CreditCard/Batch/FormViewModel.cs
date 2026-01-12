using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.ComponentModel.DataAnnotations;

namespace portal.Models.Views.AP.CreditCard.Batch
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

            Co = (byte)company.PRCo;
            Mth = mth.Date.ToShortDateString();

            Batches = new BatchListViewModel(company, mth, db);
        }

        [Key]
        public byte Co { get; set; }

        [Key]
        [UIHint("DropDownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/GLAccount/APAllMthCombo", ComboForeignKeys = "GLCo=Co")]
        [Display(Name = "Statement Month")]
        public string Mth { get; set; }


        public BatchListViewModel Batches { get; set; }

    }
}