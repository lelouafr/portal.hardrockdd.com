//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Models.Views.Bid
//{
//    public class BidPackageProposalScopeListViewModel
//    {
//        public BidPackageProposalScopeListViewModel()
//        {

//        }

//        public BidPackageProposalScopeListViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackage package)
//        {
//            if (package == null)
//            {
//                throw new System.ArgumentNullException(nameof(package));
//            }
//            #region mapping
//            Co = package.Co;
//            BidId = package.BidId;
//            PackageId = package.PackageId;
//            #endregion

//            List = package.Scopes.Select(s => new BidPackageProposalScopeViewModel(s)).ToList();
//        }
//        public BidPackageProposalScopeListViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackage package, DB.ScopeTypeEnum scopeType)
//        {
//            if (package == null)
//            {
//                throw new System.ArgumentNullException(nameof(package));
//            }
//            #region mapping
//            Co = package.Co;
//            BidId = package.BidId;
//            PackageId = package.PackageId;
//            #endregion

//            List = package.Scopes.Where(f => f.ScopeTypeId == (int)scopeType).Select(s => new BidPackageProposalScopeViewModel(s)).ToList();
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

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Bid Id")]
//        public int PackageId { get; set; }

//        public List<BidPackageProposalScopeViewModel> List { get; }
//    }

//    public class BidPackageProposalScopeViewModel
//    {
//        public BidPackageProposalScopeViewModel()
//        {

//        }

//        public BidPackageProposalScopeViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackageScope scope)
//        {
//            if (scope == null)
//            {
//                throw new System.ArgumentNullException(nameof(scope));
//            }
//            #region mapping
//            Co = scope.Co;
//            BidId = scope.BidId;
//            PackageId = scope.PackageId;
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
//        [HiddenInput]
//        [Required]
//        public int PackageId { get; set; }

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