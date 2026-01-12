using portal.Models.Views.HQ.Company;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Models.Views.Bid.Forms.Proposal
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

            CompanyInfo = new CompanyViewModel(bid.Company);
            Bid = new Bid.BidViewModel(bid);
            Packages = new Package.PackageListViewModel(bid);
            Customers = new Header.CustomerListViewModel(bid);
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

            Bid = new Bid.BidViewModel(bid);
            Packages = new Package.PackageListViewModel(bid);

            CompanyInfo = new CompanyViewModel(bid.Company);
            Customer = new Header.CustomerViewModel(bidCustomer);
            Customers = new Header.CustomerListViewModel(bid);
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

        public Bid.BidViewModel Bid { get; }

        public Package.PackageListViewModel Packages { get; }

        public Header.CustomerViewModel Customer { get; set; }

        public CompanyViewModel CompanyInfo { get; set; }

        public Header.CustomerListViewModel Customers { get; set; }

    }

}