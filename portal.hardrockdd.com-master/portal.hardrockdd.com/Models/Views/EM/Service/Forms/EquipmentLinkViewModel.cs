using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.Equipment.Service.Forms
{
    public class EquipmentLinkListViewModel
    {
        public EquipmentLinkListViewModel()
        {

        }

        public EquipmentLinkListViewModel(DB.Infrastructure.ViewPointDB.Data.EMServiceItem serviceItem)
        {
            if (serviceItem == null) 
                throw new System.ArgumentNullException(nameof(serviceItem));

            Co = serviceItem.EMCo;
            ServiceItemId = serviceItem.ServiceItemId;

            List = serviceItem.EquipmentLinks.Select(s => new EquipmentLinkViewModel(s)).ToList();
        }

        [Key]
        [HiddenInput]
        public byte Co { get; set; }

        [Key]
        [HiddenInput]
        public int ServiceItemId { get; set; }


        public List<EquipmentLinkViewModel> List { get;  }
    }
    public class EquipmentLinkViewModel
    {

        public EquipmentLinkViewModel()
        {

        }

        public EquipmentLinkViewModel(EMServiceEquipmentLink link)
        {
            if (link == null) 
                throw new System.ArgumentNullException(nameof(link));

            EMCo = link.EMCo;
            LinkId = link.LinkId;
            ServiceItemId = link.ServiceItemId;
            EquipmentId = link.EquipmentId;

            IsActive = link.IsActive;
            LastServiceDate = link.LastServiceDate;
            if (link.IsOverride)
            {
                DateIntervalTypeId = (DB.EMServiceDateIntervalEnum?)link.OverrideDateIntervalTypeId;
                DateInterval = link.OverrideDateInterval;
                OdoInterval = link.OverrideOdoInterval;
                HourInterval = link.OverrideHourInterval;
            }

            if (link.ServiceItem != null)
            {
                Description = link.ServiceItem.Description;

                DateIntervalTypeId ??= (DB.EMServiceDateIntervalEnum?)link.ServiceItem.DateIntervalTypeId;
                DateInterval ??= link.ServiceItem.DateInterval;
                OdoInterval ??= link.ServiceItem.OdoInterval;
                HourInterval ??= link.ServiceItem.HourInterval;
            }
        }

        [Key]
        [HiddenInput]
        public byte EMCo { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Link Id")]
        public int LinkId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Service Item Id")]
        public int? ServiceItemId { get; set; }


        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/Combo", ComboForeignKeys = "EMCo")]
        [Display(Name = "Equipment")]
        public string EquipmentId { get; set; }



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