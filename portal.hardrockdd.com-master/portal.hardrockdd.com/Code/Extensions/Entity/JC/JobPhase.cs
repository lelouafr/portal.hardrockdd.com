using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class JobPhase
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
                        _db = Job.db;

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }

        public PhaseMaster GetPhaseMaster()
        {
            return this.db.PhaseMasters.FirstOrDefault(f => f.PhaseGroupId == this.PhaseGroupId && f.PhaseId == this.PhaseId);
        }

        public void AddMasterPhaseCosts()
        {
            var masterPhase = GetPhaseMaster();
            if (masterPhase != null)
                masterPhase.Costs.ToList().ForEach(e => this.AddMasterPhaseCost(e));
        }

        public JobPhaseCost AddMasterPhaseCost(PhaseMasterCost mstCost)
        {
            if (mstCost == null)
                return null;

            if (mstCost.PhaseId != this.PhaseId && mstCost.PhaseGroupId != PhaseGroupId)
                return null;

            var newCost = JobPhaseCosts.FirstOrDefault(f => f.CostTypeId == mstCost.CostTypeId);
            if (newCost == null)
            {

                newCost = new JobPhaseCost
                {
                    JCCo = JCCo,
                    JobId = JobId,
                    PhaseGroupId = (byte)Job.JCCompanyParm.HQCompanyParm.PhaseGroupId,
                    PhaseId = this.PhaseId,
                    CostTypeId = mstCost.CostTypeId,
                    UM = mstCost.UM,
                    BillFlag = mstCost.BillFlag,
                    ItemUnitFlag = mstCost.ItemUnitFlag,
                    PhaseUnitFlag = mstCost.PhaseUnitFlag,
                    BuyOutYN = "N",
                    Plugged = "N",
                    ActiveYN = this.ActiveYN,
                    OrigHours = 0,
                    OrigUnits = 0,
                    OrigCost = 0,
                    ProjNotes = null,
                    SourceStatus = "J",
                    JobPhase = this,
                    //UnitofMeasure = mstCost.u
                    CostType = mstCost.CostType,
                };

                JobPhaseCosts.Add(newCost);

            }
            return newCost;
        }

        public JobPhaseCost AddCostType(byte costTypeId)
        {
            var jobPhaseCost = JobPhaseCosts.FirstOrDefault(f => f.CostTypeId == costTypeId);
            if (jobPhaseCost == null)
            {

                //var mstPhase = Job.JCCompanyParm.HQCompanyParm.PhaseGroup.JCPhases.FirstOrDefault(f => f.PhaseId == PhaseId);
                var mstPhase = db.PhaseMasters.FirstOrDefault(f => f.PhaseGroupId == PhaseGroupId && f.PhaseId == PhaseId);
                if (mstPhase == null)
                    return null;

                var mstPhaseCostType = mstPhase.AddCostType(costTypeId);
                
                if (mstPhaseCostType.PhaseId != this.PhaseId && mstPhaseCostType.PhaseGroupId != PhaseGroupId)
                    return null;

                jobPhaseCost =  AddMasterPhaseCost(mstPhaseCostType);
            }

            return jobPhaseCost;
        }
    }

    public partial class PhaseMaster
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

                    //if (_db == null)
                    //    _db = Job.db;

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }

        public PhaseMasterCost AddCostType(byte costTypeId)
        {
            var mstPhaseCostType = Costs.FirstOrDefault(f => f.CostTypeId == costTypeId);
            if (mstPhaseCostType == null)
            {
                //var costType = HQGroup.JCCostTypes.FirstOrDefault(f => f.CostTypeId == costTypeId);
                var costType = db.CostTypes.FirstOrDefault(f => f.PhaseGroupId == PhaseGroupId && f.CostTypeId == costTypeId);
                mstPhaseCostType = new PhaseMasterCost
                {
                    CostType = costType,
                    CostTypeId = costType.CostTypeId,
                    PhaseGroupId = PhaseGroupId,
                    PhaseId = PhaseId,
                    Phase = this,
                    UM = "DYS",
                    ItemUnitFlag = "N",
                    PhaseUnitFlag = "N",
                    BillFlag ="C",
                };
                Costs.Add(mstPhaseCostType);
            }

            return mstPhaseCostType;
        }
    }
}