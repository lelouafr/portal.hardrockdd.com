//using DB.Infrastructure.ViewPointDB.Data;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Models.Views.Bid
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
//            List = new List<BidPackageProductionRateViewModel>();
//            var phases = package.ProductionRates.GroupBy(g => new { g.PhaseId, g.PassId }).OrderBy(o => o.Key.PhaseId).Select(s => new { rates = s }).ToList();
//            foreach (var phase in phases)
//            {
//                var item = new BidPackageProductionRateViewModel(phase.rates.ToList());
//                List.Add(item);
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

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Package Id")]
//        public int PackageId { get; set; }

//        public List<BidPackageProductionRateViewModel> List { get; }
//    }

//    public class BidPackageProductionRateViewModel
//    {

//        public BidPackageProductionRateViewModel()
//        {

//        }

//        public BidPackageProductionRateViewModel(List<DB.Infrastructure.ViewPointDB.Data.BidPackageProductionRate> rates)
//        {
//            if (rates == null) throw new System.ArgumentNullException(nameof(rates));

//            var dirtRate = rates.FirstOrDefault(f => f.GroundDensityId == 0);
//            Co = dirtRate.Co;
//            BidId = dirtRate.BidId;
//            PackageId = dirtRate.PackageId;
//            PipeSize = dirtRate.PipeSize;
//            PhaseId = dirtRate.PhaseId;
//            BoreSize = dirtRate.BoreSize;
//            PhaseDescription = string.Format(AppCultureInfo.CInfo(), "{0} {1}", dirtRate.Phase.Description, dirtRate.PassId == 1 ? "" : ((int)dirtRate.PassId).ToString(AppCultureInfo.CInfo()));
//            PassId = (int)dirtRate.PassId;
//            BoreSize = dirtRate.BoreSize;
//            CalcType = (DB.BidProductionCalEnum)(dirtRate.ProductionCalTypeId ?? 0);
//            foreach (var item in rates)
//            {
//                if (item.ProductionCalTypeId == (int)DB.BidProductionCalEnum.Days)
//                {
//                    switch (item.GroundDensityId)
//                    {
//                        case 0: DirtProductionValue = item.ProductionDays; break;
//                        case 1: SoftRockProductionValue = item.ProductionDays; break;
//                        case 2: MediumRockProductionValue = item.ProductionDays; break;
//                        case 3: HardRockProductionValue = item.ProductionDays; break;
//                        case 4: VeryHardRockProductionValue = item.ProductionDays; break;
//                        default: VeryHardRockProductionValue = null; break;
//                    }
//                }
//                else if (item.ProductionCalTypeId == (int)DB.BidProductionCalEnum.Rate)
//                {
//                    switch (item.GroundDensityId)
//                    {
//                        case 0: DirtProductionValue = item.ProductionRate; break;
//                        case 1: SoftRockProductionValue = item.ProductionRate; break;
//                        case 2: MediumRockProductionValue = item.ProductionRate; break;
//                        case 3: HardRockProductionValue = item.ProductionRate; break;
//                        case 4: VeryHardRockProductionValue = item.ProductionRate; break;
//                        default: VeryHardRockProductionValue = null; break;
//                    }
//                }
//            }
//        }


//        public bool Validate(ModelStateDictionary modelState)
//        {
//            if (modelState == null) throw new System.ArgumentNullException(nameof(modelState));
//            using var db = new VPContext();
//            if ((BoreSize == null || BoreSize == 0) && 
//                CalcType == DB.BidProductionCalEnum.Rate && 
//                DirtProductionValue > 0)
//            {
//                if (db.ProjectBudgetCodes.Any(f => f.PMCo == Co && f.PhaseId == PhaseId && f.Radius != null && f.BudgetCodeId.Contains("BC-")))
//                {
//                    var package = db.BidBoreLines.FirstOrDefault(f => f.Co == Co && f.BidId == BidId && f.PackageId == PackageId);

//                    modelState.AddModelError("", string.Format(AppCultureInfo.CInfo(), "Bore Size is Missing in Package {0}-{1}", package.PackageId, package.Description));
//                    modelState.AddModelError("BoreSize", "Bore Size is Missing");
//                }
               
//            }
            
//            return modelState.IsValid;
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

//        [Display(Name = "Pipe Size")]
//        [UIHint("IntegerBox")]
//        public decimal? PipeSize { get; set; }

//        [Key]
//        [Required]
//        [Display(Name = "Phase Id")]
//        [UIHint("DropdownBox")]
//        [Field(Placeholder = "Select Phase", ComboUrl = "/PhaseMaster/Combo", ComboForeignKeys = "Co")]
//        public string PhaseId { get; set; }

//        [Display(Name = "Phased")]
//        [UIHint("Textbox")]
//        [Field(Placeholder = "Select Phase", ComboUrl = "/PhaseMaster/Combo", ComboForeignKeys = "Co")]
//        public string PhaseDescription { get; set; }

//        [Key]
//        [Required]
//        [Display(Name = "Pass Id")]
//        [UIHint("IntegerBox")]
//        public int PassId { get; set; }

//        [Field(Placeholder = "Select Size", ComboUrl = "/BudgetCode/PassSizeCombo", ComboForeignKeys = "Co,PhaseId")]
//        [Display(Name = "Bore Size")]
//        [UIHint("DropdownBox")]
//        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
//        public decimal? BoreSize { get; set; }

//        [Required]
//        [UIHint("EnumBox")]
//        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Project Info", FormGroupRow = 2)]
//        [Display(Name = "Calc Type")]
//        public DB.BidProductionCalEnum CalcType { get; set; }

//        [Display(Name = "Dirt")]
//        [Field(LabelSize = 0, TextSize = 12, FormGroup = "RAte", FormGroupRow = 1)]
//        [UIHint("IntegerBox")]
//        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
//        public decimal? DirtProductionValue { get; set; }


//        //[Display(Name = "Rock Rate")]
//        //[Field(LabelSize = 0, TextSize = 12, FormGroup = "RAte", FormGroupRow = 1)]
//        //[UIHint("IntegerBox")]
//        //[DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
//        //public decimal? RockProductionValue { get; set; }


//        [Display(Name = "Soft")]
//        [Field(LabelSize = 0, TextSize = 12, FormGroup = "RAte", FormGroupRow = 1)]
//        [UIHint("IntegerBox")]
//        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
//        public decimal? SoftRockProductionValue { get; set; }


//        [Display(Name = "Medium")]
//        [Field(LabelSize = 0, TextSize = 12, FormGroup = "RAte", FormGroupRow = 1)]
//        [UIHint("IntegerBox")]
//        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
//        public decimal? MediumRockProductionValue { get; set; }

//        [Display(Name = "Hard")]
//        [Field(LabelSize = 0, TextSize = 12, FormGroup = "RAte", FormGroupRow = 1)]
//        [UIHint("IntegerBox")]
//        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
//        public decimal? HardRockProductionValue { get; set; }


//        [Display(Name = "Very Hard")]
//        [Field(LabelSize = 0, TextSize = 12, FormGroup = "RAte", FormGroupRow = 1)]
//        [UIHint("IntegerBox")]
//        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
//        public decimal? VeryHardRockProductionValue { get; set; }

//    }
//}