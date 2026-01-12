
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.PM.Project.Schedule;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.PM
{
    [ControllerAccess]
    [ControllerAuthorize]
    public class PMProjectGanttController : BaseController
    {
        [HttpGet]
        [Route("PM/Project/Gantt")]
        public ActionResult Index()
        {
            ViewBag.DataController = "PMProjectGantt";
            ViewBag.DataAction = "Data";
            return View("../PM/Project/Gantt/Index");
        }

        [HttpGet]
        public ActionResult Form()
        {
            ViewBag.DataController = "PMProjectGantt";
            ViewBag.DataAction = "Data";
            return View("../PM/Project/Gantt/Form");
        }

        [HttpGet]
        public ActionResult Data()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var projectList = db.Jobs
                .Where(f => f.JobTypeId == "7" && (f.StatusId == "0" || f.StatusId == "1" || f.StatusId == "2")).ToList();

            var result = new GanttListViewModel(projectList);
            
            //result.Links = new System.Collections.Generic.List<Models.Views.DHTMLX.Gantt.GanttLink>();
            foreach (var task in result.Tasks)
            {
                //else
                //{
                    if (task.type != null && task.type.Contains("project"))
                    {
                        task.type = "project";
                    }

                    if (task.type != null && task.type.Contains("task"))
                    {
                        task.type = "task";
                    }

                //}
                if (task.type == null)
                {
                    task.type = "task";
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
        public ActionResult PopupForm(byte gtco, long taskId, bool isModel = false)
        {
            using var db = new VPContext();
            var entity = db.PMProjectGanttTasks.FirstOrDefault(f => f.GTCo == gtco && f.KeyID == taskId);

            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";


            switch ((DB.PMGanttSourceTypeEnum)entity.TaskSource)
            {
                case DB.PMGanttSourceTypeEnum.Bid:
                    break;
                case DB.PMGanttSourceTypeEnum.Project:
                case DB.PMGanttSourceTypeEnum.Job:
                case DB.PMGanttSourceTypeEnum.Phase:
                    var job = db.Jobs.FirstOrDefault(f => f.GanttId == entity.GanttId);
                    if (isModel)
                        return RedirectToAction("PartialIndex", "JobForm", new { job.JCCo, job.JobId });
                    return RedirectToAction("PopupForm", "Job", new { Area = "Project", job.JCCo, job.JobId });
                case DB.PMGanttSourceTypeEnum.Ticket:
                case DB.PMGanttSourceTypeEnum.Daily:
                    var dailyTicket = db.DTJobPhases.FirstOrDefault(f => f.GanttId == entity.GanttId && f.TaskId == entity.TaskId)?.JobTicket?.DailyTicket;
                    if (isModel)
                        return RedirectToAction("PartialIndex", "DailyTicket", new { dailyTicket.DTCo, dailyTicket.TicketId, partialView = true });
                    return RedirectToAction("PopupForm", "DailyTicket", new { dailyTicket.DTCo, dailyTicket.TicketId, partialView = true });
                default:
                    break;
            }

            return View();
        }
    }
}