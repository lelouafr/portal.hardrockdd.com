using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Purchase.Request
{
    public class PORequestRejectViewModel
    {

        public PORequestRejectViewModel()
        {

        }
        public PORequestRejectViewModel(DB.Infrastructure.ViewPointDB.Data.PORequest entity) 
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            POCo = entity.POCo;
            RequestId = entity.RequestId;
            OrderedDate = entity.OrderedDate;
            PO = entity.PO;
            Status = (DB.PORequestStatusEnum)entity.Status;
            Description = entity.Description;
            VendorId = entity.VendorId;
            JobId = entity.JobId;
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "POCo")]
        public byte POCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Id")]
        public int RequestId { get; set; }

        [Required]
        [HiddenInput]
        [Display(Name = "Status")]
        public DB.PORequestStatusEnum Status { get; set; }

        [ReadOnly(true)]
        [UIHint("DateBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "General", FormGroupRow = 1, Placeholder = "")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Order Date")]
        public DateTime OrderedDate { get; set; }

        [ReadOnly(true)]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "General", FormGroupRow = 1, Placeholder = "")]
        [UIHint("TextBox")]
        [Display(Name = "PO")]
        public string PO { get; set; }

        [ReadOnly(true)]
        [UIHint("TextAreaBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 2, Placeholder = "")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [ReadOnly(true)]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 3, ComboUrl = "/APCombo/VendorCombo", ComboForeignKeys = "VendGroupID")]
        [Display(Name = "Vendor")]
        public int? VendorId { get; set; }

        [ReadOnly(true)]
        [HiddenInput]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 4, ComboUrl = "/JCCombo/ActiveCombo", ComboForeignKeys = "JCCo")]
        [Display(Name = "Job")]
        public string JobId { get; set; }

        [UIHint("TextAreaBox")]
        [Required]
        [Display(Name = "Reject Comment")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 5)]
        public string Comments { get; set; }


    }
}