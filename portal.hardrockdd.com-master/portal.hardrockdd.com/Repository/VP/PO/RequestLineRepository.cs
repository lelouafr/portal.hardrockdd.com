using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.PO
{
    public static class RequestLineRepository 
    {
       
        public static PORequestLine Init(PORequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            //using var db = new VPContext();

            //var userId = StaticFunctions.GetUserId();
            //var usr = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            //var emp = usr.Employee.FirstOrDefault();
            var model = new PORequestLine
            {
                Request = request,
                LineId = request.Lines.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineId) + 1,
                POCo = request.POCo,
                RequestId = request.RequestId,
                TransTypeId = "A",
                ItemTypeId = (byte)DB.POItemTypeEnum.Job,
                ReqDate = request.OrderedDate,
                JobId = request.JobId,
                UM = "LS",
                Description = request.Description
            };

            return model;
        }

        public static PORequestLine Init(PORequestLine item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            var model = new PORequestLine
            {
                Request = item.Request,
                POCo = item.POCo,
                LineId = item.Request.Lines.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineId) + 1,
                RequestId = item.RequestId,
                TransTypeId = item.TransTypeId,
                ItemTypeId = item.ItemTypeId,
                ReqDate = item.ReqDate,
                JobId = item.JobId,
                PhaseGroupId = item.PhaseGroupId ?? item.Job?.JCCompanyParm?.HQCompanyParm?.PhaseGroupId,
                PhaseId = item.PhaseId,
                JCCType = item.JCCType,

                EquipmentId = item.EquipmentId,
                CostCodeId = item.CostCodeId,
                EMCType = item.EMCType
            };

            return model;
        }

        //public static int NextId(PORequestLine model)
        //{
        //    using var db = new VPContext();
        //    return db.PORequestLines
        //                    .Where(f => f.POCo == model.POCo && f.RequestId == model.RequestId)
        //                    .DefaultIfEmpty()
        //                    .Max(f => f == null ? 0 : f.LineId) + 1;
        //}

        public static Models.Views.Purchase.Request.PORequestLineViewModel ProcessUpdate(PORequestLine updObj, Models.Views.Purchase.Request.PORequestLineViewModel model)
        {
            //using var db = new VPContext();
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            if (updObj != null)
            {
                if (model.Description != null)
                    model.Description = model.Description.Substring(0, model.Description.Length > 60 ? 59 : model.Description.Length);

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

                    model.Cost = (model.Units ?? 0) * (model.UnitCost ?? 0);

                }

                if (model.TaxTypeId != DB.TaxTypeEnum.None)
                {
                    if (model.TaxCodeId != null)
                    {
                        if (updObj.TaxCodeId != model.TaxCodeId)
                        {
                            var taxCode = updObj.Request.POCompanyParm.HQCompanyParm.TaxGroup.HQTaxCodes.FirstOrDefault(f => f.TaxCodeId == model.TaxCodeId);
                            model.TaxAmount = model.Cost * taxCode.NewRate;
                            model.TaxRate = taxCode.NewRate;
                        }
                        else
                        {
                            model.TaxAmount = model.Cost * updObj.TaxCode.NewRate;
                            model.TaxRate = updObj.TaxCode.NewRate;
                        }
                    }
                    else
                    {
                        model.TaxAmount = null;
                        model.TaxRate = null;
                    }
                }
                else
                {
                    model.TaxRate = null;
                    model.TaxTypeId = null;
                    model.TaxAmount = null;
                    model.TaxCodeId = null;
                }

                /****Write the changes to object****/

                updObj.ItemTypeId = (byte)model.ItemTypeId;
                updObj.CrewId = model.CrewId;
                updObj.Description = model.Description;

                if (updObj.ItemType == DB.POItemTypeEnum.Job)
                {
                    updObj.JobId = model.JobId;
                    updObj.PhaseId = model.PhaseId;
                    updObj.JCCType = model.JobCostTypeId;
                }

                if (updObj.ItemType == DB.POItemTypeEnum.Equipment)
                {
                    updObj.EquipmentId = model.EquipmentId;
                    updObj.CostCodeId = model.CostCodeId;
                    updObj.EMCType = model.CostTypeId;
                }

                if (updObj.ItemType == DB.POItemTypeEnum.Expense)
                {
                    updObj.GLAcct = model.GLAcct;
                }

                updObj.Units = model.Units;
                updObj.UnitCost = model.UnitCost;
                updObj.Cost = model.Cost;
                updObj.UM = model.UM;

                updObj.TaxCodeId = model.TaxCodeId;
                updObj.TaxTypeId = (byte?)model.TaxTypeId;
                updObj.TaxRate = model.TaxRate;
                updObj.TaxAmount = model.TaxAmount;

            }
            return new Models.Views.Purchase.Request.PORequestLineViewModel(updObj);
        }

    }
}