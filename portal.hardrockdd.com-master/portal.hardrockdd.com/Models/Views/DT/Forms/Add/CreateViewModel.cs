using System;
using System.ComponentModel.DataAnnotations;

namespace portal.Models.Views.DailyTicket
{
    public class DailyTicketCreateViewModel
    {
        public DailyTicketCreateViewModel()
        {
            WorkDate = DateTime.Now.Date;

        }
        [Required]
        [Range(1, 11, ErrorMessage = "You must select a form")]
        [Display(Name = "Form")]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "New", FormGroupRow = 0, IconClass = "", Placeholder = "Select Form", ComboUrl = "/DailyForm/Combo", ComboForeignKeys = "")]
        public int FormId { get; set; }

        [Required]
        [UIHint("DateBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "New", FormGroupRow = 0, Placeholder = "")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Work Date")]
        public DateTime WorkDate { get; set; }

    }
}