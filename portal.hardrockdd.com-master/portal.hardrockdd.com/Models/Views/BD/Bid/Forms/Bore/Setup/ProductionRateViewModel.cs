using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Bid.Forms.Bore.Setup
{
    public class ProductionRateListViewModel
    {
        public ProductionRateListViewModel()
        {

        }

        public ProductionRateListViewModel(BidBoreLine line)
        {
            if (line == null)
            {
                throw new System.ArgumentNullException(nameof(line));
            }
            #region mapping
            BDCo = line.BDCo;
            BidId = line.BidId;
            PackageId = line.PackageId ?? 0;
            BoreId = line.BoreId;
            #endregion

            List = line.Passes
                .Where(f => f.Deleted == false)
                .GroupBy(g => new { g.PhaseId, g.PassId })
                .OrderBy(o => o.Key.PhaseId)
                .Select(s => new ProductionRateViewModel(s.ToList(), line))
                .ToList();
        }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte BDCo { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Bid Id")]
        public int BidId { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Package Id")]
        public int PackageId { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "#")]
        public int BoreId { get; set; }

        public List<ProductionRateViewModel> List { get; }
    }

    public class ProductionRateViewModel
    {

        public ProductionRateViewModel()
        {

        }

        public ProductionRateViewModel(List<DB.Infrastructure.ViewPointDB.Data.BidBoreLinePass> rates, BidBoreLine line)
        {
            if (rates == null) throw new System.ArgumentNullException(nameof(rates));
            if (line == null) throw new System.ArgumentNullException(nameof(line));

            var dirtRate = rates.FirstOrDefault(f => f.GroundDensityId == 0);
            var rockRate = rates.FirstOrDefault(f => f.GroundDensityId == line.GroundDensityId);

            var rate = dirtRate ?? rockRate;
            if (rate.BoreLine.Division == null)
            {
                rate.BoreLine.UpdateDivision(rate.BoreLine.Package.DivisionId);
                rate.db.BulkSaveChanges();
            }

            PhaseGroupId = rate.BoreLine.Division.WPDivision.HQCompany.PhaseGroupId;
            BDCo = rate.BDCo;
            BidId = rate.BidId;
            PackageId = line.PackageId ?? 0;
            BoreId = line.BoreId;
            PhaseId = rate.PhaseId;
            BoreSize = rate.BoreSize;
            PhaseDescription = string.Format(AppCultureInfo.CInfo(), "{0} {1}", rate.PhaseMaster.Description, rate.PassId == 1 ? "" : rate.PassId.ToString(AppCultureInfo.CInfo()));
            PassId = rate.PassId;

            CalcType = (DB.BidProductionCalEnum)(rate.ProductionCalTypeId ?? 0);

            if (rate.ProductionCalTypeId == (int)DB.BidProductionCalEnum.Days)
            {
                DirtProductionValue = dirtRate?.ProductionDays;
                RockProductionValue = rockRate?.ProductionDays;
            }
            else if (rate.ProductionCalTypeId == (int)DB.BidProductionCalEnum.Rate)
            {
                DirtProductionValue = dirtRate?.ProductionRate;
                RockProductionValue = rockRate?.ProductionRate;
            }
        }


        public ProductionRateViewModel(BidBoreLinePass dirtRate, BidBoreLinePass rockRate, BidBoreLine line)
        {
            if (dirtRate == null) throw new System.ArgumentNullException(nameof(dirtRate));
            if (rockRate == null) throw new System.ArgumentNullException(nameof(rockRate));
            if (line == null) throw new System.ArgumentNullException(nameof(line));

            var rate = dirtRate ?? rockRate;

            BDCo = line.BDCo;
            BidId = line.BidId;
            PackageId = line.PackageId ?? 0;
            BoreId = line.BoreId;

            PhaseGroupId = line.Package.Division.WPDivision.HQCompany.PhaseGroupId;
            PhaseId = rate.PhaseId;
            BoreSize = rate.BoreSize;
            PhaseDescription = string.Format(AppCultureInfo.CInfo(), "{0} {1}", rate.PhaseMaster?.Description, rate.PassId == 1 ? "" : rate.PassId.ToString(AppCultureInfo.CInfo()));
            PassId = rate.PassId;
            CalcType = (DB.BidProductionCalEnum)(rate.ProductionCalTypeId ?? 0);

            if (rate.ProductionCalTypeId == (int)DB.BidProductionCalEnum.Days)
            {
                DirtProductionValue = dirtRate?.ProductionDays;
                RockProductionValue = rockRate?.ProductionDays;
            }
            else if (rate.ProductionCalTypeId == (int)DB.BidProductionCalEnum.Rate)
            {
                DirtProductionValue = dirtRate?.ProductionRate;
                RockProductionValue = rockRate?.ProductionRate;
            }
        }


        public bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null) throw new System.ArgumentNullException(nameof(modelState));
            using var db = new VPContext();
            if ((BoreSize == null || BoreSize == 0) && 
                CalcType == DB.BidProductionCalEnum.Rate &&
                (DirtProductionValue > 0 || RockProductionValue > 0))
            {
                if (db.ProjectBudgetCodes.Any(f => f.PMCo == BDCo && f.PhaseId == PhaseId && f.Radius != null && f.BudgetCodeId.Contains("BC-")))
                {
                    var line = db.BidBoreLines.FirstOrDefault(f => f.BDCo == BDCo && f.BidId == BidId && f.PackageId == PackageId && f.BoreId == BoreId);

                    modelState.AddModelError("", string.Format(AppCultureInfo.CInfo(), "Bore Size is Missing in Bore {0}-{1}", line.BoreId, line.Description));
                    modelState.AddModelError("BoreSize", "Bore Size is Missing");
                }
               
            }
            
            return modelState.IsValid;
        }
        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Co")]
        public byte BDCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Bid Id")]
        public int BidId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Package Id")]
        public int PackageId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Bore Id")]
        public int BoreId { get; set; }

        public byte? PhaseGroupId { get; set; }

        [Key]
        [Required]
        [Display(Name = "Phase Id")]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PhaseMaster/Combo", ComboForeignKeys = "PhaseGroupId")]
        public string PhaseId { get; set; }


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

        [Display(Name = "Rock")]
        [UIHint("IntegerBox")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal? RockProductionValue { get; set; }


        internal ProductionRateViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var bore = db.BidBoreLines.FirstOrDefault(f => f.BDCo == BDCo && f.BidId == BidId && f.BoreId == BoreId);
            var phase = db.PhaseMasters.FirstOrDefault(f => f.PhaseGroupId == PhaseGroupId && f.PhaseId == PhaseId);
            var bidParms = bore.Bid.Company.BDCompanyParm;
            var updObjs = bore.AddProductionPasses(phase, PassId);

            if (updObjs.Any())
            {
                var dirtRate = updObjs.FirstOrDefault(f => f.GroundDensityId == 0);
                var rockRate = updObjs.FirstOrDefault(f => f.GroundDensityId == bore.GroundDensityId);

                if (dirtRate.PhaseId == bidParms.DeMobePhaseId || dirtRate.PhaseId == bidParms.MobePhaseId)
                    RockProductionValue = DirtProductionValue;

                /****Write the changes to dirt****/
                if (dirtRate.ProductionCalType != CalcType)
                {
                    DirtProductionValue = dirtRate.ProductionCalType switch
                    {
                        DB.BidProductionCalEnum.Rate => dirtRate.ProductionRate,
                        DB.BidProductionCalEnum.Days => dirtRate.ProductionDays,
                        _ => 0,
                    };
                }
                dirtRate.BoreSize = BoreSize;
                dirtRate.ProductionCalType = CalcType;
                dirtRate.ProductionValue = DirtProductionValue;
                dirtRate.UM = "LF";

                /****Write the changes to rock****/
                if (rockRate.ProductionCalType != CalcType)
                {
                    RockProductionValue = rockRate.ProductionCalType switch
                    {
                        DB.BidProductionCalEnum.Rate => rockRate.ProductionRate,
                        DB.BidProductionCalEnum.Days => rockRate.ProductionDays,
                        _ => 0,
                    };
                }
                rockRate.BoreSize = BoreSize;
                rockRate.ProductionCalType = CalcType;
                rockRate.ProductionValue = RockProductionValue;
                rockRate.UM = "LF";

                try
                {
                    if (bore.RecalcNeeded ?? false)
                        bore.Recalculate();

                    db.BulkSaveChanges();
                    return new ProductionRateViewModel(dirtRate, rockRate, bore);
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