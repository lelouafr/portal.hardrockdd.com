//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Repository.VP.BD
//{
//    public class BidBoreLinePassRepository
//    {       

//        public static BidBoreLinePass Init(BidBoreLine boreLine)
//        {
//            if (boreLine == null)
//            {
//                throw new System.ArgumentNullException(nameof(boreLine));
//            }
//            var model = new BidBoreLinePass
//            {
//                BDCo = boreLine.BDCo,
//                BidId = boreLine.BidId,
//                BoreId = boreLine.BoreId,
//                UM = "LF",
//                Multiplier = 1,
//                Deleted = false,
//                ProductionDays = 0,
//                ProductionRate = 0,
//                ProductionCalTypeId = 0,
//                BoreLine = boreLine,
//            };

//            return model;
//        }

//        //public static Models.Views.Bid.Forms.Bore.Setup.ProductionRateViewModel ProcessUpdate(Models.Views.Bid.Forms.Bore.Setup.ProductionRateViewModel model, VPContext db)
//        //{
//        //    if (model == null)
//        //        throw new System.ArgumentNullException(nameof(model));
//        //    if (db == null)
//        //        throw new System.ArgumentNullException(nameof(db));

//        //    var line = db.BidBoreLines.FirstOrDefault(f => f.BDCo == model.BDCo && f.BidId == model.BidId && f.BoreId == model.BoreId);
//        //    line.GroundDensityId ??= 1;
            
//        //    var dirtRate = line.DirtPhases.FirstOrDefault(f => f.PhaseId == model.PhaseId && f.PassId == model.PassId);
//        //    var rockRate = line.RockPhases.FirstOrDefault(f => f.PhaseId == model.PhaseId && f.PassId == model.PassId);

//        //    if (dirtRate == null)
//        //    {
//        //        dirtRate = Init(line);
//        //        dirtRate.PhaseId = model.PhaseId;
//        //        dirtRate.PassId = model.PassId;
//        //        dirtRate.BoreSize = model.BoreSize;
//        //        dirtRate.GroundDensityId = 0;
//        //        line.Passes.Add(dirtRate);
//        //    }

//        //    if (rockRate == null)
//        //    {
//        //        rockRate = Init(line);
//        //        rockRate.PhaseId = model.PhaseId;
//        //        rockRate.PassId = model.PassId;
//        //        rockRate.BoreSize = model.BoreSize;
//        //        rockRate.GroundDensityId = (int)line.GroundDensityId;
//        //        line.Passes.Add(rockRate);
//        //    }
//        //    var bidParms = line.Bid.Company.BDCompanyParm;
//        //    if (dirtRate.PhaseId == bidParms.DeMobePhaseId || dirtRate.PhaseId == bidParms.MobePhaseId)
//        //        model.RockProductionValue = model.DirtProductionValue;

//        //    if (dirtRate.ProductionCalType != model.CalcType)
//        //    {
//        //        model.DirtProductionValue = dirtRate.ProductionCalType switch
//        //        {
//        //            DB.BidProductionCalEnum.Rate => dirtRate.ProductionRate,
//        //            DB.BidProductionCalEnum.Days => dirtRate.ProductionDays,
//        //            _ => 0,
//        //        };
//        //    }

//        //    /****Write the changes to object****/
//        //    dirtRate.BoreSize = model.BoreSize;
//        //    dirtRate.ProductionCalType = model.CalcType;
//        //    dirtRate.ProductionValue = model.DirtProductionValue;
//        //    dirtRate.UM = "LF";
           
//        //    if (rockRate.ProductionCalType != model.CalcType)
//        //    {
//        //        model.RockProductionValue = rockRate.ProductionCalType switch
//        //        {
//        //            DB.BidProductionCalEnum.Rate => rockRate.ProductionRate,
//        //            DB.BidProductionCalEnum.Days => rockRate.ProductionDays,
//        //            _ => 0,
//        //        };
//        //    }
                                
//        //    /****Write the changes to object****/
//        //    rockRate.BoreSize = model.BoreSize;
//        //    rockRate.ProductionCalType = model.CalcType;
//        //    rockRate.ProductionValue = model.RockProductionValue;
//        //    rockRate.UM = "LF";

//        //    if (line.RecalcNeeded ?? false)
//        //        line.RecalculateCostUnits();

//        //    return new Models.Views.Bid.Forms.Bore.Setup.ProductionRateViewModel(dirtRate, rockRate, line);
//        //}


//    }
//}