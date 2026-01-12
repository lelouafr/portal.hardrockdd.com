using System;
using System.Linq;

namespace portal.Code.Data.VP
{

    public partial class APBatchHeader
    {

        private VPEntities _db;

        public VPEntities db
        {
            set
            {
                _db = value;
            }
            get
            {
                if (_db == null)
                {
                    _db = VPEntities.GetDbContextFromEntity(this);

                    if (_db == null)
                        _db = this.Batch.db;

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }

        public string BaseTableName { get { return "APHB"; } }

        public HQAttachment Attachment
        {
            get
            {

                if (HQAttachment == null && UniqueAttchID != null)
                {
                    HQAttachment = new HQAttachment()
                    {
                        HQCo = this.APCo,
                        UniqueAttchID = (Guid)UniqueAttchID,
                        TableKeyId = this.KeyID,
                        TableName = this.BaseTableName,
                        HQCompanyParm = this.Company,
                    };
                    db.HQAttachments.Add(HQAttachment);
                    //Company.HQAttatchments
                }
                else if (HQAttachment == null && UniqueAttchID == null)
                {
                    HQAttachment = new HQAttachment()
                    {
                        HQCo = this.APCo,
                        UniqueAttchID = Guid.NewGuid(),
                        TableKeyId = this.KeyID,
                        TableName = this.BaseTableName,
                        HQCompanyParm = this.Company,
                    };
                    UniqueAttchID = HQAttachment.UniqueAttchID;
                    db.HQAttachments.Add(HQAttachment);
                    //Company.HQAttatchments.Add(HQAttachment);
                }

                return HQAttachment;
            }
        }

        public APBatchLine AddLine(APDocumentLine line)
        {
            var parms = Company.APCompanyParm;
            var seqLine = new APBatchLine
            {
                APCo = APCo,
                Mth = Mth,
                BatchId = BatchId,
                BatchSeq = BatchSeq,
                BatchTransType = BatchTransType,
                APLineId = (short)(Lines.DefaultIfEmpty().Max(f => f == null ? 0 : f.APLineId) + 1),
                tLineTypeId = line.LineTypeId,

                CompType = line.CompTypeId,
                Component = line.Component,
                INCo = line.INCo,
                Loc = line.Loc,
                MatlGroup = line.MatlGroup,
                Material = line.MaterialId,

                GLCo = line.GLCo ?? parms.GLCo,
                tGLAcct = line.GLAcct,

                Description = line.Description.Length >= 29 ? line.Description.Substring(0, 29) : line.Description,
                UM = line.UM,
                Units = line.Units ?? 0,
                UnitCost = line.UnitCost ?? 0,
                ECM = line.UM != "LS" ? "E" : line.ECM,
                VendorGroup = line.VendorGroupId,
                Supplier = line.SupplierId,
                PayType = (line.LineTypeId == 1 ? parms.JobPayType : parms.ExpPayType) ?? 1,
                GrossAmt = line.GrossAmt ?? 0,
                MiscAmt = line.MiscAmt ?? 0,
                MiscYN = "Y",

                Retainage = line.Retainage ?? 0,
                Discount = line.Discount ?? 0,
                BurUnitCost = line.BurUnitCost ?? 0,
                SMChange = line.SMChange ?? 0,
                PaidYN = "N",
                POPayTypeYN = "N",
                POItemLine = line.POItemLine,
                SubjToOnCostYN = "N",

                Header = this,

            };

            switch (line.TaxType)
            {
                default:
                case DB.TaxTypeEnum.None:
                    seqLine.TaxGroupId = parms.HQCompanyParm.TaxGroupId;
                    seqLine.tTaxCodeId = null;
                    seqLine.tTaxTypeId = null;
                    seqLine.TaxBasis = 0;
                    seqLine.TaxAmt = 0;

                    seqLine.TaxCode = null;
                    break;
                case DB.TaxTypeEnum.Sales:
                case DB.TaxTypeEnum.Vat:
                case DB.TaxTypeEnum.Use:
                    seqLine.TaxGroupId = line.TaxGroupId ?? parms.HQCompanyParm.TaxGroupId;
                    seqLine.tTaxCodeId = line.TaxCodeId;
                    seqLine.tTaxTypeId = line.TaxTypeId;
                    seqLine.TaxBasis = line.TaxBasis ?? 0;
                    seqLine.TaxAmt = line.TaxAmt ?? 0;

                    seqLine.TaxCode = line.TaxCode;
                    break;
            }

            switch (line.LineType)
            {
                case DB.APLineTypeEnum.Job:
                    seqLine.JCCo = line.JCCo ?? parms.JCCo;
                    seqLine.tJobId = line.JobId;
                    seqLine.PhaseGroupId = line.Job?.JCCompanyParm?.HQCompanyParm?.PhaseGroupId;
                    seqLine.tPhaseId = line.PhaseId;
                    seqLine.tJCCType = line.JCCType;

                    seqLine.Job = line.Job;
                    seqLine.JobPhase = line.JobPhase;
                    seqLine.JobPhaseCost = line.JobPhaseCost;
                    break;
                case DB.APLineTypeEnum.Expense:
                    break;
                case DB.APLineTypeEnum.Equipment:
                    seqLine.EMCo = line.EMCo ?? parms.EMCo;
                    seqLine.tEquipmentId = line.EquipmentId;
                    seqLine.EMGroupId = line.EMGroupId ?? parms.HQCompanyParm.EMGroupId;
                    seqLine.tCostCodeId = line.CostCodeId;
                    seqLine.tEMCType = line.EMCType;

                    seqLine.Equipment = line.Equipment;
                    seqLine.EMCostCode = line.EMCostCode;
                    seqLine.EMCType = line.EMCType;

                    seqLine.WO = line.WO;
                    seqLine.WOItem = line.WOItem;
                    break;
                case DB.APLineTypeEnum.PO:
                    seqLine.tPO = line.PO;
                    seqLine.tPOItem = line.POItemId;
                    seqLine.tPOItemTypeId = line.POItemTypeId;
                    switch (line.POItemType)
                    {
                        case DB.POItemTypeEnum.Job:
                            seqLine.JCCo = line.JCCo ?? parms.JCCo;
                            seqLine.tJobId = line.JobId;
                            seqLine.PhaseGroupId = line.Job?.JCCompanyParm?.HQCompanyParm?.PhaseGroupId;
                            seqLine.tPhaseId = line.PhaseId;
                            seqLine.tJCCType = line.JCCType;

                            seqLine.Job = line.Job;
                            seqLine.JobPhase = line.JobPhase;
                            seqLine.JobPhaseCost = line.JobPhaseCost;
                            break;
                        case DB.POItemTypeEnum.Expense:
                            break;
                        case DB.POItemTypeEnum.Equipment:
                            seqLine.EMCo = line.EMCo ?? parms.EMCo;
                            seqLine.tEquipmentId = line.EquipmentId;
                            seqLine.EMGroupId = line.EMGroupId ?? parms.HQCompanyParm.EMGroupId;
                            seqLine.tCostCodeId = line.CostCodeId;
                            seqLine.tEMCType = line.EMCType;

                            seqLine.Equipment = line.Equipment;
                            seqLine.EMCostCode = line.EMCostCode;
                            seqLine.EMCType = line.EMCType;

                            seqLine.WO = line.WO;
                            seqLine.WOItem = line.WOItem;
                            break;
                        default:
                            break;
                    }
                    seqLine.PurchaseOrder = line.PurchaseOrder;
                    seqLine.PurchaseOrderItem = line.POItem;
                    break;
                default:
                    break;
            }

            Lines.Add(seqLine);
            return seqLine;
        }

        public APBatchLine AddLine(CreditTransactionCode creditCardLine)
        {
            var seqExistingLine = db.APTransLines.FirstOrDefault(f => f.APCo == APCo && f.CCTransId == creditCardLine.TransId && f.CCCodingSeqId == creditCardLine.SeqId);
            if (seqExistingLine == null)
            {
                var parms = this.Company.APCompanyParm;
                var seqLine = new APBatchLine
                {
                    APCo = APCo,
                    Mth = Mth,
                    BatchId = BatchId,
                    BatchSeq = BatchSeq,
                    APLineId = (short)(Lines.DefaultIfEmpty().Max(f => f == null ? 0 : f.APLineId) + 1),
                    tLineTypeId = (byte)creditCardLine.tLineTypeId,
                    BatchTransType = "A",

                    GLCo = (creditCardLine.GLCo ?? parms.GLCo),
                    tGLAcct = creditCardLine.tGLAcct,
                    Description = creditCardLine.Transaction.NewDescription ?? creditCardLine.Transaction.OrigDescription,
                    UM = creditCardLine.UM,
                    Units = creditCardLine.Units ?? 0,
                    UnitCost = creditCardLine.UnitCost ?? 0,
                    VendorGroup = creditCardLine.Transaction.Merchant.VendGroupId,
                    PayType = (creditCardLine.tLineTypeId == 1 ? parms.JobPayType : parms.ExpPayType) ?? 1,
                    GrossAmt = creditCardLine.GrossAmt ?? 0,
                    MiscAmt = creditCardLine.MiscAmt ?? 0,
                    MiscYN = "Y",

                    Retainage = 0,
                    Discount = 0,
                    BurUnitCost = 0,
                    SMChange = 0,
                    PaidYN = "N",
                    POPayTypeYN = "N",
                    POItemLine = creditCardLine.POItemLine,
                    SubjToOnCostYN = "N",
                    CCTransId = creditCardLine.TransId,
                    CCCodingSeqId = creditCardLine.SeqId,
                    TransDate = creditCardLine.Transaction.TransDate,
                    //CCReference = creditCardLine.Transaction.UniqueTransId,
                    MerchGroup = creditCardLine.Transaction.Merchant.CategoryGroup,
                    EmpNumber = creditCardLine.Transaction.EmployeeId,
                    Vendor = creditCardLine.Transaction.Merchant.Name,

                };
                switch ((DB.TaxTypeEnum?)creditCardLine.TaxTypeId)
                {
                    default:
                    case DB.TaxTypeEnum.None:
                    case DB.TaxTypeEnum.Sales:
                    case DB.TaxTypeEnum.Vat:
                    case DB.TaxTypeEnum.Use:
                        seqLine.TaxGroupId = parms.HQCompanyParm.TaxGroupId;
                        seqLine.tTaxCodeId = null;
                        seqLine.tTaxTypeId = null;
                        seqLine.TaxBasis = 0;
                        seqLine.TaxAmt = 0;

                        seqLine.TaxCode = null;
                        break;
                        //case DB.TaxTypeEnum.Sales:
                        //case DB.TaxTypeEnum.Vat:
                        //case DB.TaxTypeEnum.Use:
                        //    seqLine.TaxGroupId = creditCardLine.TaxGroupId ?? parms.HQCompanyParm.TaxGroupId;
                        //    seqLine.tTaxCodeId = creditCardLine.TaxCodeId;
                        //    seqLine.tTaxTypeId = creditCardLine.TaxTypeId;
                        //    seqLine.TaxBasis = creditCardLine.TaxBasis ?? 0;
                        //    seqLine.TaxAmt = creditCardLine.TaxAmt ?? 0;

                        //    seqLine.TaxCode = creditCardLine.TaxCode;
                        //    break;
                }


                switch (creditCardLine.LineType)
                {
                    case DB.CMCodeLineTypeEnum.Job:
                        seqLine.JCCo = creditCardLine.JCCo ?? parms.JCCo;
                        seqLine.tJobId = creditCardLine.JobId;
                        seqLine.PhaseGroupId = creditCardLine.Job?.JCCompanyParm?.HQCompanyParm?.PhaseGroupId;
                        seqLine.tPhaseId = creditCardLine.PhaseId;
                        seqLine.tJCCType = creditCardLine.JCCType;

                        seqLine.Job = creditCardLine.Job;
                        seqLine.JobPhase = creditCardLine.JobPhase;
                        seqLine.JobPhaseCost = creditCardLine.JobPhaseCost;
                        break;
                    case DB.CMCodeLineTypeEnum.Expense:
                        break;
                    case DB.CMCodeLineTypeEnum.Equipment:
                        seqLine.EMCo = creditCardLine.EMCo ?? parms.EMCo;
                        seqLine.tEquipmentId = creditCardLine.EquipmentId;
                        seqLine.EMGroupId = creditCardLine.EMGroupId ?? parms.HQCompanyParm.EMGroupId;
                        seqLine.tCostCodeId = creditCardLine.CostCodeId;
                        seqLine.tEMCType = creditCardLine.EMCType;

                        seqLine.Equipment = creditCardLine.Equipment;
                        seqLine.EMCostCode = creditCardLine.EquipmentCostCode;
                        seqLine.EMCType = creditCardLine.EMCType;

                        seqLine.WO = creditCardLine.WO;
                        seqLine.WOItem = creditCardLine.WOItem;
                        break;
                    /*
                    case DB.CMCodeLineTypeEnum.PO:
                        seqLine.tPO = line.PO;
                        seqLine.tPOItem = line.POItemId;
                        seqLine.tPOItemTypeId = line.POItemTypeId;
                        switch (line.POItemType)
                        {
                            case DB.POItemTypeEnum.Job:
                                seqLine.JCCo = line.JCCo ?? parms.JCCo;
                                seqLine.tJobId = line.JobId;
                                seqLine.PhaseGroupId = line.Job?.JCCompanyParm?.HQCompanyParm?.PhaseGroupId;
                                seqLine.tPhaseId = line.PhaseId;
                                seqLine.tJCCType = line.JCCType;

                                seqLine.Job = line.Job;
                                seqLine.JobPhase = line.JobPhase;
                                seqLine.JobPhaseCost = line.JobPhaseCost;
                                break;
                            case DB.POItemTypeEnum.Expense:
                                break;
                            case DB.POItemTypeEnum.Equipment:
                                seqLine.EMCo = line.EMCo ?? parms.EMCo;
                                seqLine.tEquipmentId = line.EquipmentId;
                                seqLine.EMGroupId = line.EMGroupId ?? parms.HQCompanyParm.EMGroupId;
                                seqLine.tCostCodeId = line.CostCodeId;
                                seqLine.tEMCType = line.EMCType;

                                seqLine.Equipment = line.Equipment;
                                seqLine.EMCostCode = line.EMCostCode;
                                seqLine.EMCType = line.EMCType;

                                seqLine.WO = line.WO;
                                seqLine.WOItem = line.WOItem;
                                break;
                            default:
                                break;
                        }
                        seqLine.PurchaseOrder = line.PurchaseOrder;
                        seqLine.PurchaseOrderItem = line.POItem;
                        break;
                        */
                    default:
                        break;
                }

                if (seqLine.Description?.Length > 30)
                {
                    seqLine.Description = seqLine.Description.Substring(0, 29);
                }
                if (seqLine.UM != "LS")
                {
                    seqLine.ECM = "E";
                }
                Lines.Add(seqLine);
                return seqLine;
            }
            else
            {
                this.BatchTransType = "C";
                var seqLine = seqExistingLine.ToBatchLine(this);
                Lines.Add(seqLine);
                return seqLine;
            }
        }

    }
}