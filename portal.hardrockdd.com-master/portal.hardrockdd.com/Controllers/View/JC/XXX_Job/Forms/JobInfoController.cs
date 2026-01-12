//using Newtonsoft.Json;
//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.JC.Job;
//using portal.Models.Views.JC.Job.Forms;
//using System.Linq;
//using System.Web.Mvc;
//using portal.Repository.VP.JC;


//namespace portal.Controllers.View.JC
//{
//    [ControllerAuthorize]
//    public class JobInfoController : BaseController
//    {
//        [HttpGet]
//        public ActionResult Index(byte jcco, string jobId)
//        {
//            using var db = new VPContext();
//            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
//            var result = new JobInfoViewModel(entity);

//            return PartialView("../JC/Job/Forms/Info/PartialIndex", result);
//        }

//        [HttpGet]
//        public ActionResult Form(byte jcco, string jobId)
//        {
//            using var db = new VPContext();
//            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
//            var result = new JobInfoViewModel(entity);

//            return PartialView("../JC/Job/Forms/Info/Form", result);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Update(JobInfoViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                using var db = new VPContext();
//                model = JobRepository.ProcessUpdate(model, db);
//                db.SaveChanges(ModelState);
//            }
//            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
//        }

//        [HttpGet]
//        public JsonResult Validate(JobViewModel model)
//        {
//            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
//        }
//    }
//}