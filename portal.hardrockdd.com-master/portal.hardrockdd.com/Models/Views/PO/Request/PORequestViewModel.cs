using portal.Models.Views.Web;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Models.Views.Purchase.Request
{

    public class PORequestViewModel
    {
        public PORequestViewModel()
        {

        }

        public PORequestViewModel(DB.Infrastructure.ViewPointDB.Data.PORequest entity)
        {
            if (entity == null)
            {
                throw new System.ArgumentNullException(nameof(entity));
            }
            POCo = entity.POCo;
            RequestId = entity.RequestId;
            OrderedDate = entity.OrderedDate;
            PO = entity.PO;
            Status = (DB.PORequestStatusEnum)entity.Status;
            Description = entity.Description;

            VendorGroupId = entity.VendorGroup ?? entity.POCompanyParm.HQCompanyParm.VendorGroupId;
            VendorId = entity.VendorId;
            JobId = entity.JobId;
            PRCo = entity.OrderedByCo;
            EmployeeId = entity.OrderedBy;

            NewVendorName = entity.NewVendorName;
            NewVendorPhone = entity.NewVendorPhone;
            NewVendorAddress = entity.NewVendorAddress;

            if (entity.Vendor != null)
            {
                VendorName = entity.Vendor.Name;
            }

            ApprovedUser = new WebUserViewModel(entity.ApprovedUser);
        }
        public bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new System.ArgumentNullException(nameof(modelState));
            }
            var ok = modelState.IsValid;
            if (VendorId == 999999)
            {
                if (NewVendorName == null)
                {
                    modelState.AddModelError("NewVendorName", "Vendor Name Is required");
                }
                if (NewVendorAddress == null)
                {
                    modelState.AddModelError("NewVendorAddress", "Vendor Address Is required");
                }
                if (NewVendorPhone == null)
                {
                    modelState.AddModelError("NewVendorPhone", "Vendor Phone Is required");
                }
            }
            return ok;
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


        public byte? PRCo { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Display(Name = "Ordered By")]
        [Field(ForeignKeys = "POCo=PRCo,EmployeeId", ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo", InfoUrl = "/HumanResource/Resource/PopupForm", InfoForeignKeys = "HRCo=PRCo, EmployeeId")]
        public int? EmployeeId { get; set; }

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

        [Required]
        [UIHint("TextAreaBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 2, Placeholder = "")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public byte? VendorGroupId { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 3, ComboUrl = "/APCombo/VendorCombo", ComboForeignKeys = "VendGroupId=VendorGroupId", SearchUrl = "/APCombo/VendorSearch", SearchForeignKeys = "VendGroupId=VendorGroupId")]
        [Display(Name = "Vendor")]
        public int? VendorId { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "General", FormGroupRow = 4, Placeholder = "")]
        [Display(Name = "Vendor Name")]
        public string NewVendorName { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "General", FormGroupRow = 4, Placeholder = "")]
        [Display(Name = "Vendor Name")]
        public string VendorName { get; set; }

        [UIHint("PhoneBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "General", FormGroupRow = 4, Placeholder = "")]
        [Display(Name = "Vendor Phone")]
        public string NewVendorPhone { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 5, Placeholder = "")]
        [Display(Name = "Vendor Address")]
        public string NewVendorAddress { get; set; }

        [HiddenInput]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 6, ComboUrl = "/JCCombo/ActiveCombo", ComboForeignKeys = "JCCo")]
        [Display(Name = "Job")]
        public string JobId { get; set; }

        [HiddenInput]
        [UIHint("WebUserBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "General", FormGroupRow = 7)]
        [Display(Name = "Approved User")]
        public WebUserViewModel ApprovedUser { get; set; }

    }
}