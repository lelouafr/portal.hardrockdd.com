using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using DB.Infrastructure.ViewPointDB.Data;

namespace portal.Models.Views.Equipment.Audit
{
    public class EquipmentAuditMeterLineListViewModel
    {
        public EquipmentAuditMeterLineListViewModel()
        {

        }

        public EquipmentAuditMeterLineListViewModel(EMAudit audit)
        {
            if (audit == null)
                throw new ArgumentNullException(nameof(audit));

            EMCo = audit.EMCo;
            AuditId = audit.AuditId;
            List = audit.Lines.OrderBy(o => o.EquipmentId).Select(s => new EquipmentAuditMeterLineViewModel(s)).ToList();
        }

        [Key]
        [HiddenInput]
        public byte EMCo { get; set; }

        [Key]
        [HiddenInput]
        public int AuditId { get; set; }

        public List<EquipmentAuditMeterLineViewModel> List { get; }
    }

    public class EquipmentAuditMeterLineViewModel : AuditBaseViewModel
    {
        public EquipmentAuditMeterLineViewModel()
        {

        }

        public EquipmentAuditMeterLineViewModel(EMAuditLine audit) : base(audit)
        {
            if (audit == null)
                throw new ArgumentNullException(nameof(audit));

            PRCo = 1;
            JCCo = 1;

            EMCo = audit.EMCo;
            AuditId = audit.AuditId;
            SeqId = audit.SeqId;
            StatusId = audit.Audit.Status;
            AuditTypeId = (DB.EMAuditTypeEnum)audit.Audit.AuditTypeId;
            AuditForm = (DB.EMAuditFormEnum)audit.Audit.AuditFormId;
            EquipmentId = audit.EquipmentId;
            if (audit.Equipment != null)
            {
                LicensePlateNo = audit.Equipment.LicensePlateNo;
            }
            ActionId = (DB.EMAuditLineActionEnum)audit.ActionId;
            MeterTypeId = (DB.EMMeterTypeEnum)(int.TryParse(audit.Equipment?.MeterTypeId, out int meterTypeOut) ? meterTypeOut : 0);
            OdoReading = audit.OdoReading;
            OdoDate = audit.OdoDate;
            HourReading = audit.HourReading;
            HourDate = audit.HourDate;
            ToCrewId = audit.ToCrewId;
            ToLocationId = audit.ToLocationId;
            ToEmployeeId = audit.ToEmployeeId;
            Completed = audit.Completed;
            Comments = audit.Comments;
            IsEquipmentIdLocked = ActionId == DB.EMAuditLineActionEnum.Update || ActionId == DB.EMAuditLineActionEnum.Remove || ActionId == DB.EMAuditLineActionEnum.Transfer;

            LastOdoReading = audit.Equipment?.OdoReading ?? 0;
            LastHourReading = audit.Equipment?.HourReading ?? 0;
        }



        public bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new System.ArgumentNullException(nameof(modelState));
            }
            if (StatusId == DB.EMAuditStatusEnum.Canceled)
            {
                return modelState.IsValid;
            }
            if (!Completed)
            {
                modelState.AddModelError("EquipmentId", "Completed Must be marked");
            }
            if (ActionId == DB.EMAuditLineActionEnum.Transfer)
            {
                if (StatusId == DB.EMAuditStatusEnum.Submitted && false)
                {
                    switch (AuditTypeId)
                    {
                        case DB.EMAuditTypeEnum.CrewAudit:
                            if (ToCrewId == null)
                            {
                                modelState.AddModelError("ToCrewId", "Crew must be selected for approval");
                            }
                            break;
                        case DB.EMAuditTypeEnum.EmployeeAudit:
                            if (ToEmployeeId == null)
                            {
                                modelState.AddModelError("ToEmployeeId", "Employee must be selected for approval");
                            }
                            break;
                        case DB.EMAuditTypeEnum.LocationAudit:
                            if (ToLocationId == null)
                            {
                                modelState.AddModelError("ToLocationId", "Location must be selected for approval");
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            else { 
                switch (MeterTypeId)
                {
                    case DB.EMMeterTypeEnum.Hours:
                        if ((HourReading ?? 0) <= 0 && ActionId != DB.EMAuditLineActionEnum.Remove)
                        {
                            modelState.AddModelError("HourReading", "Must be grater than 0");
                        }
                        break;
                    case DB.EMMeterTypeEnum.Odometer:
                        if ((OdoReading ?? 0) <= 0 && ActionId != DB.EMAuditLineActionEnum.Remove)
                        {
                            modelState.AddModelError("OdoReading", "Must be grater than 0");
                        }
                        break;
                    case DB.EMMeterTypeEnum.Both:
                        if ((HourReading ?? 0) <= 0 && ActionId != DB.EMAuditLineActionEnum.Remove)
                        {
                            modelState.AddModelError("HourReading", "Must be grater than 0");
                        }
                        if ((OdoReading ?? 0) <= 0 && ActionId != DB.EMAuditLineActionEnum.Remove)
                        {
                            modelState.AddModelError("OdoReading", "Must be grater than 0");
                        }
                        break;
                    default:
                        break;
                }
            }
            return modelState.IsValid; 
        }


        public byte? PRCo { get; set; }
        public byte? JCCo { get; set; }

        [Key]
        [HiddenInput]
        public byte EMCo { get; set; }

        [Key]
        [HiddenInput]
        public int AuditId { get; set; }

        [Key]
        [HiddenInput]
        public int SeqId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.EMAuditStatusEnum StatusId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [UIHint("EnumBox")]
        [Display(Name = "Audit Type")]
        public DB.EMAuditTypeEnum AuditTypeId { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Audit Form")]
        [Field(LabelSize = 3, TextSize = 9)]
        public DB.EMAuditFormEnum AuditForm { get; set; }

        [UIHint("EnumBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Action")]
        public DB.EMAuditLineActionEnum ActionId { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Equipment")]
        [Field(ComboUrl = "/EMCombo/Combo", ComboForeignKeys = "EMCo", SearchUrl = "/EMCombo/Search", SearchForeignKeys = "EMCo", InfoUrl = "/EquipmentForm/PopupForm", InfoForeignKeys = "EMCo,EquipmentId")]
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
        [Display(Name = "Last Odometer")]
        public decimal LastOdoReading { get; set; }

        [UIHint("LongBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Last Hour Reading")]
        public decimal LastHourReading { get; set; }

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
        [Display(Name = "To Employee")]
        [Field(ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo", InfoUrl = "/HumanResource/Resource/PopupForm", InfoForeignKeys = "HRCo=PRCo, EmployeeId=ToEmployeeId")]
        public int? ToEmployeeId { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "To Location")]
        [Field(ComboUrl = "/EquipmentLocation/Combo", ComboForeignKeys = "EMCo")]
        public string ToLocationId { get; set; }

        [UIHint("SwitchBox")]
        [Display(Name = "Completed")]
        public bool Completed { get; set; }

        public bool IsEquipmentIdLocked { get; set; }

        [UIHint("TextAreaBox")]
        [Display(Name = "Comments")]
        public string Comments { get; set; }
    }
}