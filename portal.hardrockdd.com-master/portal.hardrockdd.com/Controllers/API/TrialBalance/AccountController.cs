using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.GL.Account;
using System.Collections.Generic;
using System.Web.Http;

namespace portal.Controllers.API.TrialBalance
{
    [APIAuthorizeDevice]
    public class AccountController : ApiController
    {
        public AccountListViewModel Get(byte subsidiaryId)
        {
            using var db = new VPContext();
            var results = new AccountListViewModel(subsidiaryId, db);
            return results;
        }
    }
}
