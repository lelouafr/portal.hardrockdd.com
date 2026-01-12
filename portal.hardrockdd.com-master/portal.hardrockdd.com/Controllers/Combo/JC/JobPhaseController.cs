using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.JV
{
    [Authorize]
    public class JobPhaseController : BaseController
    {
        //private readonly JobPhaseRepository repo = new JobPhaseRepository();

        //[HttpGet]
        //public JsonResult Combo(byte co, string jobId, string selected)
        //{
        //    using var repo = new JobPhaseRepository();
        //    var list = repo.GetJobPhases(co, jobId, co);

        //    var result = new List<SelectListItem>
        //    {
        //        new SelectListItem
        //        {
        //            Text = "Select Phase"
        //        }
        //    };
        //    result.AddRange(JobPhaseRepository.GetSelectList(list, selected));

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        [HttpGet]
        public JsonResult ProductionCombo(byte co, string jobId)
        {
            //using var repo = new JobPhaseRepository();
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var list = db.JobPhases
                        .Where(f => f.JCCo == co && 
                                    f.JobId == jobId && 
                                    f.PhaseGroupId == co && 
                                    f.PhaseType == "Production" && 
                                    f.ParentPhaseId == null)
                        .ToList();

            var selectList = list.Select(s => new SelectListItem
            {
                Value = s.PhaseId.ToString(AppCultureInfo.CInfo()),
                Text = s.Description
            }).ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Phase"
                }
            };
            result.AddRange(selectList);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}