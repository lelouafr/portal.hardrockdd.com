using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Job.Controllers
{
    [RouteArea("Job")]
    public class JobController : portal.Controllers.BaseController
    {
        #region Jobs
        [HttpGet]
        [Route("All/Jobs")]
        public ActionResult Index()
        {
            using var db = new VPContext();
            var company = db.GetCurrentCompany();

            var results = new Models.Job.JobListViewModel
            {
                JCCo = company.JCCo ?? company.HQCo
            };
            ViewBag.DataController = "Job";
            ViewBag.TableAction = "Table";
            ViewBag.DataAction = "Data";
            ViewBag.Title = "Job Listing";
            return View("List/Index");
        }

        [HttpGet]
        public ActionResult Table()
        {
            using var db = new VPContext();
            var company = db.GetCurrentCompany();

            var results = new Models.Job.JobListViewModel
            {
                JCCo = company.JCCo ?? company.HQCo
            };
            ViewBag.DataController = "Job";
            ViewBag.TableAction = "Table";
            ViewBag.DataAction = "Data";
            return PartialView("List/_Table", results);
        }

        [HttpGet]
        public ActionResult Data()
        {
            using var db = new VPContext();
            var jobs = db.vJobs.OrderBy(f => f.JobId).ToList();
            var results = new Models.Job.JobListViewModel(jobs, db);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;
            return result;
        }
        #endregion

        #region Open Jobs
        [HttpGet]
        [Route("Open/Jobs")]
        public ActionResult OpenIndex()
        {
            using var db = new VPContext();
            var company = db.GetCurrentCompany();

            var results = new Models.Job.JobListViewModel
            {
                JCCo = company.JCCo ?? company.HQCo
            };
            ViewBag.DataController = "Job";
            ViewBag.TableAction = "OpenTable";
            ViewBag.DataAction = "OpenData";
            ViewBag.Title = "Open Job Listing";
            return View("List/Index");
        }

        [HttpGet]
        public ActionResult OpenTable()
        {
            using var db = new VPContext();
            var company = db.GetCurrentCompany();

            var results = new Models.Job.JobListViewModel
            {
                JCCo = company.JCCo ?? company.HQCo
            };
            ViewBag.DataController = "Job";
            ViewBag.TableAction = "OpenTable";
            ViewBag.DataAction = "OpenData";
            return PartialView("List/_Table", results);
        }

        [HttpGet]
        public ActionResult OpenData()
        {
            using var db = new VPContext();
            var jobs = db.vJobs.Where(f => f.JobStatus == 1).OrderBy(f => f.JobId).ToList();
            var results = new Models.Job.JobListViewModel(jobs, db);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;
            return result;
        }
        #endregion

        #region Job Form

        [HttpGet]
        public ActionResult PopupForm(byte jcco, string jobId)
        {
            using var db = new VPContext();
            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
            var result = new Models.Job.FormViewModel(entity);

            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";
            return View("Form/Index", result);
        }

        [HttpGet]
        public ActionResult Panel(byte jcco, string jobId)
        {

            using var db = new VPContext();
            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
            var result = new Models.Job.FormViewModel(entity);

            return PartialView("Form/_Panel", result);
        }

        [HttpGet]
        public ActionResult Form(byte jcco, string jobId)
        {
            using var db = new VPContext();
            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
            var result = new Models.Job.FormViewModel(entity);

            return PartialView("Form/_Form", result);
        }


        [HttpGet]
        public ActionResult PanelMth(byte jcco, string jobId, DateTime mth)
        {

            using var db = new VPContext();
            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
            var result = new Models.Job.FormViewModel(entity, mth);

            return PartialView("Form/_Panel", result);
        }

        [HttpGet]
        public ActionResult FormMth(byte jcco, string jobId, DateTime mth)
        {
            using var db = new VPContext();
            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
            var result = new Models.Job.FormViewModel(entity, mth);

            return PartialView("Form/_Form", result);
        }

        [HttpGet]
        public JsonResult FormValidate(Models.Job.FormViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        
        #region Job Info Form
        [HttpGet]
        public ActionResult InfoForm(byte jcco, string jobId)
        {
            using var db = new VPContext();
            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
            var result = new Models.Job.InfoViewModel(entity);

            return PartialView("Forms/Info/_Form", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InfoUpdate(Models.Job.InfoViewModel model)
        {
            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult InfoValidate(Models.Job.InfoViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Job AP Table
        [HttpGet]
        public ActionResult APInvoiceTable(byte jcco, string jobId)
        {
            using var db = new VPContext();
            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
            var result = new AccountsPayable.Models.InvoiceLineListViewModel(entity);

            return PartialView("Form/AP/_Table", result);
        }
        #endregion

        #region Job AR Table
        [HttpGet]
        public ActionResult ARInvoiceTable(byte jcco, string jobId)
        {
            using var db = new VPContext();
            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
            var result = new AccountsReceivable.Models.InvoiceLineListViewModel(entity);

            return PartialView("Form/AR/_Table", result);
        }
        #endregion

        #region Job Cost Table
        [HttpGet]
        public ActionResult CostTable(byte jcco, string jobId)
        {
            using var db = new VPContext();
            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
            var result = new Models.Cost.ListViewModel(entity);

            return PartialView("Form/Cost/_Table", result);
        }
        #endregion

        #region Job PO Table
        [HttpGet]
        public ActionResult POTable(byte jcco, string jobId)
        {
            using var db = new VPContext();
            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
            var result = new PurchaseOrder.Models.ItemListViewModel(entity);

            return PartialView("Form/PO/_Table", result);
        }
        #endregion

        #region Job Progress Table
        [HttpGet]
        public ActionResult ProductionVariancePanel(byte jcco, string jobId)
        {
            using var db = new VPContext();
            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
            var result = new Models.Job.Report.ProductionVarianceListViewModel(entity);

            return PartialView("Form/ProductionVariance/_Panel", result);
        }

        [HttpGet]
        public ActionResult ProductionVarianceTable(byte jcco, string jobId)
        {
            using var db = new VPContext();
            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
            var result = new Models.Job.Report.ProductionVarianceListViewModel(entity);

            return PartialView("Form/ProductionVariance/_Table", result);
        }
        #endregion

        #region Job IS Form
        [HttpGet]
        public ActionResult ISTable(byte jcco, string jobId)
        {
            using var db = new VPContext();
            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
            var result = new Models.Job.Report.ISFormViewModel(entity);

            return PartialView("Form/IS/_Table", result);
        }

        [HttpGet]
        public ActionResult JobISMonthTable(byte jcco, string jobId, DateTime mth)
        {
            using var db = new VPContext();
            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
            var result = new Models.Job.Report.ISFormViewModel(entity, mth);

            return PartialView("Form/IS/_TableMth", result);
        }
        #endregion


        #region Reports
        [HttpGet]
        [Route("Report/GrossMargin")]
        public ActionResult GrossMarginIndex()
        {
            var current = DateTime.Now.AddMonths(-1);
            var mth = new DateTime(current.Year, current.Month, 1);
            var results = new Models.Job.Report.GrossMarginListViewModel(mth);

            return View("Reports/GM/Index", results);
        }

        [HttpGet]
        public ActionResult GrossMarginTable(DateTime mth)
        {
            var results = new Models.Job.Report.GrossMarginListViewModel(mth);

            return PartialView("Reports/GM/_Table", results);
        }
        #endregion
    }
}