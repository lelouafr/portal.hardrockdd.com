using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.GL.TrialBalance;
using System.Web.Http;

namespace portal.Controllers.API.TrialBalance
{
    [APIAuthorizeDevice]
    public class TransactionController : ApiController
    {
        // GET: api/Transaction
        public TransactionListViewModel Get(int subsidiaryId, int year, int month, string accountNum)
        {
            if (subsidiaryId == 0 || year == 0 || month == 0 || string.IsNullOrEmpty(accountNum))
            {
                return new TransactionListViewModel();
            }

            using var db = new VPContext();
            var results = new TransactionListViewModel(subsidiaryId, year, month, accountNum, db);
            return results;
        }
    }
}
