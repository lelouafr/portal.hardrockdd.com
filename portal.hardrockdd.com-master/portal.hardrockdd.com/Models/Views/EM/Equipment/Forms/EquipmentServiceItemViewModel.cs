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
    public class EquipmentServiceItemListViewModel
    {
        public EquipmentServiceItemListViewModel()
        {
            List = new List<EquipmentServiceItemViewModel>();
        }

        public EquipmentServiceItemListViewModel(DB.Infrastructure.ViewPointDB.Data.Equipment equipment)
        {
            if (equipment == null)
            {
                throw new System.ArgumentNullException(nameof(equipment));
            }
            EMCo = equipment.EMCo;
            EquipmentId = equipment.EquipmentId;

            List = equipment.ServiceLinks.Select(s => new EquipmentServiceItemViewModel(s)).ToList();

        }



        [Key]
        public byte EMCo { get; set; }

        [Key]
        public string EquipmentId { get; set; }


        public List<EquipmentServiceItemViewModel> List { get; }
    }

    public class EquipmentServiceItemViewModel
    {
        public EquipmentServiceItemViewModel()
        {

        }

        public EquipmentServiceItemViewModel(DB.Infrastructure.ViewPointDB.Data.EMServiceEquipmentLink item)
        {
            if (item == null)
                throw new System.ArgumentNullException(nameof(item));


            EMCo = item.EMCo;
            LinkId = item.LinkId;
            EquipmentId = item.EquipmentId;
            ServiceItemId = item.ServiceItemId;
            IsActive = item.IsActive;
            LastServiceDate = item.LastServiceDate;
            if (item.IsOverride)
            {
                DateIntervalTypeId = (DB.EMServiceDateIntervalEnum?)item.OverrideDateIntervalTypeId;
                DateInterval = item.OverrideDateInterval;
                OdoInterval = item.OverrideOdoInterval;
                HourInterval = item.OverrideHourInterval;
            }

            if (item.ServiceItem != null)
            {
                Description = item.ServiceItem.Description;

                DateIntervalTypeId ??= (DB.EMServiceDateIntervalEnum?)item.ServiceItem.DateIntervalTypeId;
                DateInterval ??= item.ServiceItem.DateInterval;
                OdoInterval ??= item.ServiceItem.OdoInterval;
                HourInterval ??= item.ServiceItem.HourInterval;
            }

        }

        [Key]
        [HiddenInput]
        public byte EMCo { get; set; }

        [Key]
        [HiddenInput]
        public int LinkId { get; set; }

        [Key]
        [HiddenInput]
        public string EquipmentId { get; set; }


        [UIHint("DropDownBox")]
        [Display(Name = "Service Id")]
        [Field(ComboUrl = "/EMCombo/EMServiceCombo", ComboForeignKeys = "EMCo")]
        public int? ServiceItemId { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 0, TextSize = 6)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [UIHint("SwitchBox")]
        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Interval Type")]
        public DB.EMServiceDateIntervalEnum? DateIntervalTypeId { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Date Interval")]
        public int? DateInterval { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Odo Interval")]
        public int? OdoInterval { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Hour Interval")]
        public int? HourInterval { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Last Service")]
        public DateTime? LastServiceDate { get; set; }
    }
}