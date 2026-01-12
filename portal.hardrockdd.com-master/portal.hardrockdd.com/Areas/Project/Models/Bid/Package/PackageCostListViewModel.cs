using DB.Infrastructure.ViewPointDB.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace portal.Areas.Project.Models.Bid.Package
{
    public class PackageCostListViewModel
    {
        public PackageCostListViewModel()
        {

        }

        public PackageCostListViewModel(BidPackage package)
        {
            if (package == null)
                throw new System.ArgumentNullException(nameof(package));

            #region mapping
            BDCo = package.BDCo;
            BidId = package.BidId;
            PackageId = package.PackageId;
            Description = package.Description;
            #endregion

            List = package.fvBoreLineCostItems().Select(s => new Bore.CostDetailViewModel(s)).ToList();
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

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public List<Bore.CostDetailViewModel> List { get; }
    }
}