using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Payroll.Leave
{
    public class LeaveRequestRejectViewModel
    {

        public LeaveRequestRejectViewModel()
        {

        }
        public LeaveRequestRejectViewModel(DB.Infrastructure.ViewPointDB.Data.LeaveRequest entity) 
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            PRCo = entity.PRCo;
            RequestId = entity.RequestId;
            EmployeeId = entity.EmployeeId;
            Status = (DB.LeaveRequestStatusEnum)entity.Status;
            Comments = "";
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
        [HiddenInput]
        [Display(Name = "Status")]
        public DB.LeaveRequestStatusEnum Status { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Display(Name = "Employee")]
        [Field(ForeignKeys = "PRCo,EmployeeId", ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo", InfoUrl = "/HumanResource/Resource/PopupForm", InfoForeignKeys = "HRCo=PRCo, EmployeeId")]
        public int? EmployeeId { get; set; }

        [UIHint("TextAreaBox")]
        [Required]
        [Display(Name = "Reject Comment")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 5)]
        public string Comments { get; set; }


    }
}