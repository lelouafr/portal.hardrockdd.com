//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.ComponentModel;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Models.Views.Bid
//{
//    public class BidPackagePricingViewModel
//    {
//        public BidPackagePricingViewModel()
//        {
//            //using var empRepo = new EmployeeRepository();

//            //var user = empRepo.GetEmployee(StaticFunctions.GetUserId());
//            //Co = user.PRCo;
//        }

//        public BidPackagePricingViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackage package)
//        {
//            if (package == null)
//            {
//                throw new System.ArgumentNullException(nameof(package));
//            }
//            #region mapping
//            Co = package.Co;
//            BidId = package.BidId;
//            PackageId = package.PackageId;
//            Description = package.Description;
//            StandardGM = .4M;
//            DirtGM = package.DirtGM ?? StandardGM;
//            RockGM = package.RockGM ?? StandardGM;
//            #endregion

//            DirtGM = DirtGM == 0 ? StandardGM : DirtGM;
//            RockGM = RockGM == 0 ? StandardGM : RockGM;

//            var totalFootage = package.TotalFootage;
            
//            var costDetails = new BidCostDetailListViewModel(package);

            
//            foreach (var subPackage in package.SubPackages)
//            {
//                //var subCostDetails = new BidCostDetailListViewModel(subPackage);
//                //costDetails.List.AddRange(subCostDetails.List);

//                var totalSubFootage = subPackage.BoreLines.Where(f => f.IntersectBoreId == null && f.Status != (int)BidBoreLineStatusEnum.Deleted && f.Status != (int)BidBoreLineStatusEnum.Canceled).Sum(sum => sum.Footage ?? 0);
//                totalFootage += totalSubFootage;
//            }

//            DirtCost = Math.Round(costDetails.List.Where(f => f.GroundDensityType == "Dirt").Sum(s => s.ExtCost), 0);
//            RockCost = Math.Round(costDetails.List.Where(f => f.GroundDensityType == "Rock").Sum(s => s.ExtCost), 0);

//            if (package.ParentPackageId != null)
//            {
//                package.DirtGM = package.ParentPackage.DirtGM;
//                package.DirtPrice = package.ParentPackage.DirtPrice;
//                package.RockGM = package.ParentPackage.RockGM;
//                package.RockPrice = package.ParentPackage.RockPrice;
//            }

//            if (package.DirtPrice != null)// && DirtRevenue != 0
//            {
//                DirtRevenue = (decimal)package.DirtPrice * totalFootage;
//                DirtGM = Math.Round((DirtRevenue - DirtCost) / DirtRevenue, 4);
//                DirtLFPrice = (decimal)package.DirtPrice;
//            }
//            else
//            {
//                DirtRevenue = Math.Round(DirtCost * (1 / (1 - DirtGM)), 0);
//                if (DirtRevenue != 0 && totalFootage!= 0)
//                {
//                    DirtLFPrice = Math.Round(DirtRevenue / totalFootage, 2);
//                }
//            }

//            if (package.RockPrice != null)// && RockRevenue != 0
//            {
//                RockRevenue = (decimal)package.RockPrice * totalFootage;
//                RockGM = Math.Round((RockRevenue - RockCost) / RockRevenue,4);
//                RockLFPrice = (decimal)package.RockPrice;
//            }
//            else
//            {
//                RockRevenue = Math.Round(RockCost * (1 / (1 - RockGM)), 0);
//                if (RockRevenue != 0 && totalFootage != 0)
//                {
//                    RockLFPrice = Math.Round(RockRevenue / totalFootage, 2);
//                }
//            }

//            if (totalFootage != 0)
//            {
//                DirtLFCost = Math.Round(DirtCost / totalFootage, 2);
//                RockLFCost = Math.Round(RockCost / totalFootage, 2);

//            }

//            DirtProfit = DirtRevenue - DirtCost;
//            RockProfit = RockRevenue - RockCost;

//            RockRevenueAdder = RockLFPrice - DirtLFPrice;

//            DirtBidDays = Math.Round(costDetails.List.Where(f => f.GroundDensityType == "Dirt" && f.BudgetCodeDescription.StartsWith("Labor Per Man")).Sum(s => s.Units) ?? 0, 2, MidpointRounding.AwayFromZero);
//            RockBidDays = Math.Round(costDetails.List.Where(f => f.GroundDensityType == "Rock" && f.BudgetCodeDescription.StartsWith("Labor Per Man")).Sum(s => s.Units) ?? 0, 2, MidpointRounding.AwayFromZero);
//            //DirtGM *= 100;
//            //RockGM *= 100;
//            //StandardGM *= 100;



