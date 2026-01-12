using System;
using System.Linq;


namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class PRBatchTimeEntry 
    {
        public static string BaseTableName { get { return "budPRBH"; } }

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
                    _db = Batch.db;
                    _db ??= VPContext.GetDbContextFromEntity(this);

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
		}

		public DateTime? OldPREndDate { get; set; }

		public byte? OldPRGroup { get; set; }

		public bool? IsInBatch { get; set; }

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

        public short EarnCodeId
        {
            get
            {
                return tEarnCodeId;
            }
            set
            {
                if (tEarnCodeId != value)
                {
                    UpdateEarnCode(value);
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

        public void UpdateEarnCode(short value)
        {
            if (EarnCode == null)
            {
                var earncode = db.EarnCodes.FirstOrDefault(f => f.EarnCodeId == tEarnCodeId);
                if (earncode != null)
                {
                    EarnCode = earncode;
                }
            }

            if (tEarnCodeId != value)
            {
                var earncode = db.EarnCodes.FirstOrDefault(f => f.EarnCodeId == value);
                if (earncode != null)
                {
                    EarnCode = earncode;
                    tEarnCodeId = value;
                }
                else
                {
                    EarnCode = null;
                }

            }
        }

        public void UpdateJobInfo(string newValue)
        {
            if (JCJob == null && tJobId != null)
            {
                var job = db.Jobs.FirstOrDefault(f => f.JobId == tJobId);
                tJobId = job.JobId;
                JCCo = job.JCCo;
                PhaseGroupId = job.JCCompanyParm.HQCompanyParm.PhaseGroupId;
                JCJob = job;
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
                    JCJob = job;
                }
                else
                {
                    tJobId = null;
                    JCCo = null;
                    PhaseGroupId = null;
                    JCJob = null;
                }

                PhaseId = null;
            }
        }

        public void UpdateJobPhaseInfo(string newValue)
        {
            if (JCJob == null)
            {
                UpdateJobInfo(JobId);
            }
            if (JCJob == null || string.IsNullOrEmpty(newValue))
            {
                tPhaseId = null;
                JCJobPhase = null;
                return;
            }

            if (JCJobPhase == null && tPhaseId != null)
            {
                var phase = JCJob.Phases.FirstOrDefault(f => f.PhaseId == tPhaseId);
                tPhaseId = phase.PhaseId;
                JCCo = phase.JCCo;
                PhaseId = phase.PhaseId;
                JCJobPhase = phase;
            }

            if (tPhaseId != newValue)
            {
                var phase = JCJob.Phases.FirstOrDefault(f => f.PhaseId == newValue);
                if (phase == null) //Phase is missing from Job Phase List *Add It*
                {
                    phase = JCJob.AddMasterPhase(newValue);
                }

                if (phase != null)
                {
                    JCCo = phase.JCCo;
                    PhaseGroupId = phase.PhaseGroupId;
                    tPhaseId = phase.PhaseId;
                    JCJobPhase = phase;

                    if (phase.JobPhaseCosts.Count == 0)
                    {
                        phase = JCJob.AddMasterPhase(PhaseId, true);
                    }
                }
                else
                {
                    JCCo = null;
                    PhaseGroupId = null;
                    tPhaseId = null;
                    JCJobPhase = null;
                }
            }
        }

        public void UpdateEquipmentInfo(string newValue)
        {
            if (EMEquipment == null && tEquipmentId != null)
            {
                var equipment = db.Equipments.FirstOrDefault(f => f.EquipmentId == tEquipmentId);
                EMGroupId = equipment.EMCompanyParm.EMGroupId;
                tEquipmentId = equipment.EquipmentId;
                EMCo = equipment.EMCo;
                EMEquipment = equipment;
            }

            if (tEquipmentId != newValue)
            {
                var equipment = db.Equipments.FirstOrDefault(f => f.EquipmentId == newValue);
                if (equipment != null)
                {
                    EMGroupId = equipment.EMCompanyParm.EMGroupId;
                    tEquipmentId = equipment.EquipmentId;
                    EMCo = equipment.EMCo;
                    EMEquipment = equipment;
                }
                else
                {
                    tEquipmentId = null;
                    EMGroupId = null;
                    EMCo = null;
                    EMEquipment = null;
                }
                tCostCodeId = null;
            }
        }

        public void UpdateEquipmentCostCodeInfo(string newValue)
        {
            if (EMEquipment == null || string.IsNullOrEmpty(newValue))
            {
                tCostCodeId = null;
                return;
            }

            if (tCostCodeId != newValue)
            {
                EMGroupId = EMEquipment.EMCompanyParm.EMGroupId;
                tCostCodeId = newValue;
            }
        }

    }



}