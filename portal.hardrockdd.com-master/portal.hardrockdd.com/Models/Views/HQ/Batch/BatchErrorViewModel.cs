using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.HQ.Batch
{
    public class BatchErrorListViewModel
    {
        public BatchErrorListViewModel()
        {

        }

        public BatchErrorListViewModel(DB.Infrastructure.ViewPointDB.Data.Batch batch)
        {
            if (batch == null)
                return;


            Co = batch.Co;
            Mth = batch.Mth;
            BatchId = batch.BatchId;

            List = batch.BatchErrors.Select(s => new BatchErrorViewModel(s)).ToList();
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Company")]
        public byte Co { get; set; }

        [Key]
        [Required]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/GLAccount/APAllMthCombo", ComboForeignKeys = "GLCo=Co")]
        [Display(Name = "Batch Month")]
        public DateTime Mth { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Batch Id")]
        public int BatchId { get; set; }


        public List<BatchErrorViewModel> List { get; set; }
    }
    public class BatchErrorViewModel
    {
        public BatchErrorViewModel()
        {

        }

        public BatchErrorViewModel(DB.Infrastructure.ViewPointDB.Data.BatchError error)
        {
            if (error == null)
                return;

            Co = error.Co;
            Mth = error.Mth;
            BatchId = error.BatchId;
            Seq = error.Seq;
            ErrorText = error.ErrorText;
            Severity = error.Severity;
        }

        [Key]
        [UIHint("LongBox")]
        public byte Co { get; set; }

        [Key]
        [UIHint("DateBox")]
        public System.DateTime Mth { get; set; }

        [Key]
        [UIHint("LongBox")]
        public int BatchId { get; set; }

        [Key]
        [UIHint("LongBox")]
        public int Seq { get; set; }

        [UIHint("TextBox")]
        public string ErrorText { get; set; }

        public byte Severity { get; set; }


    }
}