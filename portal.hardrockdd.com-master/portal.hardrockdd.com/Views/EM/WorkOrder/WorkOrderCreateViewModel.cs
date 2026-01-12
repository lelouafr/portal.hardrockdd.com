using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Models.Views.EM.WorkOrder
{
    public class WorkOrderCreateViewModel
    {
        public WorkOrderCreateViewModel()
        {
            DateCreated = DateTime.Now;
            EMCo = 1;
        }

        [Key]
        [Required]
        [HiddenInput]
        public byte EMCo { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 9, ComboUrl = "/EMCombo/Combo", ComboForeignKeys = "EMCo")]
        [Display(Name = "Equipment")]
        public string EquipmentId { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Field(LabelSize = 3, TextSize = 9)]
        [Display(Name = "Description")]
        [StringLength(60, ErrorMessage = "Description cannot exceed 60 characters")]
        public string Description { get; set; }

        [UIHint("DateBox")]
        [Field(LabelSize = 3, TextSize = 9)]
        [Display(Name = "Date Due")]
        public DateTime? DateDue { get; set; }

        [UIHint("TextAreaBox")]
        [Field(LabelSize = 3, TextSize = 9)]
        [Display(Name = "Notes")]
        public string Notes { get; set; }

        public DateTime DateCreated { get; set; }
    }
}