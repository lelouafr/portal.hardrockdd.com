using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.AP.Invoice;
using portal.Models.Views.Web;
using System;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.PurchasOrders
{
    //[AuthorizePosition(PositionCodes = "CFO,COO,PRES,IT-DIR,FIN-AP,FIN-APMGR,FIN-AR,FIN-ARMGR,FIN-CTRL,HR-MGR,OF-GA,OP-DM,OP-ENGD,OP-EQADM,OP-EQMGR,OP-GM,OP-PM,OP-SF,OP-SFMGR,OP-SLS,OP-SLSMGR,OP-SUP,SHP-MGR,SHP-SUP")]

    [ControllerAuthorize]
    public class ErrorLogController : BaseController
    {
        [HttpGet]
        [Route("ErrorLog")]
        public ActionResult Index()
        {
            using var db = new VPContext();
            var results = new ErrorLogListViewModel(DB.ErrorLogStatusEnum.Error);

            ViewBag.Controller = "ErrorLog";

            return View("../Administration/ErrorLog/Index", results);
        }

        [HttpGet]
        public ActionResult Table(DB.ErrorLogStatusEnum status)
        {
            using var db = new VPContext();
            var results = new ErrorLogListViewModel(db, status);

            ViewBag.Controller = "ErrorLog";

            return PartialView("../Administration/ErrorLog/Table", results);
        }
        
        [HttpGet]
        public ActionResult Form(int errorId)
        {
            using var db = new VPContext();
            var result = db.ErrorLogs.FirstOrDefault(f => f.ErrorId == errorId);
            var model = new ErrorLogViewModel(result);
            ViewBag.ViewOnly = true;
            return PartialView("../Administration/ErrorLog/Form/PartialIndex", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FormUpdate(ErrorLogViewModel model)
        {
            using var db = new VPContext();
            var updObj = db.ErrorLogs.FirstOrDefault(f => f.ErrorId == model.ErrorId);
            var updList = db.ErrorLogs.Where(f => f.StatusCode == (int)DB.ErrorLogStatusEnum.Error &&
                                                  f.Controller == updObj.Controller &&
                                                  f.Action == updObj.Action &&
                                                  f.ErrorId != model.ErrorId &&
                                                  f.ExceptionMessage == updObj.ExceptionMessage
                                                  //&&
                                                  //f.UrlReferrer == updObj.UrlReferrer
                                                  ).ToList();
            if (updObj != null)
            {
                if (model.Fixed == true)
                {
                    updObj.StatusCode = (int)DB.ErrorLogStatusEnum.Fixed;
                    updList.ForEach(e => e.StatusCode = updObj.StatusCode);
                }
                db.SaveChanges(ModelState);
            }
            var result = new ErrorLogViewModel(updObj);
            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public ActionResult Data(DB.ErrorLogStatusEnum status)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var results = new ErrorLogListViewModel(db, status);

            JsonResult result = Json(new
            {
                data = results.List.ToArray()
            }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }
    }
}