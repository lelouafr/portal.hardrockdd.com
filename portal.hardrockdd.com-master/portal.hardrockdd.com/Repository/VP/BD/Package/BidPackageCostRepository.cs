//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.Bid;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Repository.VP.BD
//{
//    public static class BidPackageCostRepository
//    {
//        public static BidPackageCost Init(BidPackage package)
//        {
//            if (package == null)
//            {
//                throw new ArgumentNullException(nameof(package));
//            }
//            var model = new BidPackageCost
//            {
//                BDCo = package.BDCo,
//                BidId = package.BidId,
//                PackageId = package.PackageId,
//                LineId = package.PackageCosts.DefaultIfEmpty().Max(f => f == null ? 0 : f.LineId) + 1
//            };

//            return model;
//        }

//        public static void ProcessUpdate(Models.Views.Bid.Forms.Package.Setup.AllocatedCostViewModel model, VPContext db)
//        {
//            if (model == null)
//            {
//                throw new ArgumentNullException(nameof(model));
//            }
//            if (db == null)
//            {
//                throw new ArgumentNullException(nameof(db));
//            }
//            var updObj = db.BidPackageCosts.FirstOrDefault(f => f.BDCo == model.BDCo && f.BidId == model.BidId && f.PackageId == model.PackageId && f.LineId == model.LineId);
//            if (updObj != null)
//            {
//                /****Write the changes to object****/
//                updObj.CostAllocationTypeId = (byte?)model.AllocationType;
//                updObj.BudgetCodeId = model.BudgetCodeId;
//                updObj.Cost = model.Cost;

//                //foreach (var line in updObj.Package.BoreLines)
//                //{
//                //    line.RecalcNeeded = true;
//                //}
//                if (updObj.BudgetCodeId != null)
//                    updObj.Package.ApplyPackageCost();
//            }
//            return;
//        }

//        public static void ApplyPackageCost(BidPackage package)
//        {
//            if (package == null)
//            {
//                throw new ArgumentNullException(nameof(package));
//            }
//            if (!package.PackageCosts.Any(f => f.CostAllocationTypeId != null))
//            {
//                return;
//            }
//            var boreLines = package.ActiveBoreLines.OrderBy(o => o.BoreId).ToList();
//            var totFootage = boreLines.Sum(s => s.Footage);

//            boreLines.SelectMany(s => s.CostItems.Where(f => f.IsPackageCost == true)).ToList().ForEach(f => f.Cost = 0);
//            if (!package.PackageCosts.Any())
//            {
//                return;
//            }
//            foreach (var allocatedCost in package.PackageCosts)
//            {
//                BidBoreLineCostItem costItem;
//                BidBoreLine bore;
//                switch ((DB.BDPackageCostAllocationType)allocatedCost.CostAllocationTypeId)
//                {
//                    case DB.BDPackageCostAllocationType.FirstBore:
//                        bore = boreLines.FirstOrDefault();
//                        costItem = bore.CostItems.FirstOrDefault(f => f.BudgetCodeId == allocatedCost.BudgetCodeId && f.IsPackageCost == true && f.GroundDensityId == 0);
//                        if (costItem == null)
//                        {
//                            costItem = BidBoreLineCostItemRepository.Init(allocatedCost, bore, 0);
//                            bore.CostItems.Add(costItem);
//                        }
//                        costItem.Cost = allocatedCost.Cost;
//                        costItem = bore.CostItems.FirstOrDefault(f => f.BudgetCodeId == allocatedCost.BudgetCodeId && f.IsPackageCost == true && f.GroundDensityId == (int)bore.GroundDensityId);
//                        if (costItem == null)
//                        {
//                            costItem = BidBoreLineCostItemRepository.Init(allocatedCost, bore, (int)bore.GroundDensityId);
//                            bore.CostItems.Add(costItem);
//                        }
//                        costItem.Cost = allocatedCost.Cost;
//                        break;
//                    case DB.BDPackageCostAllocationType.LastBore:
//                        bore = boreLines.LastOrDefault();
//                        costItem = bore.CostItems.FirstOrDefault(f => f.BudgetCodeId == allocatedCost.BudgetCodeId && f.IsPackageCost == true && f.GroundDensityId == 0);
//                        if (costItem == null)
//                        {
//                            costItem = BidBoreLineCostItemRepository.Init(allocatedCost, bore, 0);
//                            bore.CostItems.Add(costItem);
//                        }
//                        costItem.Cost = allocatedCost.Cost;
//                        costItem = bore.CostItems.FirstOrDefault(f => f.BudgetCodeId == allocatedCost.BudgetCodeId && f.IsPackageCost == true && f.GroundDensityId == (int)bore.GroundDensityId);
//                        if (costItem == null)
//                        {
//                            costItem = BidBoreLineCostItemRepository.Init(allocatedCost, bore, (int)bore.GroundDensityId);
//                            bore.CostItems.Add(costItem);
//                        }
//                        costItem.Cost = allocatedCost.Cost;
//                        break;
//                    case DB.BDPackageCostAllocationType.AllBoreLF:
//                        foreach (var boreLine in boreLines)
//                        {
//                            costItem = boreLine.CostItems.FirstOrDefault(f => f.BudgetCodeId == allocatedCost.BudgetCodeId && f.IsPackageCost == true && f.GroundDensityId == 0);
//                            if (costItem == null)
//                            {
//                                costItem = BidBoreLineCostItemRepository.Init(allocatedCost, boreLine, 0);
//                                boreLine.CostItems.Add(costItem);
//                            }
//                            costItem.Cost = allocatedCost.Cost / totFootage * boreLine.Footage;
                            
