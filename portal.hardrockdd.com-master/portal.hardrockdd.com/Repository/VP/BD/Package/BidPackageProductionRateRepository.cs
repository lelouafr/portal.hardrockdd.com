//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Repository.VP.BD
//{
//    public class BidPackageProductionRateRepository : IDisposable
//    {
//        private VPContext db = new VPContext();

//        public BidPackageProductionRateRepository()
//        {

//        }

//        //public static BidPackageProductionRate Init()
//        //{
//        //    var model = new BidPackageProductionRate
//        //    {

//        //    };

//        //    return model;
//        //}

//        //public List<BidPackageProductionRate> GetBidPackageProductionRates(byte Co, int BidId, int PackageId)
//        //{
//        //    var qry = db.BidPackageProductionRates
//        //                .Where(f => f.Co == Co && f.BidId == BidId && f.PackageId == PackageId)
//        //                .ToList();

//        //    return qry;
//        //}

//        //public BidPackageProductionRate GetBidPackageProductionRate(byte Co, int BidId, int PackageId, int LineId)
//        //{
//        //    var qry = db.BidPackageProductionRates
//        //                .Where(f => f.Co == Co && f.BidId == BidId && f.PackageId == PackageId && f.LineId == LineId)
//        //                .FirstOrDefault();

//        //    return qry;
//        //}

//        //public static BidPackageProductionRate Create(BidPackageProductionRate model, VPContext db)
//        //{
//        //    if (model == null)
//        //    {
//        //        throw new ArgumentNullException(nameof(model));
//        //    }
//        //    model.LineId = db.BidPackageProductionRates
//        //                     .Where(f => f.Co == model.Co && f.BidId == model.BidId && f.PackageId == model.PackageId)
//        //                     .DefaultIfEmpty()
//        //                     .Max(f => f == null ? 0 : f.LineId) + 1;


//        //    db.BidPackageProductionRates.Add(model);

//        //    return model;
//        //}
        
//        //public BidPackageProductionRate Create(BidPackageProductionRate model, ModelStateDictionary modelState = null)
//        //{
//        //    if (model == null)
//        //    {
//        //        throw new ArgumentNullException(nameof(model));
//        //    }
//        //    model.LineId = db.BidPackageProductionRates
//        //                     .Where(f => f.BDCo == model.BDCo && f.BidId == model.BidId && f.PackageId == model.PackageId)
//        //                     .DefaultIfEmpty()
//        //                     .Max(f => f == null ? 0 : f.LineId) + 1;


//        //    db.BidPackageProductionRates.Add(model);
//        //    db.SaveChanges(modelState);

//        //    return model;
//        //}

//        //public static BidPackageProductionRate InitTemplate(byte Co)
//        //{
//        //    var model = new BidPackageProductionRate
//        //    {
//        //        Co = Co,
//        //        BidId = 0,
//        //        PackageId = 0,
//        //        PipeSize = 0,
//        //        GroundDensityId = 0,
//        //        PassId = 1,
//        //        BoreSize = 0,
//        //        UM = "LF",
//        //        ProductionRate = 0,
//        //        ProductionDays = 0,
//        //        ProductionCalTypeId = (int)DB.BidProductionCalEnum.Rate
//        //    };

//        //    return model;
//        //}

//        //public static BidPackageProductionRate Init(BidPackage package)
//        //{
//        //    if (package == null)
//        //    {
//        //        throw new ArgumentNullException(nameof(package));
//        //    }
//        //    var result = new BidPackageProductionRate
//        //    {
//        //        BDCo = package.BDCo,
//        //        BidId = package.BidId,
//        //        PackageId = package.PackageId,
//        //        LineId = package.ProductionRates.DefaultIfEmpty().Max(f => f == null ? 0 : f.LineId) + 1,
//        //        PipeSize = package.PipeSize,
//        //        UM = "LF",
//        //        BoreSize = 0,

//        //        Package = package,

//        //    };
//        //    return result;
//        //}

//        //public static BidPackageProductionRate Init(BidPackageProductionRate prod)
//        //{
//        //    if (prod == null)
//        //    {
//        //        throw new ArgumentNullException(nameof(prod));
//        //    }
//        //    var model = new BidPackageProductionRate
//        //    {
//        //        Co = prod.Co,
//        //        BidId = prod.BidId,
//        //        PackageId = prod.PackageId,
//        //        PipeSize = prod.PipeSize,
//        //        GroundDensityId = prod.GroundDensityId,
//        //        PassId = prod.PassId,
//        //        BoreSize = prod.BoreSize,
//        //        UM = "LF",
//        //        ProductionRate = prod.ProductionRate,
//        //        ProductionDays = prod.ProductionDays,
//        //        ProductionCalTypeId = prod.ProductionCalTypeId
//        //    };

//        //    return model;
//        //}

//        //public static BidPackageProductionRate FindCreate(BidPackageProductionRate model, VPContext db)
//        //{
//        //    if (model == null)
//        //    {
//        //        throw new ArgumentNullException(nameof(model));
//        //    }
//        //    var result = db.BidPackageProductionRates.Where(f => f.Co == model.Co &&
//        //                                                  f.BidId == model.BidId &&
//        //                                                  f.PackageId == model.PackageId &&
//        //                                                  f.PipeSize == model.PipeSize &&
//        //                                                  f.PhaseId == model.PhaseId &&
//        //                                                  f.GroundDensityId == model.GroundDensityId &&
//        //                                                  f.PassId == model.PassId
//        //                                                  ).FirstOrDefault();

