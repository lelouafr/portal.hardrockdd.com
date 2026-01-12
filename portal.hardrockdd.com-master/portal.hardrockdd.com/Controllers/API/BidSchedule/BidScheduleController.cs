using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.API.PM.BidSchedule;
using portal.Models.Views.GL.TrialBalance;
using System.Linq;
using System.Web.Http;

namespace portal.Controllers.API.Budget
{
    //[APIAuthorizeDevice]
    public class BidScheduleController : ApiController
    {
        public BidScheduleApiModel Get(int bidId)
        {
            if (bidId == 0)
            {
                return new BidScheduleApiModel();
            }

            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BidId == bidId);
            var results = new BidScheduleApiModel(bid);
            return results;
        }
    }
}
