using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Bid.Forms.Bore
{
    public class CostDetailListViewModel
    {
        public CostDetailListViewModel()
        {

        }

        public CostDetailListViewModel(BidBoreLine line)
        {
            if (line == null)
                return;

            #region mapping
            BDCo = line.BDCo;
            BidId = line.BidId;
            BoreId = line.BoreId;
            #endregion

            List = line.vBoreLineCostItems.Select(s => new CostDetailViewModel(s)).ToList();
            //List = new List<CostDetailViewModel>();

            //var costItems = line.CostItems.Where(f => f.GroundDensityId == 0 || f.GroundDensityId == line.GroundDensityId);
            //foreach (var cost in costItems)
            //{
            //    foreach (var item in cost.ItemPhases)
            //    {
            //        for (int i = 1; i <= (item.Shift ?? 1); i++)
            //        {
            //            List.Add(new CostDetailViewModel(line, item, i));
            //        }
            //    }
            //    if (!cost.ItemPhases.Any())
            //    {
            //        List.Add(new CostDetailViewModel(line, cost, 1));
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
        [Display(Name = "Bore Id")]
        public int BoreId { get; set; }


        public List<CostDetailViewModel> List { get; set; }

    }

    public class CostDetailViewModel
    {
        public CostDetailViewModel()
        {

        }
        public CostDetailViewModel(vBidBoreLineCostItem item)
        {
            if (item == null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }
            #region mapping
            BDCo = item.BDCo;
            BidId = item.BidId;
            BoreId = item.BoreId;
            Description = item.Description;
            Footage = item.Footage;
            PipeSize = item.PipeSize;
            GroundDensityType = item.GroundDensityId == 0 ? "Dirt" : "Rock";
            GroundDensityDescription = item.GroundDensityDescription;
            BudgetCategory = item.BudgetSubCategory;
            BudgetCodeDescription = item.BudgetCodeDescription;
            //if (shiftNum > 1)
            //{
            //    BudgetCodeDescription = item.BudgetCodeDescription + string.Format(AppCultureInfo.CInfo(), " Shift# {0}", shiftNum);
            //}
            BudgetCodeId = item.BudgetCodeId;
            PhaseId = item.PhaseId;
            PhaseDescription = item.PhaseDescription;
            PassId = 1;
            Units = item.Units;
            Multiplier = item.Multiplier;
            ExtUnits = item.Units * item.Multiplier;
            Cost = item.Cost;
            ExtCost = Math.Round(item.ExtCost ?? 0, 2);
            UM = item.UM;

            IntersectBoreId = item.BidBoreLine.IntersectBoreId;

            #endregion mapping

        }

        public CostDetailViewModel(tfudBDBI_SUMMARY_Result item)
        {
            if (item == null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }
            #region mapping
            BDCo = (byte)item.Co;
            BidId = (int)item.BidId;
            BoreId = (int)item.BoreId;
            Description = item.Description;
            Footage = item.Footage;
            PipeSize = item.PipeSize;
            GroundDensityType = item.GroundDensityId == 0 ? "Dirt" : "Rock";
            GroundDensityDescription = item.GroundDensityDescription;
            BudgetCategory = item.BudgetSubCategory;
            BudgetCodeDescription = item.BudgetCodeDescription;
            //if (shiftNum > 1)
            //{
            //    BudgetCodeDescription = item.BudgetCodeDescription + string.Format(AppCultureInfo.CInfo(), " Shift# {0}", shiftNum);
            //}
            BudgetCodeId = item.BudgetCodeId;
            PhaseId = item.PhaseId;
            PhaseDescription = item.PhaseDescription;
            PassId = 1;
            Units = item.Units;
            Multiplier = item.Multiplier;
            ExtUnits = item.Units * item.Multiplier;
            Cost = item.Cost;
            ExtCost = Math.Round(item.ExtCost ?? 0, 2);
            UM = item.UM;

            IntersectBoreId = item.IntersectBoreId;

            #endregion mapping

        }

        public CostDetailViewModel(BidBoreLine line, BidBoreLineCostItem item, int shiftNum)
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
            Units = item.Units ?? 0;
            Multiplier = item.Multiplier ?? 1;
            ExtUnits = (item.Units ?? 0) * (item.Multiplier ?? 1);
            Cost = item.Cost ?? 0;
            ExtCost = Math.Round(Units * Cost * Multiplier, 2);
            //if (item.ItemPhases.Count > 0)
            //{
            //    ExtCost = 0;
            //    ExtUnits = 0;
            //    Units = 0;
            //    foreach (var phase in item.ItemPhases)
            //    {
            //        Units += (phase.Units ?? 0);
            //        ExtUnits += (phase.Units ?? 0) * Multiplier;
            //        ExtCost += (phase.Units ?? 0) * (phase.Cost ?? 0) * (item.Multiplier ?? 1);
            //    }
            //}
            UM = item.BudgetCode.UM;

            IntersectBoreId = line.IntersectBoreId;

            #endregion mapping

        }

        public CostDetailViewModel(BidBoreLine line, BidBoreLineCostItemPhase item, int shiftNum)
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
            Units = item.Units ?? 0;
            Multiplier = item.CostItem.Multiplier ?? 1;
            ExtUnits = (item.Units ?? 0) * (item.CostItem.Multiplier ?? 1);
            Cost = item.CostItem.Cost ?? 0;
            ExtCost = Math.Round(Units * Cost * Multiplier, 0);
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
        [UIHint("LongBox")]
        public int BoreId { get; set; }

        [Display(Name = "#")]
        [UIHint("LongBox")]
        public int? IntersectBoreId { get; set; }

        [Required]
        [Display(Name = "Crossing")]
        [UIHint("TextBox")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Length")]
        [UIHint("IntegerBox")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal? Footage { get; set; }

        [Required]
        [Display(Name = "Pipe Size")]
        [UIHint("IntegerBox")]
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
        public decimal Units { get; set; }

        [Display(Name = "Ext Units")]
        [UIHint("IntegerBox")]
        public decimal ExtUnits { get; set; }

        [Display(Name = "Multiplier")]
        [UIHint("IntegerBox")]
        public decimal Multiplier { get; set; }

        [Display(Name = "UM")]
        [UIHint("TextBox")]
        public string UM { get; set; }

        [Display(Name = "Cost")]
        [UIHint("IntegerBox")]
        public decimal Cost { get; set; }

        [Display(Name = "Ext Cost")]
        [UIHint("IntegerBox")]
        public decimal ExtCost { get; set; }


        [Required]
        [Display(Name = "Intersect Footage")]
        [UIHint("IntegerBox")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal? IntersectFootage { get; set; }


        [Required]
        [Display(Name = "Intersect Footage2")]
        [UIHint("IntegerBox")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal? IntersectFootage2 { get; set; }
    }
}