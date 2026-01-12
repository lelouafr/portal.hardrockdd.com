//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Repository.VP.PM;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Web.Mvc;

//namespace portal.Repository.VP.BD
//{
//    public static class BidPackageRepository
//    {
//        //public static BidPackage Init(Bid bid)
//        //{
//        //    if (bid == null)
//        //    {
//        //        throw new ArgumentNullException(nameof(bid));
//        //    }

//        //    var currentGroundDensity = bid.ActivePackages.DefaultIfEmpty().Max(max => max == null ? 1 : max.GroundDensityId);
//        //    var boreTypeId = bid.ActivePackages.DefaultIfEmpty().Max(max => max == null ? null : max.BoreTypeId);
//        //    var marketId = bid.ActivePackages.DefaultIfEmpty().Max(max => max == null ? null : max.MarketId);
//        //    var pipesize = bid.ActivePackages.DefaultIfEmpty().Max(max => max == null ? null : max.PipeSize);

//        //    var model = new BidPackage
//        //    {
//        //        BDCo = bid.BDCo,
//        //        BidId = bid.BidId,
//        //        PackageId = bid.Packages.DefaultIfEmpty().Max(f => f == null ? 0 : f.PackageId) + 1,
//        //        tDivisionId = bid.DivisionId,
//        //        JCCo = bid.JCCo,
//        //        tIndustryId = bid.IndustryId,
//        //        tMarketId = marketId,
//        //        IncludeOnProposal = true,
//        //        tGroundDensityId = currentGroundDensity,
//        //        tBoreTypeId = boreTypeId,
//        //        tPipeSize = pipesize,
//        //        Bid = bid,
//        //        //Description = bid.Description,
//        //    };

//        //    return model;
//        //}

//        //public static void ImportDefaults(Bid bid)
//        //{
//        //    foreach (var package in bid.ActivePackages)
//        //    {
//        //        BidPackageProductionRateRepository.ImportDafaultTemplateProductionRates(package);
//        //        BidPackageCostItemRepository.ImportStandard(package, "CI-");
//        //        BidPackageCostItemRepository.ImportStandard(package, "RE-");
//        //    }
//        //}
        
//        public static BidPackage ProcessUpdate(Models.Views.Bid.Forms.Package.PackageViewModel model, VPContext db)
//        {
//            if (model == null)
//                throw new ArgumentNullException(nameof(model));

//            var updObj = db.BidPackages.FirstOrDefault(f => f.BDCo == model.BDCo && f.BidId == model.BidId && f.PackageId == model.PackageId);

//            if (updObj != null)
//            {
//                /****Write the changes to temp object****/                
//                updObj.Description = model.Description;
//                updObj.BoreTypeId = model.BoreTypeId;
//                updObj.NumberOfBores = model.NumberOfBores;
//                updObj.PipeSize = model.PipeSize;
//                updObj.RigCategoryId = model.RigCategoryId;
//                updObj.GroundDensityId = model.GroundDensityId;
//                updObj.MarketId = model.MarketId;
//                updObj.DivisionId = model.DivisionId;
//            }
//            return updObj;
//        }

//        public static void ProcessUpdate(Models.Views.Bid.Forms.Package.Setup.RoundDayViewModel model, VPContext db)
//        {
//            if (model == null) throw new System.ArgumentNullException(nameof(model));
//            if (db == null) throw new System.ArgumentNullException(nameof(db));

//            var updObj = db.BidPackages.FirstOrDefault(f => f.BDCo == model.BDCo && f.BidId == model.BidId && f.PackageId == model.PackageId);
//            var bidParms = updObj.Bid.Company.BDCompanyParm;

