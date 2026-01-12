using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class EMWorkOrder
    {
        public static EMWorkOrder Init(SMRequestLine requestLine)
        {
            if (requestLine == null)
                return null;
            if (requestLine == null)
                return null;

            var result = new EMWorkOrder();
            result.Equipment = requestLine.Equipment;
            result.EMCompany = requestLine.Equipment.EMCompanyParm;

            result.EMCo = requestLine.Equipment.EMCo;
            result.PRCo = requestLine.Equipment.EMCompanyParm.PRCo;
            result.EquipmentId = requestLine.tEquipmentId;
            result.Notes = requestLine.RequestComments;
            result.DateCreated = DateTime.Now.Date;
            result.DateSched = requestLine.EstServiceDate;
            result.ShopGroupId = (byte)requestLine.Equipment.EMCompanyParm.HQCompanyParm.ShopGroupId;
            result.Complete = "N";

            using var db = Infrastructure.ViewPointDB.Data.VPContext.GetDbContextFromEntity(requestLine);
            result.WorkOrderId = NextWorkOrderSeq(db, requestLine.Equipment);

            return result;
        }

        private static string NextWorkOrderSeq(VPContext db, Equipment equipment)
        {
            var workOrderId = "";
            var workOrderIdParm = new ObjectParameter("wonumber", typeof(string));
            var msgParm = new ObjectParameter("msg", typeof(string));
            var returnValue = db.bspEMWOAutoSeq(equipment.EMCo, null, equipment.EMCompanyParm.WOAutoSeq, equipment.EMCompanyParm.WorkOrderOption, null, workOrderIdParm, msgParm);
            if (returnValue == -1)
            {
                workOrderId = (string)workOrderIdParm.Value;
            }

            return workOrderId;
        }
        public static EMWorkOrder Create(VPContext db, SMRequestLine requestLine)
        {
            if (db == null)
                return null;
            if (requestLine == null)
                return null;

            var workOrder = db.EMWorkOrders.FirstOrDefault(f => f.EMCo == requestLine.EMCo && f.EquipmentId == requestLine.tEquipmentId && f.Complete == "N");

            if (workOrder == null)
            {
                workOrder = Init(requestLine);
                db.EMWorkOrders.Add(workOrder);
            }

            return workOrder;
        }

    }
}