//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.JC.Job;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Controllers.View.JC
//{
//    [AllowAnonymous]
//    public class TestJobController: Controller
//    {
//        [AllowAnonymous]
//        [HttpGet]
//        [Route("Job/DataTablesSample")]
//        public ActionResult DataTablesJobSample()
//        {
//            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
//            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == 1);

//            var results = new JobListViewModel();
//            results.List.Add(new JobViewModel());
//            results.Co = company.HQCo;
//            ViewBag.DataController = "TestJob";
//            ViewBag.DataAction = "DataTablesSample";
//            return View("../JC/Job/Summary/Test/Index", results);
//        }
//        [HttpGet]
//        public ActionResult Table()
//        {
//            using var db = new VPContext();
//            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == 1);

//            var results = new JobListViewModel();
//            results.List.Add(new JobViewModel());
//            results.Co = company.HQCo;
//            ViewBag.DataController = "TestJob";
//            ViewBag.DataAction = "DataTablesSample";
//            return PartialView("../JC/Job/Summary/Test/Table", results);
//        }

//        [AllowAnonymous]
//        [HttpGet]
//        public ActionResult DataTablesSample()
//        {
//            using var db = new VPContext();
//            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == 1);
//            var results = new JobListViewModel(company, db);

//            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);

//            result.MaxJsonLength = int.MaxValue;
//            return result;
//        }
//    }
//}