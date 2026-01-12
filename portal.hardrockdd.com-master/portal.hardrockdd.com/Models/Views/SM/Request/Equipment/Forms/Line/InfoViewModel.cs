using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace portal.Models.Views.SM.Request.Equipment.Forms.Line
{
    public class InfoViewModel
    {
        public InfoViewModel()
        {

        }

        public InfoViewModel(DB.Infrastructure.ViewPointDB.Data.SMRequestLine requestLine)
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
            Description = requestLine.Description;
            RequestComments = requestLine.RequestComments;
            CompletedComments = requestLine.CompletedComments;
            IsEquipmentDisabled = requestLine.IsEquipmentDisabled ?? false;
            Status = requestLine.Status;
            ServiceStatus = requestLine.ServiceStatus;
            EstServiceDate = requestLine.EstServiceDate;
            ActServiceDate = requestLine.ActServiceDate;
            EstHours = requestLine.EstHours;
            AsignedEmployeeId = requestLine.AsignedEmployeeId;

            ShopGroupId = requestLine.ShopGroupId;
            ShopId = requestLine.ShopId;

            IsComplete = Status == DB.SMRequestLineStatusEnum.Completed || Status == DB.SMRequestLineStatusEnum.WorkOrderCompleted;
            AddToWorkOrder = requestLine.EMWorkOrderAdd ?? false;
            WorkOrderId = requestLine.WorkOrderId;
            WorkOrderItemId = requestLine.WOItemId;

            EMMeterType = requestLine.EMMeterType;
            EMHourDate = requestLine.EMHourDate;
            EMHourReading = requestLine.EMHourReading;
            EMOdoDate = requestLine.EMOdoDate;
            EMOdoReading = requestLine.EMOdoReading;
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

        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.SMServiceStatusIdEnum ServiceStatus { get; set; }

        //SMServiceStatusIdEnum

        [UIHint("TextBox")]
        [Display(Name = "Description")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string Description { get; set; }

        [UIHint("TextAreaBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Request Comments")]
        public string RequestComments { get; set; }

        [UIHint("TextAreaBox")]
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

        [UIHint("SwitchBox")]
        [Display(Name = "Completed")]
        public bool IsComplete { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Est Date")]
        public DateTime? EstServiceDate { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Actual Date")]
        public DateTime? ActServiceDate { get; set; }


        public int? ShopGroupId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/ShopCombo", ComboForeignKeys = "ShopGroupId")]
        [Display(Name = "Shop")]
        public string ShopId { get; set; }
        
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PRCombo/EmployeeCombo", ComboForeignKeys = "PRCo")]
        [Display(Name = "Assign To")]
        public int? AsignedEmployeeId { get; set; }


        [UIHint("SmallSwitchBox")]
        [Display(Name = "Add to WO")]
        public bool AddToWorkOrder { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Work Order")]
        public string WorkOrderId { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "WO Item")]
        public short? WorkOrderItemId { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Est Hours")]
        public decimal? EstHours { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "MeterType")]
        public DB.EMMeterTypeEnum EMMeterType { get; set; }


        [UIHint("LongBox")]
        [Display(Name = "Odometer")]
        public decimal? EMOdoReading { get; set; }

        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [Display(Name = "Odo Date")]
        public DateTime? EMOdoDate { get; set; }

        [UIHint("LongBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Hour Reading")]
        public decimal? EMHourReading { get; set; }

        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [Display(Name = "Hour Date")]
        public DateTime? EMHourDate { get; set; }

        internal InfoViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.SMRequestLines.FirstOrDefault(f => f.SMCo == SMCo && f.RequestId == RequestId && f.LineId == LineId);

            if (updObj != null)
            {

                updObj.AsignedEmployeeId = AsignedEmployeeId;
                updObj.EstHours = EstHours;
                updObj.ShopId = ShopId;
                updObj.EstServiceDate = EstServiceDate;
                updObj.CompletedComments = CompletedComments;
                updObj.ServiceStatus = ServiceStatus;


                switch (updObj.EMMeterType)
                {
                    case DB.EMMeterTypeEnum.Hours:
                        updObj.EMHourDate = EMHourDate;
                        updObj.EMHourReading = EMHourReading;
                        break;
                    case DB.EMMeterTypeEnum.Odometer:
                        updObj.EMOdoDate = EMOdoDate;
                        updObj.EMOdoReading = EMOdoReading;
                        break;
                    case DB.EMMeterTypeEnum.Both:
                        updObj.EMHourDate = EMHourDate;
                        updObj.EMHourReading = EMHourReading;
                        updObj.EMOdoDate = EMOdoDate;
                        updObj.EMOdoReading = EMOdoReading;
                        break;
                    case DB.EMMeterTypeEnum.None:
                        break;
                    default:
                        break;
                }

                try
                {
                    db.BulkSaveChanges();
                    return new InfoViewModel(updObj);
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