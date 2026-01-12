//using Microsoft.AspNet.Identity;
//using DB.Infrastructure.ViewPointDB.Data;
////using portal.Models.Views.Bid;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Controllers.View.Bid
//{
//    public class BidUserSummaryController : BaseController
//    {
//        // GET: Bid
//        [Route("Bids/User/")]
//        [HttpGet]
//        public ActionResult Index()
//        {
//            using var db = new VPContext();
//            var userId = StaticFunctions.GetUserId();
//            var bids = db.Bids.Where(f => f.WorkFlow.Sequances.Where(sf => sf.Active).SelectMany(s => s.AssignedUsers).Any(au => au.AssignedTo == userId) &&
//                                           f.tStatusId != (int)DB.BidStatusEnum.Canceled)
//                             .OrderByDescending(o => o.DueDate)
//                             .ThenByDescending(o => o.BidDate)
//                             .ToList();
//            //var obj = db.Bids.Where(f => f.WorkFlows.Any(w => w.AssignedTo == userId  && w.Bid.tStatusId == w.Status && w.Active == "Y")).OrderByDescending(o => o.BidDate);
//            var model = bids.Select(s => new BidUserSummaryViewModel(s)).ToList();

//            return View("../BD/Bid/List/User/Index", model);
//        }

//        [HttpGet]
//        public ActionResult Table()
//        {
//            using var db = new VPContext();
//            var userId = StaticFunctions.GetUserId();

//            var bids = db.Bids.Where(f => f.WorkFlow.Sequances.Where(sf => sf.Active).SelectMany(s => s.AssignedUsers).Any(au => au.AssignedTo == userId) &&
//                                           f.tStatusId != (int)DB.BidStatusEnum.Canceled)
//                             .OrderByDescending(o => o.DueDate)
//                             .ThenByDescending(o => o.BidDate)
//                             .ToList();

//            var model = bids.Select(s => new BidUserSummaryViewModel(s)).ToList();

//            return PartialView("../BD/Bid/List/User/Table", model);

//        }


//    }
//}