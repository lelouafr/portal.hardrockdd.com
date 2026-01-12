using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class DTJobPhaseCost
    {
        private VPEntities db
        {
            get
            {

                var db = VPEntities.GetDbContextFromEntity(this);

                if (db == null && this.DailyJobTask != null)
                    db = this.DailyJobTask.db;

                if (db == null)
                    throw new NullReferenceException("GetDbContextFromEntity is null");

                return db;
            }
        }

        public decimal? PayrollValue
        {
            get
            {
                return tValueAdj ?? tValue;
            }
            set
            {
                if (value != PayrollValue)
                {
                    if (DailyJobTask.JobTicket.DailyTicket.Status == DB.DailyTicketStatusEnum.Approved ||
                        DailyJobTask.JobTicket.DailyTicket.Status == DB.DailyTicketStatusEnum.Processed)
                    {
                        tValueAdj = value;
                        tValue ??= value;
                    }
                    else
                    {
                        tValue = value;
                    }
                    DailyJobTask.UpdateJobActualProgress();
                }
            }
        }

        public decimal? Value
        {
            get
            {
                return tValue;
            }
            set
            {
                if (value != PayrollValue)
                {
                    if (DailyJobTask.JobTicket.DailyTicket.Status == DB.DailyTicketStatusEnum.Approved ||
                        DailyJobTask.JobTicket.DailyTicket.Status == DB.DailyTicketStatusEnum.Processed)
                    {
                        tValueAdj = value;
                        tValue ??= value;
                    }
                    else
                    {
                        tValue = value;
                    }
                    DailyJobTask.UpdateJobActualProgress();
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
                if (tJobId != value)
                {
                    UpdateJob(value);
                }
            }

        }

        public void UpdateJob(string value)
        {
            if (Job == null && tJobId != null)
            {
                var job = db.Jobs.FirstOrDefault(f => f.JobId == tJobId);
                tJobId = job.JobId;
                JCCo = job.JCCo;
                Job = job;
            }

            if (tJobId != value)
            {
                var job = db.Jobs.FirstOrDefault(f => f.JobId == value);
                if (job != null)
                {
                    tJobId = job.JobId;
                    JCCo = job.JCCo;
                    Job = job;
                }
                else
                {
                    tJobId = null;
                    JCCo = 1;
                    Job = null;
                }
                this.JobPhaseCost = null;
                this.UpdateCostType(this.JCCType);
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
                    UpdatePhase(value);
                }
            }
        }

        public void UpdatePhase(string value)
        {

            if (Job == null || string.IsNullOrEmpty(value))
            {
                tPhaseId = null;
                JobPhase = null;
                return;
            }

            if (JobPhase == null && tPhaseId != null)
            {
                var phase = Job.Phases.FirstOrDefault(f => f.PhaseId == tPhaseId);
                tPhaseId = phase.PhaseId;
                JCCo = phase.JCCo;
                PhaseId = phase.PhaseId;
                JobPhase = phase;
            }

            if (tPhaseId != value)
            {
                var phase = Job.Phases.FirstOrDefault(f => f.PhaseId == value);
                if (phase == null) //Phase is missing from Job Phase List *Add It*
                    phase = Job.AddMasterPhase(value);

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
                    JCCo = 1;
                    PhaseGroupId = 1;
                    tPhaseId = null;
                    JobPhase = null;
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
                    UpdateCostType(value);
                    UpdateBudgetCode();
                }
            }

        }

        private void UpdateCostType(byte? newValue)
        {
            if (JobPhase == null)
            {
                UpdatePhase(PhaseId);
            }

            if (JobPhase == null || newValue == null)
            {
                JobPhaseCost = null;
                tJCCType = null;
                return;
            }

            if (JobPhaseCost == null && tJCCType != null)
            {
                var phaseCost = JobPhase.JobPhaseCosts.FirstOrDefault(f => f.CostTypeId == tJCCType);
                if (phaseCost == null) //Phase Cost is missing from Job Phase Cost List *Add It*
                    phaseCost = JobPhase.AddCostType((byte)tJCCType);
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
                }
                else
                {
                    BudgetCodeId = null;
                    BudgetCode = null;
                }
            }
        }

        public void UpdateBudgetCode()
        {
            if (JobPhaseCost == null)
                return;

            if (PhaseId != null && BudgetCodeId == null)
            {
                var prefix = "";
                var description = "";
                string phaseId = null;
                ProjectBudgetCode bgtCode = null;
                if (JCCType == 1)
                {
                    if (DailyJobTask.JobTicket.Rig != null)
                    {
                        description = string.Format("Labor Per Man - {0}", DailyJobTask.JobTicket.Rig.Category.Description);
                    }
                    prefix = "LB";
                    bgtCode = Job.JCCompanyParm.HQCompanyParm.PMCompanyParm.BudgetCodes.FirstOrDefault(f => f.Prefix == prefix && f.PhaseId == phaseId && f.CostTypeId == JCCType && f.Description == description);
                }
                else if (JCCType == 34)
                {
                    phaseId = PhaseId;
                    prefix = "BC";
                    if ((DailyJobTask.PassSize ?? 0) > 0)
                    {
                        description = string.Format(AppCultureInfo.CInfo(), "{0} {1:F3}", DailyJobTask.JobPhase.Description, DailyJobTask.PassSize);
                        bgtCode = Job.JCCompanyParm.HQCompanyParm.PMCompanyParm.BudgetCodes.FirstOrDefault(f => f.Prefix == prefix && f.PhaseId == phaseId && f.CostTypeId == JCCType && f.Description == description);
                    }
                    if (bgtCode == null)
                    {
                        description = string.Format(AppCultureInfo.CInfo(), "{0}", DailyJobTask.JobPhase.Description);
                        bgtCode = Job.JCCompanyParm.HQCompanyParm.PMCompanyParm.BudgetCodes.FirstOrDefault(f => f.Prefix == prefix && f.PhaseId == phaseId && f.CostTypeId == JCCType && f.Description == description);
                    }
                }


                if (bgtCode != null)
                {
                    BudgetCodeId = bgtCode.BudgetCodeId;
                    PMCo = bgtCode.PMCo;
                    BudgetCode = bgtCode;
                }
            }
        }

        public void UpdateFromModel(Models.Views.DailyTicket.Job.DailyJobPhaseCostViewModel model)
        {
            if (model.Comments != null)
            {
                DailyJobTask.Comments = model.Comments;
            }
            else
            {
                PayrollValue = model.Value;
            }
            if (model.Radius != null)
            {
                DailyJobTask.PassSize = model.Radius;
            }
        }
    }
}