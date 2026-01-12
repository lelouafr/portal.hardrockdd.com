
namespace portal.Areas.Project.Models.Bid.Proposal
{
    public class BidViewModel: Bid.BidInfoViewModel
    {
        public BidViewModel()
        {

        }

        public BidViewModel(DB.Infrastructure.ViewPointDB.Data.Bid bid): base(bid)
        {
            if (bid == null)
                return;

            CompanyInfo = new portal.Models.Views.HQ.Company.CompanyViewModel(bid.Company);
            Notes = new BidScopeListViewModel(bid, DB.ScopeTypeEnum.Note);
            Clarifications = new BidScopeListViewModel(bid, DB.ScopeTypeEnum.Clarification);
            ScopeofWork = new BidScopeListViewModel(bid, DB.ScopeTypeEnum.Scope);
            OutofScope = new BidScopeListViewModel(bid, DB.ScopeTypeEnum.OutofScope);
        }

        public BidScopeListViewModel Notes { get; set; }

        public BidScopeListViewModel Clarifications { get; set; }

        public BidScopeListViewModel ScopeofWork { get; set; }

        public BidScopeListViewModel OutofScope { get; set; }

        public portal.Models.Views.HQ.Company.CompanyViewModel CompanyInfo { get; set; }
    }
}