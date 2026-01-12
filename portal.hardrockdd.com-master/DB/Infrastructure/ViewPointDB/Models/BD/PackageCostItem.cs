using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class BidPackageCostItem
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
                _db ??= this.Package.db;
                _db ??= VPContext.GetDbContextFromEntity(this);
                return _db;
            }
        }

        public static string BaseTableName { get { return "budBDCI"; } }


        public string BudgetCodeId { get => tBudgetCodeId; set => UpdateBudgetCode(value); }

        public decimal Multiplier { get => tMultiplier ?? 0; set => UpdateMultiplier(value); }
        
        public byte Applied { get => tApplied ?? 0; set => UpdateApplied(value); }


        public void UpdateBudgetCode(string value)
        {
            if (tBudgetCodeId != null && BudgetCode == null)
            {
                var bgt = db.ProjectBudgetCodes.FirstOrDefault(f => f.BudgetCodeId == this.BudgetCodeId && f.PMCo == this.BDCo);
                if (bgt != null)
                {
                    BudgetCode = bgt;
                }
            }

            if (value != tBudgetCodeId)
            {
                UnApplyBudgetCode();
                var bgt = db.ProjectBudgetCodes.FirstOrDefault(f => f.BudgetCodeId == value && f.PMCo == this.BDCo);
                if (bgt != null)
                {
                    BudgetCode = bgt;
                    tBudgetCodeId = bgt.BudgetCodeId;
                }
                else
                {
                    BudgetCode = null;
                    tBudgetCodeId = null;
                }
                if (Applied >= 1)
                    ApplyBudgetCode();
            }
        }
        
        public void UpdateApplied(byte value)
        {
            if (value != tApplied)
            {
                tApplied = value;
                if (tApplied == 0 )
                {
                    UnApplyBudgetCode();
                }
                else
                {
                    ApplyBudgetCode();
                }
            }
        }

        public void UpdateMultiplier(decimal value)
        {
            if (value != tMultiplier)
            {
                tMultiplier = value;
                if (Applied >= 1)
                    ApplyBudgetCode();
            }
        }

        public void ApplyBudgetCode()
        {
            if (BudgetCode == null)
                return;

            foreach (var bore in this.Package.ActiveBoreLines)
            {
                bore.AddCostItems(BudgetCode).ForEach(e => {
                    e.Cost = BudgetCode.UnitCost;
                    e.Multiplier = (int)tMultiplier;
                });
            }
        }

        public void UnApplyBudgetCode()
        {
            if (BudgetCode == null)
                return;

            foreach (var bore in this.Package.ActiveBoreLines)
            {
                var costItems = bore.CostItems.Where(f => f.BudgetCodeId == this.tBudgetCodeId && f.IsPackageCost != true).ToList();
                costItems.ForEach(e => bore.CostItems.Remove(e));
            }

        }
    }
}
