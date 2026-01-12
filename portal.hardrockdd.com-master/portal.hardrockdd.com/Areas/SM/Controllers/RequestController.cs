using portal.Areas.SM.Models.Request;
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Areas.SM.Controllers
{
    [RouteArea("SM")]
    public class RequestController : portal.Controllers.BaseController
    {
        [HttpGet]
        [Route("SM/Requests/Open")]
        public ActionResult IndexOpen()
        {
            using var db = new VPContext();
            var list = db.SMRequestLines.Where(f => f.StatusId == (int)DB.SMRequestLineStatusEnum.Pending).ToList();
            var result = new RequestLineListViewModel(list);

            return View("Index", result);
        }

        [HttpGet]
        public PartialViewResult OpenTable()
        {
            using var db = new VPContext();
            var list = db.SMRequestLines.Where(f => f.StatusId == (int)DB.SMRequestLineStatusEnum.Pending).ToList();
            var result = new RequestLineListViewModel(list);

            return PartialView("List/_Table", result);
        }

        [HttpGet]
        public PartialViewResult RequestLinePanel(byte smco, int requestId, int lineId)
        {
            using var db = new VPContext();
            var obj = db.SMRequestLines.FirstOrDefault(f => f.SMCo == smco && f.RequestId == requestId && f.LineId == lineId);
            var result = new RequestLineViewModel(obj);

            return PartialView("Form/_Panel", result);
        }

        [HttpGet]
        public PartialViewResult RequestLineForm(byte smco, int requestId, int lineId)
        {
            using var db = new VPContext();
            var obj = db.SMRequestLines.FirstOrDefault(f => f.SMCo == smco && f.RequestId == requestId && f.LineId == lineId);
            var result = new RequestLineViewModel(obj);

            return PartialView("Form/_Form", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RequestLineUpdate(RequestLineViewModel model)
        {
            if (model == null)
                ModelState.AddModelError("", "Empty Model!");
            else
            {
                using var db = new VPContext();
                model = model.ProcessUpdate(db, ModelState);
            }

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }
    }
}