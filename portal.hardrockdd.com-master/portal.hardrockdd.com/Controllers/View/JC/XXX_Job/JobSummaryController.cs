//using portal.Models.Views.JC.Job;
//using portal.Models.Views.Purchase.Order;
//using System.Linq;
//using System.Web.Mvc;


//namespace portal.Controllers.View.JC
//{

//    [ControllerAuthorize]
//    public class JobSummaryController : BaseController
//    {
//        [HttpGet]
//        //[Route("Jobs")]
//        public ActionResult Index()
//        {
//            var results = new JobSummaryListViewModel();
//            results.List.Add(new JobSummaryViewModel());

//            ViewBag.TicketDataController = "JobSummary";

//            return View("../JC/Job/Summary/Tree/Index", results);
//        }

//        [HttpGet]
//        public ActionResult Table()
//        {
//            var results = new JobSummaryListViewModel();
//            results.List.Add(new JobSummaryViewModel());

//            ViewBag.Controller = "JobSummary";

//            return View("../JC/Job/Summary/Tree/Table", results);
//        }


//        [HttpGet]
//        public ActionResult Data()
//        {
//            using var db =  new DB.Infrastructure.ViewPointDB.Data.VPContext();
//            var userId = StaticFunctions.GetUserId();
//            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
//            var emp = user.Employee.FirstOrDefault();
//            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emp.HRCo);
//            var results = new JobSummaryListViewModel(company, db);

//            JsonResult result = Json(new
//            {
//                data = results.List.ToArray()
//            }, JsonRequestBehavior.AllowGet);
//            result.MaxJsonLength = int.MaxValue;
//            return result;
//        }
//    }
//}