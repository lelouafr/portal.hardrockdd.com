using portal.Repository.VP.PR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Payroll.Leave
{
    public class LeaveRequestLineListViewModel
    {
        public LeaveRequestLineListViewModel()
        {

        }
        public LeaveRequestLineListViewModel(DB.Infrastructure.ViewPointDB.Data.LeaveRequest request)
        {
            if (request == null)
            {
                throw new System.ArgumentNullException(nameof(request));
            }
            PRCo = request.PRCo;
            RequestId = request.RequestId;

            List = request.Lines.Select(s => new LeaveRequestLineViewModel(s)).ToList();
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "PRCo")]
        public byte PRCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Request Id")]
        public int RequestId { get; set; }

        public List<LeaveRequestLineViewModel> List { get; }
    }

    public class LeaveRequestLineViewModel : AuditBaseViewModel
    {
        public LeaveRequestLineViewModel()
        {

        }

        public LeaveRequestLineViewModel(DB.Infrastructure.ViewPointDB.Data.LeaveRequestLine entry):base(entry)
        {
            if (entry == null)
            {
                throw new System.ArgumentNullException(nameof(entry));
            }
            PRCo = entry.PRCo;
            RequestId = entry.RequestId;
            LineId = entry.LineId;
            WorkDate = entry.WorkDate;
            EmployeeId = entry.EmployeeId;
            LeaveCodeId = entry.LeaveCodeId;
            Hours = entry.Hours;

            Comments = entry.Comments;
        }

        public bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new System.ArgumentNullException(nameof(modelState));
            }
            var ok = modelState.IsValid;
            if (Hours > 8)
            {
                ok = false;
                modelState.AddModelError("Hours", "Hours may not exceed 8 per day");
            }
            if (Hours == 0)
            {
                ok = false;
                modelState.AddModelError("Hours", "Hours may not be zero");
            }
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
        [Display(Name = "Ticket Id")]
        public int RequestId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Line Num")]
        public int LineId { get; set; }

        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date")]
        public DateTime? WorkDate { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "General", FormGroupRow = 3, ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo", InfoUrl = "/HumanResource/Resource/PopupForm", InfoForeignKeys = "HRCo=PRCo, EmployeeId")]
        [Display(Name = "Employee")]
        public int? EmployeeId { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PRCombo/LeaveCodeCombo", ComboForeignKeys = "PRCo")]
        [Display(Name = "LeaveCode")]
        public string LeaveCodeId { get; set; }

        [Required]
        [UIHint("IntegerBox")]
        [Display(Name = "Hours")]
        public decimal? Hours { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "Comments")]
        [Field(LabelSize = 2, TextSize = 10)]
        [TableField(InternalTableRow = 2)]
        public string Comments { get; set; }
    }

    
}