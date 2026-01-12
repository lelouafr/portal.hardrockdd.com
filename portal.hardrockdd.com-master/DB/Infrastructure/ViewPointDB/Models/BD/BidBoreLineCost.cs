using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public class BidBoreLineCost
    {
        public BidBoreLineCost()
        {

        }

        public BidBoreLineCost(Infrastructure.ViewPointDB.Data.BidBoreLine line, Infrastructure.ViewPointDB.Data.BidBoreLineCostItem item, int shiftNum)
        {
            if (line == null)
            {
                throw new System.ArgumentNullException(nameof(line));
            }
            if (item == null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }
            #region mapping
            BDCo = line.BDCo;
            BidId = line.BidId;
            BoreId = line.BoreId;
            Description = line.Description;
            Footage = line.Footage;
            PipeSize = line.PipeSize;
            GroundDensityType = item.GroundDensityId == 0 ? "Dirt" : "Rock";
            GroundDensity = item.GroundDensity;
            BudgetCode = item.BudgetCode;

            BudgetCategory = item.BudgetCode.CostType?.Description;
            if (shiftNum > 1)
            {
                BudgetCodeDescription = item.BudgetCode.Description + string.Format(" Shift# {0}", shiftNum);
            }
            BudgetCodeId = item.BudgetCodeId;
            PhaseId = item.BudgetCode.PhaseId;
            PhaseMaster = item.BudgetCode.Phase;
            PassId = 1;
            Units = item.Units ?? 0;
            Multiplier = item.Multiplier ?? 1;
            Cost = item.Cost ?? 0;

            ExtUnits = Units * Multiplier;
            ExtCost = Math.Round(Units * Cost * Multiplier, 0);

            if (item.ItemPhases.Any())
            {
                ExtCost = 0;
                ExtUnits = 0;
                foreach (var phase in item.ItemPhases)
                {
                    ExtUnits += (phase.Units ?? 0) * (item.Multiplier ?? 1);
                    ExtCost += (phase.Units ?? 0) * (phase.Cost ?? 0) * (item.Multiplier ?? 1);
                }
            }
            UM = item.BudgetCode.UM;

            IntersectBoreId = line.IntersectBoreId;

            #endregion mapping

        }

        public BidBoreLineCost(Infrastructure.ViewPointDB.Data.BidBoreLine line, Infrastructure.ViewPointDB.Data.BidBoreLineCostItemPhase item, int shiftNum)
        {
            if (line == null)
                return;
            if (item == null)
                return;

            #region mapping
            BDCo = line.BDCo;
            BidId = line.BidId;
            BoreId = line.BoreId;
            Description = line.Description;
            Footage = line.Footage;
            PipeSize = line.PipeSize;
            GroundDensityType = item.GroundDensityId == 0 ? "Dirt" : "Rock";

            GroundDensity = item.GroundDensity;
            BudgetCode = item.CostItem.BudgetCode;

            if (shiftNum > 1)
            {
                BudgetCodeDescription = item.CostItem.BudgetCode.Description + string.Format(" Shift# {0}", shiftNum);
            }

            BudgetCodeId = item.CostItem.BudgetCodeId;
            PhaseId = item.PhaseId;
            PhaseMaster = item.PhaseMaster;
            PassId = item.PassId;
            Units = item.Units ?? 0;
            Multiplier = item.CostItem.Multiplier ?? 1;
            Cost = item.CostItem.Cost ?? 0;

            ExtUnits = Units * Multiplier;
            ExtCost = Math.Round(Units * Cost * Multiplier, 0);
            UM = item.CostItem.BudgetCode.UM;
            IntersectBoreId = line.IntersectBoreId;
            #endregion mapping

        }

        public byte BDCo { get; set; }

        public int BidId { get; set; }

        public int BoreId { get; set; }

        public int? IntersectBoreId { get; set; }

        public string Description { get; set; }

        public decimal? Footage { get; set; }

        public decimal? PipeSize { get; set; }

        public string GroundDensityType { get; set; }

        public string BudgetCategory { get; set; }

        public string BudgetCodeId { get; set; }

        public string BudgetCodeDescription { get; set; }

        public string PhaseId { get; set; }

        public int PassId { get; set; }

        public decimal Units { get; set; }

        public decimal ExtUnits { get; set; }

        public decimal Multiplier { get; set; }

        public string UM { get; set; }

        public decimal Cost { get; set; }

        public decimal ExtCost { get; set; }


        public decimal? IntersectFootage { get; set; }


        public decimal? IntersectFootage2 { get; set; }

        public virtual ProjectBudgetCode BudgetCode { get; set; }
        public virtual PhaseMaster PhaseMaster { get; set; }
        public virtual BidBoreLineCostItem CostItem { get; set; }
        public virtual GroundDensity GroundDensity { get; set; }
    }
}
