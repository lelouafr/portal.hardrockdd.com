//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Models.Views.Bid.DNU
//{
//    public class BidPackageProductionRateListViewModel
//    {
//        public BidPackageProductionRateListViewModel()
//        {

//        }

//        public BidPackageProductionRateListViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackage package)
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

//            List = package.ProductionRates.Select(s => new BidPackageProductionRateViewModel(s)).ToList();
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

//        public List<BidPackageProductionRateViewModel> List { get; }
//    }

//    public class BidPackageProductionRatePipeSizeListViewModel
//    {
//        public BidPackageProductionRatePipeSizeListViewModel()
//        {

//        }

//        public BidPackageProductionRatePipeSizeListViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackage package, int pipeSize)
//        {
//            if (package == null)
//            {
//                throw new System.ArgumentNullException(nameof(package));
//            }
//            #region mapping
//            Co = package.Co;
//            BidId = package.BidId;
//            PackageId = package.PackageId;
//            PipeSize = pipeSize;
//            #endregion

//            List = package.ProductionRates.Where(f => f.PipeSize == pipeSize).Select(s => new BidPackageProductionRateViewModel(s)).ToList();
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

//        [Key]
//        [Required]
//        [HiddenInput]
//        //[Field(Placeholder = "Select Size", ComboUrl = "/BudgetCode/SizeCombo", ComboForeignKeys = "Co")]
//        [Display(Name = "Pipe Size")]
//        public decimal PipeSize { get; set; }

//        public List<BidPackageProductionRateViewModel> List { get; set; }
//    }

//    public class BidPackageProductionRateBoreSizeViewModel
//    {

//        public BidPackageProductionRateBoreSizeViewModel()
//        {

//        }

//        public BidPackageProductionRateBoreSizeViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackageProductionRate rate)
//        {
//            if (rate == null)
//            {
//                throw new System.ArgumentNullException(nameof(rate));
//            }

//            Co = rate.Co;
//            BidId = rate.BidId;
//            PackageId = rate.PackageId;
//            LineId = rate.LineId;
//            PipeSize = rate.PipeSize;
//            PhaseId = rate.PhaseId;
//            PassId = (int)rate.PassId;
//            BoreSize = rate.BoreSize;
//        }

//        public BidPackageProductionRateBoreSizeViewModel(BidPackageProductionRateViewModel rate)
//        {
//            if (rate == null)
//            {
//                throw new System.ArgumentNullException(nameof(rate));
//            }

//            Co = rate.Co;
//            BidId = rate.BidId;
//            PackageId = rate.PackageId;
//            LineId = rate.LineId;
//            PipeSize = rate.PipeSize;
//            PhaseId = rate.PhaseId;
//            PassId = rate.PassId;
//            BoreSize = rate.BoreSize;
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
//        [HiddenInput]
//        //[Field(Placeholder = "Select Size", ComboUrl = "/BudgetCode/SizeCombo", ComboForeignKeys = "Co")]
//        [Display(Name = "Pipe Size")]
//        public decimal? PipeSize { get; set; }

//        [Required]
//        [Display(Name = "Phase Id")]
//        [HiddenInput]
//        [Field(Placeholder = "Select Phase", ComboUrl = "/PhaseMaster/Combo", ComboForeignKeys = "Co")]
//        public string PhaseId { get; set; }

//        [Required]
//        [Display(Name = "Pass Id")]
//        [HiddenInput]
//        public int PassId { get; set; }

//        [Field(LabelSize = 0, TextSize = 12, FormGroup = "Size", FormGroupRow = 1, ComboUrl = "/BudgetCode/PassSizeCombo", ComboForeignKeys = "Co,PhaseId")]
//        [Display(Name = "Bore Size")]
//        [UIHint("DropdownBox")]
//        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
//        public decimal? BoreSize { get; set; }

//    }
    
//    public class BidPackageProductionRateViewModel
//    {

//        public BidPackageProductionRateViewModel()
//        {

//        }

//        public BidPackageProductionRateViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackageProductionRate rate)
//        {
//            if (rate == null)
//            {
//                throw new System.ArgumentNullException(nameof(rate));
//            }

//            Co = rate.Co;
//            BidId = rate.BidId;
//            PackageId = rate.PackageId;
//            LineId = rate.LineId;
//            PipeSize = rate.PipeSize;
//            PhaseId = rate.PhaseId;
//            BoreSize = rate.BoreSize;
//            Phase = string.Format(AppCultureInfo.CInfo(), "{0} {1}", rate.Phase.Description, rate.PassId == 1 ? "" : ((int)rate.PassId).ToString(AppCultureInfo.CInfo()));
//            GroundDensityId = (int)rate.GroundDensityId;
//            GroundDensity = rate.GroundDensity.Description;
//            PassId = (int)rate.PassId;
//            BoreSize = rate.BoreSize;
//            CalcType = (DB.BidProductionCalEnum)(rate.ProductionCalTypeId ?? 0);
//            ProductionRate = rate.ProductionRate;
//            ProductionDays = rate.ProductionDays;
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
//        [HiddenInput]
//        //[Field(Placeholder = "Select Size", ComboUrl = "/BudgetCode/SizeCombo", ComboForeignKeys = "Co")]
//        [Display(Name = "Pipe Size")]
//        public decimal? PipeSize { get; set; }

//        [Required]
//        [Display(Name = "Phase Id")]
//        [HiddenInput]
//        [Field(Placeholder = "Select Phase", ComboUrl = "/PhaseMaster/Combo", ComboForeignKeys = "Co")]
//        public string PhaseId { get; set; }

//        [HiddenInput]
//        [Display(Name = "Phase")]
//        public string Phase { get; set; }



//        [Required]
//        [Field(Placeholder = "Select Density", ComboUrl = "/GroundDensity/Combo", ComboForeignKeys = "Co")]
//        [Display(Name = "Ground Density Id")]
//        [HiddenInput]
//        public int GroundDensityId { get; set; }

//        [HiddenInput]
//        [Display(Name = "Ground Density")]
//        public string GroundDensity { get; set; }

//        [Required]
//        [Display(Name = "Pass Id")]
//        [HiddenInput]
//        public int PassId { get; set; }

//        [Required]
//        //[Field(Placeholder = "Select Size", ComboUrl = "/BudgetCode/PassSizeCombo", ComboForeignKeys = "Co,PhaseId")]
//        [Display(Name = "Bore Size")]
//        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
//        [HiddenInput]
//        public decimal? BoreSize { get; set; }

//        [Required]
//        [UIHint("EnumBox")]
//        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Project Info", FormGroupRow = 2)]
//        [Display(Name = "Calc Type")]
//        public DB.BidProductionCalEnum CalcType { get; set; }

//        [Display(Name = "Production Rate")]
//        [Field(LabelSize = 0, TextSize = 12, FormGroup = "RAte", FormGroupRow = 1)]
//        [UIHint("IntegerBox")]
//        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
//        public decimal? ProductionRate { get; set; }

//        [Display(Name = "Production Days")]
//        [Field(LabelSize = 0, TextSize = 12, FormGroup = "Rate", FormGroupRow = 1)]
//        [UIHint("IntegerBox")]
//        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
//        public decimal? ProductionDays { get; set; }

//    }

//}