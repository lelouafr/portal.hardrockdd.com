using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace portal.Models.Views.EM.WorkOrder.Forms
{
    public class InfoViewModel
    {
        public InfoViewModel()
        {

        }

        public InfoViewModel(DB.Infrastructure.ViewPointDB.Data.EMWorkOrder workOrder)
        {
            if (workOrder == null)
                return;

            EMCo = workOrder.EMCo;
            WorkOrderId = workOrder.WorkOrderId;
            EquipmentId = workOrder.EquipmentId;
            ShopGroupId = workOrder.ShopGroupId;
            ShopId = workOrder.ShopId;
            Description = workOrder.Description;
            PRCo = workOrder.PRCo;
            MechanicID = workOrder.MechanicID;
            DateCreated = workOrder.DateCreated;
            DateDue = workOrder.DateDue;
            DateSched = workOrder.DateSched;
            Notes = workOrder.Notes;

            Complete = workOrder.Complete == "Y";
        }


        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Co")]
        public byte EMCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public string WorkOrderId { get; set; }


        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/Combo", ComboForeignKeys = "EMCo")]
        [Display(Name = "Equipment")]
        public string EquipmentId { get; set; }

        public byte ShopGroupId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/ShopCombo", ComboForeignKeys = "ShopGroupId")]
        [Display(Name = "Shop")]
        public string ShopId { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [HiddenInput]
        public byte? PRCo { get; set; } = 1;

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/MechanicCombo", ComboForeignKeys = "PRCo,ShopId")]
        [Display(Name = "Mechanic")]
        public int? MechanicID { get; set; }


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