using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.SM.Request
{
    public class MechanicDashboardViewModel
    {
        public MechanicDashboardViewModel()
        {
            WorkOrders = new List<MechanicWorkOrderViewModel>();
        }

        public MechanicDashboardViewModel(List<EMWorkOrderItem> items, string mechanicName)
        {
            MechanicName = mechanicName;
            WorkOrders = items.Select(i => new MechanicWorkOrderViewModel(i)).ToList();
        }

        public string MechanicName { get; set; }
        public List<MechanicWorkOrderViewModel> WorkOrders { get; set; }

        // Summary counts
        public int AssignedCount => WorkOrders.Count(w => !w.IsCompleted);
        public int InProgressCount => WorkOrders.Count(w => !w.IsCompleted && w.Status != "Open" && w.Status != "1");
        public int CompletedTodayCount => WorkOrders.Count(w => w.IsCompleted && w.DateCompleted?.Date == DateTime.Today);
    }

    public class MechanicWorkOrderViewModel
    {
        public MechanicWorkOrderViewModel()
        {
        }

        public MechanicWorkOrderViewModel(EMWorkOrderItem item)
        {
            if (item == null) return;

            // Keys
            EMCo = item.EMCo;
            WorkOrderId = item.WorkOrderId;
            WOItem = item.WOItem;

            // Work Order Info
            DateCreated = item.DateCreated;
            DateScheduled = item.DateSched;
            DateCompleted = item.DateCompl;
            Description = item.Description;
            Notes = item.Notes;
            
            // Equipment Info
            EquipmentId = item.EquipmentId;
            if (item.Equipment != null)
            {
                EquipmentDescription = item.Equipment.Description;
                EquipmentType = item.Equipment.Type;
            }

            // Status
            StatusCode = item.StatusCodeId;
            Status = item.StatusCode?.Description ?? item.StatusCodeId ?? "Open";
            IsCompleted = item.DateCompl != null;
            
            // Priority
            Priority = item.Priority ?? "N";
            PriorityDisplay = GetPriorityDisplay(Priority);

            // Service Request info (if linked)
            if (item.SMRequestLines != null && item.SMRequestLines.Any())
            {
                var srLine = item.SMRequestLines.First();
                ServiceRequestId = srLine.RequestId;
                RequestedBy = srLine.Request?.RequestUser?.PREmployee?.FullName;
                Location = srLine.AssignedLocation;
                IsEmergency = srLine.IsEmergancy ?? false;
            }
        }

        // Keys
        public byte EMCo { get; set; }
        
        [Display(Name = "Work Order")]
        public string WorkOrderId { get; set; }
        
        public short WOItem { get; set; }

        // Work Order Info
        [Display(Name = "Created")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Scheduled")]
        public DateTime? DateScheduled { get; set; }

        [Display(Name = "Completed")]
        public DateTime? DateCompleted { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Notes")]
        public string Notes { get; set; }

        // Equipment Info
        [Display(Name = "Equipment")]
        public string EquipmentId { get; set; }

        [Display(Name = "Equipment")]
        public string EquipmentDescription { get; set; }

        [Display(Name = "Type")]
        public string EquipmentType { get; set; }

        // Status
        public string StatusCode { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        public bool IsCompleted { get; set; }

        // Priority
        public string Priority { get; set; }

        [Display(Name = "Priority")]
        public string PriorityDisplay { get; set; }

        // Service Request Info
        public int? ServiceRequestId { get; set; }

        [Display(Name = "Requested By")]
        public string RequestedBy { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Emergency")]
        public bool IsEmergency { get; set; }

        // Helper
        private string GetPriorityDisplay(string priority)
        {
            return priority switch
            {
                "E" => "Emergency",
                "H" => "High",
                "L" => "Low",
                _ => "Normal"
            };
        }
    }
}
