using System.Collections.Generic;
using System.Web.Http;

namespace portal.Controllers.API.TrialBalance
{
    [APIAuthorizeDevice]
    public class AccountGroupController : ApiController
    {
        // GET: api/TrialBalance
        public AccountGroupListViewModel Get()
        {
            //using var db = new PortalDBEntities();
            //db.SyncTransactions();
            //var result = db.AccountGroups.ToList();
            var results = new AccountGroupListViewModel();
            return results;
        }
    }


    public class AccountGroupListViewModel
    {
        public AccountGroupListViewModel()
        {
            Groups = new List<AccountGroupViewModel>();
        }

        //public AccountGroupListViewModel(List<AccountGroup> list)
        //{
        //    Groups = list.Select(s => new AccountGroupViewModel(s)).ToList();
        //}
        public List<AccountGroupViewModel> Groups { get; set; }

    }


    public class AccountGroupViewModel
    {
        public AccountGroupViewModel()
        {
            Accounts = new List<AccountGroupItemViewModel>();
        }

        //public AccountGroupViewModel(AccountGroup accountGroup)
        //{
        //    GroupId = accountGroup.GroupId;
        //    Description = accountGroup.Description;
        //    SearchString = accountGroup.SearchString;
        //    Accounts = accountGroup.AccountGroupLinks
        //        .Where(f => f.AccountId != null && f.IsActive == true)
        //        .Select(s => new AccountGroupItemViewModel(s)).ToList();
        //}

        public int GroupId { get; set; }

        public string Description { get; set; }

        public string SearchString { get; set; }

        public List<AccountGroupItemViewModel> Accounts { get; set; }

    }

    public class AccountGroupItemViewModel
    {
        public AccountGroupItemViewModel()
        {

        }

        //public AccountGroupItemViewModel(AccountGroupLink link)
        //{
        //    GroupId = link.GroupId;
        //    AccountId = (int)link.AccountId;
        //    AccountNumber = link.Account.AccountNumber ?? -999999;
            
        //}

        public int GroupId { get; set; }

        public int AccountId { get; set; }

        public int AccountNumber { get; set; }

    }
}