//        //    if (result == null)
//        //    {
//        //        result = Create(model, db);
//        //    }
//        //    return result;
//        //}

//        //public BidPackageProductionRate FindCreate(BidPackageProductionRate model)
//        //{
//        //    if (model == null)
//        //    {
//        //        throw new ArgumentNullException(nameof(model));
//        //    }
//        //    var result = db.BidPackageProductionRates.Where(f => f.Co == model.Co &&
//        //                                                  f.BidId == model.BidId &&
//        //                                                  f.PackageId == model.PackageId &&
//        //                                                  f.PipeSize == model.PipeSize &&
//        //                                                  f.PhaseId == model.PhaseId &&
//        //                                                  f.GroundDensityId == model.GroundDensityId &&
//        //                                                  f.PassId == model.PassId
//        //                                                  ).FirstOrDefault();

//        //    if (result == null)
//        //    {
//        //        result = Create(model);
//        //    }
//        //    return result;
//        //}

//        //public static BidPackageProductionRate FindCreate(byte Co, int BidId, int PackageId, int PipeSize, string PhaseId, int GroundDensityId, int PassId, VPContext db)
//        //{
//        //    var result = db.BidPackageProductionRates.Where(f => f.Co == Co &&
//        //                                                  f.BidId == BidId &&
//        //                                                  f.PackageId == PackageId &&
//        //                                                  f.PipeSize == PipeSize &&
//        //                                                  f.PhaseId == PhaseId &&
//        //                                                  f.GroundDensityId == GroundDensityId &&
//        //                                                  f.PassId == PassId
//        //                                                  ).FirstOrDefault();

//        //    if (result == null)
//        //    {
//        //        result = new BidPackageProductionRate
//        //        {
//        //            Co = Co,
//        //            BidId = BidId,
//        //            PackageId = PackageId,
//        //            PipeSize = PipeSize,
//        //            PhaseId = PhaseId,
//        //            GroundDensityId = GroundDensityId,
//        //            PassId = PassId,
//        //            ProductionCalTypeId = 0
//        //        };
//        //        result.LineId = db.BidPackageProductionRates
//        //                             .Where(f => f.Co == Co && f.BidId == BidId && f.PackageId == PackageId)
//        //                             .DefaultIfEmpty()
//        //                             .Max(f => f == null ? 0 : f.LineId) + 1;

//        //        db.BidPackageProductionRates.Add(result);
//        //    }
//        //    return result;
//        //}

//        //public BidPackageProductionRate FindCreate(byte Co, int BidId, int PackageId, int PipeSize, string PhaseId, int GroundDensityId, int PassId)
//        //{
//        //    var result = db.BidPackageProductionRates.Where(f => f.Co == Co &&
//        //                                                  f.BidId == BidId &&
//        //                                                  f.PackageId == PackageId &&
//        //                                                  f.PipeSize == PipeSize &&
//        //                                                  f.PhaseId == PhaseId &&
//        //                                                  f.GroundDensityId == GroundDensityId &&
//        //                                                  f.PassId == PassId
//        //                                                  ).FirstOrDefault();

//        //    if (result == null)
//        //    {
//        //        result = new BidPackageProductionRate
//        //        {
//        //            Co = Co,
//        //            BidId = BidId,
//        //            PackageId = PackageId,
//        //            PipeSize = PipeSize,
//        //            PhaseId = PhaseId,
//        //            GroundDensityId = GroundDensityId,
//        //            PassId = PassId,
//        //            ProductionCalTypeId = 0
//        //        };
//        //        result = Create(result);
//        //    }
//        //    return result;
//        //}

//        //public void CreateTemplateForPipeSize(byte Co, int BidId, decimal PipeSize)
//        //{
//        //    var bidParms = db.BDCompanyParms.FirstOrDefault(f => f.Co == Co);
//        //    var groundList = db.BidBoreLines.Where(f => f.Co == Co && f.BidId == BidId && f.Status != (int)BidBoreLineStatusEnum.Deleted && f.Status != (int)BidBoreLineStatusEnum.Canceled).GroupBy(g => g.GroundDensityId).Select(s => new { gnd = s.Key ?? 1 }).ToList();
//        //    groundList.Add(new { gnd = 0 });

//        //    foreach (var gnd in groundList)
//        //    {
//        //        var dbRate = InitTemplate(Co);
//        //        dbRate.PipeSize = PipeSize;
//        //        dbRate.GroundDensityId = gnd.gnd;
//        //        dbRate.PhaseId = bidParms.MobePhaseId;
//        //        dbRate.ProductionCalTypeId = (int)DB.BidProductionCalEnum.Days;
//        //        dbRate.ProductionDays = 1;
//        //        dbRate.PassId = 1;
//        //        _ = FindCreate(dbRate);

//        //        dbRate = Init(dbRate);
//        //        dbRate.PhaseId = bidParms.PilotPhaseId;
//        //        dbRate.PassId = 1;
//        //        dbRate.ProductionCalTypeId = (int)DB.BidProductionCalEnum.Rate;
//        //        dbRate.ProductionDays = null;
//        //        _ = FindCreate(dbRate);

//        //        dbRate = Init(dbRate);
//        //        dbRate.PhaseId = "   004-01-";//Pilot Casing
//        //        dbRate.PassId = 1;
//        //        dbRate.ProductionCalTypeId = (int)DB.BidProductionCalEnum.Days;
//        //        dbRate.ProductionDays = null;
//        //        _ = FindCreate(dbRate);

