using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace portal.Models.Views.Equipment.Forms
{
    public class EquipmentAssignmentViewModel : AuditBaseViewModel
    {
        public EquipmentAssignmentViewModel()
        {

        }

        public EquipmentAssignmentViewModel(DB.Infrastructure.ViewPointDB.Data.Equipment equipment): base(equipment)
        {
            if (equipment == null)
            {
                throw new System.ArgumentNullException(nameof(equipment));
            }

            EMCo = equipment.EMCo;
            EquipmentId = equipment.EquipmentId;

            AssignmentType = (DB.EMAssignmentTypeEnum?)equipment.AssignmentType;

            PRCo = equipment.PRCo;
            Operator = equipment.OperatorId;
            AssignedCrewId = equipment.AssignedCrewId;

            ParentEquimentId = equipment.ParentEquimentId;

            LocationId = equipment.LocationId;

            AssignedStatus = AssignmentType switch
            {
                DB.EMAssignmentTypeEnum.Equipment => ParentEquimentId != null ? DB.EMAssignedStatusEnum.Assigned : DB.EMAssignedStatusEnum.UnAssigned,
                DB.EMAssignmentTypeEnum.Crew => AssignedCrewId != null ? DB.EMAssignedStatusEnum.Assigned : DB.EMAssignedStatusEnum.UnAssigned,
                DB.EMAssignmentTypeEnum.Employee => Operator != null ? DB.EMAssignedStatusEnum.Assigned : DB.EMAssignedStatusEnum.UnAssigned,
                _ => DB.EMAssignedStatusEnum.UnAssigned,
            };
            if (AssignedStatus == DB.EMAssignedStatusEnum.UnAssigned)
            {
                if (LocationId != null)
                {
                    AssignedStatus = DB.EMAssignedStatusEnum.Assigned;
                }
            }

            Logs = new EquipmentAssignmentLogListViewModel(equipment);
        }

        [Key]
        [HiddenInput]
        public byte EMCo { get; set; }

        [Key]
        [HiddenInput]
        [Field(LabelSize = 4, TextSize = 8)]
        [Display(Name = "Equipment")]
        public string EquipmentId { get; set; }

        [HiddenInput]
        public byte? PRCo { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 4, TextSize = 8, ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo")]
        [Display(Name = "Employee")]
        public int? Operator { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 4, TextSize = 8, ComboUrl = "/EMCombo/Combo", ComboForeignKeys = "EMCo")]
        [Display(Name = "Parent")]
        public string ParentEquimentId { get; set; }
        //public decimal? Qty { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 4, TextSize = 8, ComboUrl = "/PRCombo/CrewCombo", ComboForeignKeys = "PRCo")]
        [Display(Name = "Crew")]
        public string AssignedCrewId { get; set; }


        [UIHint("DropdownBox")]
        [Display(Name = "Location")]
        [Field(LabelSize = 4, TextSize = 8, ComboUrl = "/EquipmentLocation/Combo", ComboForeignKeys = "EMCo")]
        public string LocationId { get; set; }

        [UIHint("EnumBox")]
        [Field(LabelSize = 4, TextSize = 8)]
        [Display(Name = "Type")]
        public DB.EMAssignmentTypeEnum? AssignmentType { get; set; }

        [HiddenInput]
        [UIHint("EnumBox")]
        [Field(LabelSize = 4, TextSize = 8)]
        [Display(Name = "Type")]
        public DB.EMAssignedStatusEnum AssignedStatus { get; set; }

        public EquipmentAssignmentLogListViewModel Logs { get; set; }

    }
}