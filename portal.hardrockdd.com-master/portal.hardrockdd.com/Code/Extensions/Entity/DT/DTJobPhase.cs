using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class DTJobPhase
    {
        public VPEntities db
        {
            get
            {

                var db = VPEntities.GetDbContextFromEntity(this);

                if (db == null && this.JobTicket?.DailyTicket != null)
                    db = this.JobTicket.DailyTicket.db;

                if (db == null)
                    throw new NullReferenceException("GetDbContextFromEntity is null");

                return db;
            }
        }

        public DateTime WorkDate
        {
            get
            {
                return tWorkDate;
            }
            set
            {
                if (value != tWorkDate)
                {
                    tWorkDate = value;
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
                    this.JobPhase = null;
                    UpdatePhase(this.PhaseId);
                    DTCostLines.ToList().ForEach(cost => {
                        cost.JCCo = job.JCCo;
                        cost.JobId = job.JobId;
                        cost.Job = job;
                    });

                }
                else
                {
                    tJobId = null;
                    JCCo = 1;
                    Job = null;
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
                    UpdatePhase(value);
                    UpdateProgressPhase();
                    UpdateProgressBudgetCode();
                    UpdatePassId();
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
                    PassId = 0;
                }
                ProgressPhaseId = null;
            }
        }

        public void UpdateProgressPhase()
        {
            if (JobPhase == null || Job == null || ProgressPhaseId != null)
                return;
            var dailyJobPhases = Job.DTJobPhases
                    .Where(f => f.JobPhase?.IsMilestone == "Y" && f.WorkDate <= WorkDate)
                    .OrderBy(o => o.WorkDate)
                    .ThenBy(o => o.TaskId)
                    .ToList();

            if (JobPhase.IsMilestone == "Y")
            {
                ProgressPhaseId = JobPhase.PhaseId;
                ProgressJobPhase = JobPhase;

                dailyJobPhases = Job.DTJobPhases
                    .Where(f => f.WorkDate <= WorkDate && f.ProgressPhaseId == null)
                    .OrderBy(o => o.WorkDate)
                    .ThenBy(o => o.TaskId)
                    .ToList();
                //Update any prior Daily Job Phases with missing progress Phase to current phase
                dailyJobPhases.Where(f => f.ProgressPhaseId == null).ToList().ForEach(dtJobPhase => {
                    dtJobPhase.ProgressPhaseId = JobPhase.PhaseId;
                    dtJobPhase.ProgressJobPhase = JobPhase;
                });
            }
            else
            {
                dailyJobPhases.Reverse();
                var jobPhase = dailyJobPhases.FirstOrDefault();

                if (jobPhase != null)
                {
                    ProgressPhaseId = jobPhase.PhaseId;
                    ProgressJobPhase = jobPhase.JobPhase;
                }
                else
                {
                    ProgressPhaseId = null;
                    ProgressJobPhase = null;
                }
            }
        }

        public void UpdatePassId()
        {
            if (ProgressPhaseId == null)
            {
                return;
            }
            var tasks = Job.DTJobPhases.Where(f => f.ProgressPhaseId == ProgressPhaseId).ToList();// && f.TaskId != TaskId
            var passId = 0;
            var bgtCode = "";

            foreach (var task in tasks)
            {
                var priorTask = tasks.FirstOrDefault(f => f.ProgressBudgetCodeId == task.ProgressBudgetCodeId);
                if (priorTask?.PassId != null && priorTask?.PassId != 0)
                {
                    task.PassId = priorTask.PassId;
                    passId = priorTask.PassId;
                    bgtCode = task.ProgressBudgetCodeId;
                }
                else
                {
                    if (string.IsNullOrEmpty(bgtCode))
                    {
                        passId = 1;
                        bgtCode = task.ProgressBudgetCodeId;
                    }
                    else if (bgtCode != task.ProgressBudgetCodeId)
                    {
                        if (!string.IsNullOrEmpty(bgtCode))
                            passId++;
                        bgtCode = task.ProgressBudgetCodeId;
                    }
                    task.PassId = passId;
                }
            }

        }

        public decimal? PassSize
        {
            get
            {
                return tPassSize;
            }
            set
            {
                if (value != tPassSize)
                {
                    UpdatePassSize(value);
                    UpdateProgressBudgetCode();
                    UpdatePassId();
                }
            }
        }

        public void UpdatePassSize(decimal? value)
        {
            if (value != tPassSize)
            {
                tPassSize = value;
                ProgressBudgetCodeId = null;
                ProgressBudgetCode = null;
            }
        }

        public void UpdateProgressBudgetCode()
        {
            if (Job == null)
                return;
            if (ProgressPhaseId != null && ProgressBudgetCodeId == null)
            {
                var prefix = "BC";
                var budgetCode = Job.JCCompanyParm.HQCompanyParm.PMCompanyParm.BudgetCodes.FirstOrDefault(f => f.Prefix == prefix && f.PhaseId == ProgressPhaseId && (f.Radius ?? 0) == (PassSize ?? 0));
                if (budgetCode == null && JobPhase.HasBudgetRadius() == false)
                {
                    budgetCode = Job.JCCompanyParm.HQCompanyParm.PMCompanyParm.BudgetCodes.FirstOrDefault(f => f.Prefix == prefix && f.PhaseId == ProgressPhaseId && (f.Radius ?? 0) == 0);
                }
                if (budgetCode == null)
                {
                    var srcBudgetCode = db.ProjectBudgetCodes.ToList().FirstOrDefault(f => f.Prefix == prefix && f.PhaseId == ProgressPhaseId && (f.Radius ?? 0) == (PassSize ?? 0));
                    if (srcBudgetCode != null)
                        budgetCode = Job.JCCompanyParm.HQCompanyParm.PMCompanyParm.AddBudgetCode(srcBudgetCode);
                }
                if (budgetCode != null)
                {
                    ProgressBudgetCodeId = budgetCode.BudgetCodeId;
                    PMCo = budgetCode.PMCo;
                    ProgressBudgetCode = budgetCode;

                    var dailyJobPhases = Job.DTJobPhases
                                .Where(f => f.WorkDate <= WorkDate && f.ProgressBudgetCodeId == null)
                                .OrderBy(o => o.WorkDate)
                                .ThenBy(o => o.TaskId)
                                .ToList();

                    //Update any prior Daily Job Phases with missing progress budget to current budget
                    dailyJobPhases.ForEach(dtJobPhase => {
                        dtJobPhase.ProgressBudgetCodeId = budgetCode.BudgetCodeId;
                        dtJobPhase.PMCo = budgetCode.PMCo;
                        dtJobPhase.ProgressBudgetCode = budgetCode;
                    });
                }
            }
        }

        public DTJobPhaseCost AddCostLine(byte costTypeId)
        {
            var line = DTCostLines.FirstOrDefault(f => f.tJCCType == costTypeId);
            if (line == null)
            {
                line = new DTJobPhaseCost()
                {
                    DTCo = DTCo,
                    TicketId = this.TicketId,
                    TaskId = TaskId,
                    LineId = DTCostLines.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineId) + 1,
                    JCCo = this.JCCo,
                    tJobId = this.JobId,
                    PhaseGroupId = this.PhaseGroupId,
                    tPhaseId = this.PhaseId,

                    Job = this.Job,
                    JobPhase = this.JobPhase,
                    DailyJobTask = this,
                };

                DTCostLines.Add(line);

                line.JCCType = costTypeId;
            }

            return line;
        }

        public void UpdateJobActualProgress()
        {
            if (PassId == 0)
            {
                PassId = 0;
                //return;
            }
            if (Job != null && !string.IsNullOrEmpty(ProgressPhaseId) && !string.IsNullOrEmpty(ProgressBudgetCodeId))
            {
                var jobProgress = Job.GetJobProgress(ProgressJobPhase, (byte)PassId);

                //Default values if null
                jobProgress.ActualStartDate ??= WorkDate;
                jobProgress.ActualDuration ??= 0;
                jobProgress.ActualUnits ??= 0;
                jobProgress.PercentCompletion ??= 0;
                jobProgress.RemainingUnits ??= 0;

                //Update the actual Values
                jobProgress.ActualDuration = this.Job.ActiveDTJobPhases.Where(f => f.ProgressPhaseId == this.ProgressPhaseId && f.ProgressBudgetCodeId == this.ProgressBudgetCodeId).DefaultIfEmpty().Sum(sum => sum == null ? 0 : sum.WeightedHours);
                jobProgress.ActualUnits = this.Job.ActiveDTJobPhases.Where(f => f.ProgressPhaseId == this.ProgressPhaseId && f.PhaseId == f.ProgressPhaseId && f.ProgressBudgetCodeId == this.ProgressBudgetCodeId).DefaultIfEmpty().Sum(sum => sum == null ? 0 : sum.Footage);

                //Calculate the percent and remaining 
                if ((Job.Footage ?? 0) != 0)
                {
                    var percentage = jobProgress.ActualUnits / Job.Footage;
                    var remainingUnits = Job.Footage - jobProgress.ActualUnits;

                    //Correct to not exceeed 100% or negative remaining units
                    percentage = percentage > 1 ? 1 : percentage;
                    remainingUnits = remainingUnits < 0 ? 0 : remainingUnits;

                    jobProgress.PercentCompletion = percentage;
                    jobProgress.RemainingUnits = remainingUnits;
                }

                //Set start date of actual
                jobProgress.ActualStartDate = jobProgress.ActualStartDate > WorkDate ? WorkDate : jobProgress.ActualStartDate;
            }
        }

        public decimal Hours
        {
            get
            {
                if (!DTCostLines.Where(f => f.JCCType == 1).Any())
                    return 0;

                return DTCostLines.Where(f => f.JCCType == 1).Sum(sum => (sum.PayrollValue ?? 0));
            }
        }

        public decimal WeightedHours
        {
            get
            {
                var dayTotalHours = Job.ActiveDTJobPhases.Where(f => f.WorkDate == WorkDate).DefaultIfEmpty().Sum(sum => sum == null ? 0 : sum.Hours);
                var taskValueWeight = dayTotalHours == 0 || Hours == 0 ? 0 : Hours / dayTotalHours;

                return taskValueWeight;
            }
        }

        public decimal Footage
        {
            get
            {
                if (!DTCostLines.Where(f => f.JCCType == 34).Any())
                    return 0;
                return DTCostLines.Where(f => f.JCCType == 34).Sum(sum => (sum.PayrollValue ?? 0));
            }
        }

        public DailyEquipmentUsage GetEquipmentUsage()
        {
            if (ProgressPhaseId == null)
            {
                return null;
            }
            var rig = this.JobTicket.Rig;
            var result = this.JobTicket.DailyTicket.EquipmentUsages.FirstOrDefault(f => PhaseGroupId == PhaseGroupId && f.JCPhaseId == this.ProgressPhaseId);

            if (result == null)
            {
                var jobTicket = this.JobTicket;
                var dailyTicket = this.JobTicket.DailyTicket;
                var revCode = rig.RevenueCodes.FirstOrDefault(f => f.RevCode == (rig.RevenueCodeId ?? "2"));

                result = new DailyEquipmentUsage
                {
                    DTCo = DTCo,
                    TicketId = TicketId,
                    SeqId = dailyTicket.EquipmentUsages.DefaultIfEmpty().Max(f => f == null ? 0 : f.SeqId) + 1,
                    ActualDate = WorkDate,
                    BatchTransType = "A",
                    EMTransType = "J",
                    EMCo = rig.EMCo,
                    EMGroupId = rig.EMGroupId,
                    EquipmentId = rig.EquipmentId,
                    RevCodeId = revCode?.RevCode,

                    PRCo = jobTicket.PRCo,
                    JCCo = jobTicket.Job.JCCo,
                    JobId = jobTicket.JobId,
                    PhaseGroupId = PhaseGroupId,
                    JCPhaseId = this.ProgressPhaseId,
                    JCCostType = rig.UsageCostType ?? rig.EMCompanyParm.udJCUsageCt,
                    Status = 0,
                    RevTimeUnits = 0,
                    RevWorkUnits = 0,
                    RevRate = revCode.Rate,
                    GLCo = Job.JCCompanyParm.GLCo,
                    PreviousHourMeter = rig.HourReading,
                    PreviousOdometer = rig.OdoReading,
                    CurrentHourMeter = rig.HourReading,
                    CurrentOdometer = rig.OdoReading,

                    DailyTicket = dailyTicket,
                    Equipment = rig,
                    Job = Job,
                    Company = Job.JCCompanyParm.HQCompanyParm,


                };



                if (result.JCCostType != null)
                    ProgressJobPhase.AddCostType((byte)result.JCCostType);

                result.GetEMGLAccount();

                dailyTicket.EquipmentUsages.Add(result);
                rig.EquipmentDailyUsage.Add(result);
            }



            return result;
        }

    }
}