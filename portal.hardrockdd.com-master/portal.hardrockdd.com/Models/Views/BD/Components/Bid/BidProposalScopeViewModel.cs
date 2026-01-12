//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Repository.VP.BD;
//using portal.Repository.VP.PM;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Models.Views.Bid
//{
//    public class BidProposalScopeListViewModel
//    {
//        public BidProposalScopeListViewModel()
//        {

//        }

//        public BidProposalScopeListViewModel(DB.Infrastructure.ViewPointDB.Data.Bid bid)
//        {
//            if (bid == null)
//            {
//                throw new System.ArgumentNullException(nameof(bid));
//            }
//            #region mapping
//            Co = bid.Co;
//            BidId = bid.BidId;
//            #endregion

//            List = bid.Scopes.Select(s => new BidProposalScopeViewModel(s)).ToList();
//        }

//        public BidProposalScopeListViewModel(DB.Infrastructure.ViewPointDB.Data.Bid bid, DB.ScopeTypeEnum scopeType)
//        {
//            if (bid == null)
//            {
//                throw new System.ArgumentNullException(nameof(bid));
//            }
//            #region mapping
//            Co = bid.Co;
//            BidId = bid.BidId;
//            #endregion

//            List = bid.Scopes.Where(f => f.ScopeTypeId ==(int)scopeType).Select(s => new BidProposalScopeViewModel(s)).ToList();
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

//        public List<BidProposalScopeViewModel> List { get; }
//    }

//    public class BidProposalScopeViewModel
//    {
//        public BidProposalScopeViewModel()
//        {

//        }

//        public BidProposalScopeViewModel(DB.Infrastructure.ViewPointDB.Data.BidProposalScope scope)
//        {
//            if (scope == null)
//            {
//                throw new System.ArgumentNullException(nameof(scope));
//            }
//            #region mapping
//            Co = scope.Co;
//            BidId = scope.BidId;
//            ScopeTypeId = scope.ScopeTypeId;
//            LineId = scope.LineId;
//            Title = scope.Title;
//            Notes = scope.Notes;
//            Status = scope.Status;
//            # endregion
            

//        }

//        [Key]
//        [Required]
//        [HiddenInput]
//        public byte Co { get; set; }

//        [Key]
//        [Required]
//        [HiddenInput]
//        public int BidId { get; set; }

//        [Key]
//        [Required]
//        [HiddenInput]
//        [TableField(Width = "5")]
//        public int ScopeTypeId { get; set; }

//        [Key]
//        [Required]
//        [Display(Name = "#")]
//        [UIHint("LongBox")]
//        public int LineId { get; set; }

//        [Required]
//        [Display(Name = "Title")]
//        [UIHint("TextBox")]
//        [TableField(Width = "20")]
//        public string Title { get; set; }

//        [Required]
//        [Display(Name = "Notes")]
//        [UIHint("TextEditorAreaBox")]
//        [TableField(Width = "20")]
//        [AllowHtml]
//        public string Notes { get; set; }

//        [Required]
//        [Display(Name = "Status")]
//        [UIHint("EnumBox")]
//        public int? Status { get; set; }
        
//    }
//}