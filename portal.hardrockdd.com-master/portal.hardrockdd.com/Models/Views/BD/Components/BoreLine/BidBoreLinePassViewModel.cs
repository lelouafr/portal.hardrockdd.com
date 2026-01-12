//using portal.Repository.VP.BD;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Models.Views.Bid.BoreLine
//{
//    public class PassListViewModel
//    {
//        public PassListViewModel()
//        {

//        }

//        public PassListViewModel(DB.Infrastructure.ViewPointDB.Data.BidBoreLine bore)
//        {
//            if (bore == null)
//            {
//                throw new System.ArgumentNullException(nameof(bore));
//            }
//            #region mapping
//            Co = bore.Co;
//            BidId = bore.BidId;
//            BoreId = bore.BoreId;
//            #endregion

//            List = new List<PassViewModel>();//bore.Passes.Select(s => new BidBoreLinePassViewModel(s)).ToList();
//            foreach (var dirtPass in bore.Passes.Where(w => w.GroundDensityId == 0 && w.Deleted != true).ToList())
//            {
//                var rockPass = dirtPass;
//                rockPass.GroundDensityId = bore.GroundDensityId ?? 1;
//                using (var repo = new BidBoreLinePassRepository())
//                {
//                    rockPass = repo.FindCreate(rockPass);
//                }
//                List.Add(new PassViewModel(bore, dirtPass, rockPass));
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
//        [Display(Name = "#")]
//        public int BoreId { get; set; }

//        public List<PassViewModel> List { get; }
//    }

//    public class PassViewModel
//    {
//        public PassViewModel()
//        {

//        }

//        public PassViewModel(DB.Infrastructure.ViewPointDB.Data.BidBoreLine bore, DB.Infrastructure.ViewPointDB.Data.BidBoreLinePass dirtPass, DB.Infrastructure.ViewPointDB.Data.BidBoreLinePass rockPass)
//        {
//            if (dirtPass == null)
//            {
//                throw new System.ArgumentNullException(nameof(dirtPass));
//            }
//            if (rockPass == null)
//            {
//                throw new System.ArgumentNullException(nameof(rockPass));
//            }
//            if (bore == null)
//            {
//                throw new System.ArgumentNullException(nameof(bore));
//            }
//            #region mapping
//            Co = dirtPass.Co;
//            BidId = dirtPass.BidId;
//            BoreId = dirtPass.BoreId;
//            PhaseId = dirtPass.PhaseId;
//            PassId = dirtPass.PassId;
//            GroundDensityId = dirtPass.GroundDensityId;
//            BoreSize = dirtPass.BoreSize;
//            CalcType = (DB.BidProductionCalEnum)(dirtPass.ProductionCalTypeId ?? 0);
//            if (dirtPass.ProductionCalTypeId == (int)DB.BidProductionCalEnum.Rate || rockPass.ProductionCalTypeId == (int)DB.BidProductionCalEnum.Rate)
//            {
//                //DirtRate = dirtPass.ProductionRate;
//                //RockRate = rockPass.ProductionRate;
//                DirtValue = dirtPass.ProductionRate;
//                RockValue = rockPass.ProductionRate;
//                //if (dirtPass.ProductionRate != null && dirtPass.ProductionRate != 0 && bore.Footage != null && bore.Footage != 0)
//                //{
//                //    DirtProductionDays = bore.Footage / dirtPass.ProductionRate ?? 1;
//                //}

//                //if (rockPass.ProductionRate != null && rockPass.ProductionRate != 0 && bore.Footage != null && bore.Footage != 0)
//                //{
//                //    RockProductionDays = bore.Footage / rockPass.ProductionRate ?? 1;
//                //}
//            }
//            else
//            {
//                //DirtProductionDays = dirtPass.ProductionDays ?? 0;
//                //RockProductionDays = rockPass.ProductionDays ?? 0;

//                DirtValue = dirtPass.ProductionDays ?? 0;
//                RockValue = rockPass.ProductionDays ?? 0;
//                //if (dirtPass.ProductionDays != null && dirtPass.ProductionDays != 0 && bore.Footage != null && bore.Footage != 0)
//                //{
//                //    DirtRate = bore.Footage / DirtProductionDays;
//                //}

//                //if (rockPass.ProductionDays != null && rockPass.ProductionDays != 0 && bore.Footage != null && bore.Footage != 0)
//                //{
//                //    RockRate = bore.Footage / RockProductionDays;
//                //}
//            }

//            #endregion
//        }
//        public bool Validate(ModelStateDictionary modelState)
//        {
//            if (modelState == null) throw new System.ArgumentNullException(nameof(modelState));
//            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
//            if ((BoreSize == null || BoreSize == 0) && ((DirtValue ?? 0) > 0 || (RockValue ?? 0) > 0))
//            {
//                if (db.ProjectBudgetCodes.Any(f => f.PMCo == Co && f.PhaseId == PhaseId && f.Radius != null))
//                {
//                    var bore = db.BidBoreLines.FirstOrDefault(f => f.Co == Co && f.BidId == BidId && f.BoreId == BoreId);

//                    modelState.AddModelError("", string.Format(AppCultureInfo.CInfo(), "Bore Size is Missing in Package {0}: {1} Bore: {2}", bore.Package.PackageId, bore.Package.Description, bore.Description));
//                    modelState.AddModelError("BoreSize", "Bore Size is Missing");
//                }

//            }

//            return modelState.IsValid;
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
//        public int BoreId { get; set; }

//        [Key]
//        [Required]
//        [Display(Name = "Phase Id")]
//        [UIHint("DropdownBox")]
//        [Field(Placeholder = "Select Phase", ComboUrl = "/PhaseMaster/Combo", ComboForeignKeys = "Co")]
//        public string PhaseId { get; set; }

//        [Key]
//        [Required]
//        [Display(Name = "Pass Id")]
//        [UIHint("LongBox")]
//        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
//        public int PassId { get; set; }

//        [Key]
//        [Required]
//        [Display(Name = "Ground Density Id")]
//        [Field(Placeholder = "Select Density", ComboUrl = "/GroundDensity/Combo", ComboForeignKeys = "Co")]
//        [UIHint("DropdownBox")]
//        public int GroundDensityId { get; set; }

//        [Display(Name = "Bore Size")]
//        [UIHint("DropdownBox")]
//        [Field(ComboUrl = "/BudgetCode/PassSizeCombo", ComboForeignKeys = "Co,PhaseId")]
//        public decimal? BoreSize { get; set; }

//        [Required]
//        [UIHint("EnumBox")]
//        [Display(Name = "Calc Type")]
//        public DB.BidProductionCalEnum CalcType { get; set; }

//        //[Display(Name = "Dirt Rate")]
//        //[UIHint("IntegerBox")]
//        //[TableField(Width = "15")]
//        //[DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
//        //public decimal? DirtRate { get; set; }

//        //[Display(Name = "Rock Rate")]
//        //[UIHint("IntegerBox")]
//        //[TableField(Width = "15")]
//        //[DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
//        //public decimal? RockRate { get; set; }

//        //[Display(Name = "Dirt Days")]
//        //[UIHint("IntegerBox")]
//        //[DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
//        //public decimal DirtProductionDays { get; set; }

//        //[Display(Name = "Rock Days")]
//        //[UIHint("IntegerBox")]
//        //[DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
//        //public decimal RockProductionDays { get; set; }

//        [Display(Name = "Dirt")]
//        [UIHint("IntegerBox")]
//        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
//        public decimal? DirtValue { get; set; }

//        [Display(Name = "Rock")]
//        [UIHint("IntegerBox")]
//        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
//        public decimal? RockValue { get; set; }

//    }
//}