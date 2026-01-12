using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.CreditCard.Batch.Form;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace portal.Models.Views.AP.CreditCard.Batch.Lines
{
    public class FormViewModel
    {
        public FormViewModel()
        {

        }

        public FormViewModel(byte ccco, DateTime mth, int batchId, string source, VPContext db)
        {
            if (db == null) throw new System.ArgumentNullException(nameof(db));
            var batch = db.vCMBatches.FirstOrDefault(f => f.CCCo == ccco && f.Mth == mth && f.BatchId == batchId && f.Source == source);
            CCCo = ccco;
            Mth = mth.Date.ToShortDateString();
            BatchId = batchId;
            Source = source;

            Lines = new BatchTransactionListViewModel(ccco, mth, batchId, source, db);
            Action = new ActionViewModel(batch);
            BatchInfo = new BatchViewModel(batch);
        }

        [Key]
        public byte CCCo { get; set; }

        [Key]
        [UIHint("DropDownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/GLAccount/APAllMthCombo", ComboForeignKeys = "GLCo=CCCo")]
        [Display(Name = "Statement Month")]
        public string Mth { get; set; }

        public int BatchId { get; set; }

        public string Source { get; set; }

        public ActionViewModel Action { get; set; }

        public BatchViewModel BatchInfo { get; set; }

        public BatchTransactionListViewModel Lines { get; set; }

    }
}