//        //        dbRate = Init(dbRate);
//        //        dbRate.PhaseId = bidParms.ReamPhaseId;
//        //        dbRate.PassId = 1;
//        //        dbRate.ProductionCalTypeId = (int)DB.BidProductionCalEnum.Rate;
//        //        _ = FindCreate(dbRate);

//        //        dbRate = Init(dbRate);
//        //        dbRate.PhaseId = bidParms.ReamPhaseId;
//        //        dbRate.PassId = 2;
//        //        _ = FindCreate(dbRate);

//        //        dbRate = Init(dbRate);
//        //        dbRate.PhaseId = bidParms.ReamPhaseId;
//        //        dbRate.PassId = 3;
//        //        _ = FindCreate(dbRate);

//        //        dbRate = Init(dbRate);
//        //        dbRate.PhaseId = bidParms.PullPipePhaseId;
//        //        dbRate.ProductionCalTypeId = (int)DB.BidProductionCalEnum.Days;
//        //        dbRate.PassId = 1;
//        //        _ = FindCreate(dbRate);

//        //        dbRate = Init(dbRate);
//        //        dbRate.PhaseId = bidParms.SwabPhaseId;
//        //        dbRate.ProductionCalTypeId = (int)DB.BidProductionCalEnum.Days;
//        //        dbRate.PassId = 1;
//        //        _ = FindCreate(dbRate);

//        //        dbRate = Init(dbRate);
//        //        dbRate.PhaseId = bidParms.TripInOutPhaseId;
//        //        dbRate.ProductionCalTypeId = (int)DB.BidProductionCalEnum.Rate;
//        //        dbRate.PassId = 1;
//        //        _ = FindCreate(dbRate);

//        //        dbRate = Init(dbRate);
//        //        dbRate.PhaseId = bidParms.DeMobePhaseId;
//        //        dbRate.ProductionCalTypeId = (int)DB.BidProductionCalEnum.Days;
//        //        dbRate.ProductionDays = 1;
//        //        dbRate.PassId = 1;
//        //        _ = FindCreate(dbRate);
//        //    }
//        //}


//        //public static void CreateTemplateForPipeSize(BidPackage package, BidPackage packageTemplate, decimal PipeSize)
//        //{
//        //    var bidParms = package.Bid.Company.BDCompanyParm;
           
//        //    var boreList = package.ActiveBoreLines;
//        //    var groundList = boreList.GroupBy(g => g.GroundDensityId).Select(s => new { gnd = s.Key ?? 1 }).ToList();
//        //    groundList.Add(new { gnd = 0 });

//        //    foreach (var gnd in groundList)
//        //    {
//        //        var dbRate = packageTemplate.ProductionRates.FirstOrDefault(f => f.PipeSize == PipeSize && f.PhaseId == bidParms.MobePhaseId && f.GroundDensityId == gnd.gnd && f.PassId == 1);
//        //        if (dbRate == null)
//        //        {
//        //            dbRate = Init(packageTemplate);
//        //            dbRate.PipeSize = PipeSize;
//        //            dbRate.GroundDensityId = gnd.gnd;
//        //            dbRate.PhaseId = bidParms.MobePhaseId;
//        //            dbRate.ProductionCalTypeId = (int)DB.BidProductionCalEnum.Days;
//        //            dbRate.ProductionDays = 1;
//        //            dbRate.PassId = 1;
//        //            packageTemplate.ProductionRates.Add(dbRate);
//        //        }

//        //        dbRate = packageTemplate.ProductionRates.FirstOrDefault(f => f.PipeSize == PipeSize && f.PhaseId == bidParms.PilotPhaseId && f.GroundDensityId == gnd.gnd && f.PassId == 1);
//        //        if (dbRate == null)
//        //        {
//        //            dbRate = Init(packageTemplate);
//        //            dbRate.PipeSize = PipeSize;
//        //            dbRate.GroundDensityId = gnd.gnd;
//        //            dbRate.PhaseId = bidParms.PilotPhaseId;
//        //            dbRate.ProductionCalTypeId = (int)DB.BidProductionCalEnum.Rate;
//        //            dbRate.PassId = 1;
//        //            packageTemplate.ProductionRates.Add(dbRate);
//        //        }

//        //        dbRate = packageTemplate.ProductionRates.FirstOrDefault(f => f.PipeSize == PipeSize && f.PhaseId == "   004-01-" && f.GroundDensityId == gnd.gnd && f.PassId == 1);
//        //        if (dbRate == null)
//        //        {
//        //            dbRate = Init(packageTemplate);
//        //            dbRate.PipeSize = PipeSize;
//        //            dbRate.GroundDensityId = gnd.gnd;
//        //            dbRate.PhaseId = "   004-01-";
//        //            dbRate.ProductionCalTypeId = (int)DB.BidProductionCalEnum.Days;
//        //            dbRate.PassId = 1;
//        //            packageTemplate.ProductionRates.Add(dbRate);
//        //        }

//        //        for (int passId = 1; passId <= 3; passId++)
//        //        {
//        //            dbRate = packageTemplate.ProductionRates.FirstOrDefault(f => f.PipeSize == PipeSize && f.PhaseId == bidParms.ReamPhaseId && f.GroundDensityId == gnd.gnd && f.PassId == passId);
//        //            if (dbRate == null)
//        //            {
//        //                dbRate = Init(packageTemplate);
//        //                dbRate.PipeSize = PipeSize;
//        //                dbRate.GroundDensityId = gnd.gnd;
//        //                dbRate.PhaseId = bidParms.ReamPhaseId;
//        //                dbRate.ProductionCalTypeId = (int)DB.BidProductionCalEnum.Rate;
//        //                dbRate.PassId = passId;
//        //                packageTemplate.ProductionRates.Add(dbRate);
//        //            }
//        //        }

