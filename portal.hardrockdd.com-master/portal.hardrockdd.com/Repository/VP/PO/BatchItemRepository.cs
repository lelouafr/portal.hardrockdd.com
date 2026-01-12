using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.PO
{
    public partial class BatchItemRepository 
    {
       
       
        public static POBatchItem Init(POBatchHeader batch, PORequestLine line)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }
            if (line == null)
            {
                throw new ArgumentNullException(nameof(line));
            }
            var model = new POBatchItem
            {
                Co = batch.Co,
                Mth = batch.Mth,
                BatchId = batch.BatchId,
                BatchSeq = batch.BatchSeq,
                POItem = (short)line.LineId,
                BatchTransType = batch.BatchTransType,
                ItemType = (byte)line.ItemTypeId,
                MatlGroup = batch.Co,
                Description = line.Description,
                UM = line.UM,
                RecvYN = "N",
                PostToCo = batch.Co,
               
                GLCo = batch.Co,
                GLAcct = line.GLAcct,
                OrigUnits = line.Units ?? 0,
                OrigCost = line.Cost ?? 0,
                OrigUnitCost = line.UnitCost ?? 0,
                CrewId = line.CrewId ?? line.Request.OrderUser.CrewId,
                TaxGroup = line.TaxGroupId,
                TaxCode = line.TaxCodeId,
                TaxType = line.TaxTypeId,
                TaxRate = line.TaxRate ?? 0,
                OrigTax = line.TaxAmount ?? 0
            };

            if ((DB.POItemTypeEnum)model.ItemType == DB.POItemTypeEnum.Job)
            {
                model.JCCo = line.JCCo;
                model.JobId = line.JobId;
                model.PhaseGroupId = line.PhaseGroupId ?? line.Job?.JCCompanyParm?.HQCompanyParm?.PhaseGroupId;
                model.Phase = line.PhaseId;
                model.JCCType = line.JCCType;
                model.Job = line.Job;
                model.PostToCo = line.Job.JCCo;
            }
            else if ((DB.POItemTypeEnum)model.ItemType == DB.POItemTypeEnum.Equipment)
            {
                model.EMCo = line.EMCo;
                model.EquipmentId = line.EquipmentId;
                model.EMGroup = line.EMGroupId;
                model.CostCode = line.CostCodeId;
                model.EMCType = line.EMCType;
                model.Equipment = line.Equipment;
                model.PostToCo = line.Equipment.EMCo;
            }


            if (model.UM != "LS")
            {
                model.OrigECM = "E";
            }
            return model;
        }
       


    }
}