using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class APTransLine
    {
        public APBatchLine ToBatchLine(APBatchHeader seq)
        {
            var line = new APBatchLine()
            {
                APCo = APCo,
                Mth = Mth,
                BatchId = seq.BatchId,
                BatchSeq = seq.BatchSeq,
                
                APLineId = APLineId,
                tLineTypeId = LineType,
                BatchTransType = "C",

                GLCo = GLCo,
                tGLAcct = GLAcct,
                Description = Description,
                UM = UM,
                Units = Units,
                UnitCost = UnitCost,
                VendorGroup = VendorGroupId,
                PayType = PayType,// (creditCardLine.tLineTypeId == 1 ? parms.JobPayType : parms.ExpPayType) ?? 1,
                GrossAmt = GrossAmt,
                MiscAmt = MiscAmt,
                MiscYN = "Y",

                Retainage = 0,
                Discount = 0,
                BurUnitCost = 0,
                SMChange = 0,
                PaidYN = "N",
                POPayTypeYN = "N",
                POItemLine = POItemLine,
                SubjToOnCostYN = "N",
                CCTransId = CCTransId,
                CCCodingSeqId = CCCodingSeqId,
                TransDate = TransDate,
                CCReference = CCReference,
                MerchGroup = MerchGroup,
                EmpNumber = EmpNumber,
                Vendor = Vendor,

                TaxGroupId = TaxGroup,
                tTaxCodeId = TaxCodeId,
                tTaxTypeId = TaxTypeId,
                TaxBasis = TaxBasis,
                TaxAmt = TaxAmt,
                ECM = ECM,
                JCCo = JCCo,
                tJobId = JobId,
                PhaseGroupId = PhaseGroupId,
                tPhaseId = PhaseId,
                tJCCType = JCCType,
                Job = Job,
                JobPhase = JobPhase,
                EMCo = EMCo,
                tEquipmentId = EquipmentId,
                EMGroupId = EMGroupId,
                tCostCodeId = CostCodeId,
                tEMCType = EMCType,
                WO = WO,
                WOItem = WOItem,



                OldGLCo = GLCo,
                OldGLAcct = GLAcct,
                OldDesc = Description,
                OldUM = UM,
                OldUnits = Units,
                OldUnitCost = UnitCost,
                OldVendorGroup = VendorGroupId,
                OldPayType = PayType,// (creditCardLine.tLineTypeId == 1 ? parms.JobPayType : parms.ExpPayType) ?? 1,
                OldGrossAmt = GrossAmt,
                OldMiscAmt = MiscAmt,
                OldMiscYN = "Y",

                OldRetainage = 0,
                OldDiscount = 0,
                OldBurUnitCost = 0,

                OldPOItemLine = POItemLine,
                OldSubjToOnCostYN = "N",                
                OldSupplier = SupplierId,

                OldTaxGroup = TaxGroup,
                OldTaxCode = TaxCodeId,
                OldTaxType = TaxTypeId,
                OldTaxBasis = TaxBasis,
                OldTaxAmt = TaxAmt,
                OldECM = ECM,
                OldJCCo = JCCo,
                OldJob = JobId,
                OldPhaseGroup = PhaseGroupId,
                OldPhase = PhaseId,
                OldJCCType = JCCType,

                OldEMCo = EMCo,
                OldEquip = EquipmentId,
                OldEMGroup = EMGroupId,
                OldCostCode = CostCodeId,
                OldEMCType = EMCType,
                OldWO = WO,
                OldWOItem = WOItem,
            };
            return line;
        }
    }
}