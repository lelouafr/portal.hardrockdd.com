using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class PMCompanyParm
    {
        public ProjectBudgetCode AddBudgetCode(ProjectBudgetCode srcBudgetCode)
        {
            var budgetCode = BudgetCodes.FirstOrDefault(f => f.BudgetCodeId == srcBudgetCode.BudgetCodeId);

            if (budgetCode == null)
            {
                budgetCode = new ProjectBudgetCode()
                {
                    PMCompanyParm = this,
                    PMCo = this.PMCo,
                    BudgetCodeId = srcBudgetCode.BudgetCodeId,
                    Description = srcBudgetCode.Description,
                    Active = srcBudgetCode.Active,
                    CostLevel = srcBudgetCode.CostLevel,
                    PhaseGroupId = srcBudgetCode.PhaseGroupId,
                    PhaseId = srcBudgetCode.PhaseId,
                    CostTypeId = srcBudgetCode.CostTypeId,
                    UM = srcBudgetCode.UM,
                    UnitCost = srcBudgetCode.UnitCost,
                    HrsPerUnit = srcBudgetCode.HrsPerUnit,
                    Percentage = srcBudgetCode.Percentage,
                    Notes = srcBudgetCode.Notes,
                    Basis = srcBudgetCode.Basis,
                    TimeUM = srcBudgetCode.TimeUM,
                    Rate = srcBudgetCode.Rate,
                    ExcludeFromLookups = srcBudgetCode.ExcludeFromLookups,
                    Radius = srcBudgetCode.Radius,
                    Hardness = srcBudgetCode.Hardness,
                    RockOnly = srcBudgetCode.RockOnly,
                    Category = srcBudgetCode.Category,
                    udGroundDensityId = srcBudgetCode.udGroundDensityId,

                    CostType = srcBudgetCode.CostType,
                };

                BudgetCodes.Add(budgetCode);
            }

            return budgetCode;
        }

        public ProjectBudgetCode AddBudgetCode(string srcBudgetCodeId)
        {
            var budgetCode = BudgetCodes.FirstOrDefault(f => f.BudgetCodeId == srcBudgetCodeId);
            if (budgetCode == null && srcBudgetCodeId != null)
            {
                var db = VPEntities.GetDbContextFromEntity(this);
                if (db == null)
                {
                    return null;
                }
                var srcBgtCode = db.ProjectBudgetCodes.FirstOrDefault(f => f.PMCo == 1 && f.BudgetCodeId == srcBudgetCodeId);
                budgetCode = AddBudgetCode(srcBgtCode);
            }
            return budgetCode;
        }

    }

    public partial class ProjectBudget
    { 
        public ProjectBudgetLine AddBudgetLine(BidBoreLineCostItemPhase item)
        {
            var bidPass = item.CostItem.BoreLine.Passes.FirstOrDefault(f => f.PassId == item.PassId && f.PhaseId == item.PhaseId && f.GroundDensityId == item.GroundDensityId && f.Deleted != true);
            var description = item.CostItem.BudgetCode.Description;
            if (item.PassId > 1)
            {
                description = string.Format(AppCultureInfo.CInfo(), "{0} Pass {1}", item.CostItem.BudgetCode.Description, item.PassId);
            }
            if (bidPass != null)
            {
                if (bidPass.BoreSize != 0)
                {
                    description = string.Format(AppCultureInfo.CInfo(), "{0} Bore Size {1:N3}", item.CostItem.BudgetCode.Description, bidPass.BoreSize);
                }
            }

            var newLine = new ProjectBudgetLine
            {
                PMCo = PMCo,
                ProjectId = ProjectId,
                BudgetNo = BudgetNo,
                SeqId = BudgetLines.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,

                Budget = this,
                GroupNo = item.CostItem.LineId,
                LineId = item.PassId,
                CostLevel = "D",
                BudgetCodeId = item.CostItem.BudgetCodeId,
                Description = description,
                PhaseGroupId = (byte)PMCompanyParm.HQCompanyParm.PhaseGroupId,
                PhaseId = item.PhaseId,
                CostTypeId = item.CostItem.BudgetCode.CostTypeId,
                Units = item.Units * item.CostItem.Multiplier,
                UM = item.CostItem.BudgetCode.UM,
                UnitCost = item.CostItem.Cost ?? item.CostItem.BudgetCode.UnitCost,
                BoreSize = bidPass?.BoreSize ?? 0,
                ProductionRate = bidPass?.ProductionRate ?? 0,
            };
            newLine.Amount = newLine.Units * newLine.UnitCost;
            PMCompanyParm.AddBudgetCode(item.CostItem.BudgetCode);
            BudgetLines.Add(newLine);

            return newLine;
        }

        public ProjectBudgetLine AddBudgetLine(BidBoreLineCostItem item)
        {            
            var newLine = new ProjectBudgetLine
            {
                PMCo = PMCo,
                ProjectId = ProjectId,
                BudgetNo = BudgetNo,
                SeqId = BudgetLines.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,

                GroupNo = item.LineId,
                LineId = item.LineId,
                CostLevel = "D",
                BudgetCodeId = item.BudgetCodeId,
                Description = item.BudgetCode.Description,
                PhaseGroupId = item.BudgetCode.PhaseGroupId ?? (byte)PMCompanyParm.HQCompanyParm.PhaseGroupId,
                PhaseId = item.BudgetCode.PhaseId,
                CostTypeId = item.BudgetCode.CostTypeId,
                Units = item.Units * item.Multiplier,
                UM = item.BudgetCode.UM,
                UnitCost = item.Cost ?? item.BudgetCode.UnitCost,
                Budget = this,
            };
            newLine.Amount = newLine.Units * newLine.UnitCost;
            PMCompanyParm.AddBudgetCode(item.BudgetCode);
            BudgetLines.Add(newLine);

            return newLine;
        }

        public void CreateBudgetLines(BidBoreLine bore, int groundDensityId)
        {

            var costItems = bore.CostItems.Where(f => f.GroundDensityId == groundDensityId && f.BudgetCode != null && f.Cost != null && f.Units != null).ToList();
            foreach (var costItem in costItems)
            {
                if (costItem.ItemPhases.Any())
                {
                    var costItemPhases = costItem.ItemPhases.ToList();
                    foreach (var itemPhase in costItemPhases)
                    {
                        var bidPass = bore.Passes.FirstOrDefault(f => f.PassId == itemPhase.PassId && f.PhaseId == itemPhase.PhaseId && f.GroundDensityId == itemPhase.GroundDensityId && f.Deleted != true);
                        var bgtLine = BudgetLines.FirstOrDefault(f => f.GroupNo == costItem.LineId &&
                                                                      f.LineId == itemPhase.PassId &&
                                                                      f.BudgetCodeId == costItem.BudgetCodeId &&
                                                                      f.PhaseId == itemPhase.PhaseId &&
                                                                      f.CostTypeId == costItem.BudgetCode.CostTypeId
                                                                      );
                        if (bgtLine == null)
                        {
                            bgtLine = AddBudgetLine(itemPhase);
                        }
                        else
                        {
                            bgtLine.Units = itemPhase.Units * itemPhase.CostItem.Multiplier;
                            bgtLine.UnitCost = itemPhase.CostItem.Cost ?? itemPhase.CostItem.BudgetCode.UnitCost;
                            bgtLine.BoreSize = bidPass?.BoreSize ?? 0;
                            bgtLine.ProductionRate = bidPass?.ProductionRate ?? 0;
                        }
                    }
                }
                else
                {
                    var bgtLine = BudgetLines.FirstOrDefault(f => f.GroupNo == costItem.LineId &&
                                                                      f.LineId == costItem.LineId &&
                                                                      f.BudgetCodeId == costItem.BudgetCodeId &&
                                                                      f.PhaseId == costItem.BudgetCode.PhaseId &&
                                                                      f.CostTypeId == costItem.BudgetCode.CostTypeId
                                                                      );

                    if (bgtLine == null)
                    {
                        bgtLine = AddBudgetLine(costItem);
                    }
                    else
                    {
                        bgtLine.Units = costItem.Units * costItem.Multiplier;
                        bgtLine.UnitCost = costItem.Cost ?? costItem.BudgetCode.UnitCost;
                    }
                }
            }
        }
    }

    public partial class ProjectBudgetLine
    {

    }

    public partial class ProjectBudgetCode
    {
        public string Prefix 
        {
            get
            {
                if (BudgetCodeId.Length >= 2)
                {
                    return BudgetCodeId.Substring(0, 2);
                }
                else
                {
                    return BudgetCodeId;
                }
            } 
        }
    }
}