using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Purchase.Request;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.AP
{
    public static class APDocumentSeqLineRepository
    {
       
        public static portal.Models.Views.AP.Document.DocumentSeqLineViewModel ProcessUpdate(portal.Models.Views.AP.Document.DocumentSeqLineViewModel model, VPContext db)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            
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

                //Convert the current Amount to unit and unit cost
                if (model.Units == null)
                    model.Units ??= 1;

                if (model.UnitCost == null)
                    model.UnitCost ??= model.GrossAmt ?? 0;

                model.GrossAmt = model.Units * model.UnitCost;
            }

            var updObj = db.APDocumentLines.FirstOrDefault(f => f.APCo == model.APCo && f.DocId == model.DocId && f.SeqId == model.SeqId && f.LineId == model.LineId);

            if (updObj == null)
                return model;

            if (updObj != null)
            {
                if (updObj.LineType != model.LineTypeId)
                    updObj.LineType = model.LineTypeId;
                else { 
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
                            updObj.POItemId = model.POItemId;
                            break;
                        default:
                            break;
                    }
                    updObj.DivisionId = model.DivisionId;
                    updObj.UM = model.UM;
                    updObj.Units = model.Units;
                    updObj.UnitCost = model.UnitCost;
                    updObj.GrossAmt = model.GrossAmt;
                    updObj.TaxAmt = model.TaxAmount;
                    updObj.TaxType = model.TaxTypeId;
                    updObj.TaxCodeId = model.TaxCodeId;
                    updObj.PayType = 1;
                    updObj.MiscAmt = 0;
                    updObj.MatlGroup = model.APCo;
                    updObj.VendorGroupId = updObj.VendorGroupId;
                    updObj.Description = model.Description; 
                }
            }

            return new Models.Views.AP.Document.DocumentSeqLineViewModel(updObj);
        }

    }
}