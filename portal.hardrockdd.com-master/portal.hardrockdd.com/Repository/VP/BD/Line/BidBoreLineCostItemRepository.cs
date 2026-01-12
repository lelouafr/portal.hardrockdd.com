//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Repository.VP.BD
//{
//    public static class BidBoreLineCostItemRepository
//    {                
//        //public static BidBoreLineCostItem Init(BidPackageCost cost, BidBoreLine bore, int groundDensityId)
//        //{
//        //    if (cost == null)
//        //    {
//        //        throw new ArgumentNullException(nameof(cost));
//        //    }
//        //    if (bore == null)
//        //    {
//        //        throw new ArgumentNullException(nameof(bore));
//        //    }
//        //    var model = new BidBoreLineCostItem
//        //    {
//        //        BDCo = bore.BDCo,
//        //        BidId = bore.BidId,
//        //        BoreId = bore.BoreId,
//        //        BudgetCodeId = cost.BudgetCodeId,
//        //        Cost = cost.Cost,
//        //        GroundDensityId = groundDensityId,
//        //        LineId = bore.CostItems.Where(f => f.GroundDensityId == groundDensityId).DefaultIfEmpty().Max(f => f == null ? 0 : f.LineId) + 1,
//        //        Multiplier = 1,
//        //        IsPackageCost = true,
//        //        CurUnits = 1,
//        //        Units = 1
//        //    };

//        //    return model;
//        //}
        
//        public static Models.Views.Bid.Forms.Bore.Setup.CostItemViewModel ProcessUpdate(Models.Views.Bid.Forms.Bore.Setup.CostItemViewModel model, VPContext db)
//        {
//            var line = db.BidBoreLines.FirstOrDefault(f => f.BDCo == model.BDCo &&
//                                                            f.BidId == model.BidId &&
//                                                            f.BoreId == model.BoreId);

//            var dirtCost = line.CostItems.FirstOrDefault(f => f.LineId == model.DirtLineId && f.GroundDensityId == 0);
//            var rockCost = line.CostItems.FirstOrDefault(f => f.LineId == model.RockLineId && f.GroundDensityId == line.GroundDensityId);

//            if (dirtCost != null)
//            {
//                dirtCost.Multiplier = model.Multiplier;
//                dirtCost.Cost = model.Cost;
//            }

//            if (rockCost != null)
//            {
//                rockCost.Multiplier = model.Multiplier;
//                rockCost.Cost = model.Cost;
//            }

//            return new Models.Views.Bid.Forms.Bore.Setup.CostItemViewModel(dirtCost, rockCost);
//        }

//        //public static void CreateCostItems(BidBoreLine bore, string budgetCodeId, int multipler)
//        //{

//        //    if (bore == null)
//        //        throw new ArgumentNullException(nameof(bore));
//        //    var bgtItem = bore.Bid.Company.ProjectBudgets.FirstOrDefault(f => f.BudgetCodeId == budgetCodeId);
//        //    if (bgtItem == null)
//        //        return;
            
//        //    if (bgtItem.RockOnly != "Y")
//        //    {
//        //        var dirtCostItem = bore.CostItems.FirstOrDefault(f => f.GroundDensityId == 0 && f.BudgetCodeId == budgetCodeId && f.IsPackageCost == false);
//        //        if (dirtCostItem == null)
//        //        {
//        //            dirtCostItem = new BidBoreLineCostItem()
//        //            {
//        //                BoreLine = bore,
//        //                BudgetCode = bgtItem,

