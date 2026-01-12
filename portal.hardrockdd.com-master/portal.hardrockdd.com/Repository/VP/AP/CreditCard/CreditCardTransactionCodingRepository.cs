using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;

namespace portal.Repository.VP.AP.CreditCard
{
    public static class CreditCardTransactionCodingRepository
    {
        public static CreditTransactionCode Init(CreditTransaction transaction)
        {
            if (transaction == null) throw new System.ArgumentNullException(nameof(transaction));

            var result = new CreditTransactionCode
            {
                CCCo = transaction.CCCo,
                TransId = transaction.TransId,
                SeqId = transaction.Coding.DefaultIfEmpty().Max(f => f == null ? 0 : f.SeqId) + 1,
                tLineTypeId = (byte)DB.APLineTypeEnum.Job,
                UM = "LS",
                Description = transaction.NewDescription ?? transaction.OrigDescription,
                //result.MatlGroup = transaction.Co;
                GLCo = transaction.CCCo,
                GrossAmt = transaction.TransAmt,
                //result.VendorGroup = transaction.VendorGroup;
                //result.PayType = 1;
                MiscAmt = 0,
                Transaction = transaction,
            };
            if (result.Description?.Length >= 60 && !string.IsNullOrEmpty(result.Description))
            {
                result.Description = result.Description.Substring(0, 59);
            }
            return result;
        }

        public static CreditTransactionCode Init(APTransLine line, CreditTransaction transaction)
        {
            if (line == null) throw new System.ArgumentNullException(nameof(line));
            if (transaction == null) throw new System.ArgumentNullException(nameof(transaction));

            var result = new CreditTransactionCode
            {
                CCCo = transaction.CCCo,
                TransId = transaction.TransId,
                SeqId = transaction.Coding.DefaultIfEmpty().Max(f => f == null ? 0 : f.SeqId) + 1,
                JCCo = line.JCCo,
                tJobId = line.JobId,
                PhaseGroupId = line.PhaseGroupId,
                tPhaseId = line.PhaseId,
                tJCCType = line.JCCType,
                WO = line.WO,
                WOItem = line.WOItem,
                EMCo = line.EMCo,
                tEquipmentId = line.EquipmentId,
                EMGroupId = line.EMGroupId,
                tCostCodeId = line.CostCodeId,
                tEMCType = line.EMCType,
                GLCo = line.GLCo,
                tGLAcct = line.GLAcct,
                Description = line.Description,
                UM = line.UM,
                Units = line.Units,
                UnitCost = line.UnitCost,
                GrossAmt = line.GrossAmt,
                MiscAmt = line.MiscAmt,
                tPO = line.PO,
                tPOItemId = line.POItem,
                POItemLine = line.POItemLine,
            };
            if (result.tJobId != null)
            {
                result.tLineTypeId = (int)DB.CMCodeLineTypeEnum.Job;
            }
            else if (result.Equipment != null)
            {
                result.tLineTypeId = (int)DB.CMCodeLineTypeEnum.Equipment;
            }
            else
            {
                result.tLineTypeId = (int)DB.CMCodeLineTypeEnum.Expense;
            }
            return result;
        }


        public static CreditTransactionCode Init(CreditTransactionCode line, CreditTransaction transaction)
        {
            if (line == null) throw new System.ArgumentNullException(nameof(line));
            if (transaction == null) throw new System.ArgumentNullException(nameof(transaction));

            var result = new CreditTransactionCode
            {
                CCCo = transaction.CCCo,
                TransId = transaction.TransId,
                SeqId = transaction.Coding.DefaultIfEmpty().Max(f => f == null ? 0 : f.SeqId) + 1,
                tLineTypeId= line.tLineTypeId,
                JCCo = line.JCCo,
                tJobId = line.JobId,
                PhaseGroupId = line.PhaseGroupId,
                tPhaseId = line.PhaseId,
                tJCCType = line.JCCType,
                WO = line.WO,
                WOItem = line.WOItem,
                EMCo = line.EMCo,
                tEquipmentId = line.EquipmentId,
                EMGroupId = line.EMGroupId,
                tCostCodeId = line.CostCodeId,
                tEMCType = line.EMCType,
                GLCo = line.GLCo,
                tGLAcct = line.GLAcct,
                Description = line.Description,
                UM = line.UM,
                Units = line.Units,
                UnitCost = line.UnitCost,
                GrossAmt = line.GrossAmt,
                MiscAmt = line.MiscAmt,
                tPO = line.PO,
                tPOItemId = line.tPOItemId,
                POItemLine = line.POItemLine,

                Transaction = transaction,
                Job = line.Job,
                Equipment = line.Equipment,
            };
            
            return result;
        }

