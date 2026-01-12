using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Bid.Forms.Package.Price
{
    public class PriceViewModel: PackageViewModel
    {
        public PriceViewModel()
        {

        }

        public PriceViewModel(BidPackage package): base(package)
        {
            if (package == null)
            {
                throw new System.ArgumentNullException(nameof(package));
            }
            #region mapping

            StandardGM = .4M;
            DirtGM = package.DirtGM ?? StandardGM;
            RockGM = package.RockGM ?? StandardGM;

            DirtGM = DirtGM == 0 ? StandardGM : DirtGM;
            RockGM = RockGM == 0 ? StandardGM : RockGM;


            var packageSummary = new BidPackageSummary(package);

            DirtCost = packageSummary.DirtCost;
            RockCost = packageSummary.RockCost;
            DirtBidDays = packageSummary.DirtBidDays;
            RockBidDays = packageSummary.RockBidDays;
            TotalFootage = packageSummary.TotalFootage;
            UniqueBoreCount = packageSummary.UniqueBoreCount;

            if (package.ParentPackageId != null)
            {
                package.DirtGM = package.ParentPackage.DirtGM;
                package.DirtPrice = package.ParentPackage.DirtPrice;
                package.RockGM = package.ParentPackage.RockGM;
                package.RockPrice = package.ParentPackage.RockPrice;
            }

            if (package.DirtPrice != null)// && DirtRevenue != 0
            {
                DirtRevenue = (decimal)package.DirtPrice * TotalFootage;
                DirtGM = Math.Round((DirtRevenue - DirtCost) / DirtRevenue, 4);
                DirtLFPrice = (decimal)package.DirtPrice;
            }
            else
            {
                DirtRevenue = Math.Round(DirtCost * (1 / (1 - DirtGM)), 0);
                if (DirtRevenue != 0 && TotalFootage != 0)
                {
                    DirtLFPrice = Math.Round(DirtRevenue / TotalFootage, 2);
                }
            }

            if (package.RockPrice != null)// && RockRevenue != 0
            {
                RockRevenue = (decimal)package.RockPrice * TotalFootage;
                RockGM = Math.Round((RockRevenue - RockCost) / RockRevenue,4);
                RockLFPrice = (decimal)package.RockPrice;
            }
            else
            {
                RockRevenue = Math.Round(RockCost * (1 / (1 - RockGM)), 0);
                if (RockRevenue != 0 && TotalFootage != 0)
                {
                    RockLFPrice = Math.Round(RockRevenue / TotalFootage, 2);
                }
            }

            if (TotalFootage != 0)
            {
                DirtLFCost = Math.Round(DirtCost / TotalFootage, 2);
                RockLFCost = Math.Round(RockCost / TotalFootage, 2);

            }

            DirtProfit = DirtRevenue - DirtCost;
            RockProfit = RockRevenue - RockCost;

            RockRevenueAdder = RockLFPrice - DirtLFPrice;
            #endregion

        }

        [ReadOnly(true)]
        [Display(Name = "Dirt Days")]
        [UIHint("LongBox")]
        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Info", FormGroupRow = 2)]
        public decimal DirtBidDays { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Rock Days")]
        [UIHint("LongBox")]
        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Info", FormGroupRow = 2)]
        public decimal RockBidDays { get; set; }


        [ReadOnly(true)]
        [Display(Name = "Dirt Cost")]
        [UIHint("CurrencyBox")]
        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Cost", FormGroupRow = 1)]
        public decimal DirtCost { get; set; }


        [ReadOnly(true)]
        [Display(Name = "Rock Cost")]
        [UIHint("CurrencyBox")]
        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Cost", FormGroupRow = 1)]
        public decimal RockCost { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Dirt LF Cost")]
        [UIHint("CurrencyBox")]
        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Cost", FormGroupRow = 2)]
        public decimal DirtLFCost { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Rock LF Cost")]
        [UIHint("CurrencyBox")]
        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Cost", FormGroupRow = 2)]
        public decimal RockLFCost { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Standard GM")]
        [UIHint("PercentBox")]
        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Gross Margin", FormGroupRow = 1)]
        public decimal StandardGM { get; set; }


        [Display(Name = "Dirt GM")]
        [UIHint("PercentBox")]
        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Gross Margin", FormGroupRow = 2)]
        public decimal DirtGM { get; set; }


        [Display(Name = "Bore Count")]
        [UIHint("IntegerBox")]
        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Gross Margin", FormGroupRow = 2)]
        public int UniqueBoreCount { get; set; }

        [Display(Name = "Total Footage")]
        [UIHint("LongBox")]
        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Revenue", FormGroupRow = 2)]
        public decimal TotalFootage { get; set; }

        [Display(Name = "Rock GM")]
        [UIHint("PercentBox")]
        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Gross Margin", FormGroupRow = 2)]
        public decimal RockGM { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Dirt Revenue")]
        [UIHint("CurrencyBox")]
        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Revenue", FormGroupRow = 1)]
        public decimal DirtRevenue { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Rock Revenue")]
        [UIHint("CurrencyBox")]
        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Revenue", FormGroupRow = 1)]
        public decimal RockRevenue { get; set; }


        [Display(Name = "Dirt LF Price")]
        [UIHint("CurrencyBox")]
        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Revenue", FormGroupRow = 2)]
        public decimal DirtLFPrice { get; set; }

        [Display(Name = "Rock LF Price")]
        [UIHint("CurrencyBox")]
        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Revenue", FormGroupRow = 2)]
        public decimal RockLFPrice { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Rock Adder")]
        [UIHint("CurrencyBox")]
        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Revenue", FormGroupRow = 4)]
        public decimal RockRevenueAdder { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Dirt Profit")]
        [UIHint("CurrencyBox")]
        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Revenue", FormGroupRow = 5)]
        public decimal DirtProfit { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Rock Profit")]
        [UIHint("CurrencyBox")]
        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Revenue", FormGroupRow = 5)]
        public decimal RockProfit { get; set; }


        internal new PriceViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.BidPackages.FirstOrDefault(f => f.BDCo == BDCo && f.BidId == BidId && f.PackageId == PackageId);

            if (updObj != null)
            {
                var packageSummary = new BidPackageSummary(updObj);
                var dirtGMUpdate = DirtGM != Math.Round(updObj.DirtGM ?? 0, 2);
                var rockGMUpdate = RockGM != Math.Round(updObj.RockGM ?? 0, 2);
                var dirtLFPriceUpdate = DirtLFPrice != updObj.DirtPrice;
                var rockLFPriceUpdate = RockLFPrice != updObj.RockPrice;

                if (dirtLFPriceUpdate)
                {
                    var rev = (DirtLFPrice * packageSummary.TotalFootage);
                    var gp = rev - packageSummary.DirtCost;
                    decimal calGM = 0;
                    if (rev != 0)
                        calGM = gp / rev;
                    DirtGM = calGM;
                }

                if (dirtGMUpdate)
                {
                    var rev = Math.Round(packageSummary.DirtCost * (1 / (1 - DirtGM)), 0);
                    if (rev != 0 && packageSummary.TotalFootage != 0)
                    {
                        DirtLFPrice = Math.Round(rev / packageSummary.TotalFootage, 2);
                    }
                }

                if (rockLFPriceUpdate)
                {
                    var rev = (RockLFPrice * packageSummary.TotalFootage);
                    var gp = rev - packageSummary.RockCost;
                    decimal calGM = 0;
                    if (rev != 0)
                        calGM = gp / rev;
                    RockGM = calGM;
                }

                if (rockGMUpdate)
                {
                    var rev = Math.Round(packageSummary.RockCost * (1 / (1 - RockGM)), 0);
                    if (rev != 0 && packageSummary.TotalFootage != 0)
                    {
                        RockLFPrice = Math.Round(rev / packageSummary.TotalFootage, 2);
                    }
                }

                updObj.DirtGM = DirtGM;
                updObj.DirtPrice = DirtLFPrice;
                updObj.RockGM = RockGM;
                updObj.RockPrice = RockLFPrice;
                try
                {
                    db.BulkSaveChanges();
                    return new PriceViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            else
            {
                modelState.AddModelError("", "Object Doesn't Exist For Update!");
                return this;
            }
        }
    }


}