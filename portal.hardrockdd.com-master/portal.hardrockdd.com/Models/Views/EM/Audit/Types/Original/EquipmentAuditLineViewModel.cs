using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DB.Infrastructure.ViewPointDB.Data;

namespace portal.Models.Views.Equipment.Audit
{
    public class EquipmentAuditLineListViewModel
    {
        public EquipmentAuditLineListViewModel()
        {

        }

        public EquipmentAuditLineListViewModel(EMAudit audit)
        {
            if (audit == null)
                throw new ArgumentNullException(nameof(audit));
            EMCo = audit.EMCo;
            AuditId = audit.AuditId;
            List = audit.Lines.Select(s => new EquipmentAuditLineViewModel(s)).ToList();
        }

        [Key]
        [HiddenInput]
        public byte EMCo { get; set; }

        [Key]
        [HiddenInput]
        public int AuditId { get; set; }

        public List<EquipmentAuditLineViewModel> List { get; }
    }

    public class EquipmentAuditLineViewModel : AuditBaseViewModel
    {
        public EquipmentAuditLineViewModel()
        {

        }

        public EquipmentAuditLineViewModel(EMAuditLine audit) : base(audit)
        {
            if (audit == null)
                throw new ArgumentNullException(nameof(audit));

            EMCo = audit.EMCo;
            AuditId = audit.AuditId;
            SeqId = audit.SeqId;
            EquipmentId = audit.EquipmentId;
            if (audit.Equipment != null)
            {
                LicensePlateNo = audit.Equipment.LicensePlateNo;
            }
            ActionId = (DB.EMAuditLineActionEnum)audit.ActionId;
            MeterTypeId = (DB.EMMeterTypeEnum)(int.TryParse(audit.MeterTypeId, out int meterTypeOut) ? meterTypeOut : 0);
            OdoReading = audit.OdoReading;
            OdoDate = audit.OdoDate;
            HourReading = audit.HourReading;
            HourDate = audit.HourDate;
            ToCrewId = audit.ToCrewId;
            //ToEmployeeId = audit.ToEmployeeId;
            ToLocationId = audit.ToLocationId;
            ToJobId = audit.ToJobId;
            Completed = audit.Completed ;
            IsEquipmentIdLocked = ActionId == DB.EMAuditLineActionEnum.Update || ActionId == DB.EMAuditLineActionEnum.Remove;
        }



        public bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new System.ArgumentNullException(nameof(modelState));
            }
            switch (MeterTypeId)
            {
                case DB.EMMeterTypeEnum.Hours:
                    if ((HourReading ?? 0) == 0 && ActionId != DB.EMAuditLineActionEnum.Remove)
                    {
                        modelState.AddModelError("HourReading", "Must be grater than 0");
                    }
                    break;
                case DB.EMMeterTypeEnum.Odometer:
                    if ((OdoReading ?? 0) == 0 && ActionId != DB.EMAuditLineActionEnum.Remove)
                    {
                        modelState.AddModelError("OdoReading", "Must be grater than 0");
                    }
                    break;
                case DB.EMMeterTypeEnum.Both:
                    if ((HourReading ?? 0) == 0 && ActionId != DB.EMAuditLineActionEnum.Remove)
                    {
                        modelState.AddModelError("HourReading", "Must be grater than 0");
                    }
                    if ((OdoReading ?? 0) == 0 && ActionId != DB.EMAuditLineActionEnum.Remove)
                    {
                        modelState.AddModelError("OdoReading", "Must be grater than 0");
                    }
                    break;
                default:
                    break;
            }
            if (!Completed)
            {
                modelState.AddModelError("EquipmentId", "Completed Must be marked");
            }
            return modelState.IsValid; 
        }


        [Key]
        [HiddenInput]
        public byte EMCo { get; set; }

        [Key]
        [HiddenInput]
        public int AuditId { get; set; }
        [Key]
        [HiddenInput]
        public int SeqId { get; set; }

        [UIHint("EnumBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Action")]
        public DB.EMAuditLineActionEnum ActionId { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Equipment")]
        [Field(ComboUrl = "/EMCombo/Combo", ComboForeignKeys = "EMCo")]
        public string EquipmentId { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Plate No")]
        public string LicensePlateNo { get; set; }

        [UIHint("EnumBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Meter Type")]
        public DB.EMMeterTypeEnum MeterTypeId { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Odometer")]
        public decimal? OdoReading { get; set; }

        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [Display(Name = "Odo Date")]
        public DateTime? OdoDate { get; set; }

        [UIHint("LongBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Hour Reading")]
        public decimal? HourReading { get; set; }

        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [Display(Name = "Hour Date")]
        public DateTime? HourDate { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "To Job")]
        [Field(ComboUrl = "/JCCombo/ActiveCombo", ComboForeignKeys = "JCCo")]
        public string ToJobId { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "To Crew")]
        [Field(ComboUrl = "/PRCombo/CrewCombo", ComboForeignKeys = "PRCo")]
        public string ToCrewId { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "To Location")]
        [Field(ComboUrl = "/EquipmentLocation/Combo", ComboForeignKeys = "EMCo")]
        public string ToLocationId { get; set; }

        [UIHint("SwitchBox")]
        [Display(Name = "Completed")]
        public bool Completed { get; set; }

        public bool IsEquipmentIdLocked { get; set; }

    }
}