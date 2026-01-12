using DB.Infrastructure.ViewPointDB.Data;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Areas.Project.Models.Bid.Package
{
    public class PriceFormViewModel
    {
        public PriceFormViewModel()
        {

        }
        public PriceFormViewModel(BidPackage package)
        {
            if (package == null)
                return;

            #region mapping
            BDCo = package.BDCo;
            BidId = package.BidId;
            PackageId = package.PackageId;
            #endregion

            PriceDetails = new PriceViewModel(package);
            CostSummary = new PackageCostListViewModel(package);

        }

        [Key]
        [Required]
        [Display(Name = "BDCo")]
        public byte BDCo { get; set; }

        [Key]
        [Required]
        [Display(Name = "Bid Id")]
        public int BidId { get; set; }

        [Key]
        [Required]
        [Display(Name = "Package Id")]
        public int PackageId { get; set; }

        public PriceViewModel PriceDetails { get; set; }

        public PackageCostListViewModel CostSummary { get; set; }
    }

}