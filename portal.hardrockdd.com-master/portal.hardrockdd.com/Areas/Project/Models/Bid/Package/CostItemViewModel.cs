using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Project.Models.Bid.Package
{
    public class CostItemListViewModel
    {
        public CostItemListViewModel()
        {

        }

        public CostItemListViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackage package, string budgetCategory)
        {
            if (package == null)
                return;


            #region mapping
            BDCo = package.BDCo;
            BidId = package.BidId;
            PackageId = package.PackageId;
            BudgetCategory = budgetCategory;
            #endregion

            List = package.CostItems.Where(f => f.tBudgetCodeId != null)
                                    .ToList()
                                    .Where(f => f.tBudgetCodeId.StartsWith(budgetCategory, StringComparison.Ordinal))
                                    .Select(s => new CostItemViewModel(s, budgetCategory)).ToList();
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

        [Key]
        [Required]
        [Display(Name = "Budget Category")]
        public string BudgetCategory { get; set; }

        public List<CostItemViewModel> List { get; }
    }

    public class CostItemViewModel
    {
        public CostItemViewModel()
        {

        }

        public CostItemViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackageCostItem bidCostItem, string budgetCategory)
        {
            if (bidCostItem == null)
                return;

            BDCo = bidCostItem.BDCo;
            BidId = bidCostItem.BidId;
            PackageId = bidCostItem.PackageId;
            LineId = bidCostItem.LineId;
            BudgetCodeId = bidCostItem.tBudgetCodeId;
            Multiplier = bidCostItem.tMultiplier;
            BudgetCategory = budgetCategory;
            Applied = bidCostItem.tApplied == 1;
            UM = bidCostItem.BudgetCode?.UM;
        }

        [Key]
        [Required]
        public byte BDCo { get; set; }

        [Key]
        [Required]
        public int BidId { get; set; }

        [Key]
        [Required]
        public int PackageId { get; set; }

        [Key]
        [Required]
        public int LineId { get; set; }

        [Required]
        [Display(Name = "Budget Category")]
        public string BudgetCategory { get; set; }

        [Required]
        [Display(Name = "Cost Item")]
        [UIHint("DropDownBox")]
        [Field(ComboUrl = "/BudgetCode/PMCostItemCombo", ComboForeignKeys = "PMCo=BDCo,BudgetCategory")]
        public string BudgetCodeId { get; set; }

        [Required]
        [Display(Name = "Apply")]
        [UIHint("SwitchBox")]
        public bool Applied { get; set; }

        [Required]
        [Display(Name = "Multiplier")]
        [UIHint("IntegerBox")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public decimal? Multiplier { get; set; }

        [Required]
        [Display(Name = "UM")]
        [UIHint("TextBox")]
        public string UM { get; set; }


        internal CostItemViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.BidPackageCostItems.FirstOrDefault(f => f.BDCo == BDCo && f.BidId == BidId && f.PackageId == PackageId && f.LineId == LineId);

            if (updObj != null)
            {
                updObj.BudgetCodeId = BudgetCodeId;
                updObj.Multiplier = Multiplier ?? 0;
                updObj.Applied = (byte)(Applied ? 1 : 0);

                try
                {
                    db.BulkSaveChanges();
                    return new CostItemViewModel(updObj, BudgetCategory);
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