//                            costItem = boreLine.CostItems.FirstOrDefault(f => f.BudgetCodeId == allocatedCost.BudgetCodeId && f.IsPackageCost == true && f.GroundDensityId == (int)boreLine.GroundDensityId);
//                            if (costItem == null)
//                            {
//                                costItem = BidBoreLineCostItemRepository.Init(allocatedCost, boreLine, (int)boreLine.GroundDensityId);
//                                boreLine.CostItems.Add(costItem);
//                            }
//                            costItem.Cost = allocatedCost.Cost / totFootage * boreLine.Footage;
//                        }
//                        break;
//                    case DB.BDPackageCostAllocationType.AllBoreDay:
//                        var boreListViewModel = new Models.Views.Bid.Forms.Bore.DetailListViewModel(package);
//                        var totalDirtDays = boreListViewModel.List.Sum(f => f.DirtDays);
//                        var totalRockDays = boreListViewModel.List.Sum(f => f.RockDays);
//                        foreach (var boreLine in boreLines)
//                        {
//                            var bidViewModel = boreListViewModel.List.FirstOrDefault(f => f.BoreId == boreLine.BoreId);

//                            costItem = boreLine.CostItems.FirstOrDefault(f => f.BudgetCodeId == allocatedCost.BudgetCodeId && f.IsPackageCost == true && f.GroundDensityId == 0);
//                            if (costItem == null)
//                            {
//                                costItem = BidBoreLineCostItemRepository.Init(allocatedCost, boreLine, 0);
//                                boreLine.CostItems.Add(costItem);
//                            }
//                            costItem.Cost = allocatedCost.Cost / totalDirtDays * bidViewModel.DirtDays;

//                            costItem = boreLine.CostItems.FirstOrDefault(f => f.BudgetCodeId == allocatedCost.BudgetCodeId && f.IsPackageCost == true && f.GroundDensityId == (int)boreLine.GroundDensityId);
//                            if (costItem == null)
//                            {
//                                costItem = BidBoreLineCostItemRepository.Init(allocatedCost, boreLine, (int)boreLine.GroundDensityId);
//                                boreLine.CostItems.Add(costItem);
//                            }
//                            costItem.Cost = allocatedCost.Cost / totalRockDays * bidViewModel.RockDays;
//                        }
//                        break;
//                    default:
//                        break;
//                }
//            }

//        }

//    }
//}