//        //        dbRate = packageTemplate.ProductionRates.FirstOrDefault(f => f.PipeSize == PipeSize && f.PhaseId == bidParms.PullPipePhaseId && f.GroundDensityId == gnd.gnd && f.PassId == 1);
//        //        if (dbRate == null)
//        //        {
//        //            dbRate = Init(packageTemplate);
//        //            dbRate.PipeSize = PipeSize;
//        //            dbRate.GroundDensityId = gnd.gnd;
//        //            dbRate.PhaseId = bidParms.PullPipePhaseId;
//        //            dbRate.ProductionCalTypeId = (int)DB.BidProductionCalEnum.Days;
//        //            dbRate.PassId = 1;
//        //            packageTemplate.ProductionRates.Add(dbRate);
//        //        }


//        //        dbRate = packageTemplate.ProductionRates.FirstOrDefault(f => f.PipeSize == PipeSize && f.PhaseId == bidParms.SwabPhaseId && f.GroundDensityId == gnd.gnd && f.PassId == 1);
//        //        if (dbRate == null)
//        //        {
//        //            dbRate = Init(packageTemplate);
//        //            dbRate.PipeSize = PipeSize;
//        //            dbRate.GroundDensityId = gnd.gnd;
//        //            dbRate.PhaseId = bidParms.SwabPhaseId;
//        //            dbRate.ProductionCalTypeId = (int)DB.BidProductionCalEnum.Days;
//        //            dbRate.PassId = 1;
//        //            packageTemplate.ProductionRates.Add(dbRate);
//        //        }

//        //        dbRate = packageTemplate.ProductionRates.FirstOrDefault(f => f.PipeSize == PipeSize && f.PhaseId == bidParms.TripInOutPhaseId && f.GroundDensityId == gnd.gnd && f.PassId == 1);
//        //        if (dbRate == null)
//        //        {
//        //            dbRate = Init(packageTemplate);
//        //            dbRate.PipeSize = PipeSize;
//        //            dbRate.GroundDensityId = gnd.gnd;
//        //            dbRate.PhaseId = bidParms.TripInOutPhaseId;
//        //            dbRate.ProductionCalTypeId = (int)DB.BidProductionCalEnum.Days;
//        //            dbRate.PassId = 1;
//        //            packageTemplate.ProductionRates.Add(dbRate);
//        //        }

//        //        dbRate = packageTemplate.ProductionRates.FirstOrDefault(f => f.PipeSize == PipeSize && f.PhaseId == bidParms.DeMobePhaseId && f.GroundDensityId == gnd.gnd && f.PassId == 1);
//        //        if (dbRate == null)
//        //        {
//        //            dbRate = Init(packageTemplate);
//        //            dbRate.PipeSize = PipeSize;
//        //            dbRate.GroundDensityId = gnd.gnd;
//        //            dbRate.PhaseId = bidParms.DeMobePhaseId;
//        //            dbRate.ProductionCalTypeId = (int)DB.BidProductionCalEnum.Days;
//        //            dbRate.ProductionDays = 1;
//        //            dbRate.PassId = 1;
//        //            packageTemplate.ProductionRates.Add(dbRate);
//        //        }
//        //    }
//        //}

//        //public void RebuildDefaultFromTemplateRates(byte Co, int BidId, int PackageId, decimal PipeSize)
//        //{
//        //    CreateTemplateForPipeSize(Co, BidId, PipeSize);
//        //    db = new VPContext();
//        //    var boreList = db.BidBoreLines.Where(f => f.Co == Co && f.BidId == BidId && f.PackageId == PackageId && f.PipeSize == PipeSize && f.Status != (int)BidBoreLineStatusEnum.Deleted && f.Status != (int)BidBoreLineStatusEnum.Canceled).ToList();
//        //    var list = db.BidPackageProductionRates.Where(f => f.Co == Co && f.BidId == 0 && f.PackageId == 0 && f.PipeSize == PipeSize)
//        //                 .ToList()
//        //                 .Where(f => boreList.Any(b => b.GroundDensityId == f.GroundDensityId || 0 == f.GroundDensityId))
//        //                 .GroupBy(g => new { g.Co, g.BidId, g.PipeSize, g.PhaseId, g.GroundDensityId, g.PassId })
//        //                 .Select(s => new
//        //                 {
//        //                     s.Key.Co,
//        //                     s.Key.BidId,
//        //                     s.Key.PipeSize,
//        //                     s.Key.PhaseId,
//        //                     s.Key.GroundDensityId,
//        //                     s.Key.PassId,
//        //                     BoreSize = s.Max(m => m.BoreSize ?? 0),
//        //                     ProductionRate = s.Max(m => m.ProductionRate ?? 0),
//        //                     ProductionDays = s.Max(m => m.ProductionDays ?? 0),
//        //                     ProductionCalTypeId = s.Max(m => m.ProductionCalTypeId ?? 0),
//        //                     UM = s.Max(m => m.UM)
//        //                 }).ToList();

//        //    foreach (var rate in list)
//        //    {
//        //        var dbRate = new BidPackageProductionRate
//        //        {
//        //            Co = rate.Co,
//        //            BidId = BidId,
//        //            PackageId = PackageId,
//        //            PipeSize = (int)rate.PipeSize,
//        //            GroundDensityId = rate.GroundDensityId,
//        //            PhaseId = rate.PhaseId,
//        //            PassId = rate.PassId,
//        //            BoreSize = rate.BoreSize,
//        //            UM = rate.UM,
//        //            ProductionRate = rate.ProductionRate,
//        //            ProductionDays = rate.ProductionDays,
//        //            ProductionCalTypeId = rate.ProductionCalTypeId
//        //        };
//        //        _ = FindCreate(dbRate);
//        //    }

