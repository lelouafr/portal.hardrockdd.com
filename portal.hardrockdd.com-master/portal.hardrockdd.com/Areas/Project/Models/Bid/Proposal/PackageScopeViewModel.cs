using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Project.Models.Bid.Proposal
{
    public class PackageScopeListViewModel
    {
        public PackageScopeListViewModel()
        {

        }

        public PackageScopeListViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackage package, DB.ScopeTypeEnum scopeType)
        {
            if (package == null)
                return;

            #region mapping
            BDCo = package.BDCo;
            BidId = package.BidId;
            PackageId = package.PackageId;
            ScopeTypeId = scopeType;
            #endregion

            List = package.Scopes.Where(f => f.ScopeTypeId == (int)scopeType).Select(s => new PackageScopeViewModel(s)).ToList();
        }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte BDCo { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Bid Id")]
        public int BidId { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Bid Id")]
        public int PackageId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        public DB.ScopeTypeEnum ScopeTypeId { get; set; }

        public List<PackageScopeViewModel> List { get; }
    }

    public class PackageScopeViewModel
    {
        public PackageScopeViewModel()
        {

        }

        public PackageScopeViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackageScope scope)
        {
            if (scope == null)
                return;

            #region mapping
            BDCo = scope.BDCo;
            BidId = scope.BidId;
            PackageId = scope.PackageId;
            ScopeTypeId = (DB.ScopeTypeEnum)scope.ScopeTypeId;
            LineId = scope.LineId;
            Title = scope.Title;
            Notes = scope.Notes;
            Status = scope.Status;
            # endregion
        }

        [Key]
        [Required]
        [HiddenInput]
        public byte BDCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        public int BidId { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        public int PackageId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        public DB.ScopeTypeEnum ScopeTypeId { get; set; }

        [Key]
        [Required]
        [Display(Name = "#")]
        [UIHint("LongBox")]
        public int LineId { get; set; }

        [Required]
        [Display(Name = "Title")]
        [UIHint("TextBox")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Notes")]
        [UIHint("TextEditorAreaBox")]
        [AllowHtml]
        public string Notes { get; set; }

        [Required]
        [Display(Name = "Status")]
        [UIHint("EnumBox")]
        public int? Status { get; set; }

        internal PackageScopeViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.BidPackageScopes.FirstOrDefault(f => f.BDCo == BDCo &&
                                                                 f.BidId == BidId &&
                                                                 f.PackageId == PackageId &&
                                                                 f.ScopeTypeId == (int)ScopeTypeId &&
                                                                 f.LineId == LineId);

            if (updObj != null)
            {
                updObj.Title = Title ?? string.Empty;
                updObj.Notes = Notes ?? string.Empty;

                try
                {
                    db.BulkSaveChanges();
                    return new PackageScopeViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            else
            {
                modelState.AddModelError("", "Object Doesn't Exist For Update!");
                return this;
            }
        }
    }
}