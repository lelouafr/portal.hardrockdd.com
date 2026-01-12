using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Equipment;
using portal.Models.Views.Equipment.Audit;
using portal.Repository.VP.EM;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.Equipment
{
    [ControllerAuthorize]
    public class EMAuditController : BaseController
    {

        #region Audit List
        [HttpGet]
        [Route("Equipment/Audit/All")]
        public ActionResult Index()
        {
            var results = new EquipmentAuditListViewModel();
            results.List.Add(new EquipmentAuditViewModel());
            ViewBag.Title = "All Audits";
            ViewBag.DataController = "EMAudit";
            return View("../EM/Audit/Summary/Index", results);
        }


        [HttpGet]
        public ActionResult Table()
        {
            ViewBag.DataController = "EMAudit";
            var results = new EquipmentAuditListViewModel();
            results.List.Add(new EquipmentAuditViewModel());
            return PartialView("../EM/Audit/Summary/List/Table", results);
        }

        [HttpGet]
        public ActionResult Data()//string statusId
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var emp = user.Employee.FirstOrDefault();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emp.HRCo);
            var results = new EquipmentAuditListViewModel(company);
            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }
        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(byte emco, int auditId, byte status)
        {
            using var db = new VPContext();
            var updObj = db.EMAudits.FirstOrDefault(f => f.EMCo == emco && f.AuditId == auditId);
            var EMParms = db.EMCompanyParms.FirstOrDefault(f => f.EMCo == updObj.EMCo);


            var model = new EquipmentAuditFormViewModel(updObj);
            if ((DB.EMAuditStatusEnum)status == DB.EMAuditStatusEnum.Canceled)
            {
                EquipmentAuditRepository.UpdateStatus(updObj, status, db);
                db.SaveChanges(ModelState);
            }
            else
            {
                ModelState.Clear();
                TryValidateModelRecursive(model);
                if (ModelState.IsValid)
                {
                    EquipmentAuditRepository.UpdateStatus(updObj, status, db);
                    db.SaveChanges(ModelState);
                }
            }
            model = new EquipmentAuditFormViewModel(updObj);

            if (updObj.Status == DB.EMAuditStatusEnum.Processed && 
                updObj.AuditForm != DB.EMAuditFormEnum.Inventory && 
                updObj.EMParameter.AuditAutoBatch == "Y" &&
                ModelState.IsValid)
            {
                var batchLines = updObj.Lines.Any(f => (f.Action == DB.EMAuditLineActionEnum.Add || f.Action == DB.EMAuditLineActionEnum.Update) &&
                                                       f.Completed == true &&
                                                       f.MeterType != DB.EMMeterTypeEnum.None);

                if (batchLines)
                {
                    using var dbBatch = new VPContext();
                    var audit = dbBatch.EMAudits.FirstOrDefault(f => f.EMCo == updObj.EMCo && f.AuditId == updObj.AuditId);
                    if (audit.Batch != null)
                    {
                        audit.Batch.ValidateBatch();
                        if (audit.Batch.StatusEnum == DB.BatchStatusEnum.ValidationOK)
                        {
                            audit.Batch.PostBatch();
                        }
                    }
                    else
                    {
                        audit.Status = DB.EMAuditStatusEnum.Approved;
                    }
                }
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public ActionResult Create()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var emp = user.Employee.FirstOrDefault();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emp.HRCo);


            var results = new EquipmentAuditCreateViewModel(company);
            return PartialView("../EM/Audit/Add/Index", results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EquipmentAuditCreateViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);
            model.Validate(ModelState);
            if (ModelState.IsValid)
            {
                using var db = new VPContext();
                var audit = EquipmentAuditRepository.Create(model, db);
                db.EMAudits.Add(audit);
                db.SaveChanges(ModelState);
                var url = Url.Action("Index", "EMAuditForm", new { Area = "", emco = audit.EMCo, auditId = audit.AuditId });

                return Json(new { success = ModelState.IsValidJson(), url });
            }

            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CreateValidate(EquipmentAuditCreateViewModel model)
        {

            model.Validate(ModelState);
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Reject(byte emco, int auditId)
        {
            using var db = new VPContext();
            var entity = db.EMAudits.First(f => f.EMCo == emco && f.AuditId == auditId);
            var results = new EquipmentAuditRejectViewModel(entity);
            return PartialView("../EM/Audit/Reject/Index", results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reject(EquipmentAuditRejectViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);
            if (ModelState.IsValid)
            {
                using var db = new VPContext();
                var audit = EquipmentAuditRepository.Reject(model, db);
                
                db.SaveChanges(ModelState);
                return Json(new { success = ModelState.IsValidJson() });
            }

            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(byte emco, int auditId)
        {
            using var db = new VPContext();
            var updObj = db.EMAudits.First(f => f.EMCo == emco && f.AuditId == auditId);
            if (updObj != null)
            {
                updObj.Status = DB.EMAuditStatusEnum.Canceled;
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson() });
        }


        [HttpGet]
        public ActionResult CreateEquipmentList(EquipmentAuditCreateViewModel model)
        {
            var result = new EquipmentListViewModel();
            if (ModelState.IsValid)
            {
                result = new EquipmentListViewModel(model);                
            }
            return PartialView("../EM/Audit/Add/List/Table", result);
        }

        #region Audit Approval
        [HttpGet]
        [Route("Equipment/Audit/Approvals")]
        public ActionResult ApprovalIndex()
        {
            var results = new EquipmentAuditListViewModel();
            results.List.Add(new EquipmentAuditViewModel());
            ViewBag.Title = "Audits Approvals";
            return View("../EM/Audit/Approval/Index", results);
        }


        [HttpGet]
        public ActionResult ApprovalTable()
        {
            var results = new EquipmentAuditListViewModel();
            results.List.Add(new EquipmentAuditViewModel());
            ViewBag.Title = "Audits Approvals";
            return PartialView("../EM/Audit/Approval/List/Table", results);
        }

        [HttpGet]
        public ActionResult ApprovalData()//string statusId
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var results = new EquipmentAuditListViewModel(user, DB.EMAuditStatusEnum.Submitted);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }
        #endregion

        #region Audit Process
        [HttpGet]
        [Route("Equipment/Audit/Process")]
        public ActionResult ProcessIndex()
        {
            var results = new EquipmentAuditListViewModel();
            results.List.Add(new EquipmentAuditViewModel());
            ViewBag.Title = "Audits Approvals";
            return View("../EM/Audit/Process/Index", results);
        }

        [HttpGet]
        public ActionResult ProcessTable()
        {
            var results = new EquipmentAuditListViewModel();
            results.List.Add(new EquipmentAuditViewModel());
            ViewBag.Title = "Audits Approvals";
            return PartialView("../EM/Audit/Process/List/Table", results);
        }

        [HttpGet]
        public ActionResult ProcessData()//string statusId
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var results = new EquipmentAuditListViewModel(user, DB.EMAuditStatusEnum.Approved);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }
        #endregion
    }
}