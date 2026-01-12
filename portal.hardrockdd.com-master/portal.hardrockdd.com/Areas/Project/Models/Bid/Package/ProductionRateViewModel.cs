using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Project.Models.Bid.Package
{
    public class ProductionRateListViewModel
    {
        public ProductionRateListViewModel()
        {

        }

        public ProductionRateListViewModel(BidPackage package)
        {
            if (package == null)
                return;

            #region mapping
            BDCo = package.BDCo;
            BidId = package.BidId;
            PackageId = package.PackageId;
            #endregion

            List = new List<ProductionRateViewModel>();
            var phases = package.ProductionRates.GroupBy(g => new { g.PhaseId, g.PassId }).OrderBy(o => o.Key.PhaseId).Select(s => new { rates = s }).ToList();
            foreach (var phase in phases)
            {
                var item = new ProductionRateViewModel(phase.rates.ToList());
                List.Add(item);
            }
        }

        [Key]
        [Required]
        [Display(Name = "Co")]
        public byte BDCo { get; set; }

        [Key]
        [Required]
        [Display(Name = "Bid Id")]
        public int BidId { get; set; }

        [Key]
        [Required]
        [Display(Name = "Package Id")]
        public int PackageId { get; set; }

        public List<ProductionRateViewModel> List { get; }
    }

    public class ProductionRateViewModel
    {

        public ProductionRateViewModel()
        {

        }

        public ProductionRateViewModel(List<DB.Infrastructure.ViewPointDB.Data.BidPackageProductionRate> rates)
        {
            if (rates == null)
                return;


            var dirtRate = rates.FirstOrDefault(f => f.GroundDensityId == 0);
            BDCo = dirtRate.BDCo;
            BidId = dirtRate.BidId;
            PackageId = dirtRate.PackageId;
            PipeSize = dirtRate.PipeSize;
            PhaseGroupId = dirtRate.Package.Division.WPDivision.HQCompany.PhaseGroupId;
            PhaseId = dirtRate.PhaseId;
            BoreSize = dirtRate.BoreSize;
            PhaseDescription = string.Format(AppCultureInfo.CInfo(), "{0} {1}", dirtRate.Phase.Description, dirtRate.PassId == 1 ? "" : ((int)dirtRate.PassId).ToString(AppCultureInfo.CInfo()));
            PassId = (int)dirtRate.PassId;
            BoreSize = dirtRate.BoreSize;
            CalcType = (DB.BidProductionCalEnum)(dirtRate.ProductionCalTypeId ?? 0);
            
            foreach (var item in rates)
            {
                if (item.ProductionCalTypeId == (int)DB.BidProductionCalEnum.Days)
                {
                    switch (item.GroundDensityId)
                    {
                        case 0: DirtProductionValue = item.ProductionDays; break;
                        case 1: SoftRockProductionValue = item.ProductionDays; break;
                        case 2: MediumRockProductionValue = item.ProductionDays; break;
                        case 3: HardRockProductionValue = item.ProductionDays; break;
                        case 4: VeryHardRockProductionValue = item.ProductionDays; break;
                        default: VeryHardRockProductionValue = null; break;
                    }
                }
                else if (item.ProductionCalTypeId == (int)DB.BidProductionCalEnum.Rate)
                {
                    switch (item.GroundDensityId)
                    {
                        case 0: DirtProductionValue = item.ProductionRate; break;
                        case 1: SoftRockProductionValue = item.ProductionRate; break;
                        case 2: MediumRockProductionValue = item.ProductionRate; break;
                        case 3: HardRockProductionValue = item.ProductionRate; break;
                        case 4: VeryHardRockProductionValue = item.ProductionRate; break;
                        default: VeryHardRockProductionValue = null; break;
                    }
                }
            }
        }


        public bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null) 
                return false;

            using var db = new VPContext();
            if ((BoreSize == null || BoreSize == 0) && 
                (DirtProductionValue > 0 || SoftRockProductionValue > 0 || MediumRockProductionValue > 0 || HardRockProductionValue > 0 || VeryHardRockProductionValue > 0))
            {
                if (db.ProjectBudgetCodes.Any(f => f.PMCo == BDCo && f.PhaseId == PhaseId && f.Radius != null && f.BudgetCodeId.Contains("BC-")))
                {
                    var package = db.BidPackages.FirstOrDefault(f => f.BDCo == BDCo && f.BidId == BidId && f.PackageId == PackageId);
                    modelState.AddModelError("", string.Format(AppCultureInfo.CInfo(), "Bore Size is Missing in Package {0}-{1}", package.PackageId, package.Description));
                    modelState.AddModelError("BoreSize", "Bore Size is Missing");
                }
               
            }
            
            return modelState.IsValid;
        }

        public byte? PhaseGroupId { get; set; }

        [Key]
        [Required]
        [Display(Name = "Co")]
        public byte BDCo { get; set; }

        [Key]
        [Required]
        [Display(Name = "Bid Id")]
        public int BidId { get; set; }

        [Key]
        [Required]
        [Display(Name = "Package Id")]
        public int PackageId { get; set; }

        [Key]
        [Required]
        [Display(Name = "Phase Id")]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PhaseMaster/Combo", ComboForeignKeys = "PhaseGroupId")]
        public string PhaseId { get; set; }


        [Display(Name = "Pipe Size")]
        [UIHint("IntegerBox")]
        public decimal? PipeSize { get; set; }

        [Display(Name = "Phased")]
        [UIHint("Textbox")]
        public string PhaseDescription { get; set; }

        [Key]
        [Required]
        [Display(Name = "Pass Id")]
        [UIHint("IntegerBox")]
        public int PassId { get; set; }

        [Field(ComboUrl = "/BudgetCode/PassSizeCombo", ComboForeignKeys = "PhaseGroupId,PhaseId")]
        [Display(Name = "Bore Size")]
        [UIHint("DropdownBox")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? BoreSize { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Calc Type")]
        public DB.BidProductionCalEnum CalcType { get; set; }

        [Display(Name = "Dirt")]
        [UIHint("IntegerBox")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal? DirtProductionValue { get; set; }

        [Display(Name = "Soft")]
        [UIHint("IntegerBox")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal? SoftRockProductionValue { get; set; }

        [Display(Name = "Medium")]
        [UIHint("IntegerBox")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal? MediumRockProductionValue { get; set; }

        [Display(Name = "Hard")]
        [UIHint("IntegerBox")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal? HardRockProductionValue { get; set; }

        [Display(Name = "Very Hard")]
        [UIHint("IntegerBox")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal? VeryHardRockProductionValue { get; set; }


        internal ProductionRateViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObjs = db.BidPackageProductionRates.Where(f =>  f.BDCo == BDCo &&
                                                                   f.BidId == BidId &&
                                                                   f.PackageId == PackageId &&
                                                                   f.PhaseId == PhaseId &&
                                                                   f.PassId == PassId).ToList();

            if (updObjs.Any())
            {

                updObjs.ForEach(rate =>
                {

                    rate.BoreSize = BoreSize;
                    rate.ProductionCalTypeId = (int)CalcType;
                    rate.ProductionDays = null;
                    rate.ProductionRate = null;

                    if (CalcType == DB.BidProductionCalEnum.Days)
                    {
                        rate.ProductionDays = rate.GroundDensityId switch
                        {
                            0 => DirtProductionValue,
                            1 => SoftRockProductionValue,
                            2 => MediumRockProductionValue,
                            3 => HardRockProductionValue,
                            4 => VeryHardRockProductionValue,
                            _ => null,
                        };
                    }
                    else
                    {
                        rate.ProductionRate = rate.GroundDensityId switch
                        {
                            0 => DirtProductionValue,
                            1 => SoftRockProductionValue,
                            2 => MediumRockProductionValue,
                            3 => HardRockProductionValue,
                            4 => VeryHardRockProductionValue,
                            _ => null,
                        };
                    }
                });

                try
                {
                    db.BulkSaveChanges();
                    return new ProductionRateViewModel(updObjs);
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