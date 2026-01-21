using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.SM.Request
{
    public class ForemanDashboardViewModel
    {
        public ForemanDashboardViewModel()
        {
            Requests = new List<ForemanRequestViewModel>();
        }

        public ForemanDashboardViewModel(List<SMRequestLine> lines)
        {
            Requests = lines.Select(l => new ForemanRequestViewModel(l)).ToList();
        }

        public List<ForemanRequestViewModel> Requests { get; set; }
        
        // Summary counts
        public int PendingCount => Requests.Count(r => r.Status == "Submitted" || r.Status == "Pending");
        public int InProgressCount => Requests.Count(r => r.Status == "In Process" || r.Status == "WO Created");
        public int CompletedTodayCount => Requests.Count(r => r.Status == "Completed" && r.RequestDate.Date == DateTime.Today);
    }

    public class ForemanRequestViewModel
    {
        public ForemanRequestViewModel()
        {
        }

        public ForemanRequestViewModel(SMRequestLine line)
        {
            if (line == null) return;

            // Keys
            SMCo = line.SMCo;
            RequestId = line.RequestId;
            LineId = line.LineId;
            EMCo = line.EMCo ?? 1;

            // Request Info
            RequestDate = line.Request?.RequestDate ?? DateTime.Now;
            RequestedBy = line.Request?.RequestUser?.PREmployee?.FullName ?? "Unknown";
            Status = line.Status.ToString().Replace("WorkOrder", "WO ");

            // Equipment Info
            EquipmentId = line.tEquipmentId;
            EquipmentDescription = line.Equipment?.Description;
            EquipmentType = line.Equipment?.Type;
            Location = line.AssignedLocation ?? line.Equipment?.LocationId;

            // Issue Info
            Description = line.RequestComments;
            IsEmergency = line.IsEmergancy ?? false;
            
            // Get custom data for Priority
            //var customData = SMCustomData.GetCustomData(line.SMCo, line.RequestId, line.LineId);
            //PriorityId = customData?.PriorityId ?? 2;
            //Priority = GetPriorityName(PriorityId);
            //RequestTypeId = customData?.RequestTypeId;
            //RequestType = GetRequestTypeName(RequestTypeId);

            // Work Order Info
            WorkOrderId = line.WorkOrderId;
            HasWorkOrder = !string.IsNullOrEmpty(line.WorkOrderId);

            // Assignment
            AssignedMechanicId = line.AsignedEmployeeId;
            if (line.AsignedEmployeeId != null)
            {
                using (var db = new VPContext())
                {
                    var emp = db.Employees.FirstOrDefault(e => e.EmployeeId == line.AsignedEmployeeId);
                    AssignedMechanicName = emp != null ? emp.FirstName + " " + emp.LastName : null;
                }
            }
        }

        // Keys
        [HiddenInput]
        public byte SMCo { get; set; }
        
        [HiddenInput]
        public int RequestId { get; set; }
        
        [HiddenInput]
        public int LineId { get; set; }
        
        [HiddenInput]
        public byte EMCo { get; set; }

        // Request Info
        [Display(Name = "Date")]
        public DateTime RequestDate { get; set; }

        [Display(Name = "Requested By")]
        public string RequestedBy { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        // Equipment Info
        [Display(Name = "Equipment")]
        public string EquipmentId { get; set; }

        [Display(Name = "Description")]
        public string EquipmentDescription { get; set; }

        [Display(Name = "Type")]
        public string EquipmentType { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        // Issue Info
        [Display(Name = "Issue")]
        public string Description { get; set; }

        [Display(Name = "Emergency")]
        public bool IsEmergency { get; set; }

        public byte? PriorityId { get; set; }

        [Display(Name = "Priority")]
        public string Priority { get; set; }

        public byte? RequestTypeId { get; set; }

        [Display(Name = "Type")]
        public string RequestType { get; set; }

        // Work Order Info
        [Display(Name = "Work Order")]
        public string WorkOrderId { get; set; }

        public bool HasWorkOrder { get; set; }

        // Assignment
        [Display(Name = "Mechanic")]
        public int? AssignedMechanicId { get; set; }

        [Display(Name = "Assigned To")]
        public string AssignedMechanicName { get; set; }

        // Helper methods
        private string GetPriorityName(byte? priorityId)
        {
            return priorityId switch
            {
                1 => "Low",
                2 => "Medium",
                3 => "High",
                4 => "Critical",
                _ => "Medium"
            };
        }

        private string GetRequestTypeName(byte? typeId)
        {
            return typeId switch
            {
                1 => "Routine",
                2 => "Inspection",
                3 => "Repair",
                4 => "Breakdown",
                _ => ""
            };
        }

        // For assigning mechanic
        public static List<SelectListItem> GetMechanicOptions(byte emco = 1)
        {
            using (var db = new VPContext())
            {
                var mechanics = db.Employees
                    .Where(e => e.PRCo == emco && e.ActiveYN == "Y")
                    .OrderBy(e => e.LastName)
                    .Take(100)
                    .ToList();

                var list = mechanics.Select(m => new SelectListItem
                {
                    Value = m.EmployeeId.ToString(),
                    Text = m.FullName
                }).ToList();

                list.Insert(0, new SelectListItem { Value = "", Text = "-- Select Mechanic --" });
                return list;
            }
        }
    }
}
