using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class JobPhaseTrack
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
                    _db ??= this.Job.db;
                    _db ??= VPContext.GetDbContextFromEntity(this);
                }
                return _db;
            }
        }


        public List<DTJobPhase> DailyJobPhases
        {
            get
            {
                return this.Job.DTJobPhases.Where(f => f.JobTicket.DailyTicket.Status != DailyTicketStatusEnum.Canceled &&
                                                    f.JobTicket.DailyTicket.Status != DailyTicketStatusEnum.Deleted &&
                                                    f.JobTicket.DailyTicket.Status != DailyTicketStatusEnum.Report &&
                                                    f.JobTicket.DailyTicket.Status != DailyTicketStatusEnum.Draft &&
                                                    f.ProgressPhaseId == PhaseId &&
                                                    f.PassId == PassId).ToList();
            }
        }

        public decimal CalculatedActualHours
        {
            get
            {
                return DailyJobPhases.Sum(sum => sum.Hours);
            }
        }

        public decimal? PercentCompletion
        {
            get
            {
                return tPercentCompletion;
            }
            set
            {
                if (value != tPercentCompletion)
                {
                    if ((value ?? 0) >= 1)
                    {
                        Job.JobPhaseTracks.Where(f => f.SortId < SortId && (f.tPercentCompletion ?? 0) < 1).ToList().ForEach(e => e.tPercentCompletion = 1);
                    }

                    tPercentCompletion = value;
                }
            }
        }

        public string ActualBudgetCodeId
        {
            get
            {
                return tActualBudgetCodeId;
            }
            set
            {
                if (value != tActualBudgetCodeId)
                {
                    UpdateActualBudgetCode(value);
                }
            }
        }

        public void UpdateActualBudgetCode(string value)
        {
            if (Job == null)
                return;
            if (Job == null || string.IsNullOrEmpty(value))
            {
                tActualBudgetCodeId = null;
                ActualBudgetCode = null;
                return;
            }

            if (ActualBudgetCode == null && tActualBudgetCodeId != null)
            {
                var bgtCode = db.ProjectBudgetCodes.FirstOrDefault(f => f.PMCo == JCCo && f.BudgetCodeId == tActualBudgetCodeId);
                tActualBudgetCodeId = bgtCode.BudgetCodeId;
                ActualBudgetCode = bgtCode;
            }

            if (tActualBudgetCodeId != value)
            {
                var bgtCode = db.ProjectBudgetCodes.FirstOrDefault(f => f.PMCo == JCCo && f.BudgetCodeId == value);

                if (bgtCode != null)
                {
                    tActualBudgetCodeId = bgtCode.BudgetCodeId;
                    ActualBudgetCode = bgtCode;
                }
                else
                {
                    tActualBudgetCodeId = null;
                    ActualBudgetCode = null;
                }
            }
        }
    }
}