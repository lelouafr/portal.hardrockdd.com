using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DB.Infrastructure.ViewPointDB.Data;

namespace portal.Models.Views.HQ.Batch
{
    public class ActionListViewModel
    {
        public ActionListViewModel()
        {

        }

        public ActionListViewModel(DB.Infrastructure.ViewPointDB.Data.Batch batch, Controller controller, VPContext db)
        {
            if (batch == null)
                return;
            if (controller == null)
                return;

            Co = batch.Co;
            Mth = batch.Mth;
            BatchId = batch.BatchId;

            if (!batch.Actions.Any())
            {
                Repository.VP.HQ.BatchRepository.AddDefaultBatchActions(batch, controller, db);
                db.SaveChanges(controller.ModelState);
            }

            List = batch.Actions.OrderBy(o => o.SeqId).Select(s => new ActionViewModel(s)).ToList();
        }


        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Company")]
        public byte? Co { get; set; }

        [Key]
        [Required]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/GLAccount/APAllMthCombo", ComboForeignKeys = "GLCo=Co")]
        [Display(Name = "Batch Month")]
        public DateTime? Mth { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Batch Id")]
        public int? BatchId { get; set; }

        public List<ActionViewModel> List { get; }
    }

    public class ActionViewModel
    {
        public ActionViewModel(DB.Infrastructure.ViewPointDB.Data.BatchAction batch)
        {
            Co = batch.Co;
            Mth = batch.Mth;
            BatchId = batch.BatchId;
            SeqId = batch.SeqId;
            ActionUrl = batch.ActionUrl;
            ActionType = batch.ActionType;
            Title = batch.Title;
            IsActive = batch.IsActive;
            StatusId = (DB.BatchStatusEnum)(batch.Status ?? (int)DB.BatchStatusEnum.Open);

            SubBatchId = batch.SubBatchId ?? batch.BatchId;
            SubBatchMth = batch.SubBatchMth ?? batch.Mth;

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

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Batch Id")]
        public int SeqId { get; set; }

        public int ActionId { get; set; }

        public string ActionType { get; set; }

        public string Title { get; set; }

        public string ActionUrl { get; set; }

        public bool IsActive { get; set; }

        public DB.BatchStatusEnum StatusId { get; set; }


        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/GLAccount/APAllMthCombo", ComboForeignKeys = "GLCo=Co")]
        [Display(Name = "Batch Month")]
        public DateTime? SubBatchMth { get; set; }

        [HiddenInput]
        [Display(Name = "Batch Id")]
        public int? SubBatchId { get; set; }
    }
}