using System.Linq;

namespace portal.Code.Data.VP
{
    public static class PhaseMasterExt
    {
        public static bool HasBudgetRadius(this PhaseMaster phase)
        {
            if (phase == null)
                return false;

            using var db = new VPEntities();
            return db.ProjectBudgetCodes.Where(f => f.PhaseGroupId == phase.PhaseGroupId && f.PhaseId == phase.PhaseId && (f.Radius ?? 0) != 0).Any();
        }

        public static bool HasBudgetRadius(this JobPhase phase)
        {
            if (phase == null)
                return false;
            
            using var db = new VPEntities();
            return db.ProjectBudgetCodes.Where(f => f.PhaseGroupId == phase.PhaseGroupId && f.PhaseId == phase.PhaseId && (f.Radius ?? 0) != 0).Any();
        }
    }
}