using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Areas.Project.Models.Bid.Proposal
{
    public class FormViewModel
    {
        public FormViewModel()
        {

        }

        public FormViewModel(DB.Infrastructure.ViewPointDB.Data.Bid bid)
        {
            if (bid == null)
                return;

            #region mapping
            BDCo = bid.BDCo;
            BidId = bid.BidId;

            CompanyInfo = new portal.Models.Views.HQ.Company.CompanyViewModel(bid.Company);
            Bid = new BidViewModel(bid);
            Packages = new PackageListViewModel(bid);
            Customers = new CustomerListViewModel(bid);
            #endregion


        }
        public FormViewModel(DB.Infrastructure.ViewPointDB.Data.BidCustomer bidCustomer)
        {
            if (bidCustomer == null)
                return;

            var bid = bidCustomer.Bid;

            #region mapping
            BDCo = bid.BDCo;
            BidId = bid.BidId;

            Bid = new BidViewModel(bid);
            Packages = new PackageListViewModel(bid);

            CompanyInfo = new portal.Models.Views.HQ.Company.CompanyViewModel(bid.Company);
            Customer = new CustomerViewModel(bidCustomer);
            Customers = new CustomerListViewModel(bid);
            #endregion

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

        public BidViewModel Bid { get; }

        public PackageListViewModel Packages { get; }

        public CustomerViewModel Customer { get; set; }

        public portal.Models.Views.HQ.Company.CompanyViewModel CompanyInfo { get; set; }

        public CustomerListViewModel Customers { get; set; }

    }

}