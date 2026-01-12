using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class CreditTransactionCode
    {

        public int? POItemTypeId
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

        public POItemTypeEnum POItemType
        {
            get
            {
                return (POItemTypeEnum)(this.tPOItemTypeId ?? 0);
            }
            set
            {
                POItemTypeId = (int)value;
            }
        }

        public byte? LineTypeId
        {
            get
            {
                return tLineTypeId;
            }
            set
            {
                if (value != tLineTypeId)
                {
                    UpdateLineType((CMCodeLineTypeEnum?)value);
                }
            }
        }

        public CMCodeLineTypeEnum? LineType
        {
            get
            {
                return (CMCodeLineTypeEnum?)(this.tLineTypeId);
            }
            set
            {
                LineTypeId = (byte?)value;
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
                    UpdateJobInfo(value);
                }
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
                    UpdateJobPhaseInfo(value);
                }
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
                    UpdateJobCostTypeInfo(value);
                }

                if (tJCCType != null && tGLAcct == null)
                {
                    UpdateGLAccountInfo(GLAcct);
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
                    UpdateEquipmentInfo(value);
                }
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
                    UpdateEquipmentCostCodeInfo(value);
                }
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
                if (tEMCType != null && tGLAcct == null)
                {
                    UpdateGLAccountInfo(GLAcct);
                }
            }

        }

        public string PO
        {
            get
            {
                return tPO;
            }
            set
            {
                if (value != tPO)
                {
                    UpdatePOInfo(value);
                }
            }
        }

        public short? POItemId
        {
            get
            {
                return tPOItemId;
            }
            set
            {
                if (value != tPOItemId)
                {
                    UpdatePOItemInfo(value);
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
                    UpdateGLAccountInfo(value);
                }
            }
        }

        public void UpdatePOInfo(string newValue)
        {
            var db = VPContext.GetDbContextFromEntity(this);
            var assignObject = true;
            if (db == null)
            {
                db = new VPContext();
                assignObject = false;
            }
            if (PurchaseOrder == null && tPO != null)
            {
                var po = db.PurchaseOrders.FirstOrDefault(f => f.PO == tPO);
                tPO = po.PO;
                POCo = po.POCo;
                if (assignObject)
                    PurchaseOrder = po;
            }

            if (tPO != newValue)
            {
                var po = db.PurchaseOrders.FirstOrDefault(f => f.PO == newValue);
                if (po != null)
                {
                    tPO = po.PO;
                    POCo = po.POCo;
                    if (assignObject)
                        PurchaseOrder = po;
                }
                else
                {
                    tPO = null;
                    POCo = null;
                    PurchaseOrder = null;
                }
            }
        }

        public void UpdatePOItemInfo(short? newValue)
        {
            var db = VPContext.GetDbContextFromEntity(this);
            var assignObject = true;
            if (db == null)
            {
                db = new VPContext();
                assignObject = false;
            }
            if (POItem == null && tPOItemId != null)
            {
                var poItem = db.PurchaseOrderItems.FirstOrDefault(f => f.PO == tPO && f.POItemId == tPOItemId);
                var purchaseOrderItemLine = db.vPOItemLines.FirstOrDefault(f => f.POCo == POCo && f.PO == tPO && f.POItemId == tPOItemId);
                POItemId = poItem.POItemId;
                POItemLine = purchaseOrderItemLine.POItemLine;
                if (assignObject)
                    POItem = poItem;
            }

            if (tPOItemId != newValue)
            {
                var poItem = db.PurchaseOrderItems.FirstOrDefault(f => f.PO == tPO && f.POItemId == newValue);
                if (poItem != null)
                {
                    var purchaseOrderItemLine = db.vPOItemLines.FirstOrDefault(f => f.POCo == poItem.POCo && f.PO == poItem.PO && f.POItemId == poItem.POItemId);
                    ;
                    POCo = poItem.POCo;
                    tPO = poItem.PO;
                    tPOItemId = poItem.POItemId;
                    POItemLine = purchaseOrderItemLine.POItemLine;
                    Description = poItem.Description;
                    tPOItemTypeId = poItem.ItemTypeId;

                    EMCo = poItem.EMCo;
                    tEquipmentId = poItem.EquipmentId;
                    tCostCodeId = poItem.CostCodeId;
                    tEMCType = poItem.EMCType;

                    JCCo = poItem.JCCo;
                    tJobId = poItem.JobId;
                    tPhaseId = poItem.PhaseId;
                    tJCCType = poItem.JCCType;

                    EMCo = poItem.EMCo;
                    WO = poItem.WO;
                    WOItem = poItem.WOItem;

                    GLCo = poItem.GLCo;
                    tGLAcct = poItem.GLAcct;

                    UM = poItem.UM;
                    UnitCost = poItem.OrigUnitCost;
                    Units = poItem.OrigUnits;
                    GrossAmt = poItem.OrigCost;
                    TaxCodeId = poItem.TaxCodeId;
                    TaxTypeId = poItem.TaxTypeId;
                    TaxAmount = poItem.OrigTax;
                    if (assignObject)
                    {
                        POItem = poItem;
                        Equipment = poItem.Equipment;
                        Job = poItem.Job;
                    }
                }
                else
                {
                    POItem = null;
                    tPOItemId = null;
                    POItemLine = null;
                    Description = null;
                    tPOItemTypeId = null;

                    EMCo = null;
                    tEquipmentId = null;
                    tCostCodeId = null;
                    tEMCType = null;
                    Equipment = null;

                    JCCo = null;
                    tJobId = null;
                    tPhaseId = null;
                    tJCCType = null;
                    Job = null;

                    EMCo = null;
                    WO = null;
                    WOItem = null;

                    GLCo = null;
                    tGLAcct = null;

                    UM = null;
                    UnitCost = null;
                    Units = null;
                    GrossAmt = null;
                    TaxCodeId = null;
                    TaxTypeId = null;
                    TaxAmount = null;
                }
            }
        }

        public void UpdateJobInfo(string newValue)
        {
            var db = VPContext.GetDbContextFromEntity(this);
            var assignObject = true;
            if (db == null)
            {
                db = new VPContext();
                assignObject = false;
            }
            if (Job == null && tJobId != null)
            {
                var job = db.Jobs.FirstOrDefault(f => f.JobId == tJobId);
                tJobId = job.JobId;
                JCCo = job.JCCo;
                PhaseGroupId = job.JCCompanyParm.HQCompanyParm.PhaseGroupId;
                if (assignObject)
                    Job = job;
            }

            if (tJobId != newValue)
            {
                var job = db.Jobs.FirstOrDefault(f => f.JobId == newValue);
                if (job != null)
                {
                    tJobId = job.JobId;
                    JCCo = job.JCCo;
                    PhaseGroupId = job.JCCompanyParm.HQCompanyParm.PhaseGroupId;
                    CrewId = job.CrewId;
                    if (assignObject)
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
                GLCo = null;
                tGLAcct = null;
            }
        }

        public void UpdateJobPhaseInfo(string newValue)
        {
            if (Job == null)
            {
                UpdateJobInfo(JobId);
            }
            if (Job == null || string.IsNullOrEmpty(newValue))
            {
                tPhaseId = null;
                JobPhase = null;
                return;
            }

            var db = VPContext.GetDbContextFromEntity(this);
            var assignObject = true;
            if (db == null)
            {
                db = new VPContext();
                assignObject = false;
            }
            if (JobPhase == null && tPhaseId != null)
            {
                var phase = Job.Phases.FirstOrDefault(f => f.PhaseId == tPhaseId);
                tPhaseId = phase.PhaseId;
                JCCo = phase.JCCo;
                PhaseId = phase.PhaseId;
                if (assignObject)
                    JobPhase = phase;
            }

            if (tPhaseId != newValue)
            {
                var phase = Job.Phases.FirstOrDefault(f => f.PhaseId == newValue);
                if (phase == null) //Phase is missing from Job Phase List *Add It*
                {
                    phase = Job.AddMasterPhase(newValue);
                }

                if (phase != null)
                {
                    JCCo = phase.JCCo;
                    PhaseGroupId = phase.PhaseGroupId;
                    tPhaseId = phase.PhaseId;
                    if (assignObject)
                        JobPhase = phase;

                    if (phase.JobPhaseCosts.Count == 0)
                    {
                        phase = Job.AddMasterPhase(PhaseId, true);
                    }
                }
                else
                {
                    JCCo = null;
                    PhaseGroupId = null;
                    tPhaseId = null;
                    JobPhase = null;
                }
                JCCType = null;
                GLCo = null;
                tGLAcct = null;
            }
        }

        public void UpdateJobCostTypeInfo(byte? newValue)
        {
            if (JobPhase == null)
            {
                UpdateJobPhaseInfo(PhaseId);
            }

            if (JobPhase == null || newValue == null)
            {
                JobPhaseCost = null;
                tJCCType = null;
                return;
            }

            var db = VPContext.GetDbContextFromEntity(this);
            var assignObject = true;
            if (db == null)
            {
                db = new VPContext();
                assignObject = false;
            }
            if (JobPhaseCost == null && tJCCType != null)
            {
                var phaseCost = JobPhase.JobPhaseCosts.FirstOrDefault(f => f.CostTypeId == tJCCType);
                if (assignObject)
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
                    if (assignObject)
                        JobPhaseCost = phaseCost;
                    else
                    {
                        tJCCType = null;
                    }
                    UpdateGLAccountInfo(GLAcct);
                }
            }
        }

        public void UpdateEquipmentInfo(string newValue)
        {
            var db = VPContext.GetDbContextFromEntity(this);
            var assignObject = true;
            if (db == null)
            {
                db = new VPContext();
                assignObject = false;
            }
            if (Equipment == null && tEquipmentId != null)
            {
                var equipment = db.Equipments.FirstOrDefault(f => f.EquipmentId == tEquipmentId);
                EMGroupId = equipment.EMCompanyParm.EMGroupId;
                tEquipmentId = equipment.EquipmentId;
                EMCo = equipment.EMCo;
                if (assignObject)
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
                    if (assignObject)
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
                GLCo = null;
                tGLAcct = null;
            }
        }

        public void UpdateEquipmentCostCodeInfo(string newValue)
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
                GLCo = null;
                tGLAcct = null;
            }
        }

        public void UpdateEquipmentCostTypeInfo(byte? newValue)
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
                UpdateGLAccountInfo(GLAcct);
            }

            if (tEMCType != null && tGLAcct == null)
            {
                UpdateGLAccountInfo(GLAcct);
            }
        }

        public void UpdatePOItemType(POItemTypeEnum newValue)
        {
            switch (newValue)
            {
                case POItemTypeEnum.Expense:
                    tJobId = null;
                    tPhaseId = null;
                    JCCType = null;
                    JCCo = null;
                    Job = null;

                    EMCo = null;
                    tEquipmentId = null;
                    tCostCodeId = null;
                    EMCType = null;

                    Equipment = null;

                    break;
                case POItemTypeEnum.Equipment:
                    tJobId = null;
                    tPhaseId = null;
                    JCCType = null;

                    Job = null;
                    break;
                case POItemTypeEnum.Job:
                    tEquipmentId = null;
                    tCostCodeId = null;
                    EMCType = null;

                    Equipment = null;
                    break;
                default:
                    break;
            }
            tGLAcct = null;
            GLCo = Transaction.Employee.GLCo;

            tPOItemTypeId = (byte)newValue;
        }

        public void UpdateLineType(CMCodeLineTypeEnum? newValue)
        {
            tGLAcct = null;
            switch (newValue)
            {
                case CMCodeLineTypeEnum.Expense:
                    tJobId = null;
                    tPhaseId = null;
                    JCCType = null;
                    JCCo = null;
                    Job = null;

                    EMCo = null;
                    tEquipmentId = null;
                    tCostCodeId = null;
                    EMCType = null;

                    Equipment = null;

                    GLAcct = Transaction.DefaultGLAcct();
                    break;
                case CMCodeLineTypeEnum.Equipment:
                    tJobId = null;
                    tPhaseId = null;
                    JCCType = null;

                    Job = null;

                    tCostCodeId = Transaction.DefaultEMCostCodeId();
                    EMCType = Transaction.DefaultEMCType();
                    break;
                case CMCodeLineTypeEnum.Job:
                    tEquipmentId = null;
                    tCostCodeId = null;
                    EMCType = null;

                    Equipment = null;

                    Job = Transaction.Employee.CurrentJob(Transaction.TransDate.Date);
                    JCCo = Job?.JCCo;
                    tJobId = Job?.JobId;
                    tPhaseId = Transaction.DefaultJCPhaseId();
                    JCCType = Transaction.DefaultJCCType();
                    break;
                default:
                    tJobId = null;
                    tPhaseId = null;
                    JCCType = null;
                    JCCo = null;
                    Job = null;

                    EMCo = null;
                    tEquipmentId = null;
                    tCostCodeId = null;
                    EMCType = null;

                    Equipment = null;

                    tPO = null;
                    tPOItemId = null;
                    tPOItemTypeId = null;
                    break;
            }
            GLCo = Transaction.Employee.GLCo;
            tLineTypeId = (byte?)newValue;
        }

        public void UpdateGLAccountInfo(string newValue)
        {
            var db = VPContext.GetDbContextFromEntity(this);
            if (db == null)
            {
                db = new VPContext();
            }
            var originalGLAcct = tGLAcct;
            //UpdateLineType(LineType);
            switch (LineType)
            {
                case CMCodeLineTypeEnum.Job:
                    if (Job != null)
                        GetJCGLAccount(db);
                    break;
                case CMCodeLineTypeEnum.Expense:
                    GLCo = Transaction.Employee.GLCo;
                    tGLAcct = newValue;
                    break;
                case CMCodeLineTypeEnum.Equipment:
                    if (Equipment != null)
                        GetEMGLAccount(db);
                    break;
                default:
                    break;
            }

            if (originalGLAcct != tGLAcct && tGLAcct != null)
            {
                if ((db.GetCurrentEmployee().EmployeeId == Transaction.EmployeeId ||
                                     db.GetCurrentEmployee().ReportsToId == Transaction.EmployeeId))
                {
                    Transaction.CodedStatusId = (int)CMTransCodeStatusEnum.EmployeeCoded;
                    if (Transaction.Status == CMTranStatusEnum.RequestedInfomation)
                        Transaction.Status = CMTranStatusEnum.Open;
                    Transaction.AddLog(CMLogEnum.EmployeeCoded, "Transaction was coded by Employee");
                    
                }
                else
                {
                    Transaction.CodedStatusId = (int)CMTransCodeStatusEnum.Coded;
                    if (Transaction.Status == CMTranStatusEnum.RequestedInfomation)
                        Transaction.Status = CMTranStatusEnum.Open;
                    Transaction.AddLog(CMLogEnum.Coded, "Transaction was coded by Employee");
                }
            }
            else if (originalGLAcct != tGLAcct && tGLAcct == null)
            {
                Transaction.CodedStatusId = (int)CMTransCodeStatusEnum.Empty;
                //Transaction.Logs.Add(Repository.VP.AP.CreditCard.CreditCardTransactionLogRepository.Init(Transaction, CMLogEnum.Coded));
            }
        }

        public void GetEMGLAccount(VPContext db)
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
                try
                {
                    GLCo = Equipment.EMCompanyParm.GLCo;
                    tGLAcct = (string)glParm.Value;
                }
                catch (Exception)
                {
                    GLCo = null;
                    tGLAcct = null;
                }
            }
            else
            {
            }
        }

        public void GetJCGLAccount(VPContext db)
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
                GLCo = null;
                tGLAcct = null;
            }
        }


    }
}