//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Models.Views.Bid
//{
//    public class BidPackageCostItemListViewModel
//    {
//        public BidPackageCostItemListViewModel()
//        {

//        }

//        public BidPackageCostItemListViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackage package)
//        {
//            if (package == null)
//            {
//                throw new ArgumentNullException(nameof(package));
//            }

//            #region mapping
//            Co = package.Co;
//            BidId = package.BidId;
//            PackageId = package.PackageId;
//            #endregion

//            List = package.CostItems.Where(f => f.BudgetCodeId != null)
//                                    .ToList()
//                                    .Where(f => f.BudgetCodeId.StartsWith("CI-", StringComparison.Ordinal))
//                                    .Select(s => new PackageCostItemViewModel(s)).ToList();
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

//        public List<PackageCostItemViewModel> List { get; }
//    }

//    public class PackageCostItemViewModel
//    {
//        public PackageCostItemViewModel()
//        {

//        }

//        public PackageCostItemViewModel(DB.Infrastructure.ViewPointDB.Data.PackageCostItem bidCostItem)
//        {
//            if (bidCostItem == null)
//            {
//                throw new ArgumentNullException(nameof(bidCostItem));
//            }
//            Co = bidCostItem.Co;
//            BidId = bidCostItem.BidId;
//            PackageId = bidCostItem.PackageId;
//            LineId = bidCostItem.LineId;
//            BudgetCodeId = bidCostItem.BudgetCodeId;
//            Multiplier = bidCostItem.Multiplier;
//            //Quantity = obj.;

//            Applied = bidCostItem.Applied == 1 ? true : false;
//            UM = bidCostItem.BudgetCode?.UM;
//        }

//        [Key]
//        [Required]
//        [HiddenInput]
//        public byte Co { get; set; }

//        [Key]
//        [Required]
//        [HiddenInput]
//        public int BidId { get; set; }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Package Id")]
//        public int PackageId { get; set; }

//        [Key]
//        [Required]
//        [HiddenInput]
//        public int LineId { get; set; }

//        [Required]
//        [Display(Name = "Cost Item")]
//        [UIHint("DropDownBox")]
//        [Field(Placeholder = "Select Item", ComboUrl = "/BudgetCode/CostItemCombo", ComboForeignKeys = "Co")]
//        [TableField(Width = "20")]
//        public string BudgetCodeId { get; set; }

//        [Required]
//        [Display(Name = "Apply")]
//        [UIHint("SwitchBox")]
//        public bool Applied { get; set; }

//        [Required]
//        [Display(Name = "Multiplier")]
//        [UIHint("IntegerBox")]
//        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
//        public decimal? Multiplier { get; set; }

//        [Required]
//        [Display(Name = "UM")]
//        [UIHint("TextBox")]
//        [TableField(Width = "20")]
//        public string UM { get; set; }
//    }
//}