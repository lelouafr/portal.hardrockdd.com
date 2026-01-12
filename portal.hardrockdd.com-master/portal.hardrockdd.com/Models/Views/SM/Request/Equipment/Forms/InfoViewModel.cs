using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace portal.Models.Views.SM.Request.Equipment.Forms
{
    public class InfoViewModel: Views.Equipment.Forms.EquipmentFormViewModel
    {
        public InfoViewModel()
        {

        }

        public InfoViewModel(DB.Infrastructure.ViewPointDB.Data.Equipment equipment): base(equipment)
        {
            if (equipment == null)
                return;

            EquipmentName = equipment.Description;
        }


        [Display(Name = "Equipment")]
        [Field(LabelSize = 0, TextSize = 6)]
        [UIHint("TextBox")]
        public string EquipmentName { get; set; }


    }
}