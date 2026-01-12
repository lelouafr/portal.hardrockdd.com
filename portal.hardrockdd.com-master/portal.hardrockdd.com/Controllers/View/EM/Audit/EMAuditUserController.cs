using portal.Models.Views.Equipment.Audit;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.Equipment
{
    [ControllerAuthorize]
    public class EMAuditUserController : BaseController
    {
        [HttpGet]
        [Route("Equipment/Audit/Assigned")]
        public ActionResult Index()
        {
            var results = new EquipmentAuditListViewModel();
            results.List.Add(new EquipmentAuditViewModel());
            ViewBag.Title = "Your Audits";
            ViewBag.DataController = "EMAuditUser";
            return View("../EM/Audit/Summary/Index", results);
        }

        [HttpGet]
        public ActionResult Table()
        {
            var results = new EquipmentAuditListViewModel();
            results.List.Add(new EquipmentAuditViewModel());
            ViewBag.DataController = "EMAuditUser";
            return PartialView("../EM/Audit/Summary/List/Table", results);
        }

        [HttpGet]
        public ActionResult Data()//string statusId
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var results = new EquipmentAuditListViewModel(user);
            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }

    }
}