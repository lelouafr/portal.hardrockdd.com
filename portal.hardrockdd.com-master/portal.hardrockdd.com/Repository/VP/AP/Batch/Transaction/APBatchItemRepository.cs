using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.AP
{
    public static  class APBatchItemRepository
    {
        public static APBatchLine ProcessUpdate(Models.Views.HQ.Batch.AP.APBatchLineViewModel model, VPContext db)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            if (db == null)
                throw new ArgumentNullException(nameof(db));

            var updObj = db.APBatchLines.FirstOrDefault(f => f.APCo == model.Co && f.Mth == model.MthDate && f.BatchId == model.BatchId && f.BatchSeq == model.BatchSeq && f.APLineId == model.APLineId);
            if (updObj != null)
            {
                if (model.CalcType == DB.POCalcTypeEnum.LumpSum)
                {
                    model.UM = "LS";
                    model.Units = 0;
                    model.UnitCost = 0;
                }
                else
                {
                    if (model.UM == "LS")
                        model.UM = null;
                    model.GrossAmt = model.Units * model.UnitCost;
                }

                /****Write the changes to object****/
                updObj.LineType = model.LineTypeId;
                if (!string.IsNullOrEmpty(model.Description))
                    updObj.Description = model.Description.Substring(0, model.Description.Length > 30 ? 29 : model.Description.Length);
                else
                    updObj.Description = null;

                switch (updObj.LineType)
                {
                    case DB.APLineTypeEnum.Job:
                        updObj.JobId = model.JobId;
                        updObj.PhaseId = model.PhaseId;
                        updObj.JCCType = model.JobCostTypeId;
                        break;
                    case DB.APLineTypeEnum.Expense:
                        updObj.GLAcct = model.GLAcct;
                        break;
                    case DB.APLineTypeEnum.Equipment:
                        updObj.EquipmentId = model.EquipmentId;
                        updObj.CostCodeId = model.CostCodeId;
                        updObj.EMCType = model.CostTypeId;
                        break;
                    case DB.APLineTypeEnum.PO:
                        updObj.PO = model.PO;
                        updObj.POItem = model.POItem;
                        break;
                    default:
                        break;
                }

                updObj.VendorGroup = updObj.Header.VendorGroupId;
                updObj.PayType = 1;
                updObj.MiscAmt = 0;
                updObj.Units = model.Units;
                updObj.UnitCost = model.UnitCost;
                updObj.GrossAmt = model.GrossAmt;
                updObj.UM = model.UM;
                updObj.TaxType = model.TaxTypeId;
                updObj.TaxAmt = model.TaxAmount;
                switch (updObj.TaxType)
                {
                    case DB.TaxTypeEnum.Sales:
                    case DB.TaxTypeEnum.Vat:
                    case DB.TaxTypeEnum.Use:
                        updObj.TaxCodeId = model.TaxCodeId;
                        break;
                    case DB.TaxTypeEnum.None:
                    default:
                        break;
                }
            }

            return updObj;
        }

    }
}