using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace portal.Models.Views.EM.WorkOrder.Item.Forms
{
    public class InfoViewModel
    {
        public InfoViewModel()
        {

        }

        public InfoViewModel(DB.Infrastructure.ViewPointDB.Data.EMWorkOrderItem item)
        {
            if (item == null)
                return;

            EMCo = item.EMCo;
            WorkOrderId = item.WorkOrderId;
            WorkOrderItemId = item.WOItem;
            EquipmentId = item.EquipmentId;

            Description = item.Description;
            PRCo = item.PRCo;
            MechanicId = item.MechanicId;
            DateCreated = item.DateCreated;
            DateDue = item.DateDue;
            DateSched = item.DateSched;
            Notes = item.Notes;
        }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Co")]
        public byte EMCo { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Id")]
        public string WorkOrderId { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "#")]
        public short WorkOrderItemId { get; set; }


        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/Combo", ComboForeignKeys = "Co=EMCo")]
        [Display(Name = "Equipment")]
        public string EquipmentId { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public byte? PRCo { get; set; }
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/ShopCombo", ComboForeignKeys = "ShopGroupId=ShopGroupId")]
        [Display(Name = "Mechanic")]
        public int? MechanicId { get; set; }


        [UIHint("DateBox")]
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }


        [UIHint("DateBox")]
        [Display(Name = "Date Due")]
        public DateTime? DateDue { get; set; }


        [UIHint("DateBox")]
        [Display(Name = "Date Sched")]
        public DateTime? DateSched { get; set; }

        [UIHint("TextAreaBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Notes")]
        public string Notes { get; set; }


        [UIHint("SwitchBox")]
        [Display(Name = "Complete")]
        public bool Complete { get; set; }

    }
}