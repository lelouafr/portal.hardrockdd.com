using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.GL.TrialBalance;
using System.Web.Http;

namespace portal.Controllers.API.TrialBalance
{
    [APIAuthorizeDevice]
    public class TrialBalanceController : ApiController
    {
        // GET: api/TrialBalance
        public ActTrialBalanceListViewModel Get(int subsidiaryId, int year)
        {
            if (subsidiaryId == 0 || year == 0)
            {
                return new ActTrialBalanceListViewModel();
            }
            //var data = portal.Models.NetSuite.GL.TrialBalanceListModel.GetRecords(subsidiaryId, year);
            //var results = new TrialBalanceListViewModel(data);

            using var db = new VPContext();
            //db.SyncTransactions();
            var results = new ActTrialBalanceListViewModel(subsidiaryId, year, db);
            return results;
        }
    }
}
