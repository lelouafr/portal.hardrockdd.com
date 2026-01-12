//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Repository.VP.BD;
//using portal.Repository.VP.PM;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Models.Views.Bid
//{
//    public class BoreLineListViewModel
//    {
//        public BoreLineListViewModel()
//        {

//        }

//        public BoreLineListViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackage package)
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

//            List = package.ActiveBoreLines.Select(s => new BoreLineViewModel(s)).ToList();

//            if (package.Bid.Status == (int)DB.BidStatusEnum.Proposal)
//            {
//                foreach (var subPackage in package.SubPackages)
//                {
//                    //var subCostDetails = new BidCostDetailListViewModel(subPackage);
//                    var subList = subPackage.ActiveBoreLines
//                                                       .Select(s => new BoreLineViewModel(s))
//                                                       .ToList();
//                    List.AddRange(subList);
//                }
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

//        public List<BoreLineViewModel> List { get; }
//    }

//    public class BoreLineViewModel
//    {
//        public BoreLineViewModel()
//        {

//        }

//        public BoreLineViewModel(DB.Infrastructure.ViewPointDB.Data.BidBoreLine line)
//        {
//            if (line == null)
//            {
//                throw new System.ArgumentNullException(nameof(line));
//            }
//            #region mapping
//            Co = line.Co;
//            BidId = line.BidId;
//            BoreId = line.BoreId;
//            PackageId = (int)line.PackageId;
//            BoreTypeId = line.BoreTypeId;
//            Description = line.Description;
//            Footage = line.Footage;
//            CurFootage = line.CurFootage;
//            PipeSize = line.PipeSize;
//            GroundDensityId = line.GroundDensityId;
//            GroundDensityDesc = line.GroundDensity?.Description;
//            RigCategoryId = line.RigCategoryId;
//            RigCategoryDesc = line.EMCategory?.Description;
//            CrewCount = line.CrewCount;
//            DirtDays = 0;
//            RockDays = 0;
//            IntersectBoreId = line.IntersectBoreId;

//            EMCo = line.Bid.Company.EMCo;
//            #endregion
//            //using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
//            //using var bgtRepo = new ProjectBudgetCodeRepository();
//            //using var costRepo = new BidBoreLineCostItemRepository();

//            //var bidParms = db.BDCompanyParms.FirstOrDefault(f => f.Co == line.Co);
//            //var bgt = bgtRepo.FindCreate(line.Co, "BC", "Bore Mobilization");
//            //if (bgt.PhaseId != bidParms.MobePhaseId)
//            //{
//            //    bgt.PhaseId = bidParms.MobePhaseId;
//            //    bgt.PhaseGroup = bidParms.Co;
//            //    bgt.UM = "DYS";
//            //    bgtRepo.ProcessUpdate(bgt, null);
//            //}

//            //var pass = line.CostItems.FirstOrDefault(f => f.BudgetCodeId == bgt.BudgetCodeId);
//            //if (pass == null)
//            //{
//            //    pass = BidBoreLineCostItemRepository.CreateCostItems(line, bgt.BudgetCodeId, 1);
//            //}
//            //MobeDays = pass.Units;

//            //bgt = bgtRepo.FindCreate(line.Co, "BC", "Bore De-Mobilization");
//            //if (bgt.PhaseId != bidParms.DeMobePhaseId)
//            //{
//            //    bgt.PhaseId = bidParms.DeMobePhaseId;
//            //    bgt.PhaseGroup = bidParms.Co;
//            //    bgt.UM = "DYS";
//            //    bgtRepo.ProcessUpdate(bgt, null);
//            //}

//            //pass = line.CostItems.FirstOrDefault(f => f.BudgetCodeId == bgt.BudgetCodeId);
//            //if (pass == null)
//            //{
//            //    pass = BidBoreLineCostItemRepository.CreateCostItems(line, bgt.BudgetCodeId, 1);
//            //}
//            //DeMobeDays = pass.Units;


//            //var costDetails = new BoreLineCostDetailListViewModel(line);

//            var boreRates = line.Passes.Where(w => w.Deleted != true).GroupBy(g => new { g.GroundDensityId })
//                                            .Select(s => new
//                                            {
//                                                GroundType = s.Key.GroundDensityId == 0 ? "Dirt" : "Rock",
//                                                List = s.Where(w => (w.ProductionRate ?? 0) != 0 || (w.ProductionDays ?? 0) != 0

//                                                ).ToList()
//                                            }).ToList();
//            DirtDays = 0;
//            RockDays = 0;
//            foreach (var gndType in boreRates)
//            {
//                foreach (var rate in gndType.List)
//                {
//                    var production = rate.ProductionRate;
//                    for (int i = 1; i < rate.ShiftCnt; i++)
//                    {
//                        production += production * (rate.Shift2ProductionPerc ?? 0);
//                    }
//                    if (gndType.GroundType == "Dirt")
//                    {
//                        DirtDays += rate.ProductionCalTypeId == (int)DB.BidProductionCalEnum.Rate? line.Footage / production : rate.ProductionDays;
//                    }
//                    if (gndType.GroundType == "Rock")
//                    {
//                        RockDays += rate.ProductionCalTypeId == (int)DB.BidProductionCalEnum.Rate ? line.Footage / production : rate.ProductionDays;
//                    }
//                }
//            }
//            var costSummary = line.CostItems.GroupBy(g => new { g.GroundDensityId })
//                                            .Select(s => new
//                                            {
//                                                GroundType = s.Key.GroundDensityId == 0 ? "Dirt" : "Rock",
//                                                TotalCost = s.Sum(sum => Math.Round((sum.Units ?? 0) * (sum.Cost ?? 0) * (sum.Multiplier ?? 1), 0))
//                                            })
//                                            .ToList();
//            if ((line.Footage ?? 0) != 0)
//            {
//                if (costSummary.Any(f => f.GroundType == "Dirt"))
//                {
//                    DirtCost = Math.Round((decimal)(costSummary.Where(f => f.GroundType == "Dirt").FirstOrDefault()?.TotalCost / line.Footage), 2);
//                }
//                if (costSummary.Any(f => f.GroundType == "Rock"))
//                {
//                    RockCost = Math.Round((decimal)(costSummary.Where(f => f.GroundType == "Rock").FirstOrDefault()?.TotalCost / line.Footage), 2);
//                }
//            }

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

//        [Required]
//        [Display(Name = "PackageId")]
//        [UIHint("LongBox")]
//        public int PackageId { get; set; }

//        [Required]
//        [Display(Name = "Crossing")]
//        [UIHint("TextBox")]
//        [Field(LabelSize =2, TextSize =10)]
//        [TableField(Width = "20")]
//        public string Description { get; set; }

//        [Required]
//        [Display(Name = "Bore Type")]
//        [Field(ComboUrl = "/BDCombo/BoreTypeCombo", ComboForeignKeys = "BDCo")]
//        [UIHint("DropdownBox")]
//        public int? BoreTypeId { get; set; }

//        [Required]
//        [Display(Name = "Length")]
//        [UIHint("IntegerBox")]
//        [TableField(Width = "15")]
//        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
//        public decimal? Footage { get; set; }

//        //[Required]
//        [Display(Name = "Current Footage")]
//        [UIHint("IntegerBox")]
//        [TableField(Width = "15")]
//        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
//        public decimal? CurFootage { get; set; }

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

//        [Display(Name = "Rock Density")]
//        [UIHint("TextBox")]
//        public string GroundDensityDesc { get; set; }

//        public byte? EMCo { get; set; }

//        [Required]
//        [Display(Name = "Rig Type")]
//        [UIHint("DropdownBox")]
//        [TableField(Width = "15")]
//        [Field(Placeholder = "Select Size", ComboUrl = "/EMCombo/RigCatCombo", ComboForeignKeys = "EMCo")]
//        public string RigCategoryId { get; set; }

//        [Display(Name = "Rig Type")]
//        [UIHint("TextBox")]
//        public string RigCategoryDesc { get; set; }

//        [Required]
//        [Display(Name = "Crew Size")]
//        [UIHint("LongBox")]
//        [TableField(Width = "15")]
//        public int? CrewCount { get; set; }


//        [Display(Name = "Dirt Cost")]
//        [UIHint("IntegerBox")]
//        [TableField(Width = "15")]
//        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
//        public decimal? DirtCost { get; set; }

//        [Display(Name = "Rock Cost")]
//        [UIHint("IntegerBox")]
//        [TableField(Width = "15")]
//        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
//        public decimal? RockCost { get; set; }

//        [Display(Name = "Dirt Days")]
//        [UIHint("IntegerBox")]
//        [TableField(Width = "15")]
//        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
//        public decimal? DirtDays { get; set; }

//        [Display(Name = "Rock Days")]
//        [UIHint("IntegerBox")]
//        [TableField(Width = "15")]
//        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
//        public decimal? RockDays { get; set; }

//        //[Display(Name = "Mobe Days")]
//        //[UIHint("IntegerBox")]
//        //[TableField(Width = "15")]
//        //[DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
//        //public decimal? MobeDays { get; set; }

//        //[Display(Name = "DeMobe Days")]
//        //[UIHint("IntegerBox")]
//        //[TableField(Width = "15")]
//        //[DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
//        //public decimal? DeMobeDays { get; set; }

//        [Display(Name = "#")]
//        [TableField(Width = "5")]
//        [UIHint("LongBox")]
//        public int? IntersectBoreId { get; set; }

//    }
//}