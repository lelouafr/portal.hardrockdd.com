using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.Equipment
{
    [ControllerAuthorize]
    public class EMAuditFormController : BaseController
    {
        [HttpGet]
        [Route("Equipment/Audit/Form/{emco}-{auditId}")]
        public ActionResult Index(byte emco, int auditId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var entity = db.EMAudits.First(f => f.EMCo == emco && f.AuditId == auditId);

            return ((DB.EMAuditFormEnum)entity.AuditFormId) switch
            {
                DB.EMAuditFormEnum.Meter => RedirectToAction("Index", "EMAuditMeterForm", new { Area = "", emco, auditId }),
                DB.EMAuditFormEnum.Inventory => RedirectToAction("Index", "EMAuditInventoryForm", new { Area = "", emco, auditId }),
                _ => RedirectToAction("Index", "EMAuditMeterForm", new { Area = "", emco, auditId }),
            };
        }

        [HttpGet]
        public ActionResult Form(byte emco, int auditId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var entity = db.EMAudits.First(f => f.EMCo == emco && f.AuditId == auditId);

            return ((DB.EMAuditFormEnum)entity.AuditFormId) switch
            {
                DB.EMAuditFormEnum.Meter => RedirectToAction("Form", "EMAuditMeterForm", new { Area = "", emco, auditId }),
                DB.EMAuditFormEnum.Inventory => RedirectToAction("Form", "EMAuditInventoryForm", new { Area = "",emco, auditId }),
                _ => RedirectToAction("Form", "EMAuditMeterForm", new { Area = "",emco, auditId }),
            };
        }


        [HttpGet]
        public ActionResult EquipmentAuditList(byte prco, string crewId, int employeeId, int weekId)
        {
            using var db = new VPContext();
            var dates = new List<DateTime>();
            if (weekId == 0)
            {
                var dateFilter = DateTime.Now.Date;
                weekId = db.Calendars.FirstOrDefault(f => f.Date == dateFilter).Week ?? 0;
                dates = db.Calendars.Where(f => f.Week <= weekId && f.Week >= weekId - 4).Select(s => s.Date).ToList();
            }
            else
            {
                dates = db.Calendars.Where(f => f.Week == weekId).Select(s => s.Date).ToList();
            }
            var crewList = db.EMAudits.Where(f => f.CreatedOn >= dates.Min() && f.CreatedOn <= dates.Max() && f.ParmCrewId == crewId).ToList();
            var empList = db.EMAudits.Where(f => f.CreatedOn >= dates.Min() && f.CreatedOn <= dates.Max() && f.ParmEmployeeId == employeeId).ToList();
            var list = crewList;
                list.AddRange(empList);
            return PartialView("../EM/Audit/Report/TabList/Panel", list);
        }
    }
}