//        //    var delList = new List<BidPackageProductionRate>();
//        //    db = new VPContext();
//        //    foreach (var dbRate in db.BidPackageProductionRates.Where(f => f.Co == Co && f.BidId == BidId && f.PackageId == PackageId && f.PipeSize == PipeSize).ToList())
//        //    {
//        //        if (!list.Any(f => f.PipeSize == dbRate.PipeSize &&
//        //                           f.GroundDensityId == dbRate.GroundDensityId &&
//        //                           f.PhaseId == dbRate.PhaseId &&
//        //                           f.PassId == dbRate.PassId
//        //                           )
//        //            )
//        //        {
//        //            delList.Add(dbRate);
//        //        }
//        //    }
//        //    if (delList.Count > 0)
//        //    {
//        //        db.BidPackageProductionRates.RemoveRange(delList);
//        //        db.SaveChanges();
//        //    }
//        //}

//        //public static void ImportDafaultTemplateProductionRates(BidPackage package)
//        //{
//        //    //using var db = new VPContext();
//        //    var boreList = package.ActiveBoreLines;
//        //    var template = package.Bid.Company.Bids.FirstOrDefault(f => f.BidId == 0);
//        //    var templatePackage = template.Packages.FirstOrDefault();

//        //    var dirtTemplateProductionCnt = templatePackage.ProductionRates.Where(f => f.PipeSize == package.PipeSize && f.GroundDensityId == 0).Count();
//        //    var rockTemplateProductionCnt = templatePackage.ProductionRates.Where(f => f.PipeSize == package.PipeSize && f.GroundDensityId == package.GroundDensityId).Count();

//        //    if (dirtTemplateProductionCnt == 0 ||
//        //        rockTemplateProductionCnt == 0)
//        //    {
//        //        CreateTemplateForPipeSize(package, templatePackage, package.PipeSize ?? 0);
//        //    }

//        //    var dirtProductionCnt = package.ProductionRates.Where(f => f.PipeSize == package.PipeSize && f.GroundDensityId == 0).Count();
//        //    var rockProductionCnt = package.ProductionRates.Where(f => f.PipeSize == package.PipeSize && f.GroundDensityId == package.GroundDensityId).Count();

//        //    if (dirtTemplateProductionCnt == dirtProductionCnt &&
//        //        rockTemplateProductionCnt == rockProductionCnt)
//        //    {
//        //        return;
//        //    }
//        //    //var boreList = package.ActiveBoreLines;
//        //    var list = templatePackage.ProductionRates
//        //                    .ToList()
//        //                    .Where(f => boreList.Any(b => (b.GroundDensityId == f.GroundDensityId || 0 == f.GroundDensityId) && f.PipeSize == b.PipeSize))
//        //                    .GroupBy(g => new { g.BDCo, g.BidId, g.PipeSize, g.PhaseId, g.GroundDensityId, g.PassId })
//        //                    .Select(s => new
//        //                    {
//        //                        s.Key.BDCo,
//        //                        s.Key.BidId,
//        //                        s.Key.PipeSize,
//        //                        s.Key.PhaseId,
//        //                        s.Key.GroundDensityId,
//        //                        s.Key.PassId,
//        //                        BoreSize = s.Max(m => m.BoreSize ?? 0),
//        //                        ProductionRate = s.Max(m => m.ProductionRate ?? 0),
//        //                        ProductionDays = s.Max(m => m.ProductionDays ?? 0),
//        //                        ProductionCalTypeId = s.Max(m => m.ProductionCalTypeId ?? 0),
//        //                        UM = s.Max(m => m.UM)
//        //                    }).ToList();

//        //    foreach (var rate in list)
//        //    {
               
//        //        var dbRate = package.ProductionRates.FirstOrDefault(f => f.PipeSize == rate.PipeSize &&
//        //                                                                  f.PhaseId == rate.PhaseId &&
//        //                                                                  f.GroundDensityId == rate.GroundDensityId &&
//        //                                                                  f.PassId == rate.PassId);
//        //        if (dbRate == null)
//        //        {
//        //            dbRate = new BidPackageProductionRate
//        //            {
//        //                BDCo = rate.BDCo,
//        //                BidId = package.BidId,
//        //                PackageId = package.PackageId,
//        //                LineId = package.ProductionRates.DefaultIfEmpty().Max(f => f == null ? 0 : f.LineId) + 1,
//        //                PipeSize = rate.PipeSize,
//        //                GroundDensityId = rate.GroundDensityId,
//        //                PhaseId = rate.PhaseId,
//        //                PassId = rate.PassId,
//        //                BoreSize = rate.BoreSize,
//        //                UM = rate.UM,
//        //                ProductionRate = rate.ProductionRate,
//        //                ProductionDays = rate.ProductionDays,
//        //                ProductionCalTypeId = rate.ProductionCalTypeId,

//        //                Package = package
//        //            };
//        //            package.ProductionRates.Add(dbRate);
//        //        }
//        //    }

//        //    var delList = new List<BidPackageProductionRate>();
//        //    foreach (var dbRate in package.ProductionRates)
//        //    {
//        //        if (!list.Any(f => f.PipeSize == dbRate.PipeSize && f.GroundDensityId == dbRate.GroundDensityId && f.PhaseId == dbRate.PhaseId && f.PassId == dbRate.PassId && package.PackageId == dbRate.PackageId))
//        //        {
//        //            delList.Add(dbRate);
//        //        }
//        //    }
//        //    delList.ForEach(del => package.ProductionRates.Remove(del));
//        //}

