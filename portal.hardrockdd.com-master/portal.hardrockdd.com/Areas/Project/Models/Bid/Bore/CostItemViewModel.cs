using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Project.Models.Bid.Bore
{
    public class CostItemListViewModel
    {
        public CostItemListViewModel()
        {

        }

        public CostItemListViewModel(DB.Infrastructure.ViewPointDB.Data.BidBoreLine bore)
        {
            if (bore == null)
                return;

            #region mapping
            BDCo = bore.BDCo;
            BidId = bore.BidId;
            BoreId = bore.BoreId;
            #endregion

            List = new List<CostItemViewModel>();
            var groundDensityId = bore.GroundDensityId ?? 1;
            var bgtList = bore.CostItems.Where(w => (w.GroundDensityId == 0 || w.GroundDensityId == groundDensityId))
                                        .OrderBy(f => f.BudgetCodeId)
                                        .GroupBy(f => new { f.BudgetCodeId, f.IsPackageCost })
                                        .Select(s => new { s.Key.BudgetCodeId, s.Key.IsPackageCost, List = s.Select(c => c) })
                                        .ToList();
            foreach (var cost in bgtList)
            {
                DB.Infrastructure.ViewPointDB.Data.BidBoreLineCostItem dirtCost = null;
                DB.Infrastructure.ViewPointDB.Data.BidBoreLineCostItem rockCost = null;
                foreach (var item in cost.List)
                {
                    if (item.GroundDensityId == 0)
                    {
                        dirtCost = item;
                    }
                    if (item.GroundDensityId == groundDensityId)
                    {
                        rockCost = item;
                    }
                }
                List.Add(new CostItemViewModel(dirtCost, rockCost));
            }
        }

        [Key]
        // [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte BDCo { get; set; }

        [Key]
        // [HiddenInput]
        [Required]
        [Display(Name = "Bid Id")]
        public int BidId { get; set; }

        [Key]
        // [HiddenInput]
        [Required]
        [Display(Name = "#")]
        public int BoreId { get; set; }

        public List<CostItemViewModel> List { get; }
    }

    public class CostItemViewModel
    {
        public CostItemViewModel()
        {

        }

        public CostItemViewModel(DB.Infrastructure.ViewPointDB.Data.BidBoreLineCostItem dirtUnit, DB.Infrastructure.ViewPointDB.Data.BidBoreLineCostItem rockUnit)
        {
            DB.Infrastructure.ViewPointDB.Data.BidBoreLineCostItem unit = dirtUnit ?? rockUnit;

            if (unit == null)
            {
                throw new System.ArgumentNullException(nameof(dirtUnit));
            }
            if (dirtUnit != null)
                DirtLineId = dirtUnit.LineId;

            //DirtLineId = dirtUnit?.LineId ?? 0;
            if (rockUnit != null)
                RockLineId = rockUnit?.LineId ?? 0;


            #region mapping
            BDCo = unit.BDCo;
            BidId = unit.BidId;
            BoreId = unit.BoreId;
            RockLineId = unit.LineId;
            GroundDensityId = unit.GroundDensityId;
            PMCo = unit.BDCo;
            BudgetCodeId = unit.BudgetCodeId;
            Multiplier = unit.Multiplier ?? 1;
            Cost = unit.Cost;
            StdCost = unit.BudgetCode?.UnitCost ?? Cost;
            UM = unit.BudgetCode?.UM;
            IsPackageCost = unit.IsPackageCost ?? false;

            DirtUnits = dirtUnit?.Units;
            RockUnits = rockUnit?.Units;
            DirtCost = DirtUnits * Multiplier * Cost;
            RockCost = RockUnits * Multiplier * Cost;
            if (dirtUnit != null)
            {
                DirtCost = 0;
                if (dirtUnit.ItemPhases.Count == 0)
                {
                    DirtCost = DirtUnits * Multiplier * Cost;
                }
                else
                {
                    foreach (var phase in dirtUnit.ItemPhases)
                    {
                        Shift = (phase.Shift ?? 1);
                        var cost = unit.Cost * (phase.Shift ?? 1);
                        StdCost = cost;
                        DirtCost += phase.Units * cost * Multiplier; 
                    }
                }
            }
            if (rockUnit != null)
            {
                RockCost = 0;
                if (rockUnit.ItemPhases.Count == 0)
                {
                    RockCost = RockUnits * Multiplier * Cost;
                }
                else
                {
                    foreach (var phase in rockUnit.ItemPhases)
                    {
                        Shift = (phase.Shift ?? 1);
                        var cost = unit.Cost * (phase.Shift ?? 1);
                        StdCost = cost;
                        RockCost += phase.Units * cost * Multiplier;
                    }
                }

            }
            CostType = unit.BudgetCode?.CostType?.Description;
            #endregion
        }
        public byte? PMCo { get; set; }

        [Key]
        [Required]
        // [HiddenInput]
        public byte BDCo { get; set; }

        [Key]
        [Required]
        // [HiddenInput]
        public int BidId { get; set; }

        [Key]
        [Required]
        [Display(Name = "#")]
        [UIHint("LongBox")]
        public int BoreId { get; set; }

        [Key]
        [Required]
        [Display(Name = "#")]
        [UIHint("LongBox")]
        public int GroundDensityId { get; set; }

        [Key]
        [Required]
        [Display(Name = "Line Id")]
        public int RockLineId { get; set; }

        [Key]
        [Required]
        [Display(Name = "Line Id")]
        public int DirtLineId { get; set; }

        [Display(Name = "Budget Code Id")]
        [UIHint("DropDownBox")]
        [Field(Placeholder = "Select Item", ComboUrl = "/BudgetCode/Combo", ComboForeignKeys = "PMCo=BDCo")]
        public string BudgetCodeId { get; set; }

        [Display(Name = "Multiplier")]
        [UIHint("IntegerBox")]
        public int? Multiplier { get; set; }

        [Display(Name = "Shift")]
        [UIHint("IntegerBox")]
        public decimal? Shift { get; set; }

        [Display(Name = "Cost")]
        [UIHint("CurrencyBox")]
        public decimal? Cost { get; set; }

        [Display(Name = "StdCost")]
        [UIHint("CurrencyBox")]
        public decimal? StdCost { get; set; }

        [Display(Name = "Dirt Ext")]
        [UIHint("CurrencyBox")]
        public decimal? DirtCost { get; set; }

        [Display(Name = "Rock Ext")]
        [UIHint("CurrencyBox")]
        public decimal? RockCost { get; set; }

        [Display(Name = "Dirt Units")]
        [UIHint("IntegerBox")]
        public decimal? DirtUnits { get; set; }

        [Display(Name = "Rock Units")]
        [UIHint("IntegerBox")]
        public decimal? RockUnits { get; set; }

        [Display(Name = "UM")]
        [UIHint("TextBox")]
        public string UM { get; set; }

        [Display(Name = "Dirt Days")]
        [UIHint("IntegerBox")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal DirtProductionDays { get; set; }

        [Display(Name = "Rock Days")]
        [UIHint("IntegerBox")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal RockProductionDays { get; set; }


        [Display(Name = "Cost Type")]
        [UIHint("TextBox")]
        public string CostType { get; set; }


        [Display(Name = "Package Cost")]
        [UIHint("TextBox")]
        public bool IsPackageCost { get; set; }

        internal CostItemViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var bore = db.BidBoreLines.FirstOrDefault(f => f.BDCo == BDCo && f.BidId == BidId && f.BoreId == BoreId);

            var updObjs = bore.CostItems.Where(f => (f.LineId == DirtLineId && f.GroundDensityId == 0) ||
                                                    (f.LineId == RockLineId && f.GroundDensityId == GroundDensityId)).ToList();

            if (updObjs.Any())
            {
                updObjs.ForEach(updObj => {
                    updObj.Multiplier = Multiplier;
                    updObj.Cost = Cost;
                });
                try
                {
                    db.BulkSaveChanges();
                    var dirtCost = updObjs.FirstOrDefault(f => f.GroundDensityId == 0);
                    var rockCost = updObjs.FirstOrDefault(f => f.GroundDensityId == GroundDensityId);
                    return new CostItemViewModel(dirtCost, rockCost);
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