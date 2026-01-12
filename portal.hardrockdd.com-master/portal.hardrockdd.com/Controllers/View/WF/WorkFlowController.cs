using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.WF;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.PM
{
    [Authorize]
    public class WorkFlowController : BaseController
    {
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public JsonResult AddUser(WorkFlowUserViewModel model)
        //{
        //    using var db = new VPContext();
        //    var workFlow = db.WorkFlows.FirstOrDefault(f => f.WorkFlowId == model.WorkFlowId);
        //    workFlow.AddUser(model.AssignedTo);
        //    db.SaveChanges(ModelState);

        //    return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        //}

        [HttpGet]
        public ActionResult AddUser(byte wfco, int workFlowId, int seqId, string assignedUser)
        {
            using var db = new VPContext();
            var workFlow = db.WorkFlows.FirstOrDefault(f => f.WFCo == wfco && f.WorkFlowId == workFlowId);
            workFlow.AddUser(assignedUser);
            db.SaveChanges(ModelState);

            var user = workFlow.CurrentSequence().AssignedUsers.FirstOrDefault(f => f.AssignedTo == assignedUser);

            var result = new WorkFlowUserViewModel(user);

            ViewBag.tableRow = "True";
            return PartialView("../WF/AssignedUsers/TableRow", result);
        }
    }
}