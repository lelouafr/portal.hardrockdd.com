using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public class BidPackageSummary: BidPackage
    {
        public BidPackageSummary(BidPackage package)
        {
            //using var db = new VPEntities();
            var packageSummary = package.vBidBoreLines;// db.vBidBoreLines.Where(f => f.Co == package.Co && f.BidId == package.BidId && f.PackageId == package.PackageId);

            //var packageSummary = package.ActiveBoreLines
            //    .SelectMany(s => s.CostItems)
            //    .Where(f => f.GroundDensityId == f.BoreLine.GroundDensityId || f.GroundDensityId == 0)
            //    //.ToList()
            //    .Select(s => new
            //    {
            //        s.GroundDensityId,
            //        ExtCost = (s.Units ?? 0) * (s.Multiplier ?? 1) * (s.Cost ?? 0),
            //        Days = s.BudgetCode.Description.StartsWith("Labor Per Man") ? s.Units ?? 0 : 0,
            //        Phases = s.ItemPhases.Select(p => new
            //        {
            //            ExtCost = (p.Units ?? 0) * (s.Multiplier ?? 1) * (p.Cost ?? 0),
            //            Days = s.BudgetCode.Description.StartsWith("Labor Per Man") ? p.Units ?? 0 : 0,
            //        }).ToList(),
            //    })
            //    .GroupBy(g => new { g.GroundDensityId })
            //    .Select(s => new
            //    {
            //        DirtCost = s.Where(f => f.GroundDensityId == 0).Sum(sum => sum.Phases.Any() ? sum.Phases.Sum(psum => psum.ExtCost) : sum.ExtCost),
            //        RockCost = s.Where(f => f.GroundDensityId != 0).Sum(sum => sum.Phases.Any() ? sum.Phases.Sum(psum => psum.ExtCost) : sum.ExtCost),
            //        DirtDays = s.Where(f => f.GroundDensityId == 0).Sum(sum => sum.Phases.Any() ? sum.Phases.Sum(psum => psum.Days) : sum.Days),
            //        RockDays = s.Where(f => f.GroundDensityId != 0).Sum(sum => sum.Phases.Any() ? sum.Phases.Sum(psum => psum.Days) : sum.Days),
            //    })
            //    .ToList();

            var mainBores = packageSummary.Where(f => f.IsIntersectBore == false).ToList();
            TotalFootage = Math.Round(mainBores.Sum(sum => sum.Footage ), 0);// package.TotalFootage();
            UniqueBoreCount = mainBores.Count;//package.ActiveBoreLines.Where(f => f.IntersectBoreId == null).Count();
            DirtCost = Math.Round(packageSummary.Sum(sum => sum.DirtCost), 0);
            RockCost = Math.Round(packageSummary.Sum(sum => sum.RockCost), 0);
            DirtBidDays = Math.Round(packageSummary.Sum(sum => sum.DirtDays), 2, MidpointRounding.AwayFromZero);
            RockBidDays = Math.Round(packageSummary.Sum(sum => sum.RockDays), 2, MidpointRounding.AwayFromZero);
        }
        public decimal TotalFootage { get; set; }

        public int UniqueBoreCount { get; set; }

        public decimal DirtCost { get; set; }

        public decimal RockCost { get; set; }

        public decimal DirtBidDays { get; set; }

        public decimal RockBidDays { get; set; }
    }
}