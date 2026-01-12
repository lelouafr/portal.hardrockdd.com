using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.PM.Project.Schedule;
using portal.Repository.VP.PM;
using System;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.HR.Forms
{
    [ControllerAuthorize]
    public class PMProjectScheduleFormController : BaseController
    {
        [HttpGet]
        public ActionResult Index(byte? bdco, int? bidId, int? packageId, byte? jcco, string projectId)
        {

            using var db = new VPContext();
            FormViewModel result = null;
            packageId = packageId == 0 ? null : packageId;
            bidId = bidId == 0 ? null : bidId;
            if (bidId != null && packageId != null)
            {
                var entity = db.BidPackages.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId && f.PackageId == packageId);
                result = new FormViewModel(entity);
            }
            else if (!string.IsNullOrEmpty(projectId) && jcco != null)
            {
                var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == projectId);
                result = new FormViewModel(entity);
            }
            return View("../PM/Project/Schedule/Forms/Index", result);
        }

        [HttpGet]
        public ActionResult Panel(byte? bdco, int? bidId, int? packageId, byte? jcco, string projectId)
        {

            using var db = new VPContext();
            FormViewModel result = null;
            packageId = packageId == 0 ? null : packageId;
            bidId = bidId == 0 ? null : bidId;
            if (bidId != null && packageId != null)
            {
                var entity = db.BidPackages.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId && f.PackageId == packageId);
                result = new FormViewModel(entity);
            }
            else if (!string.IsNullOrEmpty(projectId) && jcco != null)
            {
                BuildCrews((byte)jcco, projectId);
                var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == projectId);                
                result = new FormViewModel(entity);
            }

            return PartialView("../PM/Project/Schedule/Forms/Panel", result);
        }

        private void BuildCrews(byte co, string projectId)
        {
            using var db = new VPContext();
            var project = db.Jobs.FirstOrDefault(f => f.JCCo == co && f.JobId == projectId);
            //ProjectGanttRepository.BuildCrews(project, db);

            db.SaveChanges();
        }

        [HttpGet]
        public ActionResult Form(byte? bdco, int? bidId, int? packageId, byte? jcco, string projectId)
        {
            using var db = new VPContext();
            FormViewModel result = null;
            packageId = packageId == 0 ? null : packageId;
            bidId = bidId == 0 ? null : bidId;
            if (bidId != null && packageId != null)
            {
                var entity = db.BidPackages.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId && f.PackageId == packageId);
                result = new FormViewModel(entity);
            }
            else if (!string.IsNullOrEmpty(projectId) && jcco != null)
            {

                BuildCrews((byte)jcco, projectId);
                var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == projectId);
                result = new FormViewModel(entity);
            }

            return PartialView("../PM/Project/Schedule/Forms/Form", result);
        }


        [HttpGet]
        public ActionResult PopupForm(byte? bdco, int? bidId, int? packageId, byte? jcco, string projectId)
        {

            using var db = new VPContext();
            FormViewModel result = null;
            packageId = packageId == 0 ? null : packageId;
            bidId = bidId == 0 ? null : bidId;
            if (bidId != null && packageId != null)
            {
                var entity = db.BidPackages.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId && f.PackageId == packageId);
                result = new FormViewModel(entity);
            }
            else if (!string.IsNullOrEmpty(projectId) && jcco != null)
            {
                var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == projectId);
                result = new FormViewModel(entity);
            }
            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";
            return View("../HR/Employee/Forms/Index", result);
        }

        #region TimeLine
        [HttpGet]
        public ActionResult TimeLineData(byte co, int? bidId, int? packageId, string projectId)
        {
            using var db = new VPContext();
            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == co && f.JobId == projectId);
            var result = new TimeLineListViewModel(entity);
            var timeLineResult = new
            {
                data = result.Events,
                collections = new { 
                    elements = result.Elements
                }
            };

            var jsonResult = Json(timeLineResult, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult TimeLineUpdate(JobTimeLineEvent model)
        {
            if (model != null)
            {
                using var db = new VPContext();
                var entity = db.Jobs.FirstOrDefault(f => f.JCCo == model.Co && f.JobId == model.JobId );
                if (entity != null)
                {
                    entity.CrewId = model.section_id == "null" ? null : model.section_id;
                    entity.StartDate = model.startdate;

                    db.SaveChanges(ModelState);
                }
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }


        [HttpGet]
        public ActionResult TimeLineForm(byte bdco, int? bidId, int? packageId, string projectId)
        {
            using var db = new VPContext();
            FormViewModel result = null;
            packageId = packageId == 0 ? null : packageId;
            bidId = bidId == 0 ? null : bidId;
            if (bidId != null && packageId != null)
            {
                var entity = db.BidPackages.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId && f.PackageId == packageId);
                result = new FormViewModel(entity);
            }
            else if (!string.IsNullOrEmpty(projectId))
            {

                BuildCrews(bdco, projectId);
                var entity = db.Jobs.FirstOrDefault(f => f.JCCo == bdco && f.JobId == projectId);
                result = new FormViewModel(entity);
            }

            return PartialView("../PM/Project/Schedule/Forms/TimeLine/Form", result);
        }
        #endregion
        #region Gantt
        [HttpGet]
        public ActionResult GanttData(byte? bdco, int? bidId, int? packageId, byte? jcco, string projectId)
        {
            using var db = new VPContext();
            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == projectId);

            //if (entity.ProjectGanttTask == null)
            //{
            //    Repository.VP.PM.ProjectGanttRepository.GenerateTasks(entity, db);
            //    //db.SaveChanges();
            //}
            //foreach (var job in entity.SubJobs)
            //{
            //    if (job.ProjectGanttTask == null)
            //    {
            //        Repository.VP.PM.ProjectGanttRepository.GenerateTasks(job, db);
                    
            //    }
            //}
            //db.SaveChanges();
            var result = new GanttListViewModel(entity);
            foreach (var task in result.Tasks)
            {
                task.render = "";
                if (task.type.Contains("project"))
                {
                    task.type = "project";
                }

                if (task.type.Contains("task"))
                {
                    task.type = "task";
                }
                if (task.sourcetype == "Project")
                {
                    task.open = true;
                }
            }

            var timeLineResult = new
            {
                data = result.Tasks.OrderBy(o => o.startdate).ToList(),
                collections = new
                {
                    links = result.Links,
                    resource = result.Resources,
                }
            };

            var jsonResult = Json(timeLineResult, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        [HttpGet]
        public ActionResult GanttForm(byte? bdco, int? bidId, int? packageId, byte? jcco, string projectId)
        {
            using var db = new VPContext();
            FormViewModel result = null;
            packageId = packageId == 0 ? null : packageId;
            bidId = bidId == 0 ? null : bidId;
            if (bidId != null && packageId != null)
            {
                var entity = db.BidPackages.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId && f.PackageId == packageId);
                result = new FormViewModel(entity);
            }
            else if (!string.IsNullOrEmpty(projectId) && jcco != null)
            {

                BuildCrews((byte)jcco, projectId);
                var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == projectId);
                result = new FormViewModel(entity);
            }

            return PartialView("../PM/Project/Schedule/Forms/Gantt/Form", result);
        }

        //[HttpPost]
        //public ActionResult GanttUpdate(JobGanttEvent model)
        //{
        //    if (model != null)
        //    {
        //        using var db = new VPContext();
        //        var entity = db.Jobs.FirstOrDefault(f => f.JCCo == model.Co && f.JobId == model.JobId);
        //        if (entity != null)
        //        {
        //            entity.CrewId = model.section_id == "null" ? null : model.section_id;
        //            entity.StartDate = model.startdate;

        //            db.SaveChanges(ModelState);
        //        }
        //    }
        //    return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        //}
        #endregion
    }
}