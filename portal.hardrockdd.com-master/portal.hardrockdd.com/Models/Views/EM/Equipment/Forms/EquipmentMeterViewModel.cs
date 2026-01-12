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
    
    public class EquipmentMeterViewModel : AuditBaseViewModel
    {
        public EquipmentMeterViewModel()
        {

        }
        
        public EquipmentMeterViewModel(DB.Infrastructure.ViewPointDB.Data.Equipment equipment) : base(equipment)
        {
            if (equipment == null)
            {
                throw new System.ArgumentNullException(nameof(equipment));
            }

            EMCo = equipment.EMCo;
            EquipmentId = equipment.EquipmentId;
            MeterType = (DB.EMMeterTypeEnum?)(int.TryParse(equipment.MeterTypeId, out int meterTypeOut) ? meterTypeOut : meterTypeOut);            
            OdoReading = equipment.OdoReading;
            OdoDate = equipment.OdoDate;
            ReplacedOdoReading = equipment.ReplacedOdoReading;
            ReplacedOdoDate = equipment.ReplacedOdoDate;
            HourReading = equipment.HourReading;
            HourDate = equipment.HourDate;
            ReplacedHourReading = equipment.ReplacedHourReading;
            ReplacedHourDate = equipment.ReplacedHourDate;

            Logs = new EquipmentMeterLogListViewModel(equipment);
        }

        [Key]
        [HiddenInput]
        public byte EMCo { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Field(LabelSize = 4, TextSize = 8)]
        [Display(Name = "Equipment")]
        public string EquipmentId { get; set; }

        [UIHint("EnumBox")]
        [Field(LabelSize = 4, TextSize = 8)]
        [Display(Name = "Meter Type")]
        public DB.EMMeterTypeEnum? MeterType { get; set; }

        [UIHint("LongBox")]
        [Field(LabelSize = 4, TextSize = 8)]
        [Display(Name = "Odo Reading")]
        public decimal OdoReading { get; set; }

        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Odo Date")]
        [Field(LabelSize = 4, TextSize = 8)]
        public DateTime? OdoDate { get; set; }

        [HiddenInput]
        //[UIHint("LongBox")]
        [Field(LabelSize = 4, TextSize = 8)]
        [Display(Name = "ReplacedOdoReading")]
        public decimal ReplacedOdoReading { get; set; }

        [HiddenInput]
        //[UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "ReplacedOdoDate")]
        [Field(LabelSize = 4, TextSize = 8)]
        public DateTime? ReplacedOdoDate { get; set; }

        [UIHint("LongBox")]
        [Field(LabelSize = 4, TextSize = 8)]
        [Display(Name = "Hour Reading")]
        public decimal HourReading { get; set; }

        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Hour Date")]
        [Field(LabelSize = 4, TextSize = 8)]
        public DateTime? HourDate { get; set; }

        [HiddenInput]
        //[UIHint("LongBox")]
        [Field(LabelSize = 4, TextSize = 8)]
        [Display(Name = "Replaced Hour Reading")]
        public decimal ReplacedHourReading { get; set; }

        [HiddenInput]
        // [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "ReplacedHour Date")]
        [Field(LabelSize = 4, TextSize = 8)]
        public DateTime? ReplacedHourDate { get; set; }

        public EquipmentMeterLogListViewModel Logs { get; set; }
    }
}