//        //                BDCo = bore.BDCo,
//        //                BidId = bore.BidId,
//        //                BoreId = bore.BoreId,
//        //                LineId = bore.CostItems.Where(f => f.GroundDensityId == 0).DefaultIfEmpty().Max(f => f == null ? 0 : f.LineId) + 1,
//        //                GroundDensityId = 0,
//        //                BudgetCodeId = budgetCodeId,
//        //                Units = 1,
//        //                CurUnits = 1,
//        //                Multiplier = multipler,
//        //                Cost = bgtItem.UnitCost,
//        //                IsPackageCost = false,
//        //            }; 
//        //            bore.CostItems.Add(dirtCostItem);
//        //            bore.RecalcNeeded = true;
//        //        }
//        //    }
//        //    if (bore.GroundDensityId != null)
//        //    {
//        //        var groundDensityId = (int)bore.GroundDensityId;
//        //        var rockCostItem = bore.CostItems.FirstOrDefault(f => f.GroundDensityId == groundDensityId && f.BudgetCodeId == budgetCodeId && f.IsPackageCost != true);
//        //        if (rockCostItem == null)
//        //        {
//        //            rockCostItem = new BidBoreLineCostItem()
//        //            {
//        //                BoreLine = bore,
//        //                BudgetCode = bgtItem,

//        //                BDCo = bore.BDCo,
//        //                BidId = bore.BidId,
//        //                BoreId = bore.BoreId,
//        //                LineId = bore.CostItems.Where(f => f.GroundDensityId == groundDensityId).DefaultIfEmpty().Max(f => f == null ? 0 : f.LineId) + 1,
//        //                GroundDensityId = groundDensityId,
//        //                BudgetCodeId = budgetCodeId,
//        //                Units = 1,
//        //                CurUnits = 1,
//        //                Multiplier = multipler,
//        //                Cost = bgtItem.UnitCost,
//        //                IsPackageCost = false,
//        //            };
//        //            bore.CostItems.Add(rockCostItem); 
//        //            bore.RecalcNeeded = true;
//        //        }
//        //    }            
//        //}

//        //public static void ApplyBidBudgetCodes(BidBoreLine bore)
//        //{
//        //    //using var bgtRepo = new ProjectBudgetCodeRepository();

//        //    if (bore == null)
//        //    {
//        //        throw new ArgumentNullException(nameof(bore));
//        //    }
//        //    //var bgtCodes = bore.Package.CostItems.Where(f => f.Applied == 1).ToList();
//        //    foreach (var bgt in bore.Package.CostItems.Where(f => f.tApplied == 1).ToList())
//        //    {
//        //        if (bgt.tBudgetCodeId != null)
//        //        {
//        //            CreateCostItems(bore, bgt.tBudgetCodeId, (int)bgt.tMultiplier);
//        //            bore.RecalcNeeded = true;
//        //            //ReCalulateUnits(bore, bgt.BudgetCodeId, modelState);
//        //        }
//        //        else
//        //        {
//        //            bore.Package.CostItems.Remove(bgt);
//        //        }
//        //    }

//        //}

//        //public static void ApplyBudgetCode(BidPackage package, string budgetCodeId, int multipler)
//        //{
//        //    if (package == null)
//        //    {
//        //        throw new ArgumentNullException(nameof(package));
//        //    }
//        //    foreach (var bore in package.ActiveBoreLines)
//        //    {
//        //        CreateCostItems(bore, budgetCodeId, multipler);
//        //        bore.RecalcNeeded = true;
//        //    }
//        //}
    
//        //public static void UnApplyBudgetCode(BidPackage package, string budgetCodeId)
//        //{
//        //    if (package == null)
//        //    {
//        //        throw new ArgumentNullException(nameof(package));
//        //    }
//        //    //var boreLines = db.BidBoreLines.Where(f => f.Co == co && f.BidId == bidId && f.PackageId == packageId).ToList();
//        //    foreach (var bore in package.ActiveBoreLines)
//        //    {
//        //        var costItems = bore.CostItems.Where(f => f.BudgetCodeId == budgetCodeId && f.IsPackageCost != true).ToList();
//        //        foreach (var cost in costItems)
//        //        {
//        //            bore.CostItems.Remove(cost);
//        //        }
//        //        bore.RecalcNeeded = true;
//        //    }
//        //}
        
//    }
//}