using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Equipment.Audit
{
    public class EquipmentAuditRejectViewModel
    {

        public EquipmentAuditRejectViewModel()
        {

        }
        public EquipmentAuditRejectViewModel(DB.Infrastructure.ViewPointDB.Data.EMAudit audit)
        {
            if (audit == null)
            {
                throw new System.ArgumentNullException(nameof(audit));
            }
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();

            EMCo = audit.EMCo;
            AuditId = audit.AuditId;
            AuditType = (DB.EMAuditTypeEnum)audit.AuditTypeId;
            Description = audit.Description;
            AssignedTo = audit.AssignedTo;

        }
        [Key]
        [Required]
        [HiddenInput]
        public byte EMCo { get; set; }


        [Key]
        [Required]
        [HiddenInput]
        public int AuditId { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Audit Type")]
        [Field(LabelSize = 3, TextSize = 9)]
        public DB.EMAuditTypeEnum AuditType { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 9, ComboUrl = "/WebUsers/Combo", ComboForeignKeys = "")]
        [Display(Name = "Assigned To")]
        public string AssignedTo { get; set; }

        [UIHint("TextAreaBox")]
        [Field(LabelSize = 3, TextSize = 9)]
        public string Description { get; set; }

        [UIHint("TextAreaBox")]
        [Required]
        [Display(Name = "Reject Comment")]
        [Field(LabelSize = 3, TextSize = 9, FormGroup = "Ticket", FormGroupRow = 5)]
        public string Comments { get; set; }


    }
}