//        //public void RebuildDefaultFromTemplateRates(byte Co, int BidId, int PackageId)
//        //{
//        //    db = new VPContext();
//        //    var boreList = db.BidBoreLines.Where(f => f.Co == Co && f.BidId == BidId && f.PackageId == PackageId && f.Status != (int)BidBoreLineStatusEnum.Deleted && f.Status != (int)BidBoreLineStatusEnum.Canceled).ToList();
//        //    foreach (var pipesize in boreList.Where(f => f.PipeSize != null).GroupBy(g => (decimal)g.PipeSize).ToList())
//        //    {
//        //        CreateTemplateForPipeSize(Co, BidId, pipesize.Key);
//        //    }

//        //    db = new VPContext();
//        //    var list = db.BidPackageProductionRates.Where(f => f.Co == Co && f.BidId == 0 && f.PackageId == 0)
//        //                 .ToList()
//        //                 .Where(f => boreList.Any(b => (b.GroundDensityId == f.GroundDensityId || 0 == f.GroundDensityId) && f.PipeSize == b.PipeSize))
//        //                 .GroupBy(g => new { g.Co, g.BidId, g.PipeSize, g.PhaseId, g.GroundDensityId, g.PassId })
//        //                 .Select(s => new
//        //                 {
//        //                     s.Key.Co,
//        //                     s.Key.BidId,
//        //                     s.Key.PipeSize,
//        //                     s.Key.PhaseId,
//        //                     s.Key.GroundDensityId,
//        //                     s.Key.PassId,
//        //                     BoreSize = s.Max(m => m.BoreSize ?? 0),
//        //                     ProductionRate = s.Max(m => m.ProductionRate ?? 0),
//        //                     ProductionDays = s.Max(m => m.ProductionDays ?? 0),
//        //                     ProductionCalTypeId = s.Max(m => m.ProductionCalTypeId ?? 0),
//        //                     UM = s.Max(m => m.UM)
//        //                 }).ToList();

//        //    foreach (var rate in list)
//        //    {
//        //        var dbRate = new BidPackageProductionRate
//        //        {
//        //            Co = rate.Co,
//        //            BidId = BidId,
//        //            PackageId = PackageId,
//        //            PipeSize = rate.PipeSize,
//        //            GroundDensityId = rate.GroundDensityId,
//        //            PhaseId = rate.PhaseId,
//        //            PassId = rate.PassId,
//        //            BoreSize = rate.BoreSize,
//        //            UM = rate.UM,
//        //            ProductionRate = rate.ProductionRate,
//        //            ProductionDays = rate.ProductionDays,
//        //            ProductionCalTypeId = rate.ProductionCalTypeId
//        //        };
//        //        _ = FindCreate(dbRate);
//        //    }

//        //    var delList = new List<BidPackageProductionRate>();
//        //    db = new VPContext();
//        //    foreach (var dbRate in db.BidPackageProductionRates.Where(f => f.Co == Co && f.BidId == BidId && f.PackageId == PackageId).ToList())
//        //    {
//        //        if (!list.Any(f => f.PipeSize == dbRate.PipeSize && f.GroundDensityId == dbRate.GroundDensityId && f.PhaseId == dbRate.PhaseId && f.PassId == dbRate.PassId && PackageId == dbRate.PackageId))
//        //        {
//        //            delList.Add(dbRate);
//        //        }
//        //    }
//        //    if (delList.Count > 0)
//        //    {
//        //        db.BidPackageProductionRates.RemoveRange(delList);
//        //        db.SaveChanges();
//        //    }
//        //}

//        //public void ApplyDefaultToLines(byte Co, int BidId, int PackageId, ModelStateDictionary modelState)
//        //{
//        //    if (modelState == null) throw new System.ArgumentNullException(nameof(modelState));

//        //    //var list = db.BidBoreLinePasses.Where(f => f.Co == Co && f.BidId == BidId && f.BoreLine.PipeSize == PipeSize).ToList();
//        //    var boreList = db.BidBoreLines.Where(f => f.Co == Co && f.BidId == BidId && f.PackageId == PackageId && f.Status != (int)BidBoreLineStatusEnum.Deleted && f.Status != (int)BidBoreLineStatusEnum.Canceled).ToList();
//        //    var dftList = db.BidPackageProductionRates.Where(f => f.Co == Co && f.BidId == BidId && f.PackageId == PackageId).ToList();

//        //    using var passRepo = new BidBoreLinePassRepository();
//        //    foreach (var bore in boreList)
//        //    {
//        //        foreach (var pass in dftList.Where(f => f.GroundDensityId == 0).ToList())
//        //        {
//        //            var borePass = bore.Passes.FirstOrDefault(f => f.GroundDensityId == pass.GroundDensityId &&
//        //                                                           f.PhaseId == pass.PhaseId &&
//        //                                                           f.PassId == pass.PassId &&
//        //                                                           f.Deleted != true);
//        //            if (borePass == null)
//        //            {
//        //                borePass = BidBoreLinePassRepository.Init(bore, pass);
//        //                bore.Passes.Add(borePass);
//        //                borePass.BoreLine = bore;
//        //            }
//        //            borePass.BoreSize = pass.BoreSize == 0 ? borePass.BoreSize ?? 0 : pass.BoreSize;
//        //            if (pass.ProductionCalTypeId == (int)DB.BidProductionCalEnum.Rate)
//        //            {
//        //                borePass.ProductionRate = pass.ProductionRate;
//        //                borePass.ProductionDays = null;
//        //                borePass.ProductionCalTypeId = (int)DB.BidProductionCalEnum.Rate;
//        //            }
//        //            else if (pass.ProductionCalTypeId == (int)DB.BidProductionCalEnum.Days)
//        //            {
//        //                borePass.ProductionRate = null;
//        //                borePass.ProductionDays = pass.ProductionDays;
//        //                borePass.ProductionCalTypeId = (int)DB.BidProductionCalEnum.Days;
//        //            }
//        //            BidBoreLinePassRepository.CreateToolingBudget(borePass);
//        //        }

