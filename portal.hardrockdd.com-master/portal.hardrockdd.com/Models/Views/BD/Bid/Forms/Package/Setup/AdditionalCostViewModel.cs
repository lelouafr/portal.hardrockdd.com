using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Bid.Forms.Package.Setup
{
    public class AllocatedCostListViewModel
    {
        public AllocatedCostListViewModel()
        {

        }

        public AllocatedCostListViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackage package)
        {
            if (package == null)
            {
                throw new System.ArgumentNullException(nameof(package));
            }
            #region mapping
            BDCo = package.BDCo;
            BidId = package.BidId;
            PackageId = package.PackageId;
            #endregion
            List = package.PackageCosts.Select(s => new AllocatedCostViewModel(s)).ToList();
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

        public List<AllocatedCostViewModel> List { get; }
    }

    public class AllocatedCostViewModel
    {

        public AllocatedCostViewModel()
        {

        }

        public AllocatedCostViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackageCost cost)
        {
            if (cost == null)
            {
                throw new System.ArgumentNullException(nameof(cost));
            }
            BDCo = cost.BDCo;
            BidId = cost.BidId;
            PackageId = cost.PackageId;
            LineId = cost.LineId;
            AllocationType = (DB.BDPackageCostAllocationType?)cost.CostAllocationTypeId;
            BudgetCodeId = cost.BudgetCodeId;
            Cost = cost.Cost;
            
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
        [Display(Name = "Line Id")]
        public int LineId { get; set; }

        [Required]
        [Display(Name = "Allocation Type")]
        [UIHint("EnumBox")]
        [Field(Placeholder ="Select Type")]
        public DB.BDPackageCostAllocationType? AllocationType { get; set; }

        [Required]
        [Display(Name = "Cost Item")]
        [UIHint("DropdownBox")]
        [Field( ComboUrl = "/BudgetCode/CostItemCombo", ComboForeignKeys = "PMCo=BDCo")]
        public string BudgetCodeId { get; set; }


        [Required]
        [Display(Name = "Cost")]
        [UIHint("CurrencyBox")]
        public decimal? Cost { get; set; }

        internal AllocatedCostViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            var updObj = db.BidPackageCosts.FirstOrDefault(f => f.BDCo == BDCo && f.BidId == BidId && f.PackageId == PackageId && f.LineId == LineId);

            if (updObj != null)
            {
                /****Write the changes to object****/

                updObj.CostAllocationType = AllocationType;
                updObj.BudgetCodeId = BudgetCodeId;
                updObj.Cost = Cost;
                try
                {
                    db.SaveChanges(modelState);
                    return new AllocatedCostViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            modelState.AddModelError("", "Object Doesn't Exist For Update!");
            return this;
        }

    }
}