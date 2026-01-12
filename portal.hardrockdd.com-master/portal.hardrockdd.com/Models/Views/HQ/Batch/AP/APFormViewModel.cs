using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.HQ.Batch.AP
{
    public class APFormViewModel
    {
        public APFormViewModel()
        {

        }

        public APFormViewModel(DB.Infrastructure.ViewPointDB.Data.Batch batch, Controller controller, VPContext db)
        {
            if (batch == null)
                return;

            Co = batch.Co;
            Mth = batch.Mth;
            BatchId = batch.BatchId;

            if (batch.DatePosted != null)
            {
                PostedTransactions = new APPostedTransListViewModel(batch);
            }
            else
            {
                BatchTransactions = new APBatchTransListViewModel(batch);
            }

            Buttons = new ButtonListViewModel(batch, controller);
            Actions = new ActionListViewModel(batch, controller, db);
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

        public APBatchTransListViewModel BatchTransactions { get; set; }

        public APPostedTransListViewModel PostedTransactions { get; set; }

        public ActionListViewModel Actions { get; set; }


        public ButtonListViewModel Buttons { get; set; }
    }
}