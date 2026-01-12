using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Purchase.Order
{
    public class PurchaseOrderListViewModel
    {
        public PurchaseOrderListViewModel(DB.Infrastructure.ViewPointDB.Data.APVendor vendor)
        {
            if (vendor == null)
            {
                throw new System.ArgumentNullException(nameof(vendor));
            }
            VendorGroupId = vendor.VendorGroupId;
            var fldDate = DateTime.Now.AddYears(-1);
            List = vendor.PurchaseOrders.OrderByDescending(o => o.AddedMth).ThenByDescending(o => o.OrderDate).Where(f => f.AddedMth >= fldDate).Select(s => new PurchaseOrderViewModel(s)).ToList();
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Co")]
        public byte VendorGroupId { get; set; }

        [Key]
        [Required]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/APCombo/VendorCombo", ComboForeignKeys = "vendGroupId=VendorGroupId", SearchUrl = "/APCombo/VendorSearch", SearchForeignKeys = "vendGroupId=VendorGroupId")]
        [Display(Name = "Vendor")]
        public int? VendorId { get; set; }


        public List<PurchaseOrderViewModel> List { get; }
    }

    public class PurchaseOrderViewModel
    {
        public PurchaseOrderViewModel()
        {

        }

        public PurchaseOrderViewModel(DB.Infrastructure.ViewPointDB.Data.PurchaseOrder entity)
        {
            if (entity == null)
            {
                throw new System.ArgumentNullException(nameof(entity));
            }
            POCo = entity.POCo;
            PO = entity.PO;
            OrderDate = entity.OrderDate;
            Status = (DB.POStatusEnum)entity.Status;
            Description = entity.Description;

            VendGroupId = entity.VendorGroupId;// ?? entity.POCompanyParm.HQCompanyParm.VendorGroupId;
            VendorId = entity.VendorId;
            JobId = entity.JobId;
            OrderDate = entity.OrderDate;
            OrderedBy = entity.OrderedBy;
        }
        
        public bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new System.ArgumentNullException(nameof(modelState));
            }
            var ok = modelState.IsValid;
            
            return ok;
        }


        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Co")]
        public byte POCo { get; set; }

        [Key]
        [Required]
        [ReadOnly(true)]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "General", FormGroupRow = 1, Placeholder = "")]
        [UIHint("TextBox")]
        [Display(Name = "PO")]
        public string PO { get; set; }

        [Required]
        [HiddenInput]
        [Display(Name = "Status")]
        public DB.POStatusEnum Status { get; set; }

        [UIHint("DateBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "General", FormGroupRow = 1, Placeholder = "")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Order Date")]
        public DateTime? OrderDate { get; set; }

        [Required]
        [UIHint("TextAreaBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 2, Placeholder = "")]
        [Display(Name = "Description")]
        public string Description { get; set; }


        [UIHint("TextAreaBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 2, Placeholder = "")]
        [Display(Name = "Ordered By")]
        public string OrderedBy { get; set; }


        [UIHint("TextAreaBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 2, Placeholder = "")]
        [Display(Name = "Ordered By")]
        public string OrderedByName { get; set; }

        public byte? VendGroupId { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 3, ComboUrl = "/APCombo/VendorCombo", ComboForeignKeys = "VendGroupId", SearchUrl = "/APCombo/VendorSearch", SearchForeignKeys = "VendGroupId")]
        [Display(Name = "Vendor")]
        public int? VendorId { get; set; }

        [HiddenInput]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 6, ComboUrl = "/JCCombo/Combo", ComboForeignKeys = "JCCo")]
        [Display(Name = "Job")]
        public string JobId { get; set; }

    }
}