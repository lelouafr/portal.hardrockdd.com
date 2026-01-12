using portal.Models.Views.Payroll.Leave;
using portal.Models.Views.Purchase.Request;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View
{
    public class LeaveRequestRejectController : BaseController
    {
        [HttpGet]
        public ActionResult Index(byte prco, int requestId)
        {
            using var db =  new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var result = db.LeaveRequests.Where(f => f.PRCo == prco && f.RequestId == requestId).FirstOrDefault();
            var model = new LeaveRequestRejectViewModel(result);

            return PartialView("../LeaveRequest/Reject/Index", model);
        }

        [HttpGet]
        public ActionResult Validate(LeaveRequestRejectViewModel model)
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