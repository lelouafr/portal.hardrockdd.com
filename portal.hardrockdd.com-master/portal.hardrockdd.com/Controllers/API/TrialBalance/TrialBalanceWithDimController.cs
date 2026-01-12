using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.GL.TrialBalance;
using System.Web.Http;

namespace portal.Controllers.API.TrialBalance
{
    [APIAuthorizeDevice]
    public class TrialBalanceWithDimController : ApiController
    {
        // GET: api/TrialBalance
        public ActTrialBalanceWithDimListViewModel Get(int subsidiaryId, int year)
        {
            if (subsidiaryId == 0 || year == 0)
            {
                return new ActTrialBalanceWithDimListViewModel();
            }
            //var data = portal.Models.NetSuite.GL.TrialBalanceWithDimListModel.GetRecords(subsidiaryId, year);
            //var results = new TrialBalanceWithDimListViewModel(data);

            using var db = new VPContext();
            //db.SyncTransactions();
            var results = new ActTrialBalanceWithDimListViewModel(subsidiaryId, year, db);
            return results;
        }
    }
}
