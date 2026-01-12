using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Bid.Wizard
{
    //public class BidWizardFormViewModel
    //{
    //    public BidWizardFormViewModel()
    //    {

    //    }

    //    public BidWizardFormViewModel(DB.Infrastructure.ViewPointDB.Data.Bid bid)
    //    {
    //        if (bid == null)
    //        {
    //            throw new System.ArgumentNullException(nameof(bid));
    //        }

    //        #region mapping
    //        Co = bid.Co;
    //        BidId = bid.BidId;
    //        #endregion

    //        Bid = new BidViewModel(bid);
    //        Lines = new BidBoreLineListViewModel(bid);
    //        CostItems = new BidPackageCostItemListViewModel(bid);
    //        Rentals = new BidPackageRentalItemListViewModel(bid);
    //        ProductionRates = new BidPackageProductionRateListViewModel(bid);
    //    }

    //    [Key]
    //    [HiddenInput]
    //    [Required]
    //    [Display(Name = "Co")]
    //    public byte Co { get; set; }

    //    [Key]
    //    [HiddenInput]
    //    [Required]
    //    [Display(Name = "Bid Id")]
    //    public int BidId { get; set; }

    //    public BidViewModel Bid { get; set; }

    //    public BidBoreLineListViewModel Lines { get; set; }

    //    public BidPackageCostItemListViewModel CostItems { get; set; }

    //    public BidPackageRentalItemListViewModel Rentals { get; set; }

    //    public BidPackageProductionRateListViewModel ProductionRates { get; }
    //}
}