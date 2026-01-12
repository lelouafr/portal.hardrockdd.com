using portal.Code.Data.VP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class JCCompanyParm
    {
        public VPEntities db
        {
            get
            {

                var db = VPEntities.GetDbContextFromEntity(this);

                if (db == null && this.HQCompanyParm != null)
                    db = this.HQCompanyParm.db;

                if (db == null)
                    throw new NullReferenceException("GetDbContextFromEntity is null");

                return db;
            }
        }

        public Job TemplateJob
        {
            get
            {
                return db.Jobs.FirstOrDefault(f => f.JobId == this.JobId);
            }
        }

        public JCContract TemplateContract
        {
            get
            {
                return db.JCContracts.FirstOrDefault(f => f.ContractId == this.ContractId);
            }
        }
    }

    public partial class Job
    {
        public VPEntities db
        {
            get
            {

                var db = VPEntities.GetDbContextFromEntity(this);

                if (db == null && this.JCCompanyParm != null)
                    db = this.JCCompanyParm.db;

                if (db == null)
                    throw new NullReferenceException("GetDbContextFromEntity is null");

                return db;
            }
        }
        private bool? _IsRock;
        public bool IsRock 
        { 
            get
            {
                if (_IsRock == null)
                {
                    _IsRock = DailyFieldTickets.Any(f => f.MudMotor == true);
                }

                return (bool)_IsRock;
            } 
        }

        public DB.JCJobStatusEnum Status
        { 
            get
            {
                return (DB.JCJobStatusEnum)(int.TryParse(StatusId, out int outcurrentStatus) ? outcurrentStatus : 0);
            }
            set
            {
                StatusId = ((int)value).ToString(AppCultureInfo.CInfo());
            } 
        }

        private BidBoreLine _BidBoreLine;
        public BidBoreLine BidBoreLine
        {
            get
            {
                if(_BidBoreLine == null)
                {
                    _BidBoreLine = BidBoreLines.LastOrDefault();
                }

                return _BidBoreLine;
            }
        }

        private string _DisplayName;
        public string DisplayName 
        { 
            get
            {
                if (string.IsNullOrEmpty(_DisplayName))
                {
                    _DisplayName = string.Format("{0}: {1}", JobId, Description);
                }
                return _DisplayName;
            } 
        }

        private decimal? _ActualDays;
        public decimal ActualDays
        {
            get
            {
                if (_ActualDays == null)
                {
                    _ActualDays = 0;
                    if (SubJobs.Any())
                    {
                        SubJobs.ToList().ForEach(e => {
                            _ActualDays += e.JobPhaseTracks.Sum(sum => sum.ActualDuration) ?? 0;
                        });
                    }
                    else
                    {
                        _ActualDays += JobPhaseTracks.Sum(sum => sum.ActualDuration) ?? 0;
                    }
                    _ActualDays ??= 0;
                }
                return (decimal)_ActualDays;
            }
        }

        private decimal? _BudgetDays;
        public decimal BudgetDays
        {
            get
            {
                if (_BudgetDays == null)
                {
                    _BudgetDays = 0;
                    if (SubJobs.Any())
                    {
                        SubJobs.ToList().ForEach(e => {
                            _BudgetDays += e.JobPhaseTracks.Sum(sum => sum.ProjectedDuration) ?? 0;
                        });
                    }
                    else
                    {
                        _BudgetDays += JobPhaseTracks.Sum(sum => sum.ProjectedDuration) ?? 0;
                    }
                    _BudgetDays ??= 0;
                }
                return (decimal)_BudgetDays;
            }
        }

        private decimal? _RemainingDays;
        public decimal RemainingDays
        {
            get
            {
                if (_RemainingDays == null)
                {
                    _RemainingDays = 0;
                    if (SubJobs.Any())
                    {
                        SubJobs.ToList().ForEach(e => {
                            _RemainingDays += e.RemainingDays;
                        });
                    }
                    else
                    {
                        _RemainingDays += JobPhaseTracks.Sum(sum => (sum.ActualDuration ?? 0) > 0 ? sum.ActualDuration + (sum.RemainingDuration ?? 0) : sum.ProjectedDuration) ?? 0;
                    }
                    _RemainingDays ??= 0;


                }
                return (decimal)_RemainingDays;
            }
        }

        private decimal? _CalculatedDays;
        public decimal CalculatedDays
        {
            get
            {
                if (_CalculatedDays == null)
                {
                    _CalculatedDays = 0;
                    if (SubJobs.Any())
                    {
                        SubJobs.ToList().ForEach(e => {
                            _CalculatedDays += e.CalculatedDays;
                        });
                    }
                    else
                    {
                        _CalculatedDays += JobPhaseTracks.Sum(sum => (sum.ActualDuration ?? 0) > 0 ? sum.ActualDuration + (sum.RemainingDuration ?? 0) : sum.ProjectedDuration) ?? 0;
                    }
                    _CalculatedDays ??= 0;


                }
                return (decimal)_CalculatedDays;
            }
        }

        private decimal? _BidBudgetDays;
        public decimal BidBudgetDays
        {
            get
            {
                if (_BidBudgetDays == null)
                {
                    _BidBudgetDays = 0;
                    if (SubJobs.Any())
                    {
                        SubJobs.ToList().ForEach(e => {
                            _BidBudgetDays += e.BidBudgetDays;
                        });
                    }
                    else
                    {
                        var groundDensityType = IsRock ? "Rock" : "Dirt";
                        var budgetDescription = "Labor Per Man";
                        _BidBudgetDays += BidBoreLine.CostDetails.Where(f => f.GroundDensityType == groundDensityType && f.BudgetCodeDescription.StartsWith(budgetDescription)).Sum(sum => sum.Units);
                    }
                    _BidBudgetDays ??= 0;
                }
                return (decimal)_BidBudgetDays;
            }
        }

        private decimal? _APAmount;
        public decimal APAmount
        {
            get
            {
                if (_APAmount == null)
                {
                    _APAmount = 0;
                    if (SubJobs.Any())
                    {
                        SubJobs.ToList().ForEach(e => {
                            _APAmount += e.APAmount;
                        });
                    }
                    _APAmount += APInvoiceLines.Sum(sum => sum.GrossAmt);
                    _APAmount ??= 0;
                }
                return (decimal)_APAmount;
            }
        }

        private decimal? _APPaidAmount;
        public decimal APPaidAmount
        {
            get
            {
                if (_APPaidAmount == null)
                {
                    _APPaidAmount = 0;
                    if (SubJobs.Any())
                    {
                        SubJobs.ToList().ForEach(e => {
                            _APPaidAmount += e.APPaidAmount;
                        });
                    }
                    APInvoiceLines.ToList().ForEach(e => {
                        _APPaidAmount += e.Payments.Sum(sum => sum.Amount);
                    });
                    _APPaidAmount ??= 0;
                }
                return (decimal)_APPaidAmount;
            }
        }

        private decimal? _ARAmount;
        public decimal ARAmount
        {
            get
            {
                if (_ARAmount == null)
                {
                    _ARAmount = 0;
                    if (SubJobs.Any())
                    {
                        SubJobs.ToList().ForEach(e => {
                            _ARAmount += e.ARAmount;
                        });
                    }
                    _ARAmount += Contract.ARTrans.Where(f => f.ARTransType == "I").Sum(sum => sum.Invoiced); // || f.ARTransType == "C"
                    _ARAmount ??= 0;
                }
                return (decimal)_ARAmount;
            }
        }

        private decimal? _ARPaidAmount;
        public decimal ARPaidAmount
        {
            get
            {
                if (_ARPaidAmount == null)
                {
                    _ARPaidAmount = 0;
                    if (SubJobs.Any())
                    {
                        SubJobs.ToList().ForEach(e => {
                            _ARPaidAmount += e.ARPaidAmount;
                        });
                    }
                    ARInvoiceLines.ToList().ForEach(e => {
                        _ARPaidAmount += e.AppliedTrans.Sum(sum => sum.Amount);
                    });
                    _ARPaidAmount ??= 0;
                }
                return (decimal)_ARPaidAmount;
            }
        }
        
        private decimal? _PostedCost;
        public decimal PostedCost
        {
            get
            {
                if (_PostedCost == null)
                {
                    _PostedCost = 0;
                    if (SubJobs.Any())
                    {
                        SubJobs.ToList().ForEach(e => {
                            _PostedCost += e.PostedCost;
                        });
                    }
                    _PostedCost += JobCosts.Where(f => f.JCTransType != "OE" && f.JCTransType != "PO").Sum(sum => sum.ActualCost);
                    _PostedCost ??= 0;
                }
                return (decimal)_PostedCost;
            }
        }

        private DateTime? _CalStartDate;
        public DateTime CalStartDate
        {
            get
            {
                if (_CalStartDate == null)
                {
                    _CalStartDate = DateTime.Now;
                    if (SubJobs.Any())
                    {
                        _CalStartDate = SubJobs.ToList().Min(f => f.CalStartDate);
                    }
                    else
                    {
                        _CalStartDate = this.CalculatedStartDate();
                    }
                }
                return (DateTime)_CalStartDate;
            }
        }

        public List<DailyTicket> ActiveTickets 
        {
            get
            {
                var jobTaskTickets = DailyJobTasks.Where(f => f.DailyTicket != null)
                                            .Where(f => f.DailyTicket.tStatusId != (int)DB.DailyTicketStatusEnum.Canceled &&
                                                        f.DailyTicket.tStatusId != (int)DB.DailyTicketStatusEnum.Deleted &&
                                                        f.DailyTicket.tStatusId != (int)DB.DailyTicketStatusEnum.Report &&
                                                        f.DailyTicket.tStatusId != (int)DB.DailyTicketStatusEnum.Draft)
                                            .GroupBy(g => g.DailyTicket)
                                            .Select(s => s.Key).ToList();


                return jobTaskTickets;
            } 
        }

        #region Phase Status

        private bool? _IsRigUpComplete;
        public bool IsRigUpComplete
        {
            get
            {
                if (_IsRigUpComplete == null)
                {
                    _IsRigUpComplete = false;
                    if (SubJobs.Any())
                    {
                        var cnt = SubJobs.Count(f => f.IsRigUpComplete);
                        _IsRigUpComplete = cnt == SubJobs.Count;
                    }
                    else
                    {
                        var phase = Phases.FirstOrDefault(f => f.Description.Contains("Rig Up"));
                        _IsRigUpComplete = IsPhaseComplete(phase?.PhaseId, 1);
                    }
                }
                return (bool)_IsRigUpComplete;
            }
            set
            {
                var phase = Phases.FirstOrDefault(f => f.Description.Contains("Rig Up"));
                var progress = JobPhaseTracks.FirstOrDefault(f => f.PhaseId == phase?.PhaseId && f.PassId == 1);
                _IsRigUpComplete = value;
                if (progress != null)
                {
                    progress.ManualIsComplete = _IsRigUpComplete;
                    progress.ManualPercentComplete = _IsRigUpComplete == true ? 1 : progress.PercentCompletion;
                    //progress.ManualUnits = Footage;
                }
            }
        }

        private bool? _IsPilotComplete;
        public bool IsPilotComplete
        {
            get
            {
                if (_IsPilotComplete == null)
                {
                    _IsPilotComplete = false;
                    if (SubJobs.Any())
                    {
                        var cnt = SubJobs.Count(f => f.IsPilotComplete);
                        _IsPilotComplete = cnt == SubJobs.Count;
                    }
                    else
                    {
                        var phase = Phases.FirstOrDefault(f => f.Description.Contains("Pilot"));
                        _IsPilotComplete = IsPhaseComplete(phase?.PhaseId, 1);
                    }
                }
                return (bool)_IsPilotComplete;
            }
            set
            {
                var phase = Phases.FirstOrDefault(f => f.Description.Contains("Pilot"));
                var progress = JobPhaseTracks.FirstOrDefault(f => f.PhaseId == phase?.PhaseId && f.PassId == 1);
                _IsPilotComplete = value;
                if (progress != null)
                {
                    progress.ManualIsComplete = _IsPilotComplete;
                    progress.ManualPercentComplete = _IsPilotComplete == true ? 1 : progress.PercentCompletion;
                    progress.ManualUnits = _IsPilotComplete == true ? Footage : progress.ManualUnits;

                    _PilotPercentComplete = progress.ManualPercentComplete;
                }
            }
        }

        private bool? _IsReam1Complete;
        public bool IsReam1Complete
        {
            get
            {
                if (_IsReam1Complete == null)
                {
                    _IsReam1Complete = false;
                    if (SubJobs.Any())
                    {
                        var cnt = SubJobs.Count(f => f.IsReam1Complete);
                        _IsReam1Complete = cnt == SubJobs.Count;
                    }
                    else
                    {
                        var phase = Phases.FirstOrDefault(f => f.Description.Contains("Ream"));
                        _IsReam1Complete = IsPhaseComplete(phase?.PhaseId, 1);
                    }
                }
                return (bool)_IsReam1Complete;
            }
            set
            {
                var phase = Phases.FirstOrDefault(f => f.Description.Contains("Ream"));
                var progress = JobPhaseTracks.FirstOrDefault(f => f.PhaseId == phase?.PhaseId && f.PassId == 1);
                _IsReam1Complete = value;
                if (progress != null)
                {
                    progress.ManualIsComplete = _IsReam1Complete;
                    progress.ManualPercentComplete = _IsReam1Complete == true ? 1 : progress.PercentCompletion;
                    progress.ManualUnits = _IsReam1Complete == true ? Footage  : progress.ManualUnits;

                    _Ream1PercentComplete = progress.ManualPercentComplete;
                }
            }
        }

        private bool? _IsReam2Complete;
        public bool IsReam2Complete
        {
            get
            {
                if (_IsReam2Complete == null)
                {
                    _IsReam2Complete = false;
                    if (SubJobs.Any())
                    {
                        var cnt = SubJobs.Count(f => f.IsReam2Complete);
                        _IsReam2Complete = cnt == SubJobs.Count;
                    }
                    else
                    {
                        var phase = Phases.FirstOrDefault(f => f.Description.Contains("Ream"));
                        _IsReam2Complete = IsPhaseComplete(phase?.PhaseId, 2);
                    }
                }
                return (bool)_IsReam2Complete;
            }
            set
            {
                var phase = Phases.FirstOrDefault(f => f.Description.Contains("Ream"));
                var progress = JobPhaseTracks.FirstOrDefault(f => f.PhaseId == phase?.PhaseId && f.PassId == 2);
                _IsReam2Complete = value;
                if (progress != null)
                {
                    progress.ManualIsComplete = _IsReam2Complete;
                    progress.ManualPercentComplete = _IsReam2Complete == true ? 1 : progress.PercentCompletion;
                    progress.ManualUnits = _IsReam2Complete == true ? Footage : progress.ManualUnits;

                    _Ream2PercentComplete = progress.ManualPercentComplete;
                }
            }
        }

        private bool? _IsReam3Complete;
        public bool IsReam3Complete
        {
            get
            {
                if (_IsReam3Complete == null)
                {
                    _IsReam3Complete = false;
                    if (SubJobs.Any())
                    {
                        var cnt = SubJobs.Count(f => f.IsReam3Complete);
                        _IsReam3Complete = cnt == SubJobs.Count;
                    }
                    else
                    {
                        var phase = Phases.FirstOrDefault(f => f.Description.Contains("Ream"));
                        _IsReam3Complete = IsPhaseComplete(phase?.PhaseId, 3);
                    }
                }
                return (bool)_IsReam3Complete;
            }
            set
            {
                var phase = Phases.FirstOrDefault(f => f.Description.Contains("Ream"));
                var progress = JobPhaseTracks.FirstOrDefault(f => f.PhaseId == phase?.PhaseId && f.PassId == 3);
                _IsReam3Complete = value;
                if (progress != null)
                {
                    progress.ManualIsComplete = _IsReam3Complete;
                    progress.ManualPercentComplete = _IsReam3Complete == true ? 1 : progress.PercentCompletion;
                    progress.ManualUnits = _IsReam3Complete == true ? Footage : progress.ManualUnits;

                    _Ream3PercentComplete = progress.ManualPercentComplete;
                }
            }
        }

        private bool? _IsReam4Complete;
        public bool IsReam4Complete
        {
            get
            {
                if (_IsReam4Complete == null)
                {
                    _IsReam4Complete = false;
                    if (SubJobs.Any())
                    {
                        var cnt = SubJobs.Count(f => f.IsReam4Complete);
                        _IsReam4Complete = cnt == SubJobs.Count;
                    }
                    else
                    {
                        var phase = Phases.FirstOrDefault(f => f.Description.Contains("Ream"));
                        _IsReam4Complete = IsPhaseComplete(phase?.PhaseId, 4);
                    }
                }
                return (bool)_IsReam4Complete;
            }
            set
            {
                var phase = Phases.FirstOrDefault(f => f.Description.Contains("Ream"));
                var progress = JobPhaseTracks.FirstOrDefault(f => f.PhaseId == phase?.PhaseId && f.PassId == 4);
                _IsReam4Complete = value;
                if (progress != null)
                {
                    progress.ManualIsComplete = _IsReam4Complete;
                    progress.ManualPercentComplete = _IsReam4Complete == true ? 1 : progress.PercentCompletion;
                    progress.ManualUnits = _IsReam4Complete == true ? Footage : progress.ManualUnits;

                    _Ream4PercentComplete = progress.ManualPercentComplete;
                }
            }
        }

        private bool? _IsReam5Complete;
        public bool IsReam5Complete
        {
            get
            {
                if (_IsReam5Complete == null)
                {
                    _IsReam5Complete = false;
                    if (SubJobs.Any())
                    {
                        var cnt = SubJobs.Count(f => f.IsReam5Complete);
                        _IsReam5Complete = cnt == SubJobs.Count;
                    }
                    else
                    {
                        var phase = Phases.FirstOrDefault(f => f.Description.Contains("Ream"));
                        _IsReam5Complete = IsPhaseComplete(phase?.PhaseId, 5);
                    }
                }
                return (bool)_IsReam5Complete;
            }
            set
            {
                var phase = Phases.FirstOrDefault(f => f.Description.Contains("Ream"));
                var progress = JobPhaseTracks.FirstOrDefault(f => f.PhaseId == phase?.PhaseId && f.PassId == 5);
                _IsReam5Complete = value;
                if (progress != null)
                {
                    progress.ManualIsComplete = _IsReam5Complete;
                    progress.ManualPercentComplete = _IsReam5Complete == true ? 1 : progress.PercentCompletion;
                    progress.ManualUnits = _IsReam5Complete == true ? Footage : progress.ManualUnits;

                    _Ream5PercentComplete = progress.ManualPercentComplete;
                }
            }
        }

        private bool? _IsPullPipeComplete;
        public bool IsPullPipeComplete
        {
            get
            {
                if (_IsPullPipeComplete == null)
                {
                    _IsPullPipeComplete = false;
                    if (SubJobs.Any())
                    {
                        var cnt = SubJobs.Count(f => f.IsPullPipeComplete);
                        _IsPullPipeComplete = cnt == SubJobs.Count;
                    }
                    else
                    {
                        var phase = Phases.FirstOrDefault(f => f.Description.Contains("Pull Pipe"));
                        _IsPullPipeComplete = IsPhaseComplete(phase?.PhaseId, 1);
                    }
                }
                return (bool)_IsPullPipeComplete;
            }
            set
            {
                var phase = Phases.FirstOrDefault(f => f.Description.Contains("Pull Pipe"));
                var progress = JobPhaseTracks.FirstOrDefault(f => f.PhaseId == phase?.PhaseId && f.PassId == 1);
                _IsPullPipeComplete = value;
                if (progress != null)
                {
                    progress.ManualIsComplete = _IsPullPipeComplete;
                    progress.ManualPercentComplete = _IsPullPipeComplete == true ? 1 : progress.PercentCompletion;
                    progress.ManualUnits = _IsPullPipeComplete == true ? Footage : progress.ManualUnits;

                    _PullPipePercentComplete = progress.ManualPercentComplete;
                }
            }
        }


        private bool? _IsRigDownComplete;
        public bool IsRigDownComplete
        {
            get
            {
                if (_IsRigDownComplete == null)
                {
                    _IsRigDownComplete = false;
                    if (SubJobs.Any())
                    {
                        var cnt = SubJobs.Count(f => f.IsRigDownComplete);
                        _IsRigDownComplete = cnt == SubJobs.Count;
                    }
                    else
                    {
                        var phase = Phases.FirstOrDefault(f => f.Description.Contains("Rig Down"));
                        _IsRigDownComplete = IsPhaseComplete(phase?.PhaseId, 1);
                    }
                }
                return (bool)_IsRigDownComplete;
            }
            set
            {
                var phase = Phases.FirstOrDefault(f => f.Description.Contains("Rig Down"));
                var progress = JobPhaseTracks.FirstOrDefault(f => f.PhaseId == phase?.PhaseId && f.PassId == 1);
                _IsRigDownComplete = value;
                if (progress != null)
                {
                    progress.ManualIsComplete = _IsRigDownComplete;
                    progress.ManualPercentComplete = _IsRigDownComplete == true ? 1 : progress.PercentCompletion;

                }
            }
        }

        private bool IsPhaseComplete(string phaseId, int passId)
        {
            if (string.IsNullOrEmpty(phaseId))
                return false;

            var result = false;
            var progress = JobPhaseTracks.FirstOrDefault(f => f.PhaseId == phaseId && f.PassId == passId);

            if (progress != null)
            {
                if (progress.ManualIsComplete == true)
                {
                    result = true;
                }
                else if (progress.IsComplete == true)
                {
                    result = true;
                }
                else
                {
                    if (progress.ActualUnits >= Footage)
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        private decimal PhasePercentComplete(string phaseId, int passId)
        {
            if (string.IsNullOrEmpty(phaseId))
                return 0;

            var result = 0m;
            var progress = JobPhaseTracks.FirstOrDefault(f => f.PhaseId == phaseId && f.PassId == passId);

            if (progress != null)
            {
                if (progress.ManualIsComplete == true)
                {
                    result = 1;
                }
                else if (progress.IsComplete == true)
                {
                    result = 1;
                }
                else
                {
                    var manualPercent = progress.ManualPercentComplete ?? 0;
                    var calculatedPercent = progress.PercentCompletion ?? 0;
                    if (manualPercent > calculatedPercent)
                    {
                        result = manualPercent;
                    }
                    else
                    {
                        result = calculatedPercent;
                    }
                }
            }

            return result;
        }


        private decimal? _PilotPercentComplete;
        public decimal PilotPercentComplete
        {
            get
            {
                if (_PilotPercentComplete == null)
                {
                    _PilotPercentComplete = 0;
                    if (!SubJobs.Any())
                    {
                        var phase = Phases.FirstOrDefault(f => f.Description.Contains("Pilot"));
                        _PilotPercentComplete = PhasePercentComplete(phase?.PhaseId, 1);
                    }
                }
                return (decimal)_PilotPercentComplete;
            }
            set
            {
                var phase = Phases.FirstOrDefault(f => f.Description.Contains("Pilot"));
                var progress = JobPhaseTracks.FirstOrDefault(f => f.PhaseId == phase?.PhaseId && f.PassId == 1);
                _PilotPercentComplete = value;
                if (progress != null)
                {
                    progress.ManualPercentComplete = _PilotPercentComplete;
                    progress.ManualUnits = Footage * _PilotPercentComplete;
                }
            }
        }

        private decimal? _Ream1PercentComplete;
        public decimal Ream1PercentComplete
        {
            get
            {
                if (_Ream1PercentComplete == null)
                {
                    _Ream1PercentComplete = 0;
                    if (!SubJobs.Any())
                    {
                        var phase = Phases.FirstOrDefault(f => f.Description.Contains("Ream"));
                        _Ream1PercentComplete = PhasePercentComplete(phase?.PhaseId, 1);
                    }
                }
                return (decimal)_Ream1PercentComplete;
            }
            set
            {
                var phase = Phases.FirstOrDefault(f => f.Description.Contains("Ream"));
                var progress = JobPhaseTracks.FirstOrDefault(f => f.PhaseId == phase?.PhaseId && f.PassId == 1);
                _Ream1PercentComplete = value;
                if (progress != null)
                {
                    progress.ManualPercentComplete = _Ream1PercentComplete;
                    progress.ManualUnits = Footage * _Ream1PercentComplete;
                }
            }
        }

        private decimal? _Ream2PercentComplete;
        public decimal Ream2PercentComplete
        {
            get
            {
                if (_Ream2PercentComplete == null)
                {
                    _Ream2PercentComplete = 0;
                    if (!SubJobs.Any())
                    {
                        var phase = Phases.FirstOrDefault(f => f.Description.Contains("Ream"));
                        _Ream2PercentComplete = PhasePercentComplete(phase?.PhaseId, 2);
                    }
                }
                return (decimal)_Ream2PercentComplete;
            }
            set
            {
                var phase = Phases.FirstOrDefault(f => f.Description.Contains("Ream"));
                var progress = JobPhaseTracks.FirstOrDefault(f => f.PhaseId == phase?.PhaseId && f.PassId == 2);
                _Ream2PercentComplete = value;
                if (progress != null)
                {
                    progress.ManualPercentComplete = _Ream2PercentComplete;
                    progress.ManualUnits = Footage * _Ream2PercentComplete;
                }
            }
        }

        private decimal? _Ream3PercentComplete;
        public decimal Ream3PercentComplete
        {
            get
            {
                if (_Ream3PercentComplete == null)
                {
                    _Ream3PercentComplete = 0;
                    if (!SubJobs.Any())
                    {
                        var phase = Phases.FirstOrDefault(f => f.Description.Contains("Ream"));
                        _Ream3PercentComplete = PhasePercentComplete(phase?.PhaseId, 3);
                    }
                }
                return (decimal)_Ream3PercentComplete;
            }
            set
            {
                var phase = Phases.FirstOrDefault(f => f.Description.Contains("Ream"));
                var progress = JobPhaseTracks.FirstOrDefault(f => f.PhaseId == phase?.PhaseId && f.PassId == 3);
                _Ream3PercentComplete = value;
                if (progress != null)
                {
                    progress.ManualPercentComplete = _Ream3PercentComplete;
                    progress.ManualUnits = Footage * _Ream3PercentComplete;
                }
            }
        }

        private decimal? _Ream4PercentComplete;
        public decimal Ream4PercentComplete
        {
            get
            {
                if (_Ream4PercentComplete == null)
                {
                    _Ream4PercentComplete = 0;
                    if (!SubJobs.Any())
                    {
                        var phase = Phases.FirstOrDefault(f => f.Description.Contains("Ream"));
                        _Ream4PercentComplete = PhasePercentComplete(phase?.PhaseId, 4);
                    }
                }
                return (decimal)_Ream4PercentComplete;
            }
            set
            {
                var phase = Phases.FirstOrDefault(f => f.Description.Contains("Ream"));
                var progress = JobPhaseTracks.FirstOrDefault(f => f.PhaseId == phase?.PhaseId && f.PassId == 4);
                _Ream4PercentComplete = value;
                if (progress != null)
                {
                    progress.ManualPercentComplete = _Ream4PercentComplete;
                    progress.ManualUnits = Footage * _Ream4PercentComplete;
                }
            }
        }

        private decimal? _Ream5PercentComplete;
        public decimal Ream5PercentComplete
        {
            get
            {
                if (_Ream5PercentComplete == null)
                {
                    _Ream5PercentComplete = 0;
                    if (!SubJobs.Any())
                    {
                        var phase = Phases.FirstOrDefault(f => f.Description.Contains("Ream"));
                        _Ream5PercentComplete = PhasePercentComplete(phase?.PhaseId, 5);
                    }
                }
                return (decimal)_Ream5PercentComplete;
            }
            set
            {
                var phase = Phases.FirstOrDefault(f => f.Description.Contains("Ream"));
                var progress = JobPhaseTracks.FirstOrDefault(f => f.PhaseId == phase?.PhaseId && f.PassId == 5);
                _Ream5PercentComplete = value;
                if (progress != null)
                {
                    progress.ManualPercentComplete = _Ream5PercentComplete;
                    progress.ManualUnits = Footage * _Ream5PercentComplete;
                }
            }
        }

        private decimal? _PullPipePercentComplete;
        public decimal PullPipePercentComplete
        {
            get
            {
                if (_PullPipePercentComplete == null)
                {
                    _PullPipePercentComplete = 0;
                    if (!SubJobs.Any())
                    {
                        var phase = Phases.FirstOrDefault(f => f.Description.Contains("Pull Pipe"));
                        _PullPipePercentComplete = PhasePercentComplete(phase?.PhaseId, 1);
                    }
                }
                return (decimal)_PullPipePercentComplete;
            }
            set
            {
                var phase = Phases.FirstOrDefault(f => f.Description.Contains("Pull Pipe"));
                var progress = JobPhaseTracks.FirstOrDefault(f => f.PhaseId == phase?.PhaseId && f.PassId == 1);
                _PullPipePercentComplete = value;
                if (progress != null)
                {
                    progress.ManualPercentComplete = _PullPipePercentComplete;
                    progress.ManualUnits = Footage * _PullPipePercentComplete;
                }
            }
        }

        #endregion

        public DB.JCJobTypeEnum JobType
        {
            get
            {
                return (DB.JCJobTypeEnum)(int.TryParse(JobTypeId, out int outcurrentStatus) ? outcurrentStatus : 0);
            }
            set
            {
                JobTypeId = ((int)value).ToString(AppCultureInfo.CInfo());
            }
        }

        public void SyncPhases(Job srcJob)
        {
            if (srcJob == null)
                return;

            using var db = new VPEntities();
            var templatejob = srcJob;

            var phaseList = new List<JobPhase>();
            var costTypeList = new List<JobPhaseCost>();

            var srcPhaseList = srcJob.Phases.ToList();
            foreach (var phase in srcPhaseList)
            {
                var dstPhase = Phases.FirstOrDefault(f => f.PhaseId == phase.PhaseId);
                var srcCostList = phase.JobPhaseCosts.ToList();
                if (dstPhase == null)
                {
                    db.Entry(phase).State = System.Data.Entity.EntityState.Detached;
                    phase.JCCo = JCCo;
                    phase.JobId = JobId;
                    phase.PhaseGroupId = (byte)JCCompanyParm.HQCompanyParm.PhaseGroupId;
                    phase.ContractId = ContractId;
                    phase.JobPhaseCosts = null;
                    dstPhase = phase;
                    phaseList.Add(phase);
                }
                
                foreach (var costType in srcCostList)
                {
                    var dstCostType = dstPhase.JobPhaseCosts.FirstOrDefault(f => f.CostTypeId == costType.CostTypeId);
                    if (dstCostType == null)
                    {
                        db.Entry(costType).State = System.Data.Entity.EntityState.Detached;
                        costType.JCCo = JCCo;
                        costType.JobId = JobId;
                        costType.PhaseGroupId = (byte)JCCompanyParm.HQCompanyParm.PhaseGroupId;
                        costTypeList.Add(costType);
                    }
                }
            }

            db.BulkInsert(phaseList);
            db.BulkInsert(costTypeList);

        }
             
        public JobPhase AddMasterPhase(string phaseId, bool includeCost = true)
        {
            var mstPhases = this.JCCompanyParm.HQCompanyParm.PhaseGroup.JCPhases.ToList();
            var mstPhase = mstPhases.FirstOrDefault(f => f.PhaseId == phaseId);

            return AddMasterPhase(mstPhase, includeCost);
        }

        public JobPhase AddMasterPhase(PhaseMaster mstPhase, bool includeCost = true)
        {
            if (mstPhase == null)
                return null;

            var newPhase = Phases.FirstOrDefault(f => f.PhaseId == mstPhase.PhaseId);
            if (newPhase == null)
            {
                newPhase = new JobPhase
                {
                    JCCo = JCCo,
                    JobId = JobId,
                    ContractId = ContractId,
                    PhaseGroupId = (byte)JCCompanyParm.HQCompanyParm.PhaseGroupId,
                    PhaseId = mstPhase.PhaseId,
                    Item = "               1",
                    Description = mstPhase.Description,
                    ProjMinPct = mstPhase.ProjMinPct,
                    ActiveYN = mstPhase.ActiveYN,
                    Notes = mstPhase.Notes,
                    PhaseType = mstPhase.PhaseType,
                    ParentPhaseId = mstPhase.ParentPhaseId,
                    IsMilestone = mstPhase.IsMilestone,
                    ProductionSortId = mstPhase.SortId,
                    Job = this,
                    
                };

                Phases.Add(newPhase);

            }
            if (includeCost)
            {
                mstPhase.Costs.ToList().ForEach(e => newPhase.AddMasterPhaseCost(e));
            }

            return newPhase;
        }

        public void SyncMasterPhase()
        {
            var masterList = JCCompanyParm.HQCompanyParm.PhaseGroup.JCPhases.Where(f => f.ActiveYN == "Y").ToList();

            foreach (var mstPhase in masterList)
            {
                var dstPhase = this.AddMasterPhase(mstPhase);

                foreach (var mstCost in mstPhase.Costs)
                {
                    var newCost = dstPhase.AddMasterPhaseCost(mstCost);
                }
            }
        }

        public ProjectBudget AddBudget(BidBoreLine bore, string budgetNo)
        {
            var budget = Budgets.FirstOrDefault(f => f.BudgetNo == budgetNo);
            if (budget == null)
            {
                budget = new ProjectBudget
                {
                    PMCo = bore.Job.JCCo,
                    BudgetNo = budgetNo,
                    ProjectId = bore.JobId,
                    BidId = bore.BidId,
                    BoreId = bore.BoreId,
                    Description = bore.Package.Description,

                    Job = bore.Job,
                    BidBoreLine = bore,
                    PMCompanyParm = bore.Job.JCCompanyParm.PMCompanyParm,
                };
                Budgets.Add(budget);
            }
            
            return budget;
        }

        public void UpdateJobStatus()
        {
            Status = CalculateStatus();//StatusId
        }

        public DB.JCJobStatusEnum CalculateStatus()
        {            
            if (SubJobs.Any())
            {
                var jobSum = SubJobs
                    .Where(f => f.IntersectJobId == null)
                    .GroupBy(g => g.ParentJobId)
                    .Select(s => new {
                        Open = s.Count(f => f.Status == DB.JCJobStatusEnum.Open),
                        Scheduled = s.Count(f => f.Status == DB.JCJobStatusEnum.Scheduled),
                        InProgress = s.Count(f => f.Status == DB.JCJobStatusEnum.InProgress),
                        PulledPipe = s.Count(f => f.Status == DB.JCJobStatusEnum.PulledPipe),
                        Completed = s.Count(f => f.Status == DB.JCJobStatusEnum.Completed),
                        OnHold = s.Count(f => f.Status == DB.JCJobStatusEnum.OnHold),
                        Canceled = s.Count(f => f.Status == DB.JCJobStatusEnum.Canceled),
                        Invoiced = s.Count(f => f.Status == DB.JCJobStatusEnum.Invoiced),
                        Cnt = s.Count(),
                    }).FirstOrDefault();


                if (jobSum.Invoiced + jobSum.Canceled == jobSum.Cnt)
                {
                    return DB.JCJobStatusEnum.Invoiced;
                }
                else if (jobSum.Canceled == jobSum.Cnt)
                {
                    return DB.JCJobStatusEnum.Canceled;
                }
                else if (jobSum.Invoiced + jobSum.Canceled + jobSum.OnHold == jobSum.Cnt)
                {
                    return DB.JCJobStatusEnum.OnHold;
                }
                else if (jobSum.Invoiced + jobSum.Canceled + jobSum.OnHold + jobSum.Completed == jobSum.Cnt)
                {
                    return DB.JCJobStatusEnum.Completed;
                }
                else if (jobSum.Invoiced + jobSum.Canceled + jobSum.OnHold + jobSum.Completed + jobSum.PulledPipe == jobSum.Cnt)
                {
                    return DB.JCJobStatusEnum.InProgress;
                }
                else if (jobSum.Invoiced + jobSum.Canceled + jobSum.OnHold + jobSum.Completed + jobSum.PulledPipe + jobSum.InProgress == jobSum.Cnt)
                {
                    return DB.JCJobStatusEnum.InProgress;
                }
                else if (jobSum.OnHold + jobSum.Canceled + jobSum.Scheduled == jobSum.Cnt)
                {
                    return DB.JCJobStatusEnum.Scheduled;
                }
                if (jobSum.Invoiced + jobSum.Completed + jobSum.PulledPipe + jobSum.InProgress > 0)
                {
                    return DB.JCJobStatusEnum.InProgress;
                }

                return DB.JCJobStatusEnum.Open;
            }
            else
            {
                var currentStatus = Status;

                if (ARAmount > 0)
                {
                    return DB.JCJobStatusEnum.Invoiced;
                }
                else if (DTJobPhases.Any(f => f.PhaseId == "   007-  -" && f.Hours > 0))
                {
                    return DB.JCJobStatusEnum.PulledPipe;
                }
                else if (PostedCost > 0)
                {
                    return DB.JCJobStatusEnum.InProgress;
                }
            }
            return Status;
        }

        public void BuildProgressTrackingFromBid()
        {
            var bidPhases = IsRock ? BidBoreLine.RockPhases : BidBoreLine.DirtPhases;
            JobPhaseTracks.ToList().ForEach(e => e.SortId = 0);
            bidPhases.Where(f => f.ProductionValue > 0).ToList().ForEach(e => { BuildProgressTrackingFromBidPhase(e); });

            var delList = JobPhaseTracks.Where(f => f.PlannedBudgetCodeId == null && f.ActualBudgetCodeId == null).ToList();
            delList.ForEach(e => {
                e.ProjectGanttTask = null;
                JobPhaseTracks.Remove(e);
            });
        }

        public JobPhaseTrack BuildProgressTrackingFromBidPhase(BidBoreLinePass bidPhase)
        {
            
            bidPhase.UpdateBudgetCode(); 
            var result = JobPhaseTracks.FirstOrDefault(f => f.PhaseId == bidPhase.PhaseId && f.PassId == bidPhase.PassId);
            if (result == null)
            {
                result = new JobPhaseTrack()
                {
                    JCCo = JCCo,
                    JobId = JobId,
                    PhaseGroupId = (byte)JCCompanyParm.HQCompanyParm.PhaseGroupId,
                    PhaseId = bidPhase.PhaseId,
                    PassId = (byte)bidPhase.PassId,
                    ActualDuration = 0,
                    PercentCompletion = 0,
                    ActualUnits = 0,
                    PlannedBudgetCodeId = bidPhase.BudgetCodeId,
                    ProjectedStartDate = StartDate == null ? bidPhase.BoreLine.Bid.StartDate : StartDate,
                    ProjectedDuration = bidPhase.Duration,
                    Job = this,
                    //JobPhase = AddMasterPhase(bidPhase.PhaseId),

                    PlannedBudgetCode = JCCompanyParm.HQCompanyParm.PMCompanyParm.AddBudgetCode(bidPhase.BudgetCodeId),
                };

                JobPhaseTracks.Add(result);
            }
            result.SortId = JobPhaseTracks.DefaultIfEmpty().Max(max => max == null ? 0 : max.SortId) + 1;
            //Todo: Add Gantt Task Update/Create
            return result;
        }

        //public void UpdateProgressTrackingFromActuals()
        //{
        //    JobPhaseTracks.ToList().ForEach(e =>
        //    {
        //        e.ActualDuration = 0;
        //        e.ActualUnits = 0;
        //        e.ActualBudgetCodeId = null;
        //        e.PercentCompletion = 0;
        //        e.ActualStartDate = null;
        //        e.RemainingDuration = 0;
        //        e.RemainingUnits = 0;
        //        e.RemainingStartDate = null;
        //    });

        //    var dailyJobTasks = ActiveTickets.SelectMany(s => s.DailyJobTasks)
        //                                     .OrderBy(o => o.WorkDate)
        //                                     .ThenBy(o => o.SortOrder)
        //                                     .ThenByDescending(o => o.CostTypeId)
        //                                     .ToList();

        //    var dailyJobTaskLines = dailyJobTasks.GroupBy(g => new { g.TicketId, g.LineNum, g.WorkDate })
        //                                .Select(s => new { s.Key.TicketId, s.Key.LineNum, s.Key.WorkDate, Tasks = s })
        //                                .ToList();

        //    foreach (var line in dailyJobTaskLines)
        //    {
        //        var hoursTask = line.Tasks.FirstOrDefault(f => f.CostTypeId == 1);
        //        var progressTask = line.Tasks.FirstOrDefault(f => f.CostTypeId != 1);
        //        if (hoursTask != null)
        //        {
        //            //var jobProgress = CreateUpdate(hoursTask, progressTask, job, db, dbSaveVerbose);

        //            /** Update Daily Task Gantt Info**/
        //            //if (jobProgress != null && hoursTask != null)
        //            {
        //                //ProjectGanttRepository.CreateUpdate(hoursTask, jobProgress, db, dbSaveVerbose);
        //            }
        //        }

        //    }
        //}

        public List<DTJobPhase> ActiveDTJobPhases
        {
            get
            {
                return DTJobPhases.Where(f => f.JobTicket.DailyTicket.Status != DB.DailyTicketStatusEnum.Canceled ||
                                              f.JobTicket.DailyTicket.Status != DB.DailyTicketStatusEnum.Deleted)
                    .ToList();
            }
        }

        public JobPhaseTrack GetJobProgress(JobPhase jobPhase, byte passId)
        {
            var jobPhaseTrack = JobPhaseTracks.FirstOrDefault(f => f.PhaseGroupId == jobPhase.PhaseGroupId && f.PhaseId == jobPhase.PhaseId && f.PassId == passId);

            //Add if missing
            if (jobPhaseTrack == null)
            {
                jobPhaseTrack = new JobPhaseTrack() { 
                    JCCo = jobPhase.JCCo,
                    JobId = jobPhase.JobId,
                    PhaseGroupId = jobPhase.PhaseGroupId,
                    PhaseId = jobPhase.PhaseId,
                    PassId = passId,

                    Job = this,
                    JobPhase = jobPhase,
                };
                JobPhaseTracks.Add(jobPhaseTrack);
                var passes = JobPhaseTracks.OrderBy(o => o.JobPhase.ProductionSortId).ThenBy(o => o.PassId).ToList();
                var sortId = 1;
                foreach (var pass in passes)
                {
                    pass.SortId = sortId;
                    sortId++;
                }
            }

            return jobPhaseTrack;
        }
    }

    public static class JobExtension
    {
        public static bool IsRock(this Job job)
        {
            if (job == null)
                return false;

            if (job.DailyFieldTickets.Any(f => f.MudMotor == true))
                return true;

            return false;
        }

        public static bool HasAttachments(this Job job)
        {
            if (job == null)
                return false;

            if (job.UniqueAttchID != null)
            {
                using var db = new VPEntities();
                if (db.HQAttachmentFiles.Any(f => f.UniqueAttchID == job.UniqueAttchID))
                {
                    return true;
                }
            }
            return false;
        }

        public static DateTime CalculatedStartDate(this Job job, bool useGantt = false)
        {
            if (job == null)
                return DateTime.Now;
            DateTime? startDate;
            if (job.SubJobs.Any())
            {
                startDate = job.SubJobs.Min(min => min.JobPhaseTracks.Min(subMin => subMin.ActualStartDate));
            }
            else
            {
                startDate = job.JobPhaseTracks.Min(min => min.ActualStartDate);
            }

            if (startDate == null)
            {
                startDate ??= job.ParentJob?.StartDate;
                //startDate ??= result?.PlannedStartDate;
                startDate ??= job?.StartDate;
                if (useGantt)
                {
                    if (job.ProjectGanttTask != null)
                    {
                        var ganttStartDate = job.ProjectGanttTask.StartDate;
                        if (ganttStartDate > startDate)
                        {
                            startDate = ganttStartDate;
                        }
                    }
                }
                if (startDate == null)
                {
                    if (job.BidPackages.Any())
                    {
                        var bidInfo = job.BidPackages?.FirstOrDefault()?.Bid;
                        startDate = bidInfo.StartDate;
                    }
                }

                if (startDate <= DateTime.Now)
                {
                    startDate = DateTime.Now;
                }
            }
            return startDate ?? DateTime.Now;
        }

        public static DateTime CalculatedEndDate(this Job job, bool useGantt = false)
        {
            if (job == null)
                return DateTime.Now;           

            var startDate = job.CalculatedStartDate(useGantt);           
            var duration = job.CalculatedDays;
            var endDate = startDate.AddDays((double)duration);

            return endDate;
        }

        public static DB.JCJobStatusEnum CalculateStatus(this Job job)
        {
            if (job.SubJobs.Any())
            {
                var jobSum = job.SubJobs
                    .Where(f => f.IntersectJobId == null)
                    .GroupBy(g => g.ParentJobId)
                    .Select(s => new {
                        Open = s.Count(f => f.StatusId == ((int)DB.JCJobStatusEnum.Open).ToString()),
                        Scheduled = s.Count(f => f.StatusId == ((int)DB.JCJobStatusEnum.Scheduled).ToString()),
                        InProgress = s.Count(f => f.StatusId == ((int)DB.JCJobStatusEnum.InProgress).ToString()),
                        PulledPipe = s.Count(f => f.StatusId == ((int)DB.JCJobStatusEnum.PulledPipe).ToString()),
                        Completed = s.Count(f => f.StatusId == ((int)DB.JCJobStatusEnum.Completed).ToString()),
                        OnHold = s.Count(f => f.StatusId == ((int)DB.JCJobStatusEnum.OnHold).ToString()),
                        Canceled = s.Count(f => f.StatusId == ((int)DB.JCJobStatusEnum.Canceled).ToString()),
                        Invoiced = s.Count(f => f.StatusId == ((int)DB.JCJobStatusEnum.Invoiced).ToString()),
                        Cnt = s.Count(),
                    }).FirstOrDefault();


                if (jobSum.Invoiced + jobSum.Canceled == jobSum.Cnt)
                {
                    return DB.JCJobStatusEnum.Invoiced;
                }
                else if (jobSum.Canceled == jobSum.Cnt)
                {
                    return DB.JCJobStatusEnum.Canceled;
                }
                else if (jobSum.Invoiced + jobSum.Canceled + jobSum.OnHold == jobSum.Cnt)
                {
                    return DB.JCJobStatusEnum.OnHold;
                }
                else if (jobSum.Invoiced + jobSum.Canceled + jobSum.OnHold + jobSum.Completed == jobSum.Cnt)
                {
                    return DB.JCJobStatusEnum.Completed;
                }
                else if (jobSum.Invoiced + jobSum.Canceled + jobSum.OnHold + jobSum.Completed + jobSum.PulledPipe == jobSum.Cnt)
                {
                    return DB.JCJobStatusEnum.InProgress;
                }
                else if (jobSum.Invoiced + jobSum.Canceled + jobSum.OnHold + jobSum.Completed + jobSum.PulledPipe + jobSum.InProgress == jobSum.Cnt)
                {
                    return DB.JCJobStatusEnum.InProgress;
                }
                else if (jobSum.OnHold + jobSum.Canceled + jobSum.Scheduled == jobSum.Cnt)
                {
                    return DB.JCJobStatusEnum.Scheduled;
                }
                if (jobSum.Invoiced + jobSum.Completed + jobSum.PulledPipe + jobSum.InProgress > 0)
                {
                    return DB.JCJobStatusEnum.InProgress;
                }

                return DB.JCJobStatusEnum.Open;
            }

            else
            {
                var currentStatus = (DB.JCJobStatusEnum)(int.TryParse(job.StatusId, out int outcurrentStatus) ? outcurrentStatus : 0);

                if (job.ARAmount > 0)
                {
                    return DB.JCJobStatusEnum.Invoiced;
                }
                else if (job.DailyJobTasks.Any(f => f.PhaseId == "   007-  -" && f.Value > 0))
                {
                    return DB.JCJobStatusEnum.PulledPipe;
                }
                else if (job.PostedCost > 0)
                {
                    return DB.JCJobStatusEnum.InProgress;
                }
            }
            var statusId = (DB.JCJobStatusEnum)(int.TryParse(job.StatusId, out int outStatusId) ? outStatusId : 0);
            return statusId;
        }

        public static List<DailyTicket> ActiveTickets(this Job job)
        {
            if (job == null)
                return new List<DailyTicket>();
            var results = job.DailyJobTasks.Where(f => f.DailyTicket != null)
                                            .Where(f => f.DailyTicket.tStatusId != (int)DB.DailyTicketStatusEnum.Canceled &&
                                                        f.DailyTicket.tStatusId != (int)DB.DailyTicketStatusEnum.Deleted &&
                                                        f.DailyTicket.tStatusId != (int)DB.DailyTicketStatusEnum.Report &&
                                                        f.DailyTicket.tStatusId != (int)DB.DailyTicketStatusEnum.Draft)
                                            .GroupBy(g => g.DailyTicket)
                                            .Select(s => s.Key).ToList();
            return results;
        }


        public static List<JobCost> ActualJobCosts(this Job job)
        {
            if (job == null)
                return new List<JobCost>();

            return job.JobCosts.Where(f => f.JCTransType != "OE" && f.JCTransType != "PO").ToList();
        }
       
        public static List<JobCost> ActualJobCosts(this Job job, DateTime mth)
        {
            if (job == null)
                return new List<JobCost>();
            //(!(f.Job.JobType == DB.JCJobTypeEnum.Job || f.Job.JobType == DB.JCJobTypeEnum.Project) && f.Mth == mth) || (f.Job.JobType == DB.JCJobTypeEnum.Job || f.Job.JobType == DB.JCJobTypeEnum.Project)
            return job.JobCosts
                .Where(f => f.JCTransType != "OE" && 
                f.JCTransType != "PO" && 
                f.Mth == mth 
                //&& (!(f.Job.JobType == DB.JCJobTypeEnum.Job || f.Job.JobType == DB.JCJobTypeEnum.Project) && f.Mth == mth) || (f.Job.JobType == DB.JCJobTypeEnum.Job || f.Job.JobType == DB.JCJobTypeEnum.Project))
                
                ).ToList();
        }

        public static List<BidBoreLineCostItem> BidBudget(this Job job)
        {
            if (job == null)
                return new List<BidBoreLineCostItem>();

            return job.Bid().CostItems.ToList();
        }

        public static BidBoreLine Bid(this Job job)
        {
            if (job == null)
                return null;

            var bid = job.BidBoreLines.Where(f => f.Status != DB.BidStatusEnum.Canceled && f.Status != DB.BidStatusEnum.Deleted).LastOrDefault();
            if (bid == null)
                bid = job.BidBoreLines.LastOrDefault();
            return bid;
        }

        public static List<JobCost> UnPostedJobCosts(this Job job)
        {
            var costTypes = job.db.CostTypes.Where(f => f.PhaseGroupId == job.JCCompanyParm.HQCompanyParm.PhaseGroupId).ToList();
            var trans = job.db.vJobUnPostedCosts.Where(f => f.Job == job.JobId && f.JCTransType != "PO").ToList();
            var results = trans.Select(s => new JobCost
            {
                JCCo = (byte)s.JCCo,
                Mth = (DateTime)s.Mth,
                CostTrans = s.CostTrans ?? 0,
                JobId = s.Job,//(string.IsNullOrEmpty(s.Job) ? "" : s.Job),
                PhaseGroupId = (byte)(s.PhaseGroupId ?? 0),
                PhaseId = s.Phase,
                CostTypeID = (byte)(s.CostType ?? 0),
                PostedDate = (DateTime)(s.PostedDate ?? DateTime.MinValue),
                ActualDate = s.ActualDate ?? DateTime.MinValue,
                JCTransType = s.JCTransType,
                Source = s.Source,
                Description = s.Description,
                BatchId = s.BatchId,
                InUseBatchId = s.InUseBatchId,
                GLCo = s.GLCo,
                GLTransAcct = s.GLTransAcct,
                GLOffsetAcct = s.GLOffsetAcct,
                UM = s.UM,
                ActualUnitCost = s.ActualUnitCost ?? 0,
                PerECM = s.PerECM,
                ActualHours = s.ActualHours ?? 0,
                ActualUnits = s.ActualUnits ?? 0,
                ActualCost = s.ActualCost ?? 0,
                ProgressCmplt = s.ProgressCmplt ?? 0,
                EstHours = s.EstHours ?? 0,
                EstUnits = s.EstUnits ?? 0,
                EstCost = s.EstCost ?? 0,
                ProjHours = s.ProjHours ?? 0,
                ProjUnits = s.ProjUnits ?? 0,
                ProjCost = s.ProjCost ?? 0,
                ForecastHours = s.ForecastHours ?? 0,
                ForecastUnits = s.ForecastUnits ?? 0,
                ForecastCost = s.ForecastCost ?? 0,
                PostedUM = s.PostedUM,
                PostedECM = s.PostedECM,
                DeleteFlag = s.DeleteFlag,
                AllocCode = s.AllocCode,
                ACO = s.ACO,
                ACOItem = s.ACOItem,
                PRCo = s.PRCo,
                Employee = s.Employee,
                Craft = s.Craft,
                Class = s.Class,
                Crew = s.Crew,
                EarnFactor = s.EarnFactor,
                EarnType = s.EarnType,
                Shift = s.Shift,
                LiabilityType = s.LiabilityType,
                VendorGroup = s.VendorGroup,
                Vendor = s.Vendor,
                APCo = s.APCo,
                APTrans = s.APTrans,
                APLine = s.APLine,
                APRef = s.APRef,
                PO = s.PO,
                POItem = s.POItem,
                SL = s.SL,
                SLItem = s.SLItem,
                MO = s.MO,
                MOItem = s.MOItem,
                MatlGroup = s.MatlGroup,
                Material = s.Material,
                INCo = s.INCo,
                Loc = s.Loc,
                INStdECM = s.INStdECM,
                INStdUM = s.INStdUM,
                MSTrans = s.MSTrans,
                MSTicket = s.MSTicket,
                JBBillStatus = s.JBBillStatus,
                JBBillMonth = s.JBBillMonth,
                JBBillNumber = s.JBBillNumber,
                EMCo = s.EMCo,
                EMEquip = s.EMEquip,
                EMRevCode = s.EMRevCode,
                EMGroup = s.EMGroup,
                EMTrans = s.EMTrans,
                TaxType = s.TaxType,
                TaxGroup = s.TaxGroup,
                TaxCode = s.TaxCode,
                UniqueAttchID = s.UniqueAttchID,
                SrcJCCo = s.SrcJCCo,
                OffsetGLCo = s.OffsetGLCo,
                POItemLine = s.POItemLine,
                udCrew = s.udCrew,
                SMWorkCompletedID = s.SMWorkCompletedID,
                SMCo = s.SMCo,
                SMWorkOrder = s.SMWorkOrder,
                SMScope = s.SMScope,
                PMCostProjection = s.PMCostProjection,
                udZionsID = s.udZionsID,
                udWEXID = s.udWEXID,
                udTicketId = s.udTicketId,
            }).ToList();
            results.ForEach(e => {
                e.CostType = costTypes.FirstOrDefault(f => f.CostTypeId == e.CostTypeID);
            });
            return results;
        }

        public static List<JobCost> UnPostedJobCosts(this Job job, DateTime mth)
        {
            var costTypes = job.db.CostTypes.Where(f => f.PhaseGroupId == job.JCCompanyParm.HQCompanyParm.PhaseGroupId).ToList();
            var trans = job.db.vJobUnPostedCosts.Where(f => f.Job == job.JobId && f.JCTransType != "PO" && f.Mth == mth).ToList();
            var results = trans.Select(s => new JobCost
            {
                JCCo = (byte)s.JCCo,
                Mth = (DateTime)s.Mth,
                CostTrans = s.CostTrans ?? 0,
                JobId = s.Job,//(string.IsNullOrEmpty(s.Job) ? "" : s.Job),
                PhaseGroupId = (byte)(s.PhaseGroupId ?? 0),
                PhaseId = s.Phase,
                CostTypeID = (byte)(s.CostType ?? 0),
                PostedDate = (DateTime)(s.PostedDate ?? DateTime.MinValue),
                ActualDate = s.ActualDate ?? DateTime.MinValue,
                JCTransType = s.JCTransType,
                Source = s.Source,
                Description = s.Description,
                BatchId = s.BatchId,
                InUseBatchId = s.InUseBatchId,
                GLCo = s.GLCo,
                GLTransAcct = s.GLTransAcct,
                GLOffsetAcct = s.GLOffsetAcct,
                UM = s.UM,
                ActualUnitCost = s.ActualUnitCost ?? 0,
                PerECM = s.PerECM,
                ActualHours = s.ActualHours ?? 0,
                ActualUnits = s.ActualUnits ?? 0,
                ActualCost = s.ActualCost ?? 0,
                ProgressCmplt = s.ProgressCmplt ?? 0,
                EstHours = s.EstHours ?? 0,
                EstUnits = s.EstUnits ?? 0,
                EstCost = s.EstCost ?? 0,
                ProjHours = s.ProjHours ?? 0,
                ProjUnits = s.ProjUnits ?? 0,
                ProjCost = s.ProjCost ?? 0,
                ForecastHours = s.ForecastHours ?? 0,
                ForecastUnits = s.ForecastUnits ?? 0,
                ForecastCost = s.ForecastCost ?? 0,
                PostedUM = s.PostedUM,
                PostedECM = s.PostedECM,
                DeleteFlag = s.DeleteFlag,
                AllocCode = s.AllocCode,
                ACO = s.ACO,
                ACOItem = s.ACOItem,
                PRCo = s.PRCo,
                Employee = s.Employee,
                Craft = s.Craft,
                Class = s.Class,
                Crew = s.Crew,
                EarnFactor = s.EarnFactor,
                EarnType = s.EarnType,
                Shift = s.Shift,
                LiabilityType = s.LiabilityType,
                VendorGroup = s.VendorGroup,
                Vendor = s.Vendor,
                APCo = s.APCo,
                APTrans = s.APTrans,
                APLine = s.APLine,
                APRef = s.APRef,
                PO = s.PO,
                POItem = s.POItem,
                SL = s.SL,
                SLItem = s.SLItem,
                MO = s.MO,
                MOItem = s.MOItem,
                MatlGroup = s.MatlGroup,
                Material = s.Material,
                INCo = s.INCo,
                Loc = s.Loc,
                INStdECM = s.INStdECM,
                INStdUM = s.INStdUM,
                MSTrans = s.MSTrans,
                MSTicket = s.MSTicket,
                JBBillStatus = s.JBBillStatus,
                JBBillMonth = s.JBBillMonth,
                JBBillNumber = s.JBBillNumber,
                EMCo = s.EMCo,
                EMEquip = s.EMEquip,
                EMRevCode = s.EMRevCode,
                EMGroup = s.EMGroup,
                EMTrans = s.EMTrans,
                TaxType = s.TaxType,
                TaxGroup = s.TaxGroup,
                TaxCode = s.TaxCode,
                UniqueAttchID = s.UniqueAttchID,
                SrcJCCo = s.SrcJCCo,
                OffsetGLCo = s.OffsetGLCo,
                POItemLine = s.POItemLine,
                udCrew = s.udCrew,
                SMWorkCompletedID = s.SMWorkCompletedID,
                SMCo = s.SMCo,
                SMWorkOrder = s.SMWorkOrder,
                SMScope = s.SMScope,
                PMCostProjection = s.PMCostProjection,
                udZionsID = s.udZionsID,
                udWEXID = s.udWEXID,
                udTicketId = s.udTicketId,
            }).ToList();
            results.ForEach(e => {
                e.CostType = costTypes.FirstOrDefault(f => f.CostTypeId == e.CostTypeID);
            });
            return results;
        }

    }
     
    public partial class JobPhaseTrack
    {
        public VPEntities db
        {
            get
            {

                var db = VPEntities.GetDbContextFromEntity(this);

                if (db == null && this.Job != null)
                    db = this.Job.db;

                if (db == null)
                    throw new NullReferenceException("GetDbContextFromEntity is null");

                return db;
            }
        }

        //private IEnumerable<DTJobPhase> _DailyJobTasks;
        
        //public IEnumerable<DTJobPhase> DailyJobTasks 
        //{ 
        //    get
        //    {
        //        if (_DailyJobTasks == null)
        //        {
        //            _DailyJobTasks = this.Job.DTJobPhases
        //                                            .Where(f => f.JobTicket.DailyTicket.tStatusId != (int)DB.DailyTicketStatusEnum.Canceled &&
        //                                                        f.JobTicket.DailyTicket.tStatusId != (int)DB.DailyTicketStatusEnum.Deleted &&
        //                                                        f.JobTicket.DailyTicket.tStatusId != (int)DB.DailyTicketStatusEnum.Report &&
        //                                                        f.JobTicket.DailyTicket.tStatusId != (int)DB.DailyTicketStatusEnum.Draft)
        //                                            .Where(f => f.ProgressPhaseId == PhaseId && f.PassId == PassId);
        //        }
        //        return _DailyJobTasks;
        //    }
        //}

        public List<DTJobPhase> DailyJobPhases
        {
            get
            {
                return this.Job.DTJobPhases.Where(f => f.JobTicket.DailyTicket.Status != DB.DailyTicketStatusEnum.Canceled &&
                                                    f.JobTicket.DailyTicket.Status != DB.DailyTicketStatusEnum.Deleted &&
                                                    f.JobTicket.DailyTicket.Status != DB.DailyTicketStatusEnum.Report &&
                                                    f.JobTicket.DailyTicket.Status != DB.DailyTicketStatusEnum.Draft &&
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