using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.GL.TrialBalance;
using System.Web.Http;

namespace portal.Controllers.API.Budget
{
    [APIAuthorizeDevice]
    public class BudgetWithDimController : ApiController
    {
        // GET: api/Budget
        public BudgetWithDimListViewModel Get(int subsidiaryId, int year)
        {
            if (subsidiaryId == 0 || year == 0)
            {
                return new BudgetWithDimListViewModel();
            }
            //var data = portal.Models.NetSuite.GL.BudgetWithDimListModel.GetRecords(subsidiaryId, year);
            //var results = new BudgetWithDimListViewModel(data);

            using var db = new VPContext();
            //db.SyncTransactions();
            var results = new BudgetWithDimListViewModel(subsidiaryId, year, db);
            return results;
        }
    }
}