//        }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Co")]
//        public byte Co { get; set; }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Bid Id")]
//        public int BidId { get; set; }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Package Id")]
//        public int PackageId { get; set; }

//        [Required]
//        [Display(Name = "Description")]
//        [UIHint("TextAreaBox")]
//        [Field(LabelSize = 3, TextSize = 9, FormGroup = "Bid Info", FormGroupRow = 1)]
//        public string Description { get; set; }


//        [ReadOnly(true)]
//        [Display(Name = "Dirt Days")]
//        [UIHint("LongBox")]
//        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Info", FormGroupRow = 2)]
//        public decimal DirtBidDays { get; set; }

//        [ReadOnly(true)]
//        [Display(Name = "Rock Days")]
//        [UIHint("LongBox")]
//        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Info", FormGroupRow = 2)]
//        public decimal RockBidDays { get; set; }


//        [ReadOnly(true)]
//        [Display(Name = "Dirt Cost")]
//        [UIHint("CurrencyBox")]
//        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Cost", FormGroupRow = 1)]
//        public decimal DirtCost { get; set; }


//        [ReadOnly(true)]
//        [Display(Name = "Rock Cost")]
//        [UIHint("CurrencyBox")]
//        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Cost", FormGroupRow = 1)]
//        public decimal RockCost { get; set; }

//        [ReadOnly(true)]
//        [Display(Name = "Dirt LF Cost")]
//        [UIHint("CurrencyBox")]
//        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Cost", FormGroupRow = 2)]
//        public decimal DirtLFCost { get; set; }

//        [ReadOnly(true)]
//        [Display(Name = "Rock LF Cost")]
//        [UIHint("CurrencyBox")]
//        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Cost", FormGroupRow = 2)]
//        public decimal RockLFCost { get; set; }

//        [ReadOnly(true)]
//        [Display(Name = "Standard GM")]
//        [UIHint("PercentBox")]
//        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Gross Margin", FormGroupRow = 1)]
//        public decimal StandardGM { get; set; }


//        [Display(Name = "Dirt GM")]
//        [UIHint("PercentBox")]
//        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Gross Margin", FormGroupRow = 2)]
//        public decimal DirtGM { get; set; }


//        [Display(Name = "Rock GM")]
//        [UIHint("PercentBox")]
//        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Gross Margin", FormGroupRow = 2)]
//        public decimal RockGM { get; set; }

//        [ReadOnly(true)]
//        [Display(Name = "Dirt Revenue")]
//        [UIHint("CurrencyBox")]
//        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Revenue", FormGroupRow = 1)]
//        public decimal DirtRevenue { get; set; }

//        [ReadOnly(true)]
//        [Display(Name = "Rock Revenue")]
//        [UIHint("CurrencyBox")]
//        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Revenue", FormGroupRow = 1)]
//        public decimal RockRevenue { get; set; }


//        [Display(Name = "Dirt LF Price")]
//        [UIHint("CurrencyBox")]
//        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Revenue", FormGroupRow = 2)]
//        public decimal DirtLFPrice { get; set; }

//        [Display(Name = "Rock LF Price")]
//        [UIHint("CurrencyBox")]
//        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Revenue", FormGroupRow = 2)]
//        public decimal RockLFPrice { get; set; }

//        [ReadOnly(true)]
//        [Display(Name = "Rock Adder")]
//        [UIHint("CurrencyBox")]
//        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Revenue", FormGroupRow = 4)]
//        public decimal RockRevenueAdder { get; set; }

//        [ReadOnly(true)]
//        [Display(Name = "Dirt Profit")]
//        [UIHint("CurrencyBox")]
//        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Revenue", FormGroupRow = 5)]
//        public decimal DirtProfit { get; set; }

//        [ReadOnly(true)]
//        [Display(Name = "Rock Profit")]
//        [UIHint("CurrencyBox")]
//        [Field(LabelSize = 3, TextSize = 3, FormGroup = "Bid Revenue", FormGroupRow = 5)]
//        public decimal RockProfit { get; set; }


//    }


//}