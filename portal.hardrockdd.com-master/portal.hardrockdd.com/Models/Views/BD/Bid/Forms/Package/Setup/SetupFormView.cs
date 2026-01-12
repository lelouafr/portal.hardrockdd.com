using DB.Infrastructure.ViewPointDB.Data;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Models.Views.Bid.Forms.Package.Setup
{
    public class SetupFormView
    {
        public SetupFormView()
        {

        }
        public SetupFormView(BidPackage package)
        {
            if (package == null)
                return;

            #region mapping
            BDCo = package.BDCo;
            BidId = package.BidId;
            PackageId = package.PackageId;
            #endregion

            RoundingDaySetup = new RoundDayViewModel(package);
            ProductionRates = new ProductionRateListViewModel(package);
            CostItems = new CostItemListViewModel(package, "CI-");
            RentalItems = new CostItemListViewModel(package, "RE-");
            AdditionalCostItems = new AllocatedCostListViewModel(package);
        }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "BDCo")]
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

        public RoundDayViewModel RoundingDaySetup { get; set; }

        public ProductionRateListViewModel ProductionRates { get; set; }

        public CostItemListViewModel CostItems { get; set; }

        public CostItemListViewModel RentalItems { get; set; }

        public AllocatedCostListViewModel AdditionalCostItems { get; set; }
    }

}