using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.HQ.Batch.PR
{
    public class PRFormViewModel
    {
        public PRFormViewModel()
        {

        }

        public PRFormViewModel(DB.Infrastructure.ViewPointDB.Data.Batch batch, Controller controller)
        {
            if (batch == null)
                return;

            Co = batch.Co;
            Mth = batch.Mth;
            BatchId = batch.BatchId;

            if (batch.DatePosted != null)
            {
                PostedTransactions = new PRPostedTransListViewModel(batch);
            }
            else
            {
                BatchTransactions = new PRBatchTransListViewModel(batch);
            }

            Buttons = new ButtonListViewModel(batch, controller);
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

        public PRBatchTransListViewModel BatchTransactions { get; set; }

        public PRPostedTransListViewModel PostedTransactions { get; set; }

        public ButtonListViewModel Buttons { get; set; }
    }
}