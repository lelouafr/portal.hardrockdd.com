//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Repository.VP.BD
//{
//    public partial class BidPackageCostItemRepository
//    {
//    //    public static BidPackageCostItem Init(BidPackage package)
//    //    {
//    //        if (package == null)
//    //            throw new System.ArgumentNullException(nameof(package));

//    //        var model = new BidPackageCostItem
//    //        {
//    //            BDCo = package.BDCo,
//    //            BidId = package.BidId,
//    //            PackageId = package.PackageId,
//    //            tApplied = 0,
//    //            tMultiplier = 1,
//    //            ShiftCnt = 1,
//    //            Units = 0,

//    //        };

//    //        return model;
//    //    }

//        public static Models.Views.Bid.Forms.Package.Setup.CostItemViewModel ProcessUpdate(Models.Views.Bid.Forms.Package.Setup.CostItemViewModel model, VPContext db)
//        {

//            var updObj = db.BidPackageCostItems.FirstOrDefault(f => f.BDCo == model.BDCo && f.BidId == model.BidId && f.PackageId == model.PackageId && f.LineId == model.LineId);
//            if (updObj != null)
//            {
//                var ApplyBgtChanged = updObj.tApplied != (byte)(model.Applied ? 1 : 0);
//                var ApplyMultiplier = updObj.tMultiplier != model.Multiplier;
//                /****Write the changes to object****/
//                updObj.BudgetCodeId = model.BudgetCodeId;
//                updObj.Multiplier = model.Multiplier ?? 0;
//                updObj.Applied = (byte)(model.Applied ? 1 : 0);
//                //if (ApplyMultiplier)
//                //{
//                //    foreach (var bore in updObj.Package.ActiveBoreLines)
//                //    {
//                //        foreach (var costItem in bore.CostItems.Where(f => f.BudgetCodeId == updObj.tBudgetCodeId && f.IsPackageCost != true).ToList())
//                //        {
//                //            costItem.Multiplier = (int)updObj.tMultiplier;
//                //        }
//                //        bore.RecalcNeeded = true;
//                //    }
//                //}

//                //if (ApplyBgtChanged)
//                //{
//                //    if (model.Applied)
//                //    {
//                //        BidBoreLineCostItemRepository.ApplyBudgetCode(updObj.Package, model.BudgetCodeId, (int)model.Multiplier);
//                //    }
//                //    else
//                //    {
//                //        BidBoreLineCostItemRepository.UnApplyBudgetCode(updObj.Package, model.BudgetCodeId);
//                //    }
//                //};
//            }
//            return new Models.Views.Bid.Forms.Package.Setup.CostItemViewModel(updObj, model.BudgetCategory);
//        }

//        //public static void ImportStandard(BidPackage package, string prefix)
//        //{
//        //    //using var db = new VPContext();
//        //    if (package.CostItems.ToList().Where(f => f.tBudgetCodeId.StartsWith(prefix, StringComparison.Ordinal)).Any())
//        //    {
//        //        return;
//        //    }
//        //    var template = package.Bid.Company.Bids.FirstOrDefault(f => f.BidId == 0);
//        //    var templatePackage = template.Packages.FirstOrDefault();

//        //    var stdList = templatePackage.CostItems
//        //                                 .ToList()
//        //                                 .Where(f => f.tBudgetCodeId.StartsWith(prefix, StringComparison.Ordinal))
//        //                                 .ToList();

//        //    foreach (var costItem in stdList)
//        //    {
//        //        var packageCostItem = package.CostItems.FirstOrDefault(f => f.tBudgetCodeId == costItem.tBudgetCodeId);
//        //        if (packageCostItem == null)
//        //        {
//        //            packageCostItem = Init(package);
//        //            packageCostItem.LineId = package.CostItems.DefaultIfEmpty().Max(f => f == null ? 0 : f.LineId) + 1;
//        //            packageCostItem.tBudgetCodeId = costItem.tBudgetCodeId;
//        //            packageCostItem.Cost = costItem.BudgetCode.UnitCost;
//        //            packageCostItem.tMultiplier = 1;
//        //            packageCostItem.Units = 0;
//        //        }
//        //        package.CostItems.Add(packageCostItem);
//        //    }
//        //}

//        //public static int NextId (BidPackage package)
//        //{
//        //    return package.CostItems.DefaultIfEmpty().Max(f => f == null ? 0 : f.LineId) + 1;
//        //}

//    }
//}
