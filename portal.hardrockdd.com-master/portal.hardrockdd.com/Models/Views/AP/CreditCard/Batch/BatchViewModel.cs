using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.AP.CreditCard.Batch
{
    public class BatchListViewModel
    {
        public BatchListViewModel()
        {

        }

        public BatchListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company, DateTime mth, VPContext db)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            Co = company.HQCo;
            Mth = mth;
            List = db.vCMBatches.Where(f => f.CCCo == company.HQCo && f.Mth == mth).ToList().Select(s => new BatchViewModel(s)).OrderByDescending(o=>o.BatchDate).ToList();
        }

        [Key]

        public byte Co { get; set; }

        [Key]
        public DateTime Mth { get; set; }

        public List<BatchViewModel> List { get; }
    }

    public class BatchViewModel
    {
        public BatchViewModel()
        {

        }
        public BatchViewModel(vCMBatch batch)
        {
            if (batch == null) throw new System.ArgumentNullException(nameof(batch));

            Co = batch.CCCo;
            Mth = batch.Mth;
            BatchId = batch.BatchId;
            Source = batch.Source;
            BatchDate = batch.BatchDate;
            BatchDateStr = batch.BatchDate?.ToShortDateString();
            Status = batch.Status;
            Description = batch.Description;
            TransCnt = batch.TransCnt;
            PostAmount = batch.PostAmount;
        }

        [Key]
        public byte Co { get; set; }

        [Key]
        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        public DateTime Mth { get; set; }

        [Key]
        [UIHint("IntegerBox")]
        [Display(Name = "Batch Id")]
        public int BatchId { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Source")]
        public string Source { get; set; }

        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Batch Date")]
        public DateTime? BatchDate { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Batch Date")]
        public string BatchDateStr { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Status")]
        public string Status { get; set; }

        [UIHint("IntegerBox")]
        [Display(Name = "Trans Cnt")]
        public int? TransCnt { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Post Amount")]
        public decimal? PostAmount { get; set; }

    }

}