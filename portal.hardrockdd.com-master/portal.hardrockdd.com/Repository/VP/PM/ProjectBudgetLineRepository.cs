using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.PM
{
    public partial class ProjectBudgetLineRepository : IDisposable
    {
        private VPContext db = new VPContext();        

        public ProjectBudgetLineRepository()
        {

        }
        
        public static ProjectBudgetLine Init(ProjectBudget bgt, BidBoreLineCostItem cost)
        {
            if (cost == null)
            {
                throw new ArgumentNullException(nameof(cost));
            }
            if (bgt == null)
            {
                throw new ArgumentNullException(nameof(bgt));
            }

            var model = new ProjectBudgetLine
            {
                PMCo = bgt.PMCo,
                ProjectId = bgt.ProjectId,
                BudgetNo = bgt.BudgetNo,
                GroupNo = cost.LineId,
                LineId = cost.LineId,
                CostLevel = "D",
                BudgetCodeId = cost.BudgetCodeId,
                Description = cost.BudgetCode.Description,
                PhaseGroupId = cost.BudgetCode.PhaseGroupId ?? (byte)bgt.PMCompanyParm.HQCompanyParm.PhaseGroupId,
                PhaseId = cost.BudgetCode.PhaseId,
                CostTypeId = cost.BudgetCode.CostTypeId,
                Units = cost.Units * cost.Multiplier,
                UM = cost.BudgetCode.UM,
                UnitCost = cost.Cost ?? cost.BudgetCode.UnitCost,
                Budget = bgt,

            };
            model.Amount = model.Units * model.UnitCost;
            return model;
        }

        public static ProjectBudgetLine Init(ProjectBudget bgt, BidBoreLineCostItemPhase itemPhase)
        {
            if (itemPhase == null)
            {
                throw new ArgumentNullException(nameof(itemPhase));
            }
            if (bgt == null)
            {
                throw new ArgumentNullException(nameof(bgt));
            }
            var bidPass = itemPhase.CostItem.BoreLine.Passes.FirstOrDefault(f => f.PassId == itemPhase.PassId && f.PhaseId == itemPhase.PhaseId && f.GroundDensityId == itemPhase.GroundDensityId && f.Deleted != true);
            var description = itemPhase.CostItem.BudgetCode.Description;
            if (itemPhase.PassId > 1)
            {
                description = string.Format(AppCultureInfo.CInfo(), "{0} Pass {1}", itemPhase.CostItem.BudgetCode.Description, itemPhase.PassId);
            }
            if (bidPass != null)
            {
                if (bidPass.BoreSize != 0)
                {
                    description = string.Format(AppCultureInfo.CInfo(), "{0} Bore Size {1}", itemPhase.CostItem.BudgetCode.Description, bidPass.BoreSize);
                }
            }

            var model = new ProjectBudgetLine
            {
                PMCo = bgt.PMCo,
                ProjectId = bgt.ProjectId,
                BudgetNo = bgt.BudgetNo,
                Budget = bgt,
                GroupNo = itemPhase.CostItem.LineId,
                LineId = itemPhase.PassId,
                CostLevel = "D",
                BudgetCodeId = itemPhase.CostItem.BudgetCodeId,
                Description = description,
                PhaseGroupId = (byte)bgt.PMCompanyParm.HQCompanyParm.PhaseGroupId,
                PhaseId = itemPhase.PhaseId,
                CostTypeId = itemPhase.CostItem.BudgetCode.CostTypeId,
                Units = itemPhase.Units * itemPhase.CostItem.Multiplier,
                UM = itemPhase.CostItem.BudgetCode.UM,
                UnitCost = itemPhase.CostItem.Cost ?? itemPhase.CostItem.BudgetCode.UnitCost,
                BoreSize = bidPass?.BoreSize ?? 0,
                ProductionRate = bidPass?.ProductionRate ?? 0,
            };
            model.Amount = model.Units * model.UnitCost;
            return model;
        }

        public static void GenerateFromBore(BidBoreLine bore, ProjectBudget bgt, int groundDensityId, VPContext db, bool verboseSave = false)
        {
            if (bgt == null)
            {
                throw new System.ArgumentNullException(nameof(bgt));
            }
            if (bore == null)
            {
                throw new System.ArgumentNullException(nameof(bore));
            }
            var seq = bgt.BudgetLines.DefaultIfEmpty().Max(f => f == null ? 0 : f.SeqId) + 1;
           

            foreach (var cost in bore.CostItems.Where(f => f.GroundDensityId == groundDensityId).ToList())
            {

                if (cost.ItemPhases.Count == 0)
                {
                    var bgtLine = bgt.BudgetLines.FirstOrDefault(f => f.GroupNo == cost.LineId &&
                                                                      f.LineId == cost.LineId &&
                                                                      f.BudgetCodeId == cost.BudgetCodeId &&
                                                                      f.PhaseId == cost.BudgetCode.PhaseId &&
                                                                      f.CostTypeId == cost.BudgetCode.CostTypeId
                                                                      );
                    if (bgtLine == null)
                    {
                        bgtLine = Init(bgt, cost);
                        bgtLine.SeqId = seq;
                        seq++;
                        bgt.BudgetLines.Add(bgtLine);
                    }
                    else
                    {
                        bgtLine.Units = cost.Units * cost.Multiplier;
                        bgtLine.UnitCost = cost.Cost ?? cost.BudgetCode.UnitCost;
                        //bgtLine.BoreSize = 
                    }
                    if (verboseSave)
                        db.SaveChanges();
                }
                else
                {
                    foreach (var itemPhase in cost.ItemPhases)
                    {
                        var bidPass = bore.Passes.FirstOrDefault(f => f.PassId == itemPhase.PassId && f.PhaseId == itemPhase.PhaseId && f.GroundDensityId == itemPhase.GroundDensityId && f.Deleted != true);
                        var bgtLine = bgt.BudgetLines.FirstOrDefault(f => f.GroupNo == cost.LineId &&
                                                                      f.LineId == itemPhase.PassId &&
                                                                      f.BudgetCodeId == cost.BudgetCodeId &&
                                                                      f.PhaseId == itemPhase.PhaseId &&
                                                                      f.CostTypeId == cost.BudgetCode.CostTypeId
                                                                      );
                        if (bgtLine == null)
                        {
                            bgtLine = Init(bgt, itemPhase);
                            bgtLine.SeqId = seq;

                            bgtLine.BoreSize = bidPass?.BoreSize ?? 0;
                            bgtLine.ProductionRate = bidPass?.ProductionRate ?? 0;
                            seq++;
                            bgt.BudgetLines.Add(bgtLine);
                        }
                        else
                        {
                            bgtLine.Units = itemPhase.Units * itemPhase.CostItem.Multiplier;
                            bgtLine.UnitCost = itemPhase.CostItem.Cost ?? itemPhase.CostItem.BudgetCode.UnitCost;
                            bgtLine.BoreSize = bidPass?.BoreSize ?? 0;
                            bgtLine.ProductionRate = bidPass?.ProductionRate ?? 0;
                        }
                        if (verboseSave)
                            db.SaveChanges();
                    }
                }
            }


            //return newList;
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ProjectBudgetLineRepository()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
            }
        }
    }
}
