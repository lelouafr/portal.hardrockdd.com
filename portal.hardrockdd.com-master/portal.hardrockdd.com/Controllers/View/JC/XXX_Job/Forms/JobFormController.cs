//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.JC.Job.Forms;
//using portal.Repository.VP.PM;
//using System;
//using System.Linq;
//using System.Web.Mvc;


//namespace portal.Controllers.View.Job
//{
//    [ControllerAuthorize]
//    public class JobFormController : BaseController
//    {

//        [HttpPost]
//        public ActionResult RebuildGantt(byte jcco, string jobId)
//        {
//            using var db = new VPContext();
//            var job = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
//            //ProjectGanttRepository.GenerateTasks(job, db);


//            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
//        }


//        #region Job List

//        #endregion

//        #region Job Form
//        //[HttpGet]
//        //[Route("Job/Details/{co-jobId}")]
//        //public ActionResult Index(byte jcco, string jobId)
//        //{

//        //    using var db = new VPContext();
//        //    var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
//        //    var result = new JobFormViewModel(entity);

//        //    return View("../JC/Job/Forms/PartialIndex", result);
//        //}

//        [HttpGet]
//        public ActionResult JobPanel(byte jcco, string jobId)
//        {

//            using var db = new VPContext();
//            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
//            var result = new JobFormViewModel(entity);

//            return PartialView("../JC/Job/Forms/Panel", result);
//        }

//        [HttpGet]
//        public ActionResult JobPopupForm(byte jcco, string jobId)
//        {
//            using var db = new VPContext();
//            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
//            var result = new JobFormViewModel(entity);
           
//            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";
//            return View("../JC/Job/Forms/Index", result);
//        }

//        [HttpGet]
//        public ActionResult JobForm(byte jcco, string jobId)
//        {
//            using var db = new VPContext();
//            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
//            var result = new JobFormViewModel(entity);

//            return PartialView("../JC/Job/Forms/Form", result);
//        }

//        [HttpGet]
//        public ActionResult JobFormMth(byte jcco, string jobId, DateTime mth)
//        {
//            using var db = new VPContext();
//            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
//            var result = new JobFormViewModel(entity, mth);

//            return PartialView("../JC/Job/Forms/Form", result);
//        }

//        [HttpGet]
//        public ActionResult JobPanelMth(byte jcco, string jobId, DateTime mth)
//        {
//            using var db = new VPContext();
//            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
//            var result = new JobFormViewModel(entity, mth);

//            return PartialView("../JC/Job/Forms/Panel", result);
//        }

//        [HttpGet]
//        public JsonResult JobFormValidate(JobFormViewModel model)
//        {
//            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
//        }
//        #endregion
        
//        #region Job Info Form
//        [HttpGet]
//        public ActionResult JobInfoPanel(byte jcco, string jobId)
//        {
//            using var db = new VPContext();
//            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
//            var result = new JobInfoViewModel(entity);

//            return PartialView("../JC/Job/Forms/Info/Panel", result);
//        }

//        [HttpGet]
//        public ActionResult JobInfoForm(byte jcco, string jobId)
//        {
//            using var db = new VPContext();
//            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
//            var result = new JobInfoViewModel(entity);

//            return PartialView("../JC/Job/Forms/Info/Form", result);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult JobInfoUpdate(JobInfoViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                using var db = new VPContext();
//                model = Repository.VP.JC.JobRepository.ProcessUpdate(model, db);
//                db.SaveChanges(ModelState);
//            }
//            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
//        }

//        [HttpGet]
//        public JsonResult JobInfoValidate(JobInfoViewModel model)
//        {
//            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
//        }
//        #endregion

//        #region Job AP Table
//        [HttpGet]
//        public ActionResult JobAPInvoiceTable(byte jcco, string jobId)
//        {
//            using var db = new VPContext();
//            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
//            var result = new Models.Views.AP.Invoice.InvoiceLineListViewModel(entity);

//            return PartialView("../JC/Job/Forms/AP/Table", result);
//        }
//        #endregion

//        #region Job AR Table
//        [HttpGet]
//        public ActionResult JobARInvoiceTable(byte jcco, string jobId)
//        {
//            using var db = new VPContext();
//            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
//            var result = new Models.Views.AR.Invoice.InvoiceListViewModel(entity);

//            return PartialView("../JC/Job/Forms/AR/Table", result);
//        }
//        #endregion

//        #region Job Cost Table
//        [HttpGet]
//        public ActionResult JobCostTable(byte jcco, string jobId)
//        {
//            using var db = new VPContext();
//            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
//            var result = new Models.Views.JC.Job.Cost.JobCostListViewModel(entity);

//            return PartialView("../JC/Job/Forms/Cost/Table", result);
//        }
//        #endregion

//        #region Job PO Table
//        [HttpGet]
//        public ActionResult JobPOTable(byte jcco, string jobId)
//        {
//            using var db = new VPContext();
//            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
//            var result = new Models.Views.Purchase.Order.PurchaseOrderItemListViewModel(entity);

//            return PartialView("../JC/Job/Forms/PO/Table", result);
//        }
//        #endregion

//        #region Job Progress Table
//        [HttpGet]
//        public ActionResult JobProgressTable(byte jcco, string jobId)
//        {
//            using var db = new VPContext();
//            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
//            var result = new Models.Views.JC.Job.JobProductionBgtVsActListViewModel(entity);

//            return PartialView("../JC/Job/Forms/PO/Table", result);
//        }
//        #endregion

//        #region Job IS Form
//        [HttpGet]
//        public ActionResult JobISTable(byte jcco, string jobId)
//        {
//            using var db = new VPContext();
//            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
//            var result = new Models.Views.JC.Job.Forms.JobISFormViewModel(entity);

//            return PartialView("../JC/Job/Forms/IS/Table", result);
//        }

//        [HttpGet]
//        public ActionResult JobISMonthTable(byte jcco, string jobId, DateTime mth)
//        {
//            using var db = new VPContext();
//            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
//            var result = new Models.Views.JC.Job.Forms.JobISFormViewModel(entity, mth);

//            return PartialView("../JC/Job/Forms/IS/Table", result);
//        }
//        #endregion
//    }
//}