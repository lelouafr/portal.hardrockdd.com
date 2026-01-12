using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class EMWorkOrderItem
    {
        public static EMWorkOrderItem Init(EMWorkOrder workOrder, SMRequestLine requestLine)
        {
            if (workOrder == null)
                return null;
            if (requestLine == null)
                return null;

            var itemId = workOrder.Items.DefaultIfEmpty().Max(max => max == null ? (short)0 : max.WOItem);
            itemId++;
            var result = new EMWorkOrderItem();
            /**Assign Objects First**/
            result.WorkOrder = workOrder;
            result.EMCompany = workOrder.EMCompany;
            result.Equipment = requestLine.Equipment;
            result.Mechanic = requestLine.AssignedEmployee;
            result.CostCode = workOrder.EMCompany.LaborCostCode;
            result.StatusCode = workOrder.EMCompany.WOBeginStatusCode;
            result.SMRequestLines.Add(requestLine);

            /**Assign Properties**/
            result.EMCo = workOrder.EMCo;
            result.WorkOrderId = workOrder.WorkOrderId;
            result.WOItem = itemId;
            result.EquipmentId = workOrder.EquipmentId;
            result.Description = requestLine.Description;
            result.Notes = requestLine.RequestComments;
            result.DateCreated = DateTime.Now.Date;
            result.DateSched = requestLine.EstServiceDate;
            result.EMGroup = requestLine.Equipment.EMGroupId ?? workOrder.EMCompany.EMGroupId;
            result.InHseSubFlag = "I";
            result.RepairType = "1";
            result.CostCodeId = workOrder.EMCompany.LaborCostCode.CostCodeId;
            result.StatusCodeId = workOrder.EMCompany.WOBeginStat;

            result.PRCo = requestLine.PRCo ?? workOrder.EMCompany.PRCo;
            result.MechanicId = requestLine.AsignedEmployeeId;

            result.Priority = "N";

            return result;
        }

        public static EMWorkOrderItem Create(VPContext db, SMRequestLine requestLine, bool addToExisting = true)
        {
            if (db == null)
                return null;
            if (requestLine == null)
                return null;
            EMWorkOrder workOrder = null;

            if (addToExisting)
            {
                workOrder = db.EMWorkOrders.FirstOrDefault(f => f.EMCo == requestLine.EMCo && f.EquipmentId == requestLine.tEquipmentId && f.Complete == "N");
            }
            if (workOrder == null)
                workOrder = EMWorkOrder.Create(db, requestLine);

            if (string.IsNullOrEmpty(workOrder.WorkOrderId))
            {
                throw new Exception("Work Order Creation Failed");
            }
            var item = Init(workOrder, requestLine);
            workOrder.Items.Add(item);
            
            requestLine.WorkorderItem = item;
            requestLine.WorkOrderId = workOrder.WorkOrderId;
            requestLine.WOItemId = item.WOItem;
            requestLine.Status = SMRequestLineStatusEnum.WorkOrderCreated;
            return item;
        }

        public string StatusCodeId
        {
            get
            {
                return pStatusCodeId;
            }
            set
            {
                pStatusCodeId = value;
                if (StatusCode == null)
                {
                    StatusCode = EMCompany.WOStatusCodes.FirstOrDefault(f => f.StatusCodeId == StatusCodeId);
                }

                //Correct the Status Code Object to the new Status Code
                if (StatusCode.StatusCodeId != StatusCodeId)
                {
                    StatusCode = EMCompany.WOStatusCodes.FirstOrDefault(f => f.StatusCodeId == StatusCodeId);
                }

                if (!WorkOrder.Items.Any(f => f.StatusCode.StatusType == "N"))
                    WorkOrder.Complete = "Y";

                if (SMRequestLines.Any())
                {
                    var requestStatus = (SMRequestLineStatusEnum)SMRequestLines.Min(min => min == null ? 0 : min.StatusId);
                    if (StatusCode.StatusType == "N")
                    {
                        if (StatusCode.Description.ToLower().Contains("pending"))
                            requestStatus = SMRequestLineStatusEnum.WorkOrderCreated;

                        if (StatusCode.Description.ToLower().Contains("progress"))
                            requestStatus = SMRequestLineStatusEnum.WorkOrderInProcess;

                    }
                    if (StatusCode.StatusType == "F")
                    {

                        if (StatusCode.Description.ToLower().Contains("complete"))
                            requestStatus = SMRequestLineStatusEnum.WorkOrderCompleted;

                        if (StatusCode.Description.ToLower().Contains("canceled"))
                            requestStatus = SMRequestLineStatusEnum.WorkOrderCanceled;

                        SMRequestLines.ToList().ForEach(e => e.WorkFlow.CompleteWorkFlow());

                    }
                    SMRequestLines.ToList().ForEach(e => e.Status = requestStatus);

                }
            }

        }
    }
}