//        //        foreach (var pass in dftList.Where(f => f.GroundDensityId == bore.GroundDensityId).ToList())
//        //        {
//        //            var borePass = bore.Passes.FirstOrDefault(f => f.GroundDensityId == pass.GroundDensityId &&
//        //                                                            f.PhaseId == pass.PhaseId &&
//        //                                                            f.PassId == pass.PassId &&
//        //                                                            f.Deleted != true);
//        //            if (borePass == null)
//        //            {
//        //                borePass = BidBoreLinePassRepository.Init(bore, pass);
//        //                bore.Passes.Add(borePass);
//        //                borePass.BoreLine = bore;
//        //            }
//        //            borePass.BoreSize = pass.BoreSize == 0 ? borePass.BoreSize ?? 0 : pass.BoreSize;
//        //            if (pass.ProductionCalTypeId == (int)DB.BidProductionCalEnum.Rate)
//        //            {
//        //                borePass.ProductionRate = pass.ProductionRate;
//        //                borePass.ProductionDays = null;
//        //                borePass.ProductionCalTypeId = (int)DB.BidProductionCalEnum.Rate;
//        //            }
//        //            else if (pass.ProductionCalTypeId == (int)DB.BidProductionCalEnum.Days)
//        //            {
//        //                borePass.ProductionRate = null;
//        //                borePass.ProductionDays = pass.ProductionDays;
//        //                borePass.ProductionCalTypeId = (int)DB.BidProductionCalEnum.Days;
//        //            }
//        //            BidBoreLinePassRepository.CreateToolingBudget(borePass);
//        //        }

//        //        db.SaveChanges(modelState);
//        //    }
//        //}

//        //public void ApplyDefaultToLine(byte co, int bidId, int boreId)
//        //{
//        //    db.udBDBL_DefaultProductionRateForLineInsert(co, bidId, boreId);
//        //}

//        //public void ApplyDefaultToPackage(byte co, int bidId, int PackageId, int overRide = 0)
//        //{
//        //    db.udBDBL_DefaultProductionRateForPackageInsert(co, bidId, PackageId, overRide);
//        //}

//        public static void ApplyDefaultToPackage(VPContext db, byte co, int bidId, int PackageId, int overRide = 0)
//        {
//            if (db == null) throw new System.ArgumentNullException(nameof(db));

//            db.udBDBL_DefaultProductionRateForPackageInsert(co, bidId, PackageId, overRide);
//        }

//        public static void ApplyDefaultToLine(VPContext db, byte co, int bidId, int boreId)
//        {
//            if (db == null) throw new System.ArgumentNullException(nameof(db));

//            db.udBDBL_DefaultProductionRateForLineInsert(co, bidId, boreId);
//        }

//        //public Models.Views.Bid.BidPackageProductionRateViewModel ProcessUpdate(Models.Views.Bid.BidPackageProductionRateViewModel model, ModelStateDictionary modelState)
//        //{
//        //    if (model == null) throw new System.ArgumentNullException(nameof(model));
//        //    if (modelState == null) throw new System.ArgumentNullException(nameof(modelState));

//        //    var updObj = GetBidPackageProductionRate(model.Co, model.BidId, model.PackageId, model.LineId);
//        //    if (updObj != null)
//        //    {
//        //        /****Write the changes to object****/

//        //        if (model.CalcType == DB.BidProductionCalEnum.Days)
//        //        {
//        //            updObj.ProductionDays = model.ProductionDays;
//        //            updObj.ProductionCalTypeId = (int)model.CalcType;
//        //            updObj.ProductionRate = null;
//        //        }
//        //        else if (model.CalcType == DB.BidProductionCalEnum.Rate)
//        //        {
//        //            updObj.ProductionDays = null;
//        //            updObj.ProductionCalTypeId = (int)model.CalcType;
//        //            updObj.ProductionRate = model.ProductionRate;
//        //        }

//        //        db.SaveChanges(modelState);
//        //    }

//        //    return new Models.Views.Bid.BidPackageProductionRateViewModel(updObj);
//        //}


//        public static Models.Views.Bid.Forms.Package.Setup.ProductionRateViewModel ProcessUpdate(Models.Views.Bid.Forms.Package.Setup.ProductionRateViewModel model, VPContext db)
//        {
//            var updObjs = db.BidPackageProductionRates.Where(f => f.BDCo == model.BDCo &&
//                                                           f.BidId == model.BidId &&
//                                                           f.PackageId == model.PackageId &&
//                                                           f.PhaseId == model.PhaseId &&
//                                                           f.PassId == model.PassId).ToList();

//            var bidParms = db.BDCompanyParms.FirstOrDefault(f => f.BDCo == model.BDCo);
//            if (model.PhaseId == bidParms.MobePhaseId || model.PhaseId == bidParms.DeMobePhaseId)
//            {
//                model.SoftRockProductionValue = model.DirtProductionValue;
//                model.MediumRockProductionValue = model.DirtProductionValue;
//                model.HardRockProductionValue = model.DirtProductionValue;
//                model.VeryHardRockProductionValue = model.DirtProductionValue;
//            }

