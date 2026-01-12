using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.Equipment.Audit
{
    public class EquipmentAuditCreateViewModel
    {
        public EquipmentAuditCreateViewModel()
        {
            using var db = new VPContext();

            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var emp = user.Employee.FirstOrDefault();
            PRCo = (byte)emp.PRCo;
            EMCo = 1;
            IncludCrewLeaderEquipment = true;
            IncludeDirectReportEmployeeEquipment = true;
            IncludeSubEquipment = true;
            AssignedTo = StaticFunctions.GetUserId();
            AuditForm = DB.EMAuditFormEnum.Meter;

        }

        public EquipmentAuditCreateViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));

            PRCo = (byte)company.PRCo;
            EMCo = 1;
            IncludCrewLeaderEquipment = true;
            IncludeDirectReportEmployeeEquipment = true;
            IncludeSubEquipment = true;
            AssignedTo = StaticFunctions.GetUserId();
            EquipmentList = new EquipmentListViewModel();
            AuditForm = DB.EMAuditFormEnum.Meter;
        }

        public bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null)
                throw new System.ArgumentNullException(nameof(modelState));
            var ok = true;

            switch (AuditType)
            {
                case DB.EMAuditTypeEnum.CrewAudit:
                    if (CrewId == null)
                    {
                        ok &= false;
                        modelState.AddModelError("CrewId", "Crew is Required");
                    }
                    break;
                case DB.EMAuditTypeEnum.EmployeeAudit:
                    if (EmployeeId == null)
                    {
                        ok &= false;
                        modelState.AddModelError("EmployeeId", "Employee is Required");
                    }
                    break;
                case DB.EMAuditTypeEnum.LocationAudit:
                    if (LocationId == null)
                    {
                        ok &= false;
                        modelState.AddModelError("LocationId", "Location is Required");
                    }
                    break;
                default:
                    break;
            }

            return ok;
        }
        public byte PRCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        public byte EMCo { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Audit Type")]
        [Field(LabelSize = 3, TextSize = 9)]
        public DB.EMAuditTypeEnum AuditType { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Audit Form")]
        [Field(LabelSize = 3, TextSize = 9)]
        public DB.EMAuditFormEnum AuditForm { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 9, ComboUrl = "/WebUsers/Combo", ComboForeignKeys = "")]
        [Display(Name = "Assigned To")]
        public string AssignedTo { get; set; }

        [UIHint("TextAreaBox")]
        [Field(LabelSize = 3, TextSize = 9)]
        public string Description { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 9, ComboUrl = "/PRCombo/CrewCombo", ComboForeignKeys = "PRCo")]
        [Display(Name = "Crew")]
        public string CrewId { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 9, ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo", InfoUrl = "/HumanResource/Resource/PopupForm", InfoForeignKeys = "HRCo=PRCo, EmployeeId")]
        [Display(Name = "Employee")]
        public int? EmployeeId { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 9, ComboUrl = "/EquipmentLocation/Combo", ComboForeignKeys = "EMCo")]
        [Display(Name = "Location")]
        public string LocationId { get; set; }

        [UIHint("CheckBox")]
        [Display(Name = "Include Crew Leader Assigned Equipment")]
        [Field(LabelSize = 11, TextSize = 1)]
        public bool IncludCrewLeaderEquipment { get; set; }

        [UIHint("CheckBox")]
        [Display(Name = "Include Direct Report Employees Equipment")]
        [Field(LabelSize = 11, TextSize = 1)]
        public bool IncludeDirectReportEmployeeEquipment { get; set; }

        [UIHint("CheckBox")]
        [Display(Name = "Include Sub Equipment")]
        [Field(LabelSize = 11, TextSize = 1)]
        public bool IncludeSubEquipment { get; set; }

        public EquipmentListViewModel EquipmentList { get; set; }
    }
}