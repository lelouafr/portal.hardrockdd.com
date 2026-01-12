using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Equipment.Audit
{
    public class EquipmentStatusLogListViewModel
    {
        public EquipmentStatusLogListViewModel()
        {

        }

        public EquipmentStatusLogListViewModel(DB.Infrastructure.ViewPointDB.Data.EMAudit audit)
        {
            if (audit == null)
            {
                throw new System.ArgumentNullException(nameof(audit));
            }
            EMCo = audit.EMCo;
            AuditId = audit.AuditId;
            List = audit.StatusLogs
                               //.Where(f => !(f.CreatedBy == f.CreatedBy ))
                               .Select(s => new EquipmentStatusLogViewModel(s))
                               .OrderBy(o => o.CreatedOn)
                               .ThenBy(o => o.LineNum)
                               .ToList();
        }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "EMCo")]
        public byte EMCo { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Audit Id")]
        public int AuditId { get; set; }

        public List<EquipmentStatusLogViewModel> List { get; }
    }

    public class EquipmentStatusLogViewModel
    {
        public EquipmentStatusLogViewModel()
        {

        }

        public EquipmentStatusLogViewModel(DB.Infrastructure.ViewPointDB.Data.EMAuditStatusLog t)
        {
            if (t == null)
            {
                throw new System.ArgumentNullException(nameof(t));
            }
            EMCo = t.EMCo;
            AuditId = t.AuditId;
            LineNum = t.LineNum;
            CreatedOn = t.CreatedOn;
            CreatedUser = new Web.WebUserViewModel(t.CreatedUser);
            Status = (DB.EMAuditStatusEnum)t.Status;
            Comments = t.Comments;
        }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "EMCo")]
        public byte EMCo { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Audit Id")]
        public int AuditId { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Line Num")]
        public int LineNum { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Comments")]
        public string Comments { get; set; }

        [UIHint("DateBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Log Date")]
        public DateTime? CreatedOn { get; set; }

        [UIHint("WebUserBox")]
        [Display(Name = "Created User")]
        [Field(LabelSize = 2, TextSize = 4)]
        public Web.WebUserViewModel CreatedUser { get; set; }

        [UIHint("EnumBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Status")]
        public DB.EMAuditStatusEnum Status { get; set; }

    }
}