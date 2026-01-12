using System;
using System.Linq;

namespace DB.Infrastructure.ViewPointDB.Data
{

    public partial class APBatchLine
    {
        private VPContext _db;

        public VPContext db
        {
            set
            {
                _db = value;
            }
            get
            {
                if (_db == null)
                {
                    _db = VPContext.GetDbContextFromEntity(this);

                    if (_db == null)
                        _db = this.Header.db;

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }

        public string TaxCodeId
        {
            get
            {
                return tTaxCodeId;
            }
            set
            {
                if (tTaxCodeId != value)
                {
                    UpdateTaxCode(value);
                }
            }
        }

        private void UpdateTaxCode(string value)
        {
            if (!string.IsNullOrEmpty(tTaxCodeId) && TaxGroupId == null)
            {
                var taxCode = db.TaxCodes.FirstOrDefault(f => f.TaxCodeId == TaxCodeId && f.TaxGroupId == Header.Batch.Company.TaxGroupId);
                if (taxCode != null)
                {
                    tTaxCodeId = taxCode.TaxCodeId;
                    TaxGroupId = taxCode.TaxGroupId;
                    TaxCode = taxCode;
                }
                else
                {
                    tTaxCodeId = null;
                    TaxGroupId = null;
                    TaxCode = null;
                    TaxAmt = 0;
                }
            }
            if (value != tTaxCodeId)
            {
                var taxCode = db.TaxCodes.FirstOrDefault(f => f.TaxCodeId == value && f.TaxGroupId == Header.Batch.Company.TaxGroupId);
                if (taxCode != null)
                {
                    tTaxCodeId = taxCode.TaxCodeId;
                    TaxGroupId = taxCode.TaxGroupId;
                    TaxAmt = GrossAmt * (taxCode.NewRate ?? 0);
                    TaxCode = taxCode;
                }
                else
                {
                    tTaxCodeId = null;
                    TaxGroupId = null;
                    TaxAmt = 0;
                    TaxCode = null;
                }
            }
        }

        public byte? TaxTypeId
        {
            get
            {
                return tTaxTypeId;
            }
            set
            {
                if (tTaxTypeId != value)
                {
                    //UpdateTaxCode(value);

                    tTaxTypeId = value;

                    switch (TaxType)
                    {
                        case TaxTypeEnum.None:
                        default:
                            TaxCodeId = null;
                            TaxAmt = 0;
                            TaxBasis = 0;
                            break;
                        case TaxTypeEnum.Sales:
                        case TaxTypeEnum.Vat:
                        case TaxTypeEnum.Use:
                            TaxBasis = GrossAmt;
                            break;
                    }
                }
            }
        }

        public TaxTypeEnum? TaxType
        {
            get
            {
                return (TaxTypeEnum?)TaxTypeId;
            }
            set
            {
                if ((byte?)value != TaxTypeId)
                {
                    TaxTypeId = (byte?)value;
                }
            }
        }

        public string JobId
        {
            get
            {
                return tJobId;
            }
            set
            {
                if (value != tJobId)
                {
                    UpdateJobId(value);
                }
            }
        }

        private void UpdateJobId(string value)
        {
            if (Job == null && tJobId != null)
            {
                var job = db.Jobs.FirstOrDefault(f => f.JobId == tJobId);
                tJobId = job.JobId;
                JCCo = job.JCCo;
                PhaseGroupId = job.JCCompanyParm.HQCompanyParm.PhaseGroupId;
                Job = job;
            }

            if (tJobId != value)
            {
                var job = db.Jobs.FirstOrDefault(f => f.JobId == value);
                if (job != null)
                {
                    tJobId = job.JobId;
                    JCCo = job.JCCo;
                    PhaseGroupId = job.JCCompanyParm.HQCompanyParm.PhaseGroupId;
                    Job = job;
                }
                else
                {
                    tJobId = null;
                    JCCo = null;
                    PhaseGroupId = null;
                    Job = null;
                }

                PhaseId = null;
                JCCType = null;
                GLCo = (byte)Header.Company.GLCo;
                tGLAcct = null;
            }
        }

        public string PhaseId
        {
            get
            {
                return tPhaseId;
            }
            set
            {
                if (value != tPhaseId)
                {
                    UpdateJobPhaseId(value);
                }
            }
        }

        private void UpdateJobPhaseId(string newValue)
        {
            if (Job == null || string.IsNullOrEmpty(newValue))
            {
                tPhaseId = null;
                JobPhase = null;
                return;
            }

            if (JobPhase == null && tPhaseId != null)
            {
                var phase = Job.Phases.FirstOrDefault(f => f.PhaseId == tPhaseId);
                tPhaseId = phase.PhaseId;
                PhaseGroupId = phase.PhaseGroupId;
                JCCo = phase.JCCo;
                JobPhase = phase;
            }

            if (tPhaseId != newValue)
            {
                var phase = Job.Phases.FirstOrDefault(f => f.PhaseId == newValue);
                if (phase == null)
                    phase = Job.AddMasterPhase(newValue);

                if (phase != null)
                {
                    JCCo = phase.JCCo;
                    PhaseGroupId = phase.PhaseGroupId;
                    tPhaseId = phase.PhaseId;
                    JobPhase = phase;

                    if (phase.JobPhaseCosts.Count == 0)
                        phase = Job.AddMasterPhase(PhaseId, true);
                }
                else
                {
                    JCCo = null;
                    PhaseGroupId = null;
                    tPhaseId = null;
                    JobPhase = null;
                }
                JCCType = null;
                GLCo = (byte)Header.Company.GLCo;
                tGLAcct = null;
            }
        }

        public byte? JCCType
        {
            get
            {
                return tJCCType;
            }
            set
            {
                if (tJCCType != value)
                {
                    UpdateJCCType(value);
                }
            }

        }

        private void UpdateJCCType(byte? newValue)
        {
            if (JobPhase == null || newValue == null)
            {
                //JobPhaseCost = null;
                tJCCType = null;
                return;
            }

            if (JobPhaseCost == null && tJCCType != null)
            {
                var phaseCost = JobPhase.JobPhaseCosts.FirstOrDefault(f => f.CostTypeId == tJCCType);
                JobPhaseCost = phaseCost;
            }

            if (tJCCType != newValue)
            {
                var phaseCost = JobPhase.JobPhaseCosts.FirstOrDefault(f => f.CostTypeId == newValue);
                if (phaseCost == null) //Phase Cost is missing from Job Phase Cost List *Add It*
                    phaseCost = JobPhase.AddCostType((byte)newValue);

                if (phaseCost != null)
                {
                    tJCCType = phaseCost.CostTypeId;
                    JobPhaseCost = phaseCost;
                    UpdateGLAccount(GLAcct);
                }
            }
        }

        public string EquipmentId
        {
            get
            {
                return tEquipmentId;
            }
            set
            {
                if (value != tEquipmentId)
                {
                    UpdateEquipmentId(value);
                }
            }
        }

        private void UpdateEquipmentId(string newValue)
        {
            if (Equipment == null && tEquipmentId != null)
            {
                var equipment = db.Equipments.FirstOrDefault(f => f.EquipmentId == tEquipmentId);
                EMGroupId = equipment.EMCompanyParm.EMGroupId;
                tEquipmentId = equipment.EquipmentId;
                EMCo = equipment.EMCo;
                Equipment = equipment;
            }

            if (tEquipmentId != newValue)
            {
                var equipment = db.Equipments.FirstOrDefault(f => f.EquipmentId == newValue);
                if (equipment != null)
                {
                    EMGroupId = equipment.EMCompanyParm.EMGroupId;
                    tEquipmentId = equipment.EquipmentId;
                    EMCo = equipment.EMCo;
                    Equipment = equipment;
                }
                else
                {
                    tEquipmentId = null;
                    tEMCType = null;
                    EMGroupId = null;
                    EMCo = null;
                    Equipment = null;
                }
                tCostCodeId = null;
                tEMCType = null;
                GLCo = (byte)Header.Batch.Company.GLCo;
                tGLAcct = null;
            }
        }

        public string CostCodeId
        {
            get
            {
                return tCostCodeId;
            }
            set
            {
                if (value != tCostCodeId)
                {
                    UpdateEquipmentCostCodeId(value);
                }
            }
        }

        private void UpdateEquipmentCostCodeId(string newValue)
        {
            if (Equipment == null || string.IsNullOrEmpty(newValue))
            {
                tCostCodeId = null;
                tEMCType = null;
                return;
            }

            if (tCostCodeId != newValue)
            {
                EMGroupId = Equipment.EMCompanyParm.EMGroupId;
                tCostCodeId = newValue;
                tEMCType = null;
                GLCo = (byte)Header.Batch.Company.GLCo;
                tGLAcct = null;
            }
        }

        public byte? EMCType
        {
            get
            {
                return tEMCType;
            }
            set
            {
                if (tEMCType != value)
                {
                    UpdateEquipmentCostTypeInfo(value);
                }
            }

        }

        private void UpdateEquipmentCostTypeInfo(byte? newValue)
        {
            if (Equipment == null || newValue == null)
            {
                tEMCType = null;
                return;
            }

            if (tEMCType != newValue)
            {
                EMGroupId = Equipment.EMCompanyParm.EMGroupId;
                tEMCType = newValue;

                UpdateGLAccount(GLAcct);
            }
        }

        public byte LineTypeId
        {
            get
            {
                return tLineTypeId;
            }
            set
            {
                if (value != tLineTypeId)
                {
                    UpdateLineType(value);
                }
            }
        }

        public APLineTypeEnum LineType
        {
            get
            {
                return (APLineTypeEnum)this.LineTypeId;
            }
            set
            {
                LineTypeId = (byte)value;
            }
        }

        private void UpdateLineType(byte newValue)
        {
            tLineTypeId = newValue;
            switch (LineType)
            {
                case APLineTypeEnum.Expense:
                    tJobId = null;
                    tPhaseId = null;
                    PhaseGroupId = null;
                    tJCCType = null;
                    JCCo = null;
                    Job = null;
                    JobPhase = null;
                    JobPhaseCost = null;

                    EMCo = null;
                    tEquipmentId = null;
                    tCostCodeId = null;
                    tEMCType = null;
                    Equipment = null;

                    PO = null;
                    POItem = null;
                    PurchaseOrder = null;
                    PurchaseOrderItem = null;
                    break;
                case APLineTypeEnum.Equipment:
                    tJobId = null;
                    tPhaseId = null;
                    PhaseGroupId = null;
                    tJCCType = null;
                    Job = null;
                    JobPhase = null;
                    JobPhaseCost = null;

                    PO = null;
                    POItem = null;
                    PurchaseOrder = null;
                    PurchaseOrderItem = null;
                    break;
                case APLineTypeEnum.Job:
                    tEquipmentId = null;
                    tCostCodeId = null;
                    tEMCType = null;

                    Equipment = null;

                    PO = null;
                    POItem = null;
                    PurchaseOrder = null;
                    PurchaseOrderItem = null;
                    break;
                case APLineTypeEnum.PO:
                    tJobId = null;
                    tPhaseId = null;
                    PhaseGroupId = null;
                    tJCCType = null;
                    JCCo = null;
                    Job = null;
                    JobPhase = null;
                    JobPhaseCost = null;

                    EMCo = null;
                    tEquipmentId = null;
                    tCostCodeId = null;
                    tEMCType = null;
                    Equipment = null;
                    break;
                default:
                    break;
            }
            tGLAcct = null;
            GLCo = (byte)Header.Company.GLCo;
        }

        public POItemTypeEnum? POItemType
        {
            get
            {
                return (POItemTypeEnum?)POItemTypeId;
            }
            set
            {
                POItemTypeId = (byte?)value;
            }
        }

        public byte? POItemTypeId
        {
            get
            {
                return tPOItemTypeId;
            }
            set
            {
                if (value != tPOItemTypeId)
                {
                    UpdatePOItemType((POItemTypeEnum)(value ?? 0));
                }
            }
        }

        private void UpdatePOItemType(POItemTypeEnum newValue)
        {
            switch (newValue)
            {
                case POItemTypeEnum.Expense:
                    tJobId = null;
                    tPhaseId = null;
                    PhaseGroupId = null;
                    tJCCType = null;
                    JCCo = null;
                    Job = null;
                    JobPhase = null;
                    JobPhaseCost = null;

                    EMCo = null;
                    tEquipmentId = null;
                    tCostCodeId = null;
                    tEMCType = null;
                    Equipment = null;

                    break;
                case POItemTypeEnum.Equipment:
                    tJobId = null;
                    tPhaseId = null;
                    PhaseGroupId = null;
                    tJCCType = null;
                    Job = null;
                    JobPhase = null;
                    JobPhaseCost = null;

                    break;
                case POItemTypeEnum.Job:
                    tEquipmentId = null;
                    tCostCodeId = null;
                    tEMCType = null;
                    Equipment = null;
                    break;
                default:
                    break;
            }
            tGLAcct = null;
            GLCo = (byte)Header.Company.GLCo;

            tPOItemTypeId = (byte)newValue;
        }

        public string PO
        {
            get
            {
                return tPO;
            }
            set
            {
                if (LineType == APLineTypeEnum.PO)
                {
                    if (value != tPO)
                    {
                        UpdatePO(value);
                    }
                }
                else
                {
                    //UpdatePO(null);
                }
            }
        }

        public void UpdatePO(string value)
        {
            if (PurchaseOrder == null && !string.IsNullOrEmpty(tPO))
            {
                var po = db.PurchaseOrders.FirstOrDefault(f => f.POCo == APCo && f.PO == tPO);

                if (po != null)
                {
                    PurchaseOrder = po;
                    tPO = value;
                }
                else
                {
                    tPO = null;
                    PurchaseOrder = null;
                }

            }

            if (tPO != value)
            {
                var po = db.PurchaseOrders.FirstOrDefault(f => f.POCo == APCo && f.PO == value);
                if (po != null)
                {
                    PurchaseOrder = po;
                    tPO = value;
                }
                else
                {
                    tPO = null;
                    PurchaseOrder = null;
                }
            }
        }

        public short? POItem
        {
            get
            {
                return tPOItem;
            }
            set
            {
                if (LineType == APLineTypeEnum.PO)
                {
                    if (value != tPOItem)
                    {
                        UpdatePOItem(value);
                    }
                }
                else
                {
                    //UpdatePOItem(null);
                }
            }
        }

        public void UpdatePOItem(short? value)
        {
            if (PurchaseOrderItem == null && tPOItem != null)
            {
                var poItem = db.PurchaseOrderItems.FirstOrDefault(f => f.POCo == APCo && f.PO == tPO && f.POItemId == tPOItem);
                if (poItem != null)
                {
                    var poItemLine = db.vPOItemLines.FirstOrDefault(f => f.POITKeyID == poItem.KeyID);
                    Description = poItem.Description;
                    tPOItemTypeId = poItem.ItemTypeId;
                    POItemLine = poItemLine.POItemLine;
                    switch (POItemType)
                    {
                        case POItemTypeEnum.Expense:
                            break;
                        case POItemTypeEnum.Equipment:
                            tEquipmentId = poItem.EquipmentId;
                            tCostCodeId = poItem.CostCodeId;
                            tEMCType = poItem.EMCType;
                            break;
                        case POItemTypeEnum.Job:
                            tJobId = poItem.JobId;
                            tPhaseId = poItem.PhaseId;
                            tJCCType = poItem.JCCType;
                            break;
                        default:
                            break;
                    }

                    UnitCost = poItem.OrigUnitCost;
                    Units = poItem.OrigUnits;
                    GrossAmt = poItem.OrigCost;
                    TaxGroupId = poItem.TaxGroup;
                    tTaxCodeId = poItem.TaxCodeId;
                    TaxTypeId = poItem.TaxTypeId;
                    TaxAmt = poItem.OrigTax;
                    tGLAcct = poItem.GLAcct;

                    UM = poItem.UM;

                }
            }

            if (tPOItem != value)
            {
                var poItem = db.PurchaseOrderItems.FirstOrDefault(f => f.POCo == APCo && f.PO == tPO && f.POItemId == value);
                if (poItem != null)
                {
                    var poItemLine = db.vPOItemLines.FirstOrDefault(f => f.POITKeyID == poItem.KeyID);
                    Description = poItem.Description;
                    tPOItemTypeId = poItem.ItemTypeId;
                    POItemLine = poItemLine.POItemLine;
                    switch (POItemType)
                    {
                        case POItemTypeEnum.Expense:
                            break;
                        case POItemTypeEnum.Equipment:
                            tEquipmentId = poItem.EquipmentId;
                            tCostCodeId = poItem.CostCodeId;
                            tEMCType = poItem.EMCType;
                            break;
                        case POItemTypeEnum.Job:
                            tJobId = poItem.JobId;
                            tPhaseId = poItem.PhaseId;
                            tJCCType = poItem.JCCType;
                            break;
                        default:
                            break;
                    }
                    UnitCost = poItem.OrigUnitCost;
                    Units = poItem.OrigUnits;
                    GrossAmt = poItem.OrigCost;
                    TaxGroupId = poItem.TaxGroup;
                    tTaxCodeId = poItem.TaxCodeId;
                    TaxTypeId = poItem.TaxTypeId;
                    TaxAmt = poItem.OrigTax;
                    tGLAcct = poItem.GLAcct;
                    UM = poItem.UM;
                }
            }
        }

        public string GLAcct
        {
            get
            {
                return tGLAcct;
            }
            set
            {
                if (value != tGLAcct)
                {
                    UpdateGLAccount(value);
                }
            }
        }

        private void UpdateGLAccount(string newValue)
        {
            switch (LineType)
            {
                case APLineTypeEnum.Job:
                    GetJCGLAccount(db);
                    break;
                case APLineTypeEnum.Expense:
                    GLCo = (byte)Header.Company.GLCo;
                    tGLAcct = newValue;
                    break;
                case APLineTypeEnum.Equipment:
                    GetEMGLAccount(db);
                    break;
                case APLineTypeEnum.PO:
                    switch (POItemType)
                    {
                        case POItemTypeEnum.Expense:
                            GLCo = (byte)Header.Company.GLCo;
                            tGLAcct = newValue;
                            break;
                        case POItemTypeEnum.Equipment:
                            GetEMGLAccount(db);
                            break;
                        case POItemTypeEnum.Job:
                            GetJCGLAccount(db);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        private void GetEMGLAccount(VPContext db)
        {
            var glParm = new System.Data.Entity.Core.Objects.ObjectParameter("gltransacct", typeof(string));
            var glORParm = new System.Data.Entity.Core.Objects.ObjectParameter("GLOverride", typeof(string));
            var msgParm = new System.Data.Entity.Core.Objects.ObjectParameter("msg", typeof(string));
            var glErr = db.bspEMGlacctDflt(
                EMCo,
                EMGroupId,
                EMCType,
                CostCodeId,
                EquipmentId,
                glParm,
                glORParm,
                msgParm);
            if (glErr == -1)
            {
                GLCo = Equipment.EMCompanyParm.GLCo;
                tGLAcct = (string)glParm.Value;
            }
            else
            {
                GLCo = (byte)Header.Company.GLCo;
                tGLAcct = null;
            }
        }

        private void GetJCGLAccount(VPContext db)
        {
            var glParm = new System.Data.Entity.Core.Objects.ObjectParameter("glacct", typeof(string));
            var msgParm = new System.Data.Entity.Core.Objects.ObjectParameter("msg", typeof(string));
            var glErr = db.bspJCCAGlacctDflt(
                JCCo,
                JobId,
                PhaseGroupId,
                PhaseId,
                JCCType,
                "N",
                glParm,
                msgParm);
            if (glErr == -1)
            {
                if (glParm.Value is string)
                {
                    GLCo = Job.JCCompanyParm.GLCo;
                    tGLAcct = (string)glParm.Value;
                }
            }
            else
            {
                GLCo = (byte)Header.Company.GLCo;
                tGLAcct = null;
            }
        }
    }
}