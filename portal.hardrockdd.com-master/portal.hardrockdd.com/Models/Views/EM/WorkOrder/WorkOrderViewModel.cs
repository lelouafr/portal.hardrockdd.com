using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.EM.WorkOrder
{
    public class WorkOrderListViewModel
    {
        public WorkOrderListViewModel()
        {

        }

        public WorkOrderListViewModel(List<EMWorkOrder> list)
        {
            List = list.Select(s => new WorkOrderViewModel(s)).ToList();
        }

        public WorkOrderListViewModel(DB.Infrastructure.ViewPointDB.Data.EMCompanyParm company)
        {
            if (company == null)
                return;

            List = company.EMWorkOrders.Select(s => new WorkOrderViewModel(s)).ToList();

        }

        public WorkOrderListViewModel(DB.Infrastructure.ViewPointDB.Data.Equipment equipment)
        {
            if (equipment == null)
                return;

            List = equipment.WorkOrders.Select(s => new WorkOrderViewModel(s)).ToList();

        }

        public List<WorkOrderViewModel> List { get; }
    }

    public class WorkOrderViewModel
    {
        public WorkOrderViewModel()
        {
            //RequestDate = DateTime.Now;
            //RequestByName = StaticFunctions.GetCurrentEmployee().FullName;
            //RequestType = SMRequestType.Equipment;
        }

        public WorkOrderViewModel(DB.Infrastructure.ViewPointDB.Data.EMWorkOrder workOrder)
        {
            if (workOrder == null)
                return;

            EMCo = workOrder.EMCo;
            WorkOrderId = workOrder.WorkOrderId;
            EquipmentId = workOrder.EquipmentId;
            EquipmentName = workOrder.Equipment.Description;
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
        [Display(Name = "WO #")]
        public string WorkOrderId { get; set; }


        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/Combo", ComboForeignKeys = "Co=EMCo")]
        [Display(Name = "Equipment")]
        public string EquipmentId { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "Equipment Name")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string EquipmentName { get; set; }

        public byte ShopGroupId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/ShopCombo", ComboForeignKeys = "ShopGroupId=ShopGroupId")]
        [Display(Name = "Shop")]
        public string ShopId { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public byte? PRCo { get; set; }
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/ShopCombo", ComboForeignKeys = "ShopGroupId=ShopGroupId")]
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