//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace portal.Models.Views.Bid
//{
//    public class BidCustomerProposalViewModel
//    {
//        public BidCustomerProposalViewModel()
//        {

//        }

//        public BidCustomerProposalViewModel(DB.Infrastructure.ViewPointDB.Data.Bid bid, DB.Infrastructure.ViewPointDB.Data.BidCustomer bidCustomer)
//        {
//            if (bid == null)
//            {
//                throw new System.ArgumentNullException(nameof(bid));
//            }

//            #region mapping
//            Co = bid.Co;
//            BidId = bid.BidId;
//            #endregion

//            Bid = new BidViewModel(bid);

//            Customer = new BidCustomerViewModel(bidCustomer);
//            Packages = new PackageListViewModel(bid); 
//            Scopes = new BidProposalScopeListViewModel(bid);
//        }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Co")]
//        public byte Co { get; set; }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Bid Id")]
//        public int BidId { get; set; }

//        public BidViewModel Bid { get; set; }

//        public BidCustomerViewModel Customer { get; set; }

//        public PackageListViewModel Packages { get; set; }

//        public BidProposalScopeListViewModel Scopes { get; }
//    }

//}