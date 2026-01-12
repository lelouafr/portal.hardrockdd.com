using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.HQ.Batch
{
    public class ButtonListViewModel
    {
        public ButtonListViewModel()
        {

        }

        public ButtonListViewModel(DB.Infrastructure.ViewPointDB.Data.Batch batch, Controller controller)
        {
            if (batch == null)
                return;
            if (controller == null)
                return;

            Co = batch.Co;
            Mth = batch.Mth;
            BatchId = batch.BatchId;

            List = new List<ButtonViewModel>();
            List.Add(new ButtonViewModel() { Title = "Process", ActionType = "Get", ActionUrl = controller.Url.Action("ProcessBatchModal", "Batch", new { Area = "",co = batch.Co, mth = batch.Mth.ToShortDateString(), batchId = batch.BatchId }), ButtonClass = "btn-primary", ActionRedirect = "Refresh" });
            List.Add(new ButtonViewModel() { Title = "Clear", ActionType = "Post", ActionUrl = controller.Url.Action("ClearBatch", "Batch", new { Area = "",co = batch.Co, mth = batch.Mth.ToShortDateString(), batchId = batch.BatchId }), ButtonClass = "btn-danger pull-right", ActionRedirect = "RefreshTable" });

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

        public List<ButtonViewModel> List { get; }
    }

    public class ButtonViewModel
    {
        public int ActionId { get; set; }
        public string ActionType { get; set; }

        public string Title { get; set; }

        public string ButtonClass { get; set; }

        public string ActionUrl { get; set; }

        public DB.BidStatusEnum GotoStatusId { get; set; }

        public string ActionRedirect { get; set; }
    }
}