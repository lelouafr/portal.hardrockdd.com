//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Models.Views.Bid
//{
//    public class BidPackageCostListViewModel
//    {
//        public BidPackageCostListViewModel()
//        {

//        }

//        public BidPackageCostListViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackage package)
//        {
//            if (package == null)
//            {
//                throw new System.ArgumentNullException(nameof(package));
//            }
//            #region mapping
//            Co = package.Co;
//            BidId = package.BidId;
//            PackageId = package.PackageId;
//            #endregion
//            List = package.PackageCosts.Select(s => new BidPackageCostViewModel(s)).ToList();
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

//        public List<BidPackageCostViewModel> List { get; }
//    }

//    public class BidPackageCostViewModel
//    {

//        public BidPackageCostViewModel()
//        {

//        }

//        public BidPackageCostViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackageCost cost)
//        {
//            if (cost == null)
//            {
//                throw new System.ArgumentNullException(nameof(cost));
//            }
//            Co = cost.Co;
//            BidId = cost.BidId;
//            PackageId = cost.PackageId;
//            LineId = cost.LineId;
//            AllocationType = (DB.BDPackageCostAllocationType?)cost.CostAllocationTypeId;
//            BudgetCodeId = cost.BudgetCodeId;
//            Cost = cost.Cost;
            
//        }
        

//        [Key]
//        [Required]
//        [HiddenInput]
//        [Display(Name = "Co")]
//        public byte Co { get; set; }

//        [Key]
//        [Required]
//        [HiddenInput]
//        [Display(Name = "Bid Id")]
//        public int BidId { get; set; }

//        [Key]
//        [Required]
//        [HiddenInput]
//        [Display(Name = "Package Id")]
//        public int PackageId { get; set; }


//        [Key]
//        [Required]
//        [HiddenInput]
//        [Display(Name = "Line Id")]
//        public int LineId { get; set; }

//        [Required]
//        [Display(Name = "Allocation Type")]
//        [UIHint("EnumBox")]
//        [Field(Placeholder ="Select Type")]
//        public DB.BDPackageCostAllocationType? AllocationType { get; set; }

//        [Required]
//        [Display(Name = "Cost Item")]
//        [UIHint("DropdownBox")]
//        [Field( ComboUrl = "/BudgetCode/CostItemCombo", ComboForeignKeys = "Co")]
//        public string BudgetCodeId { get; set; }


//        [Required]
//        [Display(Name = "Cost")]
//        [UIHint("CurrencyBox")]
//        public decimal? Cost { get; set; }


//    }
//}