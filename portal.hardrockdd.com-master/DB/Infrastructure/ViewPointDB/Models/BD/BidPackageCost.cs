using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class BidPackageCost
    {

        public byte? CostAllocationTypeId
        {
            get
            {
                return tCostAllocationTypeId;
            }
            set
            {
                if (value != tCostAllocationTypeId)
                {
                    tCostAllocationTypeId = value;
                    if (tCostAllocationTypeId != null)
                        Package.ApplyPackageCost();
                }
            }
        }

        public BDPackageCostAllocationType? CostAllocationType
        {
            get
            {
                return (BDPackageCostAllocationType?)tCostAllocationTypeId;
            }
            set
            {
                if ((BDPackageCostAllocationType?)tCostAllocationTypeId != value)
                {
                    CostAllocationTypeId = (byte?)value;
                    Package.ApplyPackageCost();
                }
            }
        }

        public string BudgetCodeId
        {
            get
            {
                return tBudgetCodeId;
            }
            set
            {
                if (value != tBudgetCodeId)
                {
                    UpdateBudgetCode(value);
                }
            }
        }

        public decimal? Cost
        {
            get
            {
                return tCost;
            }
            set
            {
                if (tCost != value)
                {
                    var costItems = Package.ActiveBoreLines
                        .SelectMany(s => s.CostItems)
                        .Where(f => f.IsPackageCost == true && f.BudgetCodeId == BudgetCodeId)
                        .ToList();

                    tCost = value;
                    costItems.ForEach(e => e.Cost = 0);
                    Package.ApplyPackageCost();
                }
            }
        }

        //public void UpdateFromModel(Models.Views.Bid.Forms.Package.Setup.AllocatedCostViewModel model)
        //{
        //    if (model == null)
        //        return;

        //    CostAllocationType = model.AllocationType;
        //    BudgetCodeId = model.BudgetCodeId;
        //    Cost = model.Cost;
        //}

        private void UpdateBudgetCode(string value)
        {
            var costItems = Package.ActiveBoreLines
                .SelectMany(s => s.CostItems)
                .Where(f => f.IsPackageCost == true && f.BudgetCodeId == BudgetCodeId)
                .ToList();

            tBudgetCodeId = value;
            costItems.ForEach(e => e.BudgetCodeId = BudgetCodeId);

            Package.ApplyPackageCost();
        }
    }
}