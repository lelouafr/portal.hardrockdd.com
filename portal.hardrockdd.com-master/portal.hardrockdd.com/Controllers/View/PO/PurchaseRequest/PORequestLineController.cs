using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Purchase.Request;
using portal.Repository.VP.PO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Controllers.View.PurchasOrders
{
    [ControllerAuthorize]
    public class PORequestLineController : BaseController
    {

        [HttpGet]
        public ActionResult FormPanel(byte poco, int requestId, int lineId)
        {
            using var db = new VPContext();
            var result = db.PORequestLines.FirstOrDefault(f => f.POCo == poco && f.RequestId == requestId && f.LineId == lineId);
            var model = new PORequestLineViewModel(result);
            var formModel = new PORequstFormViewModel(result.Request);
            var userId = StaticFunctions.GetUserId();
            ViewBag.ViewOnly = formModel.Action.Access == DB.SessionAccess.Edit ? "False" : "True";
            return PartialView("../PO/Request/Form/Lines/Form/Panel", model);
        }

        [HttpGet]
        public ActionResult Form(byte poco, int requestId, int lineId)
        {
            using var db = new VPContext();
            var result = db.PORequestLines.FirstOrDefault(f => f.POCo == poco && f.RequestId == requestId && f.LineId == lineId);
            var model = new PORequestLineViewModel(result);
            var formModel = new PORequstFormViewModel(result.Request);
            var userId = StaticFunctions.GetUserId();
            ViewBag.ViewOnly = formModel.Action.Access == DB.SessionAccess.Edit ? "False" : "True";
            return PartialView("../PO/Request/Form/Lines/Form/Form", model);
        }


        [HttpGet]
        public ActionResult Panel(byte poco, int requestId)
        {
            using var db = new VPContext();
            var result = db.PORequests.FirstOrDefault(f => f.POCo == poco && f.RequestId == requestId);
            var model = new PORequestLineListViewModel(result);
            var formModel = new PORequstFormViewModel(result);
            var userId = StaticFunctions.GetUserId();
            ViewBag.ViewOnly = formModel.Action.Access == DB.SessionAccess.Edit ? "False" : "True";
            return PartialView("../PO/Request/Form/Lines/List/Panel", model);
        }

        [HttpGet]
        public ActionResult Table(byte poco, int requestId)
        {
            using var db = new VPContext();
            var result = db.PORequests.FirstOrDefault(f => f.POCo == poco && f.RequestId == requestId);
            var model = new PORequestLineListViewModel(result);
            var formModel = new PORequstFormViewModel(result);
            var userId = StaticFunctions.GetUserId();
            ViewBag.ViewOnly = formModel.Action.Access == DB.SessionAccess.Edit ? "False" : "True";
            return PartialView("../PO/Request/Form/Lines/List/Table", model);
        }


        [HttpGet]
        public PartialViewResult Add(byte poco, int requestId)
        {
            using var db = new VPContext();

            var request = db.PORequests.FirstOrDefault(f => f.POCo == poco && f.RequestId == requestId);
            PORequestLine result = null;
            if (request.Lines.Count > 0)
            {
                var lastLine = request.Lines.OrderByDescending(o => o.KeyID).FirstOrDefault();
                result = request.AddLine(lastLine);
            }
            else
            {
                result = request.AddLine();
            }
            db.SaveChanges(ModelState);

            var model = new PORequestLineViewModel(result);
            ViewBag.ViewOnly = request.WorkFlow.IsUserInWorkFlow(StaticFunctions.GetUserId()) ? "False" : "True";

            return PartialView("../PO/Request/Form/Lines/List/TableRow", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(byte poco, int requestId, int lineId)
        {
            using var db = new VPContext();
            var delObj = db.PORequestLines.FirstOrDefault(f => f.POCo == poco && f.RequestId == requestId && f.LineId == lineId);
            var model = new PORequestLineViewModel(delObj);
            if (delObj != null)
            {
                db.PORequestLines.Remove(delObj);
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson(), model });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(PORequestLineViewModel model)
        {
            using var db = new VPContext();

            var updObj = db.PORequestLines.FirstOrDefault(f => f.POCo == model.POCo && f.RequestId == model.RequestId && f.LineId == model.LineId);            
            var result = RequestLineRepository.ProcessUpdate(updObj, model);
            db.SaveChanges(ModelState);

            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult Validate(PORequestLineViewModel model)
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