        public static CreditTransactionCode ProcessUpdate(Models.Views.AP.CreditCard.Form.CodingInfoViewModel model, VPContext db)
        {
            if (model == null) throw new System.ArgumentNullException(nameof(model));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            var updObj = db.CreditTransactionCodes.FirstOrDefault(f => f.CCCo == model.CCCo && f.TransId == model.TransId && f.SeqId == model.SeqId);
            if (updObj != null)
            {
                /****Write the changes to object****/

                updObj.LineType = model.LineTypeId;
                updObj.Description = model.Description;

                if (updObj.LineType == DB.CMCodeLineTypeEnum.Job)
                {
                    updObj.JobId = model.JobId;
                    updObj.PhaseId = model.PhaseId;
                    updObj.JCCType = model.JobCostTypeId;
                }

                if (updObj.LineType == DB.CMCodeLineTypeEnum.Equipment)
                {
                    updObj.EquipmentId = model.EquipmentId;
                    updObj.CostCodeId = model.CostCodeId;
                    updObj.EMCType = model.CostTypeId;
                }

                if (updObj.LineType == DB.CMCodeLineTypeEnum.Expense)
                {
                    updObj.GLAcct = model.GLAcct;
                }

                updObj.GrossAmt = model.GrossAmt;
            }
            return updObj;
        }

