//using NetSuiteDB.Infrastructure.Data;
using System.Web.Http;

namespace portal.Controllers.API.TrialBalance
{
    [APIAuthorizeDevice]
    public class SyncDataController : ApiController
    {
        public bool Post()
        {
            //using var db = new PortalDBEntities();
            //db.SyncTransactions();
            //db.SyncBudgets();
            //db.SyncBudgetTransactions();
            return true;
        }
    }
}
