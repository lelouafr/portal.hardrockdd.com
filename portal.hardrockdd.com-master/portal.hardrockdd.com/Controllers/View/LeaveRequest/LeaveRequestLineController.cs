using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Payroll.Leave;
using portal.Repository.VP.PR;
using System;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View.PurchasOrders
{
    public class LeaveRequestLineController : BaseController
    {

        [HttpGet]
        public ActionResult FormPanel(byte prco, int requestId, int lineId)
        {
            using var db = new VPContext();
            var result = db.LeaveRequestLines.FirstOrDefault(f => f.PRCo == prco && f.RequestId == requestId && f.LineId == lineId);
            var model = new LeaveRequestLineViewModel(result);
            var formModel = new LeaveRequstFormViewModel(result.Request);
            var userId = StaticFunctions.GetUserId();
            ViewBag.ViewOnly = formModel.Action.Access == DB.SessionAccess.Edit ? "False" : "True";
            return PartialView("../LeaveRequest/Lines/Form/Panel", model);
        }
        [HttpGet]
        public ActionResult Form(byte prco, int requestId, int lineId)
        {
            using var db = new VPContext();
            var result = db.LeaveRequestLines.FirstOrDefault(f => f.PRCo == prco && f.RequestId == requestId && f.LineId == lineId);
            var model = new LeaveRequestLineViewModel(result);
            var formModel = new LeaveRequstFormViewModel(result.Request);
            var userId = StaticFunctions.GetUserId();
            ViewBag.ViewOnly = formModel.Action.Access == DB.SessionAccess.Edit ? "False" : "True";
            return PartialView("../LeaveRequest/Lines/Form/Form", model);
        }


        [HttpGet]
        public ActionResult Panel(byte prco, int requestId)
        {
            using var db = new VPContext();
            var result = db.LeaveRequests.FirstOrDefault(f => f.PRCo == prco && f.RequestId == requestId);
            var model = new LeaveRequestLineListViewModel(result);
            var formModel = new LeaveRequstFormViewModel(result);
            var userId = StaticFunctions.GetUserId();
            ViewBag.ViewOnly = formModel.Action.Access == DB.SessionAccess.Edit ? "False" : "True";
            return PartialView("../LeaveRequest/Lines/Panel", model);
        }

        [HttpGet]
        public ActionResult Table(byte prco, int requestId)
        {
            using var db = new VPContext();
            var result = db.LeaveRequests.FirstOrDefault(f => f.PRCo == prco && f.RequestId == requestId);
            var model = new LeaveRequestLineListViewModel(result);
            var formModel = new LeaveRequstFormViewModel(result);
            var userId = StaticFunctions.GetUserId();
            ViewBag.ViewOnly = formModel.Action.Access == DB.SessionAccess.Edit ? "False" : "True";
            return PartialView("../LeaveRequest/Lines/Table", model);
        }


        [HttpGet]
        public PartialViewResult Add(byte prco, int requestId)
        {
            using var db = new VPContext();

            var request = db.LeaveRequests.FirstOrDefault(f => f.PRCo == prco && f.RequestId == requestId);
            var result = RequestLineRepository.Init(request);
            result.LineId = RequestLineRepository.NextId(result);
            var userId = StaticFunctions.GetUserId();
            request.Lines.Add(result);
            db.SaveChanges(ModelState);

            var model = new LeaveRequestLineViewModel(result);

            ViewBag.ViewOnly = request.WorkFlows.Any(w => w.AssignedTo == userId && w.Status == w.Status && w.Active == "Y") ? "False" : "True";

            return PartialView("../LeaveRequest/Lines/TableRow", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(byte prco, int requestId, int lineId)
        {
            using var db = new VPContext();
            var delObj = db.LeaveRequestLines.FirstOrDefault(f => f.PRCo == prco && f.RequestId == requestId && f.LineId == lineId);
            var model = new LeaveRequestLineViewModel(delObj);
            if (delObj != null)
            {
                db.LeaveRequestLines.Remove(delObj);
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson(), model });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(LeaveRequestLineViewModel model)
        {   
            var result = RequestLineRepository.ProcessUpdate(model, ModelState);

            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            //var jsonResult = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult Validate(LeaveRequestLineViewModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            ModelState.Clear();
            TryValidateModel(model);
            model.Validate(ModelState);
            if (!ModelState.IsValid)
            {
                var errorModel = ModelState.ModelErrors();
                    return Json(new { success = ModelState.IsValidJson(), errorModel }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        }
    }
}