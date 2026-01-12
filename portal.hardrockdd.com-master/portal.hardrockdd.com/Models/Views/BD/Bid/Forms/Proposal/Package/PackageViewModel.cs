using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Bid.Forms.Proposal.Package
{
    public class PackageListViewModel
    {
        public PackageListViewModel()
        {

        }

        public PackageListViewModel(DB.Infrastructure.ViewPointDB.Data.Bid bid)
        {
            if (bid == null)
            {
                throw new System.ArgumentNullException(nameof(bid));
            }
            #region mapping
            BDCo = bid.BDCo;
            BidId = bid.BidId;
            #endregion
            List = bid.ActivePackages.Select(s => new PackageProposalViewModel(s)).ToList();
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

        public List<PackageProposalViewModel> List { get; }
    }

    public class PackageProposalViewModel: Forms.Package.Price.PriceViewModel
    {
        public PackageProposalViewModel()
        {

        }

        public PackageProposalViewModel(DB.Infrastructure.ViewPointDB.Data.BidPackage package): base(package)
        {
            if (package == null)
            {
                throw new System.ArgumentNullException(nameof(package));
            }
            IncludeOnProposal = package.IncludeOnProposal ?? true;
            Notes = new ProposalScopeListViewModel(package, DB.ScopeTypeEnum.Note);
            Clarifications = new ProposalScopeListViewModel(package, DB.ScopeTypeEnum.Clarification);
            ScopeofWork = new ProposalScopeListViewModel(package, DB.ScopeTypeEnum.Scope);
        }

        [Display(Name = "Include On Proposal")]
        [Field(LabelSize = 8, TextSize = 4)]
        [UIHint("SwitchBoxGreen")]
        public bool IncludeOnProposal { get; set; }

        public ProposalScopeListViewModel Notes { get; set; }
        public ProposalScopeListViewModel Clarifications { get; set; }
        public ProposalScopeListViewModel ScopeofWork { get; set; }

        internal new PackageProposalViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.BidPackages.FirstOrDefault(f => f.BDCo == BDCo && f.BidId == BidId && f.PackageId == PackageId);

            if (updObj != null)
            {
                updObj.Description = Description;
                updObj.IncludeOnProposal = IncludeOnProposal;

                try
                {
                    db.BulkSaveChanges();
                    return new PackageProposalViewModel(updObj);
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