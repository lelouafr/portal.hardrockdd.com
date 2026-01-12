using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.HQ.Batch
{
    public class BatchListViewModel
    {
        public BatchListViewModel()
        {

        }

        public BatchListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company, DateTime mth)
        {
            if (company == null)
                return;
            Co = company.HQCo;
            Mth = mth.ToShortDateString();
            Source = "";
            List = company.Batches.Where(f => f.Mth == mth).Select(s => new BatchViewModel(s)).ToList();
        }

        public BatchListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company, DateTime mth, string source)
        {
            if (company == null)
                return;
            Co = company.HQCo;
            Mth = mth.ToShortDateString();
            Source = source;
            List = company.Batches
                           .Where(f => f.Source.Trim() == source && f.Mth == mth)
                           .Select(s => new BatchViewModel(s))
                           .ToList();
        }

        [Key]
        public byte Co { get; set; }

        [Key]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/GLAccount/APAllMthCombo", ComboForeignKeys = "GLCo=Co")]
        [Display(Name = "Batch Month")]
        public string Mth { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "Source")]
        public string Source { get; set; }

        public List<BatchViewModel> List { get; set; }
    }

    public class BatchViewModel
    {
        public BatchViewModel()
        {

        }

        public BatchViewModel(DB.Infrastructure.ViewPointDB.Data.Batch batch)
        {
            if (batch == null)
                return;

            Co = batch.Co;
            Mth = batch.Mth;
            BatchId = batch.BatchId;
            Source = batch.Source;
            TableName = batch.TableName;

            InUseBy = batch.InUseBy?.Replace(@"HARDROCKDD\", "");
            DateCreated = batch.DateCreated;
            CreatedBy = batch.CreatedBy?.Replace(@"HARDROCKDD\", "");
            DatePosted = batch.DatePosted;
            PREndDate = batch.PREndDate;
            Status = batch.StatusEnum;


        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Co")]
        public byte Co { get; set; }

        [Key]
        [Required]
        [UIHint("DateBox")]
        [Display(Name = "Mth")]
        public DateTime Mth { get; set; }

        [Key]
        [Required]
        [UIHint("LongBox")]
        [Display(Name = "Batch Id")]
        public int BatchId { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Source")]
        public string Source { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Table Name")]
        public string TableName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "In Use By")]
        public string InUseBy { get; set; }

        [Required]
        [UIHint("DateBox")]
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.BatchStatusEnum Status { get; set; }

        public string StatusStr { get { return Status.ToString(); } }

        [UIHint("DateBox")]
        [Display(Name = "PR End Date")]
        public DateTime? PREndDate { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Date Posted")]
        public DateTime? DatePosted { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Date Closed")]
        public DateTime? DateClosed { get; set; }

    }
}