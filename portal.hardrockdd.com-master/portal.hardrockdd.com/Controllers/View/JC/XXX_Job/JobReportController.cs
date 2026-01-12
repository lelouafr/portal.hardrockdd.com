//using portal.Models.Views.JC.Job.Report;
//using portal.Models.Views.JC.Job;
//using portal.Models.Views.Purchase.Order;
//using System;
//using System.Linq;
//using System.Web.Mvc;


//namespace portal.Controllers.View.JC
//{

//    [ControllerAuthorize]
//    public class JobReportController : BaseController
//    {
//        [HttpGet]
//        public ActionResult GrossMarginIndex()
//        {
//            var current = DateTime.Now.AddMonths(-1);
//            var mth = new DateTime(current.Year, current.Month, 1);
//            var results = new JobGrossMarginListViewModel(mth);

//            ViewBag.Controller = "JobReport";

//            return View("../JC/Job/Report/GM/Index", results);
//        }

//        [HttpGet]
//        public ActionResult GrossMarginTable(DateTime mth)
//        {
//            var results = new JobGrossMarginListViewModel(mth);

//            ViewBag.Controller = "JobReport";

            
//            return PartialView("../JC/Job/Report/GM/List/Table", results);
//        }


//        [HttpGet]
//        public ActionResult GrossMarginData()
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