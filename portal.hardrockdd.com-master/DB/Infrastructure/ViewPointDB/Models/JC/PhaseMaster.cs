using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class PhaseMaster
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
                _db ??= VPContext.GetDbContextFromEntity(this);
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
                    BillFlag = "C",
                };
                Costs.Add(mstPhaseCostType);
            }

            return mstPhaseCostType;
        }

        public bool HasBudgetRadius()
        {
            return db.ProjectBudgetCodes.Where(f => f.PhaseGroupId == this.PhaseGroupId && f.PhaseId == this.PhaseId && (f.Radius ?? 0) != 0).Any();
        }
    }
}
