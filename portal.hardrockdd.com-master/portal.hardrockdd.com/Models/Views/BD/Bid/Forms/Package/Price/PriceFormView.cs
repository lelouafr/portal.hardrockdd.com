using DB.Infrastructure.ViewPointDB.Data;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Models.Views.Bid.Forms.Package.Price
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

            PriceDetails = new PriceViewModel(package);
            CostSummary = new Summary.PackageCostListViewModel(package);

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

        public PriceViewModel PriceDetails { get; set; }


        public Summary.PackageCostListViewModel CostSummary { get; set; }
    }

}