//            foreach (var item in updObjs)
//            {
//                item.BoreSize = model.BoreSize;
//                item.ProductionCalTypeId = (int)model.CalcType;
//                item.ProductionDays = null;
//                item.ProductionRate = null;
//                switch (item.GroundDensityId)
//                {
//                    case 0:
//                        if (model.CalcType == DB.BidProductionCalEnum.Days)
//                            item.ProductionDays = model.DirtProductionValue;
//                        else
//                            item.ProductionRate = model.DirtProductionValue;
//                        break;
//                    case 1:
//                        if (model.CalcType == DB.BidProductionCalEnum.Days)
//                            item.ProductionDays = model.SoftRockProductionValue;
//                        else
//                            item.ProductionRate = model.SoftRockProductionValue;
//                        break;
//                    case 2:
//                        if (model.CalcType == DB.BidProductionCalEnum.Days)
//                            item.ProductionDays = model.MediumRockProductionValue;
//                        else
//                            item.ProductionRate = model.MediumRockProductionValue;
//                        break;
//                    case 3:
//                        if (model.CalcType == DB.BidProductionCalEnum.Days)
//                            item.ProductionDays = model.HardRockProductionValue;
//                        else
//                            item.ProductionRate = model.HardRockProductionValue;
//                        break;
//                    case 4:
//                        if (model.CalcType == DB.BidProductionCalEnum.Days)
//                            item.ProductionDays = model.VeryHardRockProductionValue;
//                        else
//                            item.ProductionRate = model.VeryHardRockProductionValue;
//                        break;
//                    default:
//                        break;
//                }
//            }

//            return new Models.Views.Bid.Forms.Package.Setup.ProductionRateViewModel(updObjs);

//        }

//        //public Models.Views.Bid.BidPackageProductionRateViewModel ProcessUpdate(Models.Views.Bid.BidPackageProductionRateViewModel model, ModelStateDictionary modelState)
//        //{
//        //    if (model == null)
//        //    {
//        //        throw new System.ArgumentNullException(nameof(model));
//        //    }
//        //    var updObjs = db.BidPackageProductionRates.Where(f => f.Co == model.Co &&
//        //                                                   f.BidId == model.BidId &&
//        //                                                   f.PackageId == model.PackageId &&
//        //                                                   f.PhaseId == model.PhaseId &&
//        //                                                   f.PassId == model.PassId).ToList();

//        //    foreach (var item in updObjs)
//        //    {
//        //        item.BoreSize = model.BoreSize;
//        //        item.ProductionCalTypeId = (int)model.CalcType;
//        //        item.ProductionDays = null;
//        //        item.ProductionRate = null;
//        //        switch (item.GroundDensityId)
//        //        {
//        //            case 0:
//        //                if (model.CalcType == DB.BidProductionCalEnum.Days)
//        //                    item.ProductionDays = model.DirtProductionValue;
//        //                else
//        //                    item.ProductionRate = model.DirtProductionValue;
//        //                break;
//        //            case 1:
//        //                if (model.CalcType == DB.BidProductionCalEnum.Days)
//        //                    item.ProductionDays = model.SoftRockProductionValue;
//        //                else
//        //                    item.ProductionRate = model.SoftRockProductionValue;
//        //                break;
//        //            case 2:
//        //                if (model.CalcType == DB.BidProductionCalEnum.Days)
//        //                    item.ProductionDays = model.MediumRockProductionValue;
//        //                else
//        //                    item.ProductionRate = model.MediumRockProductionValue;
//        //                break;
//        //            case 3:
//        //                if (model.CalcType == DB.BidProductionCalEnum.Days)
//        //                    item.ProductionDays = model.HardRockProductionValue;
//        //                else
//        //                    item.ProductionRate = model.HardRockProductionValue;
//        //                break;
//        //            case 4:
//        //                if (model.CalcType == DB.BidProductionCalEnum.Days)
//        //                    item.ProductionDays = model.VeryHardRockProductionValue;
//        //                else
//        //                    item.ProductionRate = model.VeryHardRockProductionValue;
//        //                break;
//        //            default:
//        //                break;
//        //        }
//        //    }

//        //    db.SaveChanges(modelState);


//        //    return new Models.Views.Bid.BidPackageProductionRateViewModel(updObjs);
//        //}

//        //public Models.Views.Bid.BidPackageProductionRateBoreSizeViewModel ProcessUpdate(Models.Views.Bid.BidPackageProductionRateBoreSizeViewModel model, ModelStateDictionary modelState)
//        //{
//        //    if (model == null)
//        //    {
//        //        throw new System.ArgumentNullException(nameof(model));
//        //    }

//        //    var list = db.BidPackageProductionRates
//        //                .Where(f => f.Co == model.Co && f.BidId == model.BidId && f.PackageId == model.PackageId && f.PhaseId == model.PhaseId && f.PassId == model.PassId)
//        //                .ToList();

//        //    foreach (var updObj in list)
//        //    {
//        //        updObj.BoreSize = model.BoreSize;
//        //    }
//        //    db.SaveChanges(modelState);
//        //    return model;
//        //}

//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        ~BidPackageProductionRateRepository()
//        {
//            // Finalizer calls Dispose(false)
//            Dispose(false);
//        }

//        protected virtual void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                if (db != null)
//                {
//                    db.Dispose();
//                    db = null;
//                }
//            }
//        }
//    }
//}
