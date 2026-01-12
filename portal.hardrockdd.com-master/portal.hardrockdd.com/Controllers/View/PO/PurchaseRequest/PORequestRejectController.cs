using portal.Models.Views.Purchase.Request;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View
{
    //[AuthorizePosition(PositionCodes = "CFO,COO,PRES,FIN-AP,IT-DIR,FIN-APMGR,FIN-AR,FIN-CTRL,FLD-CL,HR-MGR,IT-DIR,OP-DM,OP-EQADM,OP-EQMGR,OP-GM,OP-SFMGR,SHP-MGR,SHP-SUP")]
    [ControllerAuthorize]
    public class PORequestRejectController : BaseController
    {
        [HttpGet]
        public ActionResult Index(byte poco, int requestId)
        {
            using var db =  new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var result = db.PORequests.FirstOrDefault(f => f.POCo == poco && f.RequestId == requestId);
            var model = new PORequestRejectViewModel(result);

            return PartialView("../PO/Request/Reject/Model",model);
        }

        [HttpGet]
        public ActionResult Validate(PORequestRejectViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);

            if (!ModelState.IsValid)
            {
                var errorModel = ModelState.ModelErrors();
                    return Json(new { success = ModelState.IsValidJson(), errorModel }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        }

    }
}