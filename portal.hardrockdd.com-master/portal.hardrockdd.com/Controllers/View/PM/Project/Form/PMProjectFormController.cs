using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.PM.Project.Form;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.HR.Forms
{
    [ControllerAuthorize]
    public class PMProjectFormController : BaseController
    {
        #region Project List
        [HttpGet]
        [Route("PM/Project/Summary")]
        public ActionResult ListIndex()
        {
            var results = new Models.Views.PM.Project.Summary.ProjectListViewModel();
            return View("../PM/Project/Summary/Index", results);
        }

        [HttpGet]
        public ActionResult ListTable()
        {
            var results = new Models.Views.PM.Project.Summary.ProjectListViewModel();
            return PartialView("../PM/Project/Summary/List/Table", results);
        }

        [HttpGet]
        public ActionResult ListData()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);
            var results = new Models.Views.PM.Project.Summary.ProjectListViewModel(company);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }
        #endregion

        #region Project Index 
        [HttpGet]
        public ActionResult ProjectIndex(byte jcco,  string projectId)
        {

            using var db = new VPContext();
            FormViewModel result = null;
            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == projectId);
            result = new FormViewModel(entity);

            return View("../PM/Project/Forms/Index", result);
        }

        [HttpGet]
        public ActionResult ProjectPanel(byte co,  string projectId)
        {

            using var db = new VPContext();
            FormViewModel result = null;
            //BuildCrews(co, projectId);
            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == co && f.JobId == projectId);                
            result = new FormViewModel(entity);

            return PartialView("../PM/Project/Forms/Panel", result);
        }

        [HttpGet]
        public ActionResult ProjectForm(byte co,  string projectId)
        {
            using var db = new VPContext();
            FormViewModel result = null;
            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == co && f.JobId == projectId);
            result = new FormViewModel(entity);
            return PartialView("../PM/Project/Forms/Form", result);
        }


        [HttpGet]
        public ActionResult ProjectPopupForm(byte jcco,  string projectId)
        {

            using var db = new VPContext();
            FormViewModel result = null;
            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == projectId);
            result = new FormViewModel(entity);

            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";
            return View("./PM/Project/Forms/Index", result);
        }
        #endregion
        
        #region Info
        [HttpGet]
        public ActionResult InfoForm(byte jcco, string jobId)
        {
            using var db = new VPContext();
            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == jobId);
            var result = new ProjectInfoViewModel(entity);

            return PartialView("../JC/Job/Forms/Info/Form", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InfoUpdate(ProjectInfoViewModel model)
        {
            if (model == null)
                model = new ProjectInfoViewModel();

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult InfoValidate(ProjectInfoViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        
        #region Crew
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrewUpdate(ProjectCrewViewModel model)
        {
            if (ModelState.IsValid && model != null)
            {
                using var db = new VPContext();
                var entity = db.PMProjectCrews.FirstOrDefault(f => f.PMCo == model.PMCo && f.ProjectId == model.ProjectId && f.SeqId == model.SeqId);
                if (entity != null)
                {
                    entity.CrewId = model.CrewId;
                    entity.StartDate = model.StartDate;
                }

                //Repository.VP.PM.ProjectGanttRepository.AssignCrews(entity.Project, db);
                //Repository.VP.PM.ProjectGanttRepository.LinkJobs(entity.Project, db);

                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult CrewValidate(ProjectCrewViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CrewTable(byte jcco,  string projectId)
        {
            using var db = new VPContext();
            ProjectCrewListViewModel result = null;
            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == projectId);
            result = new ProjectCrewListViewModel(entity);
            return PartialView("../PM/Project/Forms/ProjectCrew/Table", result);
        }

        [HttpGet]
        public PartialViewResult CrewAdd(byte jcco,  string projectId)
        {
            using var db = new VPContext();
            ProjectCrewViewModel result = null;
            var entity = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == projectId);
            var crew = new PMProjectCrew()
            {
                PMCo = entity.JCCo,
                ProjectId = entity.JobId,
                SeqId = entity.Crews.DefaultIfEmpty().Max(f => f == null ? 0 : f.SeqId) + 1,
                StartDate = entity.StartDate,
                AutoSchedule = true,
            };

            entity.Crews.Add(crew);

            result = new ProjectCrewViewModel(crew);

            db.SaveChanges(ModelState);
            ViewBag.IsTable = true;
            return PartialView("../PM/Project/Forms/ProjectCrew/TableRow", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrewDelete(byte jcco,  string projectId, int seqId)
        {
            using var db = new VPContext();
            var entity = db.PMProjectCrews.FirstOrDefault(f => f.PMCo == jcco && f.ProjectId == projectId && f.SeqId == seqId);
            db.PMProjectCrews.Remove(entity);

            var job = db.Jobs.FirstOrDefault(f => f.JCCo == jcco && f.JobId == projectId);
            //Repository.VP.PM.ProjectGanttRepository.AssignCrews(job, db);
            //Repository.VP.PM.ProjectGanttRepository.LinkJobs(job, db);

            db.SaveChanges(ModelState);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        
        #region Job
        //JobSummaryListViewModel

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult JobPhaseSummaryUpdate(JobPhaseSummaryViewModel model)
        {
            if (model != null)
            {
                using var db = new VPContext();
                var entity = db.Jobs.FirstOrDefault(f => f.JCCo == model.JCCo && f.JobId == model.JobId);
                if (entity != null)
                {
                    var statusChanged = entity.Status != model.StatusId;
                    entity.Status = model.StatusId;
                    entity.IsRigUpComplete = model.IsRigUpComplete;

                    var isPilotCompleteChanged = entity.IsPilotComplete != model.IsPilotComplete;
                    var isReam1CompleteChanged = entity.IsReam1Complete != model.IsReam1Complete;
                    var isReam2CompleteChanged = entity.IsReam2Complete != model.IsReam2Complete;
                    var isReam3CompleteChanged = entity.IsReam3Complete != model.IsReam3Complete;
                    var isReam4CompleteChanged = entity.IsReam4Complete != model.IsReam4Complete;
                    var isReam5CompleteChanged = entity.IsReam5Complete != model.IsReam5Complete;
                    var isPullPipeCompleteChanged = entity.IsPullPipeComplete != model.IsPullPipeComplete;


                    entity.IsPilotComplete = model.IsPilotComplete;
                    if (!isPilotCompleteChanged)
                        entity.PilotPercentComplete = model.PilotPercentComplete;

                    entity.IsReam1Complete = model.IsReam1Complete;
                    if (!isReam1CompleteChanged)
                        entity.Ream1PercentComplete = model.Ream1PercentComplete;

                    entity.IsReam2Complete = model.IsReam2Complete;
                    if (!isReam2CompleteChanged)
                        entity.Ream2PercentComplete = model.Ream2PercentComplete;

                    entity.IsReam3Complete = model.IsReam3Complete;
                    if (!isReam3CompleteChanged)
                        entity.Ream3PercentComplete = model.Ream3PercentComplete;

                    entity.IsReam4Complete = model.IsReam4Complete;
                    if (!isReam4CompleteChanged)
                        entity.Ream4PercentComplete = model.Ream4PercentComplete;

                    entity.IsReam5Complete = model.IsReam5Complete;
                    if (!isReam5CompleteChanged)
                        entity.Ream5PercentComplete = model.Ream5PercentComplete;

                    entity.IsPullPipeComplete = model.IsPullPipeComplete;
                    if (!isPullPipeCompleteChanged)
                        entity.PullPipePercentComplete = model.PullPipePercentComplete;

                    entity.IsRigDownComplete = model.IsRigDownComplete;
                    if (statusChanged && entity.ParentJob != null)
                    {
                        entity.ParentJob.Status = entity.ParentJob.CalculateStatus();
                    }


                    entity.UpdateJobPhaseTracks();
                    db.SaveChanges(ModelState);
                }
                model = new JobPhaseSummaryViewModel(entity);

            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }
        #endregion
    }
}