using System;
using System.ComponentModel.DataAnnotations;

namespace portal.Models.Views.AP.CreditCard.Batch.Form
{
    public class InfoViewModel
    {
        public InfoViewModel()
        {

        }

        public InfoViewModel(DB.Infrastructure.ViewPointDB.Data.Batch batch)
        {
            if (batch == null) throw new System.ArgumentNullException(nameof(batch));


            Co = batch.Co;
            Mth = batch.Mth;
            BatchId = batch.BatchId;

        }

        [Key]
        public byte Co { get; set; }
        [Key]
        public DateTime Mth { get; set; }
        [Key]
        public int BatchId { get; set; }
        
        [UIHint("DateBox")]
        public int BatchDate { get; set; }


    }
}