        public static CreditTransactionCode ProcessUpdate2(Models.Views.AP.CreditCard.Form.CodingInfoViewModel model, VPContext db)
        {
            if (model == null) throw new System.ArgumentNullException(nameof(model));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            var updObj = db.CreditTransactionCodes.FirstOrDefault(f => f.CCCo == model.CCCo && f.TransId == model.TransId && f.SeqId == model.SeqId);
            if (updObj != null)
            {
                var lineType = (DB.APLineTypeEnum?)model.LineTypeId;
                var typeChange = (int?)model.LineTypeId != updObj.tLineTypeId;

                switch (lineType)
                {
                    case DB.APLineTypeEnum.Job:
                        model.EquipmentId = null;
                        model.CostCodeId = null;
                        model.CostTypeId = null;
                        model.PO = null;
                        model.POItem = null;
                        model.POItemTypeId = null;
                        if (model.PhaseId != updObj.tPhaseId)
                        {
                            model.JobCostTypeId = null;
                        }

                        break;
                    case DB.APLineTypeEnum.Expense:
                        model.EquipmentId = null;
                        model.CostCodeId = null;
                        model.CostTypeId = null;
                        model.JobId = null;
                        model.PhaseId = null;
                        model.JobCostTypeId = null;
                        model.PO = null;
                        model.POItem = null;
                        model.POItemTypeId = null;
                        //model.GLAcct ??= updObj.GLAcct;
                        break;
                    case DB.APLineTypeEnum.Equipment:
                        model.JobId = null;
                        model.PhaseId = null;
                        model.JobCostTypeId = null;
                        model.PO = null;
                        model.POItem = null;
                        model.POItemTypeId = null;

                        if (model.CostCodeId != updObj.tCostCodeId)
                        {
                            model.CostTypeId = null;
                        }
                        break;
                    case DB.APLineTypeEnum.PO:
                        if (model.PO != updObj.tPO)
                        {
                            model.POItem = null;
                            model.POItemTypeId = null;
                            model.GLAcct = null;
                            model.UnitCost = null;
                            model.Units = null;
                            model.GrossAmt = null;
                            model.TaxCodeId = null;
                            model.TaxTypeId = null;
                            model.TaxAmount = null;
                            model.EquipmentId = null;
                            model.CostCodeId = null;
                            model.CostTypeId = null;
                            model.JobId = null;
                            model.PhaseId = null;
                            model.JobCostTypeId = null;
                        }
                        if (model.PO != null && model.POItem != null && model.POItem != updObj.tPOItemId)
                        {
                            var PO = db.PurchaseOrderItems.FirstOrDefault(f => f.POCo == model.POCo && f.PO == model.PO && f.POItemId == model.POItem);

                            model.Description = PO.Description;
                            model.POItemTypeId = (DB.POItemTypeEnum)PO.ItemTypeId;
                            model.EquipmentId = PO.EquipmentId;
                            model.CostCodeId = PO.CostCodeId;
                            model.CostTypeId = PO.EMCType;


                            model.JobId = PO.JobId;
                            model.PhaseId = PO.PhaseId;
                            model.JobCostTypeId = PO.JCCType;
                            model.GLAcct = PO.GLAcct;
                            model.UM = PO.UM;
                            if (PO.UM == "LS")
                            {
                                model.CalcType = DB.POCalcTypeEnum.LumpSum;
                            }
                            else
                            {
                                model.CalcType = DB.POCalcTypeEnum.Units;
                            }
                            model.UnitCost = PO.OrigUnitCost;
                            model.Units = PO.OrigUnits;
                            model.GrossAmt = PO.OrigCost;
                            model.TaxCodeId = PO.TaxCodeId;
                            model.TaxTypeId = (DB.TaxTypeEnum?)PO.TaxTypeId;
                            model.TaxAmount = PO.OrigTax;

                            var poItemLine = db.vPOItemLines.FirstOrDefault(f => f.POITKeyID == PO.KeyID);
                            updObj.POItemLine = poItemLine.POItemLine;
                        }
                        if (model.CalcType == DB.POCalcTypeEnum.Units)
                        {
                            if (updObj.GrossAmt != model.GrossAmt)
                            {
                                model.UnitCost = model.GrossAmt / model.Units;
                            }
                        }
                        break;
                    default:
                        model.EquipmentId = null;
                        model.CostCodeId = null;
                        model.CostTypeId = null;
                        model.PO = null;
                        model.POItem = null;
                        model.POItemTypeId = null;
                        model.JobId = null;
                        model.PhaseId = null;
                        model.JobCostTypeId = null;
                        break;
                }
                if (typeChange)
                {
                    model.GLAcct = null;
                }
                if (model.CalcType == DB.POCalcTypeEnum.LumpSum)
                {
                    model.UM = "LS";
                    model.Units = 0;
                    model.UnitCost = 0;
                }
                else
                {
                    if (model.UM == "LS")
                    {
                        model.UM = null;
                    }
                    model.GrossAmt = (model.Units ?? 0) * (model.UnitCost ?? 0);

                }
                if (model.TaxTypeId != DB.TaxTypeEnum.None)
                {
                    if (model.TaxCodeId != null)
                    {
                        var taxCode = db.TaxCodes.FirstOrDefault(f => f.TaxGroupId == updObj.CCCo && f.TaxCodeId == model.TaxCodeId);
                        model.TaxAmount = model.GrossAmt * taxCode.NewRate;
                        //model.TaxRate = taxCode.NewRate;
                    }
                    else
                    {
                        model.TaxAmount = null;
                        //model.TaxRate = null;
                    }
                }
                else
                {
                    //model.TaxRate = null;
                    model.TaxTypeId = null;
                    model.TaxAmount = null;
                    model.TaxCodeId = null;
                }

                if (model.PhaseId != null && model.JobId != null)
                {
                    //JC.JobPhaseRepository.FindCreateJobPhase((byte)updObj.JCCo, model.JobId, (byte)updObj.PhaseGroup, model.PhaseId, db);
                }
                var glChanged = updObj.tGLAcct != model.GLAcct;
                /****Write the changes to object****/
                if (model.Description != null)
                    updObj.Description = model.Description.Substring(0, model.Description.Length > 30 ? 29 : model.Description.Length);
                else
                    updObj.Description = model.Description;

                updObj.tLineTypeId = (byte?)model.LineTypeId;

                updObj.tPO = model.PO;
                updObj.tPOItemId = model.POItem;
                updObj.POItemTypeId = (byte?)model.POItemTypeId;

                updObj.JCCo = model.JCCo;
                updObj.tJobId = model.JobId;

                updObj.PhaseGroupId = model.PhaseGroupId;
                updObj.tPhaseId = model.PhaseId;
                updObj.tJCCType = model.JobCostTypeId;
                //updObj.JCCostType = model.JobCostTypeId

                updObj.EMCo = model.EMCo;
                updObj.EMGroupId = model.EMGroupId;
                updObj.tEquipmentId = model.EquipmentId;
                updObj.tCostCodeId = model.CostCodeId;
                updObj.tEMCType = model.CostTypeId;

                //updObj.MatlGroup = model.Co;
                updObj.GLCo = model.GLCo;
                //updObj.VendorGroup = updObj.Header.VendorGroup;
                //updObj.PayType = 1;
                updObj.MiscAmt = 0;




                updObj.Units = model.Units;
                updObj.UnitCost = model.UnitCost;
                updObj.GrossAmt = model.GrossAmt;
                updObj.UM = model.UM;

                updObj.TaxCodeId = model.TaxCodeId;
                if (model.TaxTypeId == DB.TaxTypeEnum.None || model.TaxTypeId == null)
                {
                    updObj.TaxTypeId = null;
                    //updObj.TaxBasis = 0;
                }
                else
                {
                    updObj.TaxTypeId = (byte)model.TaxTypeId;
                    //updObj.TaxBasis = model.GrossAmt;
                }
                //updObj.TaxRate = model.TaxRate;
                updObj.TaxAmount = model.TaxAmount;

                if (updObj.tPhaseId != null && updObj.tJobId != null)
                {
                    //using var tmpDB = new VPContext();
                    //JC.JobPhaseRepository.FindCreateJobPhase((byte)updObj.JCCo, updObj.JobId, (byte)updObj.PhaseGroup, updObj.PhaseId, tmpDB);
                    //tmpDB.SaveChanges();
                    updObj.JobPhase = updObj.Job.AddMasterPhase(updObj.tPhaseId);
                    //JC.JobPhaseRepository.FindCreateJobPhase((byte)updObj.JCCo, updObj.JobId, (byte)updObj.PhaseGroup, updObj.PhaseId, db);
                }
                if (updObj.tLineTypeId == (byte)DB.APLineTypeEnum.Job && updObj.tJobId != null && updObj.PhaseGroupId != null && updObj.tPhaseId != null && updObj.tJCCType != null)
                {
                    var glParm = new ObjectParameter("glacct", typeof(string));
                    var msgParm = new ObjectParameter("msg", typeof(string));
                    var glErr = db.bspJCCAGlacctDflt(updObj.JCCo, updObj.tJobId, updObj.PhaseGroupId, updObj.tPhaseId, updObj.tJCCType, "N", glParm, msgParm);
                    if (glErr == -1)
                    {
                        try
                        {
                            updObj.tGLAcct = (string)glParm.Value;
                        }
                        catch (Exception)
                        {
                            updObj.tGLAcct = null;
                        }
                    }
                }
                else if (updObj.tLineTypeId == (byte)DB.APLineTypeEnum.Equipment && updObj.tEquipmentId != null && updObj.tEMCType != null && updObj.tCostCodeId != null)
                {
                    var glParm = new ObjectParameter("gltransacct", typeof(string));
                    var glORParm = new ObjectParameter("GLOverride", typeof(string));
                    var msgParm = new ObjectParameter("msg", typeof(string));
                    var glErr = db.bspEMGlacctDflt(updObj.EMCo, updObj.EMGroupId, updObj.tEMCType, updObj.tCostCodeId, updObj.tEquipmentId, glParm, glORParm, msgParm);
                    if (glErr == -1)
                    {
                        try
                        {
                            updObj.tGLAcct = (string)glParm.Value;
                        }
                        catch (Exception)
                        {
                            updObj.tGLAcct = null;
                        }
                    }
                }
                else if (updObj.tLineTypeId == (byte)DB.APLineTypeEnum.Expense)
                {
                    updObj.tGLAcct = model.GLAcct;
                }
                else if (updObj.tLineTypeId == (byte)DB.APLineTypeEnum.PO)
                {
                    updObj.tGLAcct = model.GLAcct;
                }
                else
                {
                    updObj.tGLAcct = null;
                }

                if (updObj.tGLAcct != null)
                {
                    if (glChanged && (StaticFunctions.GetCurrentEmployee().EmployeeId == updObj.Transaction.EmployeeId ||
                                      StaticFunctions.GetCurrentEmployee().ReportsToId == updObj.Transaction.EmployeeId))
                    {
                        updObj.Transaction.CodedStatusId = (int)DB.CMTransCodeStatusEnum.EmployeeCoded;
                        updObj.Transaction.Logs.Add(CreditCardTransactionLogRepository.Init(updObj.Transaction, DB.CMLogEnum.EmployeeCoded, "Transaction was coded by Employee"));
                    }
                    else if (glChanged)
                    {
                        updObj.Transaction.CodedStatusId = (int)DB.CMTransCodeStatusEnum.Coded;
                        updObj.Transaction.Logs.Add(CreditCardTransactionLogRepository.Init(updObj.Transaction, DB.CMLogEnum.Coded));
                    }
                    else
                    {
                        updObj.Transaction.CodedStatusId = (int)DB.CMTransCodeStatusEnum.Coded;
                        updObj.Transaction.Logs.Add(CreditCardTransactionLogRepository.Init(updObj.Transaction, DB.CMLogEnum.Coded));
                    }
                }
                else
                {
                    updObj.Transaction.CodedStatusId = (int)DB.CMTransCodeStatusEnum.Empty;
                }
            }

            return updObj;

        }
    }
}