//            if (updObj != null)
//            {
//                /****Write the changes to object****/
//                updObj.RoundingPhaseId = model.RoundingPhaseId ?? bidParms.PilotPhaseId;
//                updObj.TotalDayRounding = model.DayRounding switch
//                {
//                    DB.BDDayRoundEnum.None => (decimal?)0,
//                    DB.BDDayRoundEnum.QuarterDay => .25m,
//                    DB.BDDayRoundEnum.HalfDay => .5m,
//                    DB.BDDayRoundEnum.FullDay => 1m,
//                    _ => (decimal?)0,
//                };

//                foreach (var bore in updObj.ActiveBoreLines)
//                {
//                    bore.RecalcNeeded = true;
//                }
//            }
//        }

//        public static Models.Views.Bid.Forms.Proposal.Package.PackageProposalViewModel ProcessUpdate(Models.Views.Bid.Forms.Proposal.Package.PackageProposalViewModel model, VPContext db)
//        {

//            var updObj = db.BidPackages.FirstOrDefault(f => f.BDCo == model.BDCo && f.BidId == model.BidId && f.PackageId == model.PackageId);
//            if (updObj != null)
//            {
//                updObj.Description = model.Description;
//                updObj.IncludeOnProposal = model.IncludeOnProposal;

//                model = new Models.Views.Bid.Forms.Proposal.Package.PackageProposalViewModel(updObj);
//            }
//            return model;
//        }

//        public static Models.Views.Bid.Forms.Package.Price.PriceViewModel ProcessUpdate(Models.Views.Bid.Forms.Package.Price.PriceViewModel model, VPContext db)
//        {

//            var updObj = db.BidPackages.FirstOrDefault(f => f.BDCo == model.BDCo && f.BidId == model.BidId && f.PackageId == model.PackageId);
//            if (updObj != null)
//            {
//                var packageSummary = new BidPackageSummary(updObj);
//                var dirtGMUpdate = model.DirtGM != Math.Round(updObj.DirtGM ?? 0, 2);
//                var rockGMUpdate = model.RockGM != Math.Round(updObj.RockGM ?? 0, 2);
//                var dirtLFPriceUpdate = model.DirtLFPrice != updObj.DirtPrice;
//                var rockLFPriceUpdate = model.RockLFPrice != updObj.RockPrice;

//                if (dirtLFPriceUpdate)
//                {
//                    var rev = (model.DirtLFPrice * packageSummary.TotalFootage);
//                    var gp = rev - packageSummary.DirtCost;
//                    decimal calGM = 0;
//                    if (rev != 0)
//                        calGM = gp / rev;
//                    model.DirtGM = calGM;
//                }

//                if (dirtGMUpdate)
//                {
//                    var rev = Math.Round(packageSummary.DirtCost * (1 / (1 - model.DirtGM)), 0);
//                    if (rev != 0 && packageSummary.TotalFootage != 0)
//                    {
//                        model.DirtLFPrice = Math.Round(rev / packageSummary.TotalFootage, 2);
//                    }
//                }

//                if (rockLFPriceUpdate)
//                {
//                    var rev = (model.RockLFPrice * packageSummary.TotalFootage);
//                    var gp = rev - packageSummary.RockCost;
//                    decimal calGM = 0;
//                    if (rev != 0)
//                        calGM = gp / rev;
//                    model.RockGM = calGM;
//                }

//                if (rockGMUpdate)
//                {
//                    var rev = Math.Round(packageSummary.RockCost * (1 / (1 - model.RockGM)), 0);
//                    if (rev != 0 && packageSummary.TotalFootage != 0)
//                    {
//                        model.RockLFPrice = Math.Round(rev / packageSummary.TotalFootage, 2);
//                    }
//                }

//                /****Write the changes to temp object****/

//                updObj.DirtGM = model.DirtGM;
//                updObj.DirtPrice = model.DirtLFPrice;
//                updObj.RockGM = model.RockGM;
//                updObj.RockPrice = model.RockLFPrice;

//                model = new Models.Views.Bid.Forms.Package.Price.PriceViewModel(updObj);
//            }
//            return model;
//        }

//        public static Models.Views.Bid.Forms.Package.Job.JobViewModel ProcessUpdate(Models.Views.Bid.Forms.Package.Job.JobViewModel model, VPContext db)
//        {
//            if (model == null)
//                throw new ArgumentNullException(nameof(model));
//            if (db == null)
//                throw new ArgumentNullException(nameof(db));

//            //ProcessUpdate((Models.Views.Bid.Forms.Package.Price.PriceViewModel)model, db);
//            var updObj = db.BidPackages.FirstOrDefault(f => f.BDCo == model.BDCo && f.BidId == model.BidId && f.PackageId == model.PackageId);
//            if (updObj != null)
//            {

//                /****Write the changes to temp object****/
//                updObj.DivisionId = model.DivisionId;
//                updObj.CustomerReference = model.CustomerReference;
//                updObj.AwardStatus = model.AwardStatus;
//                updObj.JobId = model.JobId;
//                updObj.Description = model.Description;
//                updObj.CustomerId = model.CustomerId;

//                if (updObj.Bid.BidType == (int)DB.BidTypeEnum.QuickBid)
//                {
//                    updObj.DirtPrice = model.DirtLFPrice;
//                    updObj.RockPrice = model.RockLFPrice;
//                }


//                model = new Models.Views.Bid.Forms.Package.Job.JobViewModel(updObj);
//            }
//            return model;
//        }

//        public static Models.Views.Bid.Forms.Package.Award.AwardViewModel ProcessUpdate(Models.Views.Bid.Forms.Package.Award.AwardViewModel model, VPContext db)
//        {
//            if (model == null)
//                throw new ArgumentNullException(nameof(model));
//            if (db == null)
//                throw new ArgumentNullException(nameof(db));

//            //ProcessUpdate((Models.Views.Bid.Forms.Package.Price.PriceViewModel)model, db);
//            var updObj = db.BidPackages.FirstOrDefault(f => f.BDCo == model.BDCo && f.BidId == model.BidId && f.PackageId == model.PackageId);
//            if (updObj != null)
//            {

//                /****Write the changes to temp object****/
//                updObj.DivisionId = model.DivisionId;
//                updObj.CustomerReference = model.CustomerReference;
//                updObj.AwardStatus = model.AwardStatus;
//                updObj.Description = model.Description;
//                updObj.CustomerId = model.CustomerId;

//                if (updObj.Bid.BidType == (int)DB.BidTypeEnum.QuickBid)
//                {
//                    updObj.DirtPrice = model.DirtLFPrice;
//                    updObj.RockPrice = model.RockLFPrice;
//                }

//                model = new Models.Views.Bid.Forms.Package.Award.AwardViewModel(updObj);
//            }
//            return model;
//        }


//        //public static void GenerateJobs(BidPackage package, VPContext db, bool verboseSave = false)
//        //{
//        //    if (package == null)
//        //    {
//        //        throw new ArgumentNullException(nameof(package));
//        //    }
//        //    package.CreateProject();
//        //}
        
//        //public static BidPackage CopyPackage(BidPackage bidPackage)
//        //{
//        //    using var db = new VPContext();

//        //    var newBid = db.BidPackages
//        //            .Include(x => x.BoreLines.Select(s => s.CostItems))
//        //            .Include(x => x.BoreLines.Select(s => s.Passes))
//        //            .Include(x => x.CostItems)
//        //            .Include(x => x.ProductionRates)
//        //            .Include(x => x.Scopes)
//        //            .AsNoTracking()
//        //            .FirstOrDefault(x => x.BidId == bidPackage.BidId && x.PackageId == bidPackage.PackageId );
//        //    db.Entry(newBid).State = EntityState.Detached;
//        //    return newBid;
//        //}
        
//        //public static int NextId(Bid bid)
//        //{
//        //    if (bid == null)
//        //        return 1;
//        //    return bid.Packages.DefaultIfEmpty().Max(f => f == null ? 0 : f.PackageId) + 1;
//        //}

//    }
//}
