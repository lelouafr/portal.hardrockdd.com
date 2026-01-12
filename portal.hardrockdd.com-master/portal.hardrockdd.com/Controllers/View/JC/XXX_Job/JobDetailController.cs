//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.JC.Job;
//using portal.Models.Views.Purchase.Order;
//using System.Linq;
//using System.Web.Mvc;


//namespace portal.Controllers.View.JC
//{

//    [ControllerAuthorize]
//    public class JobDetailController : BaseController
//    {
//        [HttpGet]
//        [Route("Jobs/Details/{jobId}-{co}")]
//        public ActionResult Index(byte co, string jobId)
//        {
//            using var db = new VPContext();
//            var result = db.Jobs.FirstOrDefault(f => f.JCCo == co && f.JobId == jobId);
//            var model = new JobDetailViewModel(result, db); 
//            ViewBag.Controller = "JobDetail";
//            ViewBag.TicketDataController = "TicketSummary";
//            ViewBag.TicketDataAction = "JobData";

//            return View("../JC/Job/Detail/Index", model);
//        }

//        [HttpGet]
//        public ActionResult Panel(byte co, string jobId)
//        {
//            using var db = new VPContext();
//            var result = db.Jobs.FirstOrDefault(f => f.JCCo == co && f.JobId == jobId);
//            var model = new JobDetailViewModel(result, db);
//            ViewBag.Controller = "JobDetail";
//            ViewBag.TicketDataController = "TicketSummary";
//            ViewBag.TicketDataAction = "JobData";

//            return PartialView("../JC/Job/Detail/Panel", model);
//        }


//    }
//}