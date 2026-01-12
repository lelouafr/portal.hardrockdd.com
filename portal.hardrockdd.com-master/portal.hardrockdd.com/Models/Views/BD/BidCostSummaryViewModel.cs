using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.Bid
{
    public class BidCostDetailListViewModel
    {
        public BidCostDetailListViewModel()
        {

        }

        public BidCostDetailListViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackage package)
        {
            if (package == null)
            {
                throw new System.ArgumentNullException(nameof(package));
            }
            #region mapping
            BDCo = package.BDCo;
            BidId = package.BidId;
            PackageId = package.PackageId;
            Description = package.Description;
            #endregion

            List = new List<BoreLineCostDetailViewModel>();

            foreach (var bore in package.ActiveBoreLines)
            {
                foreach (var cost in bore.CostItems)
                {
                    foreach (var item in cost.ItemPhases)
                    {
                        for (int i = 1; i <= (item.Shift ?? 1); i++)
                        {
                            List.Add(new BoreLineCostDetailViewModel(bore, item, i));
                        }
                    }
                    if (!cost.ItemPhases.Any())
                    {
                        List.Add(new BoreLineCostDetailViewModel(bore, cost, 1));
                    }
                }
            }
            foreach (var subPackage in package.SubPackages)
            {
                foreach (var bore in subPackage.ActiveBoreLines)
                {
                    foreach (var cost in bore.CostItems)
                    {
                        foreach (var item in cost.ItemPhases)
                        {
                            for (int i = 1; i <= (item.Shift ?? 1); i++)
                            {
                                List.Add(new BoreLineCostDetailViewModel(bore, item, i));
                            }
                        }
                        if (!cost.ItemPhases.Any())
                        {
                            List.Add(new BoreLineCostDetailViewModel(bore, cost, 1));
                        }
                    }
                }
            }
            //foreach (var bore in package.BoreLines)
            //{
            //    foreach (var cost in bore.CostItems)
            //    {

            //        if (cost.ItemPhases.Count == 0)
            //        {
            //            List.Add(new BoreLineCostDetailViewModel(bore, cost));
            //        }
            //        else
            //        {
            //            foreach (var item in cost.ItemPhases)
            //            {
            //                List.Add(new BoreLineCostDetailViewModel(bore, item));
            //            }
            //        }
            //    }
            //}
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

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public List<BoreLineCostDetailViewModel> List { get; }
    }
    
    public class BoreLineCostDetailListViewModel
    {
        public BoreLineCostDetailListViewModel()
        {

        }

        public BoreLineCostDetailListViewModel(DB.Infrastructure.ViewPointDB.Data.BidBoreLine bore)
        {
            if (bore == null)
            {
                throw new System.ArgumentNullException(nameof(bore));
            }
            #region mapping
            BDCo = bore.BDCo;
            BidId = bore.BidId;
            BoreId = bore.BoreId;

            #endregion

            List = new List<BoreLineCostDetailViewModel>();
            foreach (var cost in bore.CostItems)
            {
                foreach (var item in cost.ItemPhases)
                {
                    for (int i = 1; i <= (item.Shift ?? 1); i++)
                    {
                        List.Add(new BoreLineCostDetailViewModel(bore, item, i));
                    }
                }
                if (!cost.ItemPhases.Any())
                {
                    List.Add(new BoreLineCostDetailViewModel(bore, cost, 1));
                }
            }


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
        [Display(Name = "Bore Id")]
        public int BoreId { get; set; }

        public List<BoreLineCostDetailViewModel> List { get; }
    }

    public class BoreLineCostDetailViewModel
    {
        public BoreLineCostDetailViewModel()
        {

        }

        public BoreLineCostDetailViewModel(DB.Infrastructure.ViewPointDB.Data.BidBoreLine line, DB.Infrastructure.ViewPointDB.Data.BidBoreLineCostItem item, int shiftNum)
        {
            if (line == null)
            {
                throw new System.ArgumentNullException(nameof(line));
            }
            if (item == null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }
            #region mapping
            BDCo = line.BDCo;
            BidId = line.BidId;
            BoreId = line.BoreId;
            Description = line.Description;
            Footage = line.Footage;
            PipeSize = line.PipeSize;
            GroundDensityType = item.GroundDensityId == 0 ? "Dirt" : "Rock";
            GroundDensityDescription = item.GroundDensity.Description;
            BudgetCategory = item.BudgetCode.CostType?.Description;
            BudgetCodeDescription = item.BudgetCode.Description;
            if (shiftNum > 1)
            {
                BudgetCodeDescription = item.BudgetCode.Description + string.Format(AppCultureInfo.CInfo(), " Shift# {0}", shiftNum);
            }
            BudgetCodeId = item.BudgetCodeId;
            PhaseId = item.BudgetCode.PhaseId;
            PhaseDescription = item.BudgetCode.Phase?.Description;
            PassId = 1;
            Units = item.Units;
            Multiplier = item.Multiplier ?? 1;
            ExtUnits = (item.Units ?? 0) * (item.Multiplier ?? 1);
            Cost = item.Cost;
            ExtCost = Math.Round((Units ?? 0) * (Cost ?? 0) * (item.Multiplier ?? 1),0);
            if (item.ItemPhases.Count > 0)
            {
                ExtCost = 0;
                ExtUnits = 0;
                foreach (var phase in item.ItemPhases)
                {
                    //var cost = (phase.Cost ?? 0) * (phase.Shift ?? 1);
                    //Cost = (phase.Cost ?? 0);
                    ExtUnits += (phase.Units ?? 0) * (item.Multiplier ?? 1);
                    ExtCost += (phase.Units ?? 0) * (phase.Cost ?? 0) * (item.Multiplier ?? 1);
                }
            }
            UM = item.BudgetCode.UM;

            IntersectBoreId = line.IntersectBoreId;

            #endregion mapping

        }

        public BoreLineCostDetailViewModel(DB.Infrastructure.ViewPointDB.Data.BidBoreLine line, DB.Infrastructure.ViewPointDB.Data.BidBoreLineCostItemPhase item, int shiftNum)
        {
            if (line == null)
            {
                throw new System.ArgumentNullException(nameof(line));
            }
            if (item == null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }
            #region mapping
            BDCo = line.BDCo;
            BidId = line.BidId;
            BoreId = line.BoreId;
            Description = line.Description;
            Footage = line.Footage;
            PipeSize = line.PipeSize;
            GroundDensityType = item.GroundDensityId == 0 ? "Dirt" : "Rock";
            GroundDensityDescription = item.GroundDensity.Description;

            BudgetCategory = item.CostItem.BudgetCode.CostType.Description;
            BudgetCodeDescription = item.CostItem.BudgetCode.Description;

            if (shiftNum > 1)
            {
                BudgetCodeDescription = item.CostItem.BudgetCode.Description + string.Format(AppCultureInfo.CInfo(), " Shift# {0}", shiftNum);
            }
            BudgetCodeId = item.CostItem.BudgetCodeId;
            PhaseId = item.PhaseId;
            PhaseDescription = item.PhaseMaster.Description;
            PassId = item.PassId;
            Units = item.Units;
            Multiplier = item.CostItem.Multiplier ?? 1;
            ExtUnits = (item.Units ?? 0) * (item.CostItem.Multiplier ?? 1);
            Cost = item.CostItem.Cost ;
            ExtCost = Math.Round((Units ?? 0) * (Cost ?? 0) * (item.CostItem.Multiplier ?? 1),0);
            //if (item.CostItem.ItemPhases.Count > 0)
            //{
            //    ExtCost = 0;
            //    foreach (var phase in item.CostItem.ItemPhases)
            //    {
            //        ExtCost += (phase.Units ?? 0) * (phase.Cost ?? 0)  * (item.CostItem.Multiplier ?? 1);
            //    }
            //}
            UM = item.CostItem.BudgetCode.UM;


            IntersectBoreId = line.IntersectBoreId;
            #endregion mapping

        }

        [Key]
        [Required]
        [HiddenInput]
        public byte BDCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        public int BidId { get; set; }

        [Key]
        [Required]
        [Display(Name = "#")]
        [TableField(Width = "5")]
        [UIHint("LongBox")]
        public int BoreId { get; set; }

        [Display(Name = "#")]
        [TableField(Width = "5")]
        [UIHint("LongBox")]
        public int? IntersectBoreId { get; set; }

        [Required]
        [Display(Name = "Crossing")]
        [UIHint("TextBox")]
        [TableField(Width = "20")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Length")]
        [UIHint("IntegerBox")]
        [TableField(Width = "15")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal? Footage { get; set; }

        [Required]
        [Display(Name = "Pipe Size")]
        [UIHint("IntegerBox")]
        [TableField(Width = "15")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? PipeSize { get; set; }

        [Required]
        [Display(Name = "Ground Type")]
        [UIHint("TextBox")]
        public string GroundDensityType { get; set; }

        [Required]
        [Display(Name = "Rock Density")]
        [UIHint("TextBox")]
        public string GroundDensityDescription { get; set; }

        [Display(Name = "Budget Category")]
        [UIHint("TextBox")]
        public string BudgetCategory { get; set; }

        [Display(Name = "Budget Code Id")]
        [UIHint("TextBox")]
        public string BudgetCodeId { get; set; }

        [Display(Name = "Budget Description")]
        [UIHint("TextBox")]
        public string BudgetCodeDescription { get; set; }

        [Display(Name = "Phase Id")]
        [UIHint("TextBox")]
        public string PhaseId { get; set; }

        [Display(Name = "Phase Description")]
        [UIHint("TextBox")]
        public string PhaseDescription { get; set; }

        [Display(Name = "Pass Id")]
        [UIHint("LongBox")]
        public int PassId { get; set; }

        [Display(Name = "Units")]
        [UIHint("IntegerBox")]
        public decimal? Units { get; set; }

        [Display(Name = "Ext Units")]
        [UIHint("IntegerBox")]
        public decimal? ExtUnits { get; set; }

        [Display(Name = "Multiplier")]
        [UIHint("IntegerBox")]
        public decimal? Multiplier { get; set; }

        [Display(Name = "UM")]
        [UIHint("TextBox")]
        public string UM { get; set; }
        
        [Display(Name = "Cost")]
        [UIHint("IntegerBox")]
        public decimal? Cost { get; set; }

        [Display(Name = "Ext Cost")]
        [UIHint("IntegerBox")]
        public decimal ExtCost { get; set; }


        [Required]
        [Display(Name = "Intersect Footage")]
        [UIHint("IntegerBox")]
        [TableField(Width = "15")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal? IntersectFootage { get; set; }


        [Required]
        [Display(Name = "Intersect Footage2")]
        [UIHint("IntegerBox")]
        [TableField(Width = "15")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal? IntersectFootage2 { get; set; }
    }
}