using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.SM.Request.Forms
{
    public class EquipmentLineListViewModel
    {
        public EquipmentLineListViewModel()
        {

        }

        public EquipmentLineListViewModel(DB.Infrastructure.ViewPointDB.Data.SMRequest request)
        {
            if (request == null) 
                throw new System.ArgumentNullException(nameof(request));

            SMCo = request.SMCo;
            RequestId = request.RequestId;

            List = request.Lines.Select(s => new EquipmentLineViewModel(s)).ToList();
        }

        [Key]
        [HiddenInput]
        public byte SMCo { get; set; }

        [Key]
        [HiddenInput]
        public int RequestId { get; set; }


        public List<EquipmentLineViewModel> List { get;  }
    }
    
    public class EquipmentLineViewModel
    {

        public EquipmentLineViewModel()
        {

        }

        public EquipmentLineViewModel(SMRequestLine line)
        {
            if (line == null) 
                throw new System.ArgumentNullException(nameof(line));
            
            EmployeeId = line.Request.RequestUser.PREmployee.EmployeeId;

            SMCo = line.SMCo;
            RequestId = line.RequestId;
            LineId = line.LineId;
            EMCo = line.EMCo ?? 1;
            EquipmentId = line.tEquipmentId;
            EquipmentName = line.Equipment?.Description;
            RequestComments = line.RequestComments;
            HasAttachments = line.Attachment.Files.Any();

            // Auto-fill from Equipment
            EquipmentType = line.Equipment?.Type;
            
            // Existing DB fields
            AssignedLocation = line.AssignedLocation;
            Mileage = line.EMOdoReading ?? line.Equipment?.OdoReading;
            Hours = line.EMHourReading ?? line.Equipment?.HourReading;
            IsEmergency = line.IsEmergancy ?? false;
            
            // Custom table fields - load using raw SQL helper
            var custom = SMCustomData.GetCustomData(line.SMCo, line.RequestId, line.LineId);
            if (custom != null)
            {
                PriorityId = custom.PriorityId ?? 2;
                MaintenanceRequestTypeId = custom.RequestTypeId;
            }
            else
            {
                PriorityId = 2; // Default to Medium
                MaintenanceRequestTypeId = null;
            }
        }

        [Key]
        [HiddenInput]
        public byte SMCo { get; set; }

        [Key]
        [HiddenInput]
        public int RequestId { get; set; }

        [Key]
        [HiddenInput]
        public int LineId { get; set; }

        [HiddenInput]
        public int EmployeeId { get; set; }

        [HiddenInput]
        public byte? EMCo { get; set; }

        // ========== EQUIPMENT ==========
        
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/AssingedEquipmentCombo", ComboForeignKeys = "EMCo,EmployeeId")]
        [Display(Name = "Equipment")]
        public string EquipmentId { get; set; }

        [Display(Name = "Name")]
        public string EquipmentName { get; set; }

        [Display(Name = "Type")]
        public string EquipmentType { get; set; }

        // ========== LOCATION & METERS ==========

        [UIHint("TextBox")]
        [Display(Name = "Location")]
        public string AssignedLocation { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Mileage")]
        public decimal? Mileage { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Hours")]
        public decimal? Hours { get; set; }

        // ========== REQUEST DETAILS (from custom table) ==========

        [UIHint("DropdownBox")]
        [Display(Name = "Request Type")]
        public byte? MaintenanceRequestTypeId { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Priority")]
        public int PriorityId { get; set; }

        [UIHint("TextArea")]
        [Display(Name = "Description of Issue")]
        public string RequestComments { get; set; }

        [UIHint("SwitchBox")]
        [Display(Name = "Emergency")]
        public bool IsEmergency { get; set; }

        public bool HasAttachments { get; set; }

        // ========== DROPDOWN OPTIONS ==========

        public static List<SelectListItem> GetPriorityOptions()
        {
            return SMCustomData.GetPriorityOptions();
        }

        public static List<SelectListItem> GetRequestTypeOptions()
        {
            return SMCustomData.GetRequestTypeOptions();
        }
    }
}
