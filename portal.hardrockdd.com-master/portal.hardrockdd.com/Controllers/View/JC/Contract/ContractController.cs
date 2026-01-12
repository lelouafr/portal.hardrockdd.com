//using DB.Infrastructure.ViewPointDB.Data;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Controllers.View.JC.Contract
//{
//    [ControllerAuthorize]
//    public class ContractController : BaseController
//    {
//        [HttpGet]
//        [Route("Contract/Open")]
//        public ActionResult OpenIndex()
//        {
//            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
//            var userId = StaticFunctions.GetUserId();
//            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
//            var emp = user.Employee.FirstOrDefault();
//            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emp.HRCo);

//            var results = new JobListViewModel();
//            results.List.Add(new JobViewModel());
//            results.Co = company.HQCo;
//            ViewBag.DataController = "Job";
//            ViewBag.DataAction = "OpenData";
//            return View("../JC/Job/Summary/List/Index", results);
//        }


//        [HttpGet]
//        public ActionResult OpenTable()
//        {
//            using var db = new VPContext();
//            var userId = StaticFunctions.GetUserId();
//            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
//            var emp = user.Employee.FirstOrDefault();
//            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emp.HRCo);

//            var results = new JobListViewModel();
//            results.List.Add(new JobViewModel());
//            results.Co = company.HQCo;
//            ViewBag.DataController = "Job";
//            ViewBag.DataAction = "OpenData";
//            return PartialView("../JC/Job/Summary/List/Table", results);
//        }

//        [HttpGet]
//        public ActionResult OpenData()
//        {
//            using var db = new VPContext();
//            //var userId = StaticFunctions.GetUserId();
//            //var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
//            //var emp = user.Employee.FirstOrDefault();
//            //var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emp.HRCo);
//            var jobs = db.vJobs.Where(f => f.JobStatus == 1).OrderBy(f => f.JobId).ToList();
//            var results = new JobListViewModel(jobs, db);

//            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);

//            result.MaxJsonLength = int.MaxValue;
//            return result;
//        }

//        [HttpGet]
//        [Route("Contract")]
//        public ActionResult AllIndex()
//        {
//            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
//            var userId = StaticFunctions.GetUserId();
//            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
//            var emp = user.Employee.FirstOrDefault();
//            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emp.HRCo);

//            var results = new JobListViewModel();
//            results.List.Add(new JobViewModel());
//            results.Co = company.HQCo;
//            ViewBag.DataController = "Job";
//            ViewBag.DataAction = "AllData";
//            return View("../JC/Job/Summary/List/Index", results);
//        }


//        [HttpGet]
//        public ActionResult AllTable()
//        {
//            using var db = new VPContext();
//            var userId = StaticFunctions.GetUserId();
//            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
//            var emp = user.Employee.FirstOrDefault();
//            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emp.HRCo);

//            var results = new JobListViewModel();
//            results.List.Add(new JobViewModel());
//            results.Co = company.HQCo;
//            ViewBag.DataController = "Job";
//            ViewBag.DataAction = "AllData";
//            return PartialView("../JC/Job/Summary/List/Table", results);
//        }

//        [HttpGet]
//        public ActionResult AllData()
//        {
//            using var db = new VPContext();
//            var userId = StaticFunctions.GetUserId();
//            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
//            var emp = user.Employee.FirstOrDefault();
//            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emp.HRCo);
//            var results = new JobListViewModel(company, db);

//            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);

//            result.MaxJsonLength = int.MaxValue;
//            return result;
//        }

//        [HttpGet]
//        public PartialViewResult ProductionActVsBgt(byte jcco, string jobId)
//        {
//            using var db = new VPContext();
//            var result = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);

//            var model = new JobProductionBgtVsActListViewModel(result);

//            return PartialView("../JC/Job/ProductionBgtVsAct/Table", model);
//        }
//    }
//}