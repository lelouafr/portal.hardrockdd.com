using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Bid.Forms.Bore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.Bid.Forms.Package.Summary
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

            List = package.fvBoreLineCostItems().Select(s => new CostDetailViewModel(s)).ToList();
            //List = package.vBoreLineCostItems.Select(s => new CostDetailViewModel(s)).ToList();
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

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public List<CostDetailViewModel> List { get; }
    }
}