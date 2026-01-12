using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.AP.Payment
{
    public class PostedBatchListViewModel
    {
        public PostedBatchListViewModel()
        {

        }

        public PostedBatchListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company, DateTime mth)
        {
            APCo = company.HQCo;
            Mth = mth.ToShortDateString();
            List =  company.db.APPayments
                    .Where(f => f.PaidMth == mth && f.APCo == company.APCo)
                    .GroupBy(grp => new { grp.APCo, grp.PaidMth, grp.BatchId, grp.CMAcct, grp.HQBatch })
                    .ToList()
                    .Select(s => new PostedBatchViewModel
                    {
                        APCo = s.Key.APCo,
                        Mth = s.Key.PaidMth.ToShortDateString(),
                        BatchId = (int)s.Key.BatchId,
                        CMAcct = s.Key.CMAcct,
                        DatePosted = s.Key.HQBatch.DatePosted,
                        DateCreated = s.Key.HQBatch.DateCreated,
                        TransCount = s.Count()
                    }).ToList();
        }

        public List<PostedBatchViewModel> List { get; }

        [Key]
        public byte APCo { get; set; }

        [Key]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/GLAccount/APAllMthCombo", ComboForeignKeys = "GLCo=APCo")]
        [Display(Name = "Batch Month")]
        public string Mth { get; set; }
    }

    public class PostedBatchViewModel
    {
        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Co")]
        public byte APCo { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Mth")]
        public string Mth { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Batch Id")]
        public int BatchId { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "CM Acct")]
        public int CMAcct { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Date Posted")]
        public DateTime? DatePosted { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Date Created")]
        public DateTime? DateCreated { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Trans Count")]
        public int TransCount { get; set; }
    }
}