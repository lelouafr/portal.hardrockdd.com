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
    public class InfoViewModel
    {
        public InfoViewModel()
        {

        }

        public InfoViewModel(DB.Infrastructure.ViewPointDB.Data.EMServiceItem service)
        {
            if (service == null) throw new System.ArgumentNullException(nameof(service));

            EMCo = service.EMCo;
            ServiceItemId = service.ServiceItemId;
            Description = service.Description;
            //ParentServiceItemId = service.ParentServiceItemId;
        }

        [Key]
        [HiddenInput]
        public byte EMCo { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "#")]
        public int ServiceItemId { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Parent")]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/EMCombo/ServiceParentCombo", ComboForeignKeys = "EMCo, ServiceItemId")]
        public int? ParentServiceItemId { get; set; }



    }
}