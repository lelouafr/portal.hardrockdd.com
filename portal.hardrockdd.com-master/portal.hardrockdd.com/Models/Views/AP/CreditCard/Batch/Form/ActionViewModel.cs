using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AP.CreditCard.Batch.Form
{
    public class ActionViewModel
    {
        public ActionViewModel()
        {


        }

        public ActionViewModel(vCMBatch entity)
        {
            if (entity == null)
            {
                throw new System.ArgumentNullException(nameof(entity));
            }
            CCCo = entity.CCCo;
            Mth = entity.Mth;
            BatchId = entity.BatchId;
            Source = entity.Source;
            CanAddtoBatch = false;

            if (BatchId == 0)
            {
                CanAddtoBatch = true;
            }
        }

        [Key]
        public byte CCCo { get; set; }
        [Key]
        public DateTime Mth { get; set; }
        [Key]
        public int BatchId { get; set; }
        [Key]
        public string Source { get; set; }

        public bool CanAddtoBatch { get; set; }
    }


}