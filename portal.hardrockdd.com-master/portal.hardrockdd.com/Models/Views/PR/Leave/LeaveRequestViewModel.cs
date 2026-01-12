using portal.Models.Views.Web;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Models.Views.Payroll.Leave
{

    public class LeaveRequestViewModel
    {
        public LeaveRequestViewModel()
        {

        }

        public LeaveRequestViewModel(DB.Infrastructure.ViewPointDB.Data.LeaveRequest entity)
        {
            if (entity == null)
            {
                throw new System.ArgumentNullException(nameof(entity));
            }
            PRCo = entity.PRCo;
            RequestId = entity.RequestId;
            Status = (DB.LeaveRequestStatusEnum)entity.Status;
            Comments = entity.Comments;
            EmployeeId = entity.EmployeeId;

            CreatedUser = new WebUserViewModel(entity.CreatedUser);
            CreatedOn = entity.CreatedOn;
            //ApprovedUser = new WebUserViewModel(entity.ApprovedUser);
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
        [Display(Name = "PRCo")]
        public byte PRCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Id")]
        public int RequestId { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.LeaveRequestStatusEnum Status { get; set; }


        [Required]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo", InfoUrl = "/HumanResource/Resource/PopupForm", InfoForeignKeys = "HRCo=PRCo, EmployeeId")]
        [Display(Name = "Employee")]
        public int? EmployeeId { get; set; }

        [Required]
        [UIHint("TextAreaBox")]
        [Display(Name = "Comments")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string Comments { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [UIHint("WebUserBox")]
        [Display(Name = "Approved User")]
        public WebUserViewModel ApprovedUser { get; set; }


        [ReadOnly(true)]
        [UIHint("WebUserBox")]
        [Display(Name = "Created User")]
        public WebUserViewModel CreatedUser { get; set; }


        [ReadOnly(true)]
        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }

    }
}