using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Areas.SM.Models.Request
{
    public class RequestLineListViewModel
    {
        public RequestLineListViewModel()
        {

        }

        public RequestLineListViewModel(DB.Infrastructure.ViewPointDB.Data.SMRequest request)
        {
            if (request == null)
                return;

            SMCo = request.SMCo;
            RequestId = request.RequestId;

            List = request.Lines.Select(s => new RequestLineViewModel(s)).ToList();
        }

        public RequestLineListViewModel(List<SMRequestLine> lines)
        {
            if (lines == null)
                return;

            List = lines.Select(s => new RequestLineViewModel(s)).ToList();
        }

        [Key]
        [HiddenInput]
        public byte? SMCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Request Id")]
        public int? RequestId { get; set; }

        public List<RequestLineViewModel>? List { get; }
    }

    public class RequestLineViewModel
    {
        public RequestLineViewModel()
        {

        }

        public RequestLineViewModel(SMRequestLine requestLine)
        {
            if (requestLine == null)
                return;

            SMCo = requestLine.SMCo;
            RequestId = requestLine.RequestId;
            LineId = requestLine.LineId;
            RequestType = requestLine.Request.RequestType;
            RequestDate = requestLine.Request.RequestDate;
            RequestByName = requestLine.Request.RequestUser.PREmployee.FullName;
            EMCo = requestLine.EMCo;
            EquipmentId = requestLine.tEquipmentId;
            EquipmentName = requestLine.Equipment?.Description;
            RequestComments = requestLine.RequestComments;
            CompletedComments = requestLine.CompletedComments;
            IsEquipmentDisabled = requestLine.IsEquipmentDisabled ?? false;
            Status = requestLine.Status;
            EstServiceDate = requestLine.EstServiceDate;

            IsComplete = Status == DB.SMRequestLineStatusEnum.Completed || Status == DB.SMRequestLineStatusEnum.WorkOrderCompleted;
            AddToWorkOrder = requestLine.EMWorkOrderAdd ?? false;
            WorkOrderId = requestLine.WorkOrderId;
            WorkOrderItemId = requestLine.WOItemId;

            //requestLine.Attachment



        }

        [Key]
        [HiddenInput]
        public byte SMCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Request Id")]
        public int RequestId { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Link Id")]
        public int LineId { get; set; }

        [HiddenInput]
        public byte? EMCo { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Equipment")]
        public string EquipmentId { get; set; }

        [Display(Name = "Equipment")]
        [UIHint("TextBox")]
        public string EquipmentName { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Type")]
        public DB.SMRequestTypeEnum RequestType { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.SMRequestLineStatusEnum Status { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Request Comments")]
        public string RequestComments { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Service Comments")]
        public string CompletedComments { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Request Date")]
        public DateTime RequestDate { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Request By")]
        public string RequestByName { get; set; }

        [UIHint("SwitchBox")]
        [Display(Name = "Disabled Equipment")]
        public bool IsEquipmentDisabled { get; set; }

        [UIHint("SmallSwitchBox")]
        [Display(Name = "Completed")]
        public bool IsComplete { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Est Date")]
        public DateTime? EstServiceDate { get; set; }

        [UIHint("SmallSwitchBox")]
        [Display(Name = "Add to WO")]
        public bool AddToWorkOrder { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Work Order")]
        public string WorkOrderId { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "WO Item")]
        public short? WorkOrderItemId { get; set; }



        internal RequestLineViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.SMRequestLines.FirstOrDefault(f => f.SMCo == this.SMCo && f.RequestId == this.RequestId && f.LineId == this.LineId);

            if (updObj != null)
            {
                updObj.EquipmentId = this.EquipmentId;
                updObj.RequestComments = this.RequestComments;
                updObj.CompletedComments = this.CompletedComments;
                updObj.IsEquipmentDisabled = this.IsEquipmentDisabled;
                updObj.Status = this.Status;
                updObj.EstServiceDate = this.EstServiceDate;

                try
                {
                    db.BulkSaveChanges();
                    return new RequestLineViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            else
            {
                modelState.AddModelError("", "Object Doesn't Exist For Update!");
                return this;
            }
        }
    }
}