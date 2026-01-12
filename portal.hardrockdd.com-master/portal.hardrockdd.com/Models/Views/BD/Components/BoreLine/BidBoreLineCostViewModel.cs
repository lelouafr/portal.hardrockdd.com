//using portal.Repository.VP.PM;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Models.Views.Bid.BoreLine
//{
//    public class CostItemListViewModel
//    {
//        public CostItemListViewModel()
//        {

//        }

//        public CostItemListViewModel(DB.Infrastructure.ViewPointDB.Data.BidBoreLine bore)
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

//            List = new List<CostItemViewModel>();
//            var groundDensityId = bore.GroundDensityId ?? 1;
//            var bgtList = bore.CostItems.Where(w => (w.GroundDensityId == 0 || w.GroundDensityId == groundDensityId))
//                                        .OrderBy(f => f.BudgetCodeId)
//                                        .GroupBy(f => new { f.BudgetCodeId, f.IsPackageCost } )
//                                        .Select(s => new { s.Key.BudgetCodeId, s.Key.IsPackageCost, List = s.Select(c => c) })
//                                        .ToList();
//            foreach (var cost in bgtList)
//            {
//                DB.Infrastructure.ViewPointDB.Data.BidBoreLineCostItem dirtCost = null;
//                DB.Infrastructure.ViewPointDB.Data.BidBoreLineCostItem rockCost = null;
//                foreach (var item in cost.List)
//                {
//                    if (item.GroundDensityId == 0)
//                    {
//                        dirtCost = item;
//                    }
//                    if (item.GroundDensityId != 0)
//                    {
//                        rockCost = item;
//                    }
//                }
//                List.Add(new CostItemViewModel(dirtCost, rockCost));
//            }

//            //foreach (var dirtUnit in bore.CostItems.Where(w => w.GroundDensityId == 0 && (w.BudgetCodeId.StartsWith("RE-") || w.BudgetCodeId.StartsWith("CI-"))).ToList().ToList())
//            //{
//            //    var rockUnit = dirtUnit;
//            //    rockUnit.GroundDensityId = bore.GroundDensityId ?? 1;
//            //    using (var repo = new BidBoreLineCostItemRepository())
//            //    {
//            //        rockUnit = repo.FindCreate(BidBoreLineCostItemRepository.EntityToModel(rockUnit, "")).ToEntity();
//            //    }
//            //    List.Add(new CostItemViewModel(dirtUnit, rockUnit));
//            //}
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

//        public List<CostItemViewModel> List { get; }
//    }

//    public class CostItemViewModel
//    {
//        public CostItemViewModel()
//        {

//        }

//        public CostItemViewModel(DB.Infrastructure.ViewPointDB.Data.BidBoreLineCostItem dirtUnit, DB.Infrastructure.ViewPointDB.Data.BidBoreLineCostItem rockUnit)
//        {
//            DB.Infrastructure.ViewPointDB.Data.BidBoreLineCostItem unit = dirtUnit ?? rockUnit;
            
//            if (unit == null)
//            {
//                throw new System.ArgumentNullException(nameof(dirtUnit));
//            }
//            #region mapping
//            Co = unit.Co;
//            BidId = unit.BidId;
//            BoreId = unit.BoreId;
//            LineId = unit.LineId;
//            BudgetCodeId = unit.BudgetCodeId;
//            Multiplier = unit.Multiplier ?? 1;
//            Cost = unit.Cost;
//            StdCost = unit.BudgetCode?.UnitCost ?? Cost;
//            UM = unit.BudgetCode?.UM ;
//            IsPackageCost = unit.IsPackageCost ?? false;

//            DirtUnits = dirtUnit?.Units;
//            RockUnits = rockUnit?.Units;
//            DirtCost = DirtUnits * Multiplier * Cost;
//            RockCost = RockUnits * Multiplier * Cost;
//            if (dirtUnit != null)
//            {
//                DirtCost = 0;
//                if (dirtUnit.ItemPhases.Count == 0)
//                {
//                    DirtCost = DirtUnits * Multiplier * Cost;
//                }
//                else
//                {
//                    foreach (var phase in dirtUnit.ItemPhases)
//                    {
//                        Shift = (phase.Shift ?? 1);
//                        var cost = unit.Cost * (phase.Shift ?? 1);
//                        StdCost = cost;
//                        DirtCost += phase.Units * cost * Multiplier; //* phase.Multiplier
//                    }
//                }
//            }
//            if (rockUnit != null)
//            {
//                RockCost = 0;
//                if (rockUnit.ItemPhases.Count == 0)
//                {
//                    RockCost = RockUnits * Multiplier * Cost;
//                }
//                else
//                {
//                    foreach (var phase in rockUnit.ItemPhases)
//                    {
//                        Shift = (phase.Shift ?? 1);
//                        var cost = unit.Cost * (phase.Shift ?? 1);
//                        StdCost = cost;
//                        RockCost += phase.Units * cost * Multiplier;
//                    }
//                }

//            }



//            CostType = unit.BudgetCode?.CostType?.Description;
//            #endregion
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
//        [Display(Name = "Line Id")]
//        public int LineId { get; set; }

//        [Display(Name = "Budget Code Id")]
//        [UIHint("DropDownBox")]
//        [Field(Placeholder = "Select Item", ComboUrl = "/BudgetCode/Combo", ComboForeignKeys = "Co")]
//        public string BudgetCodeId { get; set; }

//        [Display(Name = "Multiplier")]
//        [UIHint("IntegerBox")]
//        public int? Multiplier { get; set; }

//        [Display(Name = "Shift")]
//        [UIHint("IntegerBox")]
//        public decimal? Shift { get; set; }

//        [Display(Name = "Cost")]
//        [UIHint("CurrencyBox")]
//        public decimal? Cost { get; set; }

//        [Display(Name = "StdCost")]
//        [UIHint("CurrencyBox")]
//        public decimal? StdCost { get; set; }

//        [Display(Name = "Dirt Ext")]
//        [UIHint("CurrencyBox")]
//        public decimal? DirtCost { get; set; }

//        [Display(Name = "Rock Ext")]
//        [UIHint("CurrencyBox")]
//        public decimal? RockCost { get; set; }

//        [Display(Name = "Dirt Units")]
//        [UIHint("IntegerBox")]
//        public decimal? DirtUnits { get; set; }

//        [Display(Name = "Rock Units")]
//        [UIHint("IntegerBox")]
//        public decimal? RockUnits { get; set; }

//        [Display(Name = "UM")]
//        [UIHint("TextBox")]
//        public string UM { get; set; }

//        [Display(Name = "Dirt Days")]
//        [UIHint("IntegerBox")]
//        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
//        public decimal DirtProductionDays { get; set; }

//        [Display(Name = "Rock Days")]
//        [UIHint("IntegerBox")]
//        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
//        public decimal RockProductionDays { get; set; }


//        [Display(Name = "Cost Type")]
//        [UIHint("TextBox")]
//        public string CostType { get; set; }


//        [Display(Name = "Package Cost")]
//        [UIHint("TextBox")]
//        public bool IsPackageCost { get; set; }

//    }
//}