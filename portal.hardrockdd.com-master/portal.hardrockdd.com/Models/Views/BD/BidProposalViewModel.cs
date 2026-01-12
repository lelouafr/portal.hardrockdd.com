//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace portal.Models.Views.Bid
//{
//    public class BidProposalViewModel
//    {
//        public BidProposalViewModel()
//        {

//        }
//        public BidProposalViewModel(DB.Infrastructure.ViewPointDB.Data.Bid bid)
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
//            Scopes = new BidProposalScopeListViewModel(bid);
//            Packages = new Wizard.Estimate.PackageListViewModel(bid, DB.BidStatusEnum.SalesReview, true);
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

//        public BidViewModel Bid { get; }

//        public BidProposalScopeListViewModel Scopes { get; }

//        public Wizard.Estimate.PackageListViewModel Packages { get; }
//    }
//}