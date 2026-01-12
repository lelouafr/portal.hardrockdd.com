//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Models.Views.Bid
//{
//    public class BidPackageRentalItemListViewModel
//    {
//        public BidPackageRentalItemListViewModel()
//        {

//        }

//        public BidPackageRentalItemListViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackage package)
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

//            List = package.CostItems.Where(f => f.BudgetCodeId != null)
//                                    .ToList()
//                                    .Where(f => f.BudgetCodeId.StartsWith("RE-", StringComparison.Ordinal))
//                                    .Select(s => new PackageCostItemRentalViewModel(s))
//                                    .ToList();
            
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

//        public List<PackageCostItemRentalViewModel> List { get; }
//    }

//    public class PackageCostItemRentalViewModel
//    {
//        public PackageCostItemRentalViewModel()
//        {

//        }

//        public PackageCostItemRentalViewModel(DB.Infrastructure.ViewPointDB.Data.PackageCostItem costItem)
//        {
//            if (costItem == null)
//            {
//                throw new System.ArgumentNullException(nameof(costItem));
//            }
//            Co = costItem.Co;
//            BidId = costItem.BidId;
//            PackageId = costItem.PackageId;
//            LineId = costItem.LineId;
//            BudgetCodeId = costItem.BudgetCodeId;
//            Multiplier = costItem.Multiplier;

//            Applied = costItem.Applied == 1 ? true : false;
//            UM = costItem.BudgetCode?.UM;
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
//        [Required]
//        [HiddenInput]
//        public int PackageId { get; set; }

//        [Key]
//        [Required]
//        [HiddenInput]
//        public int LineId { get; set; }

//        [Required]
//        [Display(Name = "Rental")]
//        [UIHint("DropDownBox")]
//        [Field(Placeholder = "Select Rental", ComboUrl = "/BudgetCode/RentedCombo", ComboForeignKeys = "Co")]
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