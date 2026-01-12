using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Equipment.Audit;
using System;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.Equipment
{
    [ControllerAuthorize]
    public class EMAuditInventoryFormController : BaseController
    {
        [HttpGet]
        [Route("Equipment/Audit/Inventory/{emco}-{auditId}")]
        public ActionResult Index(byte emco, int auditId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var entity = db.EMAudits.First(f => f.EMCo == emco && f.AuditId == auditId);
            var results = new EquipmentAuditInventoryFormViewModel(entity);
            ViewBag.ViewOnly = results.Action.Access == DB.SessionAccess.Edit ? "False" : "True";
            return View("../EM/Audit/Types/Inventory/Index", results);
        }
        [HttpGet]
        public ActionResult Form(byte emco, int auditId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var entity = db.EMAudits.First(f => f.EMCo == emco && f.AuditId == auditId);
            var results = new EquipmentAuditInventoryFormViewModel(entity);
            ViewBag.ViewOnly = results.Action.Access == DB.SessionAccess.Edit ? "False" : "True";
            return PartialView("../EM/Audit/Types/Inventory/Form", results);
        }

        [HttpGet]
        public ActionResult Table(byte emco, int auditId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var entity = db.EMAudits.First(f => f.EMCo == emco && f.AuditId == auditId);
            var results = new EquipmentAuditInventoryFormViewModel(entity);
            ViewBag.ViewOnly = results.Action.Access == DB.SessionAccess.Edit ? "False" : "True";
            return PartialView("../EM/Audit/Types/Inventory/List/Table", results.Lines);
        }

        [HttpGet]
        public ActionResult Add(byte emco, int auditId)
        {
            using var db = new VPContext();
            var audit = db.EMAudits.First(f => f.EMCo == emco && f.AuditId == auditId);
            var line = new EMAuditLine();
            line.EMCo = audit.EMCo;
            line.AuditId = audit.AuditId;
            line.SeqId = audit.Lines.DefaultIfEmpty().Max(f => f == null ? 0 : f.SeqId) + 1;
            line.ActionId = (byte)DB.EMAuditLineActionEnum.Add;
            
            
            audit.Lines.Add(line);
            db.SaveChanges(ModelState);
            var results = new EquipmentAuditInventoryLineViewModel(line);           
            return PartialView("../EM/Audit/Types/Inventory/List/Row/TableRow", results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(EquipmentAuditInventoryLineViewModel model)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            //using var repo = new DailyJobEmployeeRepository();
            ////model.Perdiem ??= DB.PerdiemEnum.No;
            //var result = repo.ProcessUpdate(model, ModelState);
            ////var result = repo.GetDailyJobEmployee(model.EMCo, model.TicketId, model.LineNum);

            using var db = new VPContext();
            var updObj = db.EMAuditLines.FirstOrDefault(f => f.EMCo == model.EMCo && f.AuditId == model.AuditId && f.SeqId == model.SeqId);
            if (updObj != null)
            {
                if (updObj.ActionId == (byte)DB.EMAuditLineActionEnum.Add)
                {
                    model.ActionId = DB.EMAuditLineActionEnum.Add;
                }
                if (updObj.ActionId != (byte)DB.EMAuditLineActionEnum.Add && model.ActionId == DB.EMAuditLineActionEnum.Add)
                {
                    model.ActionId = (DB.EMAuditLineActionEnum)updObj.ActionId;
                }

                if (model.ActionId == DB.EMAuditLineActionEnum.Transfer)
                {
                    switch ((DB.EMAuditTypeEnum)updObj.Audit.AuditTypeId)
                    {
                        case DB.EMAuditTypeEnum.CrewAudit:
                            updObj.ToCrewId = model.ToCrewId;
                            break;
                        case DB.EMAuditTypeEnum.EmployeeAudit:
                            updObj.ToEmployeeId = model.ToEmployeeId;
                            break;
                        case DB.EMAuditTypeEnum.LocationAudit:
                            updObj.ToLocationId = model.ToLocationId;
                            break;
                        default:
                            break;
                    }
                    //updObj.ToLocationId = model.ToLocationId;
                }
                else
                {
                    updObj.ToCrewId = null;
                    updObj.ToLocationId = null;
                    updObj.ToEmployeeId = null;
                    switch ((DB.EMAuditTypeEnum)updObj.Audit.AuditTypeId)
                    {
                        case DB.EMAuditTypeEnum.CrewAudit:
                            updObj.ToCrewId = updObj.Audit.ParmCrewId;
                            break;
                        case DB.EMAuditTypeEnum.EmployeeAudit:
                            updObj.ToEmployeeId = updObj.Audit.ParmEmployeeId;
                            break;
                        case DB.EMAuditTypeEnum.LocationAudit:
                            updObj.ToLocationId = updObj.Audit.ParmLocationId;
                            break;
                        default:
                            break;
                    }
                }
                if (model.ActionId == DB.EMAuditLineActionEnum.Add)
                {
                    if (updObj.EquipmentId != model.EquipmentId)
                    {
                        var eqp = db.Equipments.FirstOrDefault(f => f.EMCo == updObj.EMCo && f.EquipmentId == model.EquipmentId);
                        updObj.MeterTypeId = eqp?.MeterTypeId;
                    }
                    updObj.EquipmentId = model.EquipmentId;
                }
                updObj.Completed = model.Completed;
                updObj.ActionId = (byte)model.ActionId;
                updObj.Comments = model.Comments;
                if (updObj.Audit.Status == DB.EMAuditStatusEnum.New)
                {
                    updObj.Audit.Status = DB.EMAuditStatusEnum.Started;
                }
                db.SaveChanges(ModelState);
            }
            var result = new EquipmentAuditLineViewModel(updObj);

            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(byte emco, int auditId, int seqId)
        {
            using var db = new VPContext();
            var updObj = db.EMAuditLines.First(f => f.EMCo == emco && f.AuditId == auditId && f.SeqId == seqId);
            if (updObj != null)
            {
                db.EMAuditLines.Remove(updObj);
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson()});
        }

        [HttpGet]
        public JsonResult Validate(EquipmentAuditInventoryLineViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            ModelState.Clear();
            TryValidateModel(model);
            model.Validate(ModelState);
            
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
    }
}