using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Equipment;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.Equipment
{
    [ControllerAuthorize]
    public class EquipmentLogController : BaseController
    {
        [HttpGet]
        [Route("Equipment/{equipmentId}-{emco}/Log")]
        public ActionResult Index(byte emco, string equipmentId)
        {
            using var db = new VPContext();
            var equipment = db.Equipments.FirstOrDefault(f => f.EMCo == emco && f.EquipmentId == equipmentId);
            var results = new EquipmentLogListViewModel(equipment);
            ViewBag.DataController = "EquipmentLog";
            ViewBag.DataAction = "Data";
            return View("../EM/Equipment/Log/Index", results);
        }

        [HttpGet]
        public ActionResult Table(byte emco, string equipmentId)
        {
            using var db = new VPContext();
            var equipment = db.Equipments.FirstOrDefault(f => f.EMCo == emco && f.EquipmentId == equipmentId);
            var results = new EquipmentLogListViewModel(equipment);
            ViewBag.DataController = "EquipmentLog";
            ViewBag.DataAction = "Data";

            return PartialView("../EM/Equipment/Log/List/Table", results);
        }

        [HttpGet]
        public ActionResult TableRow(byte emco, string equipmentId, int seqId)
        {
            using var db = new VPContext();
            var log = db.EquipmentLogs.FirstOrDefault(f => f.EMCo == emco && f.EquipmentId == equipmentId && f.SeqId == seqId);
            var results = new EquipmentLogViewModel(log);
            ViewBag.DataController = "EquipmentLog";
            ViewBag.DataAction = "Data";

            return PartialView("../EM/Equipment/Log/List/TableRow", results);
        }

        [HttpGet]
        public ActionResult Data(byte emco, string equipmentId)
        {
            using var db = new VPContext();
            var equipment = db.Equipments.FirstOrDefault(f => f.EMCo == emco && f.EquipmentId == equipmentId);
            var results = new EquipmentLogListViewModel(equipment);

            JsonResult result = Json(new
            {
                data = results.List.ToArray()
            }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }
    }
}