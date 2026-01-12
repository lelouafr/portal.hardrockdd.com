using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class BidBoreLinePass
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
                        _db = this.BoreLine.db;

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }
        public DB.BidProductionCalEnum ProductionCalType
        {
            get
            {
                return (DB.BidProductionCalEnum)(ProductionCalTypeId ?? 0);
            }
            set
            {
                if (value != (DB.BidProductionCalEnum)(ProductionCalTypeId ?? 0))
                {
                    ProductionCalTypeId = (int)value;
                    BoreLine.RecalcNeeded = true;
                }
            }
        }

        public decimal? ProductionValue
        {
            get
            {
                return ProductionCalType switch
                {
                    DB.BidProductionCalEnum.Rate => ProductionRate ?? 0,
                    DB.BidProductionCalEnum.Days => ProductionDays ?? 0,
                    _ => 0,
                };
            }
            set
            {
                var originalVal = ProductionValue;
                value ??= 0;
                if (originalVal != value)
                {
                    BoreLine.RecalcNeeded = true;
                    switch (ProductionCalType)
                    {
                        case DB.BidProductionCalEnum.Rate:
                            ProductionRate = value;
                            if (value == 0)
                                ProductionDays = 0;
                            else
                                ProductionDays = BoreLine.Footage / value;
                            break;
                        case DB.BidProductionCalEnum.Days:
                            ProductionDays = value;
                            if (value == 0)
                                ProductionRate = 0;
                            else
                                ProductionRate = BoreLine.Footage / value;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public decimal? BoreSize
        {
            get
            {
                return tBoreSize;
            }
            set
            {
                if (tBoreSize != value)
                {
                    tBoreSize = value;
                    UpdateBudgetCode(true);
                    UpdateToolingCostLine();
                }
            }
        }

        public decimal? Duration
        {
            get
            {
                return ProductionCalType switch
                {
                    DB.BidProductionCalEnum.Rate => ProductionDays,
                    DB.BidProductionCalEnum.Days => BoreLine.Footage == 0 || ProductionRate == 0 ? 0 : BoreLine.Footage / ProductionRate,
                    _ => 0,
                };
            }
            set
            {
                switch (ProductionCalType)
                {
                    case DB.BidProductionCalEnum.Rate:
                        ProductionRate = value == 0 ? 0 : BoreLine.Footage / value;
                        break;
                    case DB.BidProductionCalEnum.Days:
                        ProductionDays = value;
                        break;
                    default:
                        break;
                }
            }
        }

        public string Description
        {
            get
            {
                var desc = PhaseMaster?.Description ?? string.Empty;
                if (PassId > 1)
                {
                    desc = string.Format("{0} pass {1}", desc, PassId);
                }
                return desc;
            }
        }

        public void UpdateBudgetCode(bool forceUpdate = false)
        {
            if (BudgetCodeId == null || forceUpdate)
            {
                //var db = VPEntities.GetDbContextFromEntity(this);;
                if (db != null)
                {
                    var pmco = BoreLine.Job?.JCCo ?? BDCo;
                    var bgtCode = db.ProjectBudgetCodes
                                    .OrderBy(o => (o.Radius ?? 0))
                                    .FirstOrDefault(f => f.PMCo == pmco &&
                                                         f.BudgetCodeId.Contains("BC-") &&
                                                         f.PhaseId == f.PhaseId &&
                                                         (f.Radius ?? 0) == (BoreSize ?? 0));
                    if (bgtCode != null)
                    {
                        BudgetCodeId = bgtCode.BudgetCodeId;
                    }
                }
            }
        }

        public void UpdateToolingCostLine()
        {
            var groundDensityStr = GroundDensityId.ToString(AppCultureInfo.CInfo());
            var bgt = BoreLine
                        .Bid
                        .Company
                        .PMCompanyParm
                        .BudgetCodes.FirstOrDefault(f => f.PhaseId == PhaseId &&
                                                        f.Hardness == groundDensityStr &&
                                                        f.BudgetCodeId.Contains("TM-") &&
                                                        f.Radius == BoreSize);
            if (bgt != null)
            {
                var bgtItem = ToolingCost;
                if (bgtItem == null)
                {
                    bgtItem = new BidBoreLineCostItem
                    {
                        BDCo = BoreLine.BDCo,
                        BidId = BoreLine.BidId,
                        BoreId = BoreLine.BoreId,
                        GroundDensityId = GroundDensityId,
                        LineId = BoreLine.CostItems.Where(f => f.GroundDensityId == GroundDensityId).DefaultIfEmpty().Max(f => f == null ? 0 : f.LineId) + 1,
                        BudgetCodeId = bgt.BudgetCodeId,
                        Cost = bgt.UnitCost,
                        Multiplier = 1,
                        IsPackageCost = false,
                        Units = BoreLine.Footage,
                        CurUnits = BoreLine.Footage,
                    };
                    BoreLine.CostItems.Add(bgtItem);
                    ToolingCost = bgtItem;
                }
                else
                {
                    bgtItem.BudgetCodeId = bgt.BudgetCodeId;
                    bgtItem.Units = BoreLine.Footage;
                }
            }
        }
    }
}