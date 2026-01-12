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
    public class EquipmentMeterLogListViewModel
    {
        public EquipmentMeterLogListViewModel()
        {
            List = new List<EquipmentMeterLogViewModel>();
        }

        public EquipmentMeterLogListViewModel(DB.Infrastructure.ViewPointDB.Data.Equipment equipment)
        {
            if (equipment == null)
            {
                throw new System.ArgumentNullException(nameof(equipment));
            }
            EMCo = equipment.EMCo;
            EquipmentId = equipment.EquipmentId;
           
            List = equipment.Logs.Where(f => f.OldHourReading != f.NewHourReading || f.OldOdoReading != f.NewOdoReading)
                                 .OrderByDescending(o => o.LogDate)
                                 .Select(s => new EquipmentMeterLogViewModel(s))
                                 .ToList();

        }

       

        public byte EMCo { get; set; }

        public string EquipmentId { get; set; }


        public List<EquipmentMeterLogViewModel> List { get;  }
    }

    public class EquipmentMeterLogViewModel
    {
        public EquipmentMeterLogViewModel()
        {

        }
        
        public EquipmentMeterLogViewModel(DB.Infrastructure.ViewPointDB.Data.EquipmentLog equipment)
        {
            if (equipment == null)
            {
                throw new System.ArgumentNullException(nameof(equipment));
            }

            EMCo = equipment.EMCo;
            EquipmentId = equipment.EquipmentId;
            SeqId = equipment.SeqId;
            LogTypeId = (DB.EMLogTypeEnum)equipment.LogTypeId;
            LogDate = equipment.LogDate;
            LoggedBy = equipment.LoggedUser.PREmployee.FullName(false);

            HourReading = equipment.NewHourReading;
            OdoReading = equipment.NewOdoReading;
            Notes = equipment.Notes;
        }

        [Key]
        [HiddenInput]
        public byte EMCo { get; set; }

        [Key]
        [HiddenInput]
        public string EquipmentId { get; set; }

        [Key]
        [HiddenInput]
        public int SeqId { get; set; }

        [Display(Name = "Type")]
        [UIHint("EnumBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        public DB.EMLogTypeEnum LogTypeId { get; set; }

        [Display(Name = "LogDate")]
        [UIHint("Datebox")]
        public DateTime LogDate { get; set; }

        [HiddenInput]
        //[UIHint("TextBox")]
        [Display(Name = "Logged By")]
        public string LoggedBy { get; set; }

        [UIHint("TextAreaBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Notes")]
        public string Notes { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Hours")]
        public decimal? HourReading { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Odo")]
        public decimal? OdoReading { get; set; }
    }
}