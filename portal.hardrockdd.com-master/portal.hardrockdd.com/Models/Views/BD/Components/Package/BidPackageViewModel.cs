//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Repository.VP.PM;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Models.Views.Bid
//{
//    public class PackageListViewModel
//    {
//        public PackageListViewModel()
//        {

//        }

//        public PackageListViewModel(DB.Infrastructure.ViewPointDB.Data.Bid bid)
//        {
//            if (bid == null)
//            {
//                throw new System.ArgumentNullException(nameof(bid));
//            }
//            #region mapping
//            Co = bid.Co;
//            BidId = bid.BidId;
//            #endregion
//            if (bid.Status == (int)DB.BidStatusEnum.Proposal)
//            {
//                List = bid.ActivePackages.Where(f => f.ParentPackageId == null).Select(s => new PackageViewModel(s)).ToList();
//            }
//            else
//            {
//                List = bid.ActivePackages.Select(s => new PackageViewModel(s)).ToList();
//            }
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

//        public List<PackageViewModel> List { get; }
//    }

//    public class PackageViewModel
//    {
//        public PackageViewModel()
//        {

//        }

//        public PackageViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackage package)
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
//            BoreTypeId = package.BoreTypeId; 
//            NumberOfBores = package.NumberOfBores;
//            PipeSize = package.PipeSize;
//            RigCategoryId = package.RigCategoryId;
//            GroundDensityId = package.GroundDensityId;
//            IncludeOnProposal = package.IncludeOnProposal ?? true;
//            #endregion

//            PackageRates = new BidPackageProductionRateListViewModel(package);
//            CostItems = new BidPackageCostItemListViewModel(package);
//            RentalItems = new BidPackageRentalItemListViewModel(package);
//            LineDetails = new BoreLineListViewModel(package);
//            CostDetails = new BidCostDetailListViewModel(package);
//            Pricing = new BidPackagePricingViewModel(package);
//            BoreLines = new BoreLineListViewModel(package);
//            Scopes = new BidPackageProposalScopeListViewModel(package);
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
//        [Display(Name = "#")]
//        [TableField(Width = "5")]
//        [UIHint("LongBox")]
//        public int PackageId { get; set; }

//        [Required]
//        [Display(Name = "Description")]
//        [UIHint("TextBox")]
//        [TableField(Width = "20")]
//        public string Description { get; set; }

//        [Required]
//        [Display(Name = "Bore Type")]
//        [Field(ComboUrl = "/BDCombo/BoreTypeCombo", ComboForeignKeys = "BDCo")]
//        [UIHint("DropdownBox")]
//        public int? BoreTypeId { get; set; }

//        [Required]
//        [Display(Name = "# Of Bores")]
//        [UIHint("IntegerBox")]
//        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
//        public int? NumberOfBores { get; set; }

//        [Required]
//        [Display(Name = "Pipe Size")]
//        [UIHint("IntegerBox")]
//        [TableField(Width = "15")]
//        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
//        public decimal? PipeSize { get; set; }

//        [Required]
//        [Display(Name = "Rock Density")]
//        [Field(Placeholder = "Select Density", ComboUrl = "/GroundDensity/ComboNoDirt", ComboForeignKeys = "Co")]
//        [UIHint("DropdownBox")]
//        public int? GroundDensityId { get; set; }

//        public byte? EMCo { get; set; }

//        [Required]
//        [Display(Name = "Rig Type")]
//        [UIHint("DropdownBox")]
//        [TableField(Width = "15")]
//        [Field(Placeholder = "Select Size", ComboUrl = "/EMCombo/RigCatCombo", ComboForeignKeys = "Co")]
//        public string RigCategoryId { get; set; }

//        [Display(Name = "Include On Proposal")]
//        [Field(LabelSize = 8, TextSize = 4)]
//        [UIHint("SwitchBoxGreen")]
//        public bool IncludeOnProposal { get; set; }

//        public BidPackagePricingViewModel Pricing { get; set; }

//        public BidPackageProductionRateListViewModel PackageRates { get; set; }

//        public BidPackageCostItemListViewModel CostItems { get; set; }

//        public BidPackageRentalItemListViewModel RentalItems { get; set; }

//        public Bid.BoreLineListViewModel LineDetails { get; set; }

//        public BidCostDetailListViewModel CostDetails { get; set; }

//        public BoreLineListViewModel BoreLines { get; set; }

//        public BidPackageProposalScopeListViewModel Scopes { get; set; }
//    }
//}