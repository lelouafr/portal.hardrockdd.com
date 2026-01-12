using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace portal.Models.Views.Equipment.Service.Forms
{
    public class TriggerViewModel
    {
        public TriggerViewModel()
        {

        }

        public TriggerViewModel(DB.Infrastructure.ViewPointDB.Data.EMServiceItem service)
        {
            if (service == null) throw new System.ArgumentNullException(nameof(service));

            Co = service.EMCo;
            ServiceItemId = service.ServiceItemId;
            DateIntervalTypeId = (DB.EMServiceDateIntervalEnum ?)service.DateIntervalTypeId;
            DateInterval = service.DateInterval;
            OdoInterval = service.OdoInterval;
            HourInterval = service.HourInterval;
        }

        [Key]
        [HiddenInput]
        public byte Co { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "#")]
        public int ServiceItemId { get; set; }

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






    }
}