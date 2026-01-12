using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.HQ.Batch.PO
{
    public class POBatchTransListViewModel
    {
        public POBatchTransListViewModel()
        {

        }

        public POBatchTransListViewModel(DB.Infrastructure.ViewPointDB.Data.Batch batch)
        {
            if (batch == null)
                return;

            Co = batch.Co;
            Mth = batch.Mth;
            BatchId = batch.BatchId;

            List = batch.POBatchs.Select(s => new POBatchTransViewModel(s)).ToList();
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Company")]
        public byte Co { get; set; }

        [Key]
        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/GLAccount/POAllMthCombo", ComboForeignKeys = "GLCo")]
        [Display(Name = "Batch Month")]
        public DateTime Mth { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Batch Id")]
        public int BatchId { get; set; }

        public List<POBatchTransViewModel> List { get; }
    }

    public class POBatchTransViewModel
    {
        public POBatchTransViewModel()
        {

        }

        public POBatchTransViewModel(DB.Infrastructure.ViewPointDB.Data.POBatchHeader transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            #region mapping
            Co = transaction.Co;
            Mth = transaction.Mth.ToShortDateString();
            BatchId = transaction.BatchId;
            BatchSeq = transaction.BatchSeq;
            BatchTransType = transaction.BatchTransType;

            PO = transaction.PO;
            VendorId = transaction.VendorId;

            if (transaction.Vendor != null)
            {
                Vendor = string.Format(AppCultureInfo.CInfo(), "{0} - {1}", transaction.VendorId, transaction.Vendor.Name.ToCamelCase());
                VendorDescription = transaction.Vendor.Name.ToCamelCase();
                VendorIdDisplay = transaction.Vendor.VendorId.ToString(AppCultureInfo.CInfo());
            }
            Description = transaction.Description;

            OrderDate = transaction.OrderDate;
            Status = (DB.POStatusEnum)transaction.Status;
            Description = transaction.Description;
            VendorId = transaction.VendorId;
            OrderDate = transaction.OrderDate;
            OrderedBy = transaction.OrderedBy;

            AllMth = transaction.Mth.ToShortDateString();
            Mth = transaction.Mth.ToShortDateString();
            MthDate = transaction.Mth;
            #endregion

        }

        public bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new System.ArgumentNullException(nameof(modelState));
            }
            var ok = true;

            //if (AwardStatus == DB.BidAwardStatusEnum.Awarded && CustomerId == null)
            //{
            //    modelState.AddModelError("CustomerId", "Customer Field is Required");
            //    ok &= false;
            //}
            return ok;
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Company")]
        public byte Co { get; set; }

        [Key]
        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/GLAccount/POAllMthCombo", ComboForeignKeys = "GLCo")]
        [Display(Name = "Batch Month")]
        public string Mth { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Batch Id")]
        public int BatchId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Seq Id")]
        public int BatchSeq { get; set; }

        [HiddenInput]
        [Display(Name = "Trans Id")]
        public int? POTransId { get; set; }

        [Required]
        [HiddenInput]
        [Display(Name = "Status")]
        public DB.POStatusEnum Status { get; set; }


        [Required]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/POCombo/Combo", ComboForeignKeys = "POCo", SearchUrl = "/POCombo/Search", SearchForeignKeys = "VendGroupId=POCo")]
        [Display(Name = "Vendor")]
        public int? VendorId { get; set; }

        [UIHint("TextBox")]
        [Field()]
        [Display(Name = "Vendor")]
        public string Vendor { get; set; }

        [UIHint("TextBox")]
        [Field()]
        [Display(Name = "Vendor")]
        public string VendorDescription { get; set; }

        [UIHint("TextBox")]
        [Field()]
        [Display(Name = "Vendor")]
        public string VendorIdDisplay { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/POCombo/VendorCombo", ComboForeignKeys = "POCo, VendorId", SearchUrl = "/POCombo/Search", SearchForeignKeys = "VendGroupId, VendorId")]
        [Display(Name = "PO")]
        public string PO { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Description")]
        public string Description { get; set; }



        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/GLAccount/POAllMthCombo", ComboForeignKeys = "GLCo")]
        [Display(Name = "Batch Month")]
        public string AllMth { get; set; }

        [Required]
        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Mth Date")]
        public DateTime MthDate { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Order Date")]
        public DateTime? OrderDate { get; set; }

        [UIHint("TextAreaBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Ordered By")]
        public string OrderedBy { get; set; }


        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/HQCombo/HQBatchTransType", ComboForeignKeys = "")]
        [Display(Name = "Trans Type")]
        public string BatchTransType { get; set; }
    }
}