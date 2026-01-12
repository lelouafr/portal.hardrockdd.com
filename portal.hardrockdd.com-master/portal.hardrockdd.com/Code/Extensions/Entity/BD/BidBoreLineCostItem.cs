using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class BidBoreLineCostItem
    {
        public void ReCalulateCostItem()
        {
            if ((Cost ?? 0) == 0 || (Cost != BudgetCode.UnitCost && BudgetCode.UnitCost != 0M))
            {
                Cost = BudgetCode.UnitCost;
            }
            var skipCostitem = (GroundDensityId == 0 && BudgetCode.RockOnly == "Y");
            //skipCostitem |= (GroundDensityId != 0 && BudgetCode.Description.Contains("Dirt Tooling"));
            var isTooling = BudgetCode.Description.Contains("Dirt Tooling");
            //if (!(GroundDensityId == 0 && BudgetCode.RockOnly == "Y") &&
            //    !(GroundDensityId != 0 && BudgetCode.Description == "Dirt Tooling"))
            if (isTooling)
            {
                return;
            }

            if (!skipCostitem)
            {
                if (BudgetCode.UM == "BBL")
                {
                    using var db = new VPEntities();
                    var footage = BoreLine.Footage;
                    var curFootage = BoreLine.CurFootage ?? BoreLine.Footage;
                    var radius = (double)((BoreLine.PipeSize ?? 0) / 12M) / 2;
                    var CYUoM = db.UnitofMeasures.Where(f => f.UM == "CY").FirstOrDefault();
                    var CFtoCYFactor = CYUoM.Conversions.FirstOrDefault(f => f.ToUM == "CF");
                    var CYFactor = (double)CFtoCYFactor.Factor;

                    var volCF = Math.Pow(radius, 2) * (double)(footage ?? 0) * Math.PI;
                    var volCY = volCF / CYFactor;

                    var volCurCF = Math.Pow(radius, 2) * (double)(curFootage ?? 0) * Math.PI;
                    var volCurCY = volCurCF / CYFactor;

                    var factor = (double)BudgetCode.UnitofMeasure.Conversions.FirstOrDefault(f => f.ToUM == "CY").Factor;
                    if (BoreLine.Bid.Status <= DB.BidStatusEnum.ContractApproval)
                    {
                        Units = (decimal)(volCY / factor);
                        CurUnits = (decimal)(volCY / factor);
                    }
                    else
                    {
                        Units = (decimal)(volCY / factor);
                        CurUnits = (decimal)(volCurCY / factor);
                    }
                }
                if (BudgetCode.UM == "LF")
                {
                    var footage = BoreLine.Footage;
                    var curFootage = BoreLine.CurFootage;
                    Units = footage;
                    if (BoreLine.Bid.Status <= DB.BidStatusEnum.ContractApproval)
                        CurUnits = footage;
                    else
                        CurUnits = curFootage;

                }
                if (BudgetCode.UM == "EA")
                {
                    //Units = 1;
                }
                if (BudgetCode.UM == "DYS" || BudgetCode.UM == "MOS")
                {
                    decimal days = 0;
                    decimal curDays = 0;
                    var bgtPhases = BudgetCode.Phases.ToList();
                    var costPhases = BoreLine.Passes.Where(f => bgtPhases.Any(w => w.PhaseId == f.PhaseId) &&
                                                                f.GroundDensityId == GroundDensityId
                                                                ).ToList();
                    foreach (var pass in costPhases)
                    {
                        var itemPhase = ItemPhases.FirstOrDefault(f => f.PhaseId == pass.PhaseId && f.PassId == pass.PassId);
                        if (pass.Deleted == true)
                        {
                            if (itemPhase != null)
                            {
                                itemPhase.Cost = 0;
                                itemPhase.Multiplier = 0;
                                itemPhase.Shift = 0;
                                itemPhase.Units = 0;
                            }
                        }
                        else
                        {
                            pass.Multiplier ??= 1;
                            decimal tmpDay = 0;
                            decimal curTmpDay = 0;

                            if (pass.ProductionCalTypeId == (int)DB.BidProductionCalEnum.Rate)
                            {
                                var productionRate = pass.ProductionRate ?? 0;
                                var footage = BoreLine.Footage ?? 0;
                                var curfootage = BoreLine.CurFootage ?? footage;

                                if (productionRate == 0 || footage == 0)
                                {
                                    tmpDay = 0;
                                    curTmpDay = 0;
                                }
                                else
                                {
                                    for (int i = 1; i < pass.ShiftCnt; i++)
                                    {
                                        productionRate += productionRate * (pass.Shift2ProductionPerc ?? 0);
                                    }
                                    tmpDay = footage / productionRate;
                                    curTmpDay = curfootage / productionRate;
                                }
                            }
                            else if (pass.ProductionCalTypeId == (int)DB.BidProductionCalEnum.Days)
                            {
                                var productionDays = pass.ProductionDays ?? 0;

                                tmpDay = productionDays;
                                curTmpDay = productionDays;
                            }

                            if (BudgetCode.UM == "MOS")
                            {
                                tmpDay /= 30;
                                curTmpDay /= 30;
                            }

                            tmpDay = Math.Round(tmpDay, 6);
                            curTmpDay = Math.Round(curTmpDay, 6);

                            //Find Create Item Phase Calculation
                            if (itemPhase == null)
                            {
                                itemPhase = new BidBoreLineCostItemPhase
                                {
                                    BDCo = BDCo,
                                    BidId = BidId,
                                    BoreId = BoreId,
                                    GroundDensityId = GroundDensityId,
                                    LineId = LineId,
                                    Locked = "N",
                                    PhaseId = pass.PhaseId,
                                    PassId = pass.PassId,
                                    Units = tmpDay
                                };
                                ItemPhases.Add(itemPhase);
                            }
                            itemPhase.Cost = Cost;
                            itemPhase.Multiplier = 1;
                            itemPhase.Shift = pass.ShiftCnt ?? 1;
                            itemPhase.Units = tmpDay;
                            if (BoreLine.Bid.Status <= DB.BidStatusEnum.ContractApproval)
                                itemPhase.CurUnits = tmpDay;
                            else
                                itemPhase.CurUnits = curTmpDay;

                            //sum total the pass duration
                            days += tmpDay;
                            curDays += curTmpDay;
                        }
                    }

                    days = Math.Round(days, 5);
                    curDays = Math.Round(curDays, 5);
                    if (Units != days || CurUnits != curDays)
                    {
                        Units = days;
                        if (BoreLine.Bid.Status <= DB.BidStatusEnum.ContractApproval)
                            CurUnits = days;
                        else
                            CurUnits = curDays;
                    }

                    if (BoreLine.Package.RoundingPhaseId != null)
                    {
                        var rndPhase = ItemPhases.FirstOrDefault(f => f.PhaseId == BoreLine.Package.RoundingPhaseId);
                        if (rndPhase == null)
                        {
                            rndPhase = ItemPhases.FirstOrDefault();
                        }
                        if (rndPhase != null)
                        {
                            var roundto = BoreLine.Package.TotalDayRounding ?? 0;
                            var units = Units ?? 0;
                            var result = units;
                            if (roundto != 0)
                            {
                                result = Math.Ceiling(units / roundto) * roundto;
                            }
                            var dif = result - Units;
                            Units = result;
                            rndPhase.Units += dif;
                        }
                    }
                }


                foreach (var costPhase in ItemPhases.ToList())
                {
                    if (!BudgetCode.Phases.Any(f => f.PhaseId == costPhase.PhaseId))
                    {
                        ItemPhases.Remove(costPhase);
                    }
                }

                //this.BoreLine.Bid.db.SaveChanges();
            }

            var costItems = BoreLine.CostItems.Where(f => f.BudgetCodeId == BudgetCodeId && f.IsPackageCost != true).ToList();
            foreach (var cost in costItems.GroupBy(g => g.GroundDensityId).Select(s => new { GroundDensityId = s.Key, items = s }).ToList())
            {
                if (cost.items.Count() > 1)
                {
                    foreach (var item in cost.items.Where(f => f.KeyID != cost.items.FirstOrDefault().KeyID).ToList())
                    {
                        //Delete(item, modelState);
                        BoreLine.CostItems.Remove(item);
                    }
                }
            }
        }

    }
}