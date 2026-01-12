
using portal.Models.Views.PM.Project.Schedule;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.PM
{
    [ControllerAccess]
    [ControllerAuthorize]
    public class PMProjectScheduleController : BaseController
    {
        [HttpGet]
        [Route("PM/Project/Schedule")]
        public ActionResult Index()
        {
            var results = new ScheduleListViewModel();
            results.List.Add(new ScheduleViewModel());
            results.Co = StaticFunctions.GetCurrentCompany().HQCo;

            ViewBag.DataController = "PMProjectSchedule";
            ViewBag.DataAction = "Data";
            return View("../PM/Project/Schedule/Index", results);
        }

        [HttpGet]
        public ActionResult Table()
        {
            var results = new ScheduleListViewModel();
            results.List.Add(new ScheduleViewModel());
            results.Co = StaticFunctions.GetCurrentCompany().HQCo;

            ViewBag.DataController = "PMProjectSchedule";
            ViewBag.DataAction = "Data";
            return PartialView("../PM/Project/Schedule/List/Table", results);
        }

        [HttpGet]
        public ActionResult Data()
        {
            using var db =  new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var jobTypeStr = ((int)DB.JCJobTypeEnum.Project).ToString();
            var jobs = db.Jobs.Where(f => f.JobTypeId == jobTypeStr).ToList().Where(f => f.Status == DB.JCJobStatusEnum.Open ||
                                                                                        f.Status == DB.JCJobStatusEnum.Scheduled ||
                                                                                        f.Status == DB.JCJobStatusEnum.InProgress).ToList();
            var bids = db.BidPackages.Where(f => f.Bid.tStatusId == (int)(DB.BidStatusEnum.PendingAward)).ToList();
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);
            var results = new ScheduleListViewModel(jobs, bids);
            
            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }


    }
}