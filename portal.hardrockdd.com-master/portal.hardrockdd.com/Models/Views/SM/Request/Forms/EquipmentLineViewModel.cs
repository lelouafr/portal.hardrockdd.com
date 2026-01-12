using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.SM.Request.Forms
{
    public class EquipmentLineListViewModel
    {
        public EquipmentLineListViewModel()
        {

        }

        public EquipmentLineListViewModel(DB.Infrastructure.ViewPointDB.Data.SMRequest request)
        {
            if (request == null) 
                throw new System.ArgumentNullException(nameof(request));

            SMCo = request.SMCo;
            RequestId = request.RequestId;

            List = request.Lines.Select(s => new EquipmentLineViewModel(s)).ToList();
        }

        [Key]
        [HiddenInput]
        public byte SMCo { get; set; }

        [Key]
        [HiddenInput]
        public int RequestId { get; set; }


        public List<EquipmentLineViewModel> List { get;  }
    }
    public class EquipmentLineViewModel
    {

        public EquipmentLineViewModel()
        {

        }

        public EquipmentLineViewModel(SMRequestLine line)
        {
            if (line == null) 
                throw new System.ArgumentNullException(nameof(line));
            
            EmployeeId = line.Request.RequestUser.PREmployee.EmployeeId;

            SMCo = line.SMCo;
            RequestId = line.RequestId;
            LineId = line.LineId;
            EMCo = line.EMCo ?? 1;
            EquipmentId = line.tEquipmentId;
            EquipmentName = line.Equipment?.Description;
            RequestComments = line.RequestComments;
            IsEquipmentDisabled = line.IsEquipmentDisabled ?? false;
            HasAttachments = line.Attachment.Files.Any();
        }

        [Key]
        [HiddenInput]
        public byte SMCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Request Id")]
        public int RequestId { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Link Id")]
        public int LineId { get; set; }


        public int EmployeeId { get; set; }

        [HiddenInput]
        public byte? EMCo { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/AssingedEquipmentCombo", ComboForeignKeys = "EMCo,EmployeeId")]
        [Display(Name = "Equipment")]
        public string EquipmentId { get; set; }


        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Name")]
        public string EquipmentName { get; set; }


        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Request Comments (Description of the Issue)")]
        public string RequestComments { get; set; }

        [UIHint("SwitchBox")]
        [Display(Name = "Disabled Equipment")]
        public bool IsEquipmentDisabled { get; set; }

        public bool HasAttachments { get; set; }
    }
}