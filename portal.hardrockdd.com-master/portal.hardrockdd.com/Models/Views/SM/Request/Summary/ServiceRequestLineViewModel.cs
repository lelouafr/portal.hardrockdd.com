using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.SM.Request.Summary
{
    public class ServiceRequestLineListViewModel
    {
        public ServiceRequestLineListViewModel()
        {

        }

        public ServiceRequestLineListViewModel(List<SMRequest> list)
        {
            var lines = list.SelectMany(s => s.Lines).ToList();
            List = lines.Select(s => new ServiceRequestLineViewModel(s)).ToList();
        }

        public List<ServiceRequestLineViewModel> List { get; }
    }

    public class ServiceRequestLineViewModel
    {
        public ServiceRequestLineViewModel()
        {
        }

        public ServiceRequestLineViewModel(DB.Infrastructure.ViewPointDB.Data.SMRequestLine line)
        {
            if (line == null)
                return;

            SMCo = line.SMCo;
            RequestId = line.Request.RequestId;
            LineId = line.LineId;

            RequestType = line.Request.RequestType;
            RequestDate = line.Request.RequestDate;
            RequestByName = line.Request.RequestUser.PREmployee.FullName;
            Comments = line.Request.Comments;

            Status = line.Status;
            EMCo = line.EMCo ?? 1;
            EquipmentId = line.tEquipmentId;
            EquipmentName = string.Format("{0}: {1}", line.Equipment?.EquipmentId, line.Equipment?.Description);
            RequestComments = line.RequestComments;

            EstServiceDate = line.EstServiceDate;
            WorkOrderId = line.WorkOrderId;
            WorkOrderItemId = line.WOItemId;

            HasAttachments = line.Attachment.Files.Any();
        }


        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Co")]
        public byte SMCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int RequestId { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Link Id")]
        public int LineId { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Request Date")]
        public DateTime RequestDate { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Type")]
        public DB.SMRequestTypeEnum RequestType { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Request By")]
        public string RequestByName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Notes")]
        public string Comments { get; set; }



        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.SMRequestLineStatusEnum Status { get; set; }

        public int EmployeeId { get; set; }

        [HiddenInput]
        public byte? EMCo { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/AssingedEquipmentCombo", ComboForeignKeys = "EMCo,EmployeeId")]
        [Display(Name = "Equipment")]
        public string EquipmentId { get; set; }



        [UIHint("TextBox")]
        [Display(Name = "Equipment")]
        public string EquipmentName { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Request Comments (Description of the Issue)")]
        public string RequestComments { get; set; }



        [UIHint("DateBox")]
        [Display(Name = "Est Date")]
        public DateTime? EstServiceDate { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Work Order")]
        public string WorkOrderId { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "WO Item")]
        public short? WorkOrderItemId { get; set; }


        public bool HasAttachments { get; set; }
    }
}