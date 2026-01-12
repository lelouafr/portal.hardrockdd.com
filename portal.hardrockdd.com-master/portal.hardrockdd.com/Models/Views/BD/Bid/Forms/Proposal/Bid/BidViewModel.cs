namespace portal.Models.Views.Bid.Forms.Proposal.Bid
{
    public class BidViewModel: Header.BidInfoViewModel
    {
        public BidViewModel()
        {

        }

        public BidViewModel(DB.Infrastructure.ViewPointDB.Data.Bid bid): base(bid)
        {
            if (bid == null)
                return;


            CompanyInfo = new HQ.Company.CompanyViewModel(bid.Company);
            Notes = new ProposalScopeListViewModel(bid, DB.ScopeTypeEnum.Note);
            Clarifications = new ProposalScopeListViewModel(bid, DB.ScopeTypeEnum.Clarification);
            ScopeofWork = new ProposalScopeListViewModel(bid, DB.ScopeTypeEnum.Scope);
            OutofScope = new ProposalScopeListViewModel(bid, DB.ScopeTypeEnum.OutofScope);
        }
        public ProposalScopeListViewModel Notes { get; set; }
        public ProposalScopeListViewModel Clarifications { get; set; }
        public ProposalScopeListViewModel ScopeofWork { get; set; }
        public ProposalScopeListViewModel OutofScope { get; set; }
        public HQ.Company.CompanyViewModel CompanyInfo { get; set; }
    }
}