//using Newtonsoft.Json;
//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.Equipment;
//using portal.Models.Views.Equipment.Audit;
//using portal.Models.Views.JC.Job;
//using portal.Models.Views.Purchase.Order;
//using portal.Repository.VP.EM;
//using System;
//using System.Linq;
//using System.Web.Mvc;


//namespace portal.Controllers.View.Equipment
//{
//    [ControllerAuthorize]
//    public class EMAuditApprovalController : BaseController
//    {
//        [HttpGet]
//        [Route("Equipment/Audit/Approvals")]
//        public ActionResult Index()
//        {
//            var results = new EquipmentAuditListViewModel();
//            results.List.Add(new EquipmentAuditViewModel());
//            ViewBag.Title = "Audits Approvals";
//            ViewBag.DataController = "EMAuditApproval";
//            return View("../EM/Audit/Approval/Index", results);
//        }


//        [HttpGet]
//        public ActionResult Table()
//        {
//            var results = new EquipmentAuditListViewModel();
//            results.List.Add(new EquipmentAuditViewModel());
//            ViewBag.Title = "Audits Approvals";
//            ViewBag.DataController = "EMAuditApproval";
//            return PartialView("../EM/Audit/Approval/List/Table", results);
//        }

//        [HttpGet]
//        public ActionResult Data()//string statusId
//        {
//            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
//            var userId = StaticFunctions.GetUserId();
//            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
//            var results = new EquipmentAuditListViewModel(user, DB.EMAuditStatusEnum.Submitted);
//            var resultsApproved = new EquipmentAuditListViewModel(user, DB.EMAuditStatusEnum.Approved);
//            results.List.AddRange(resultsApproved.List);
//            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);
//            result.MaxJsonLength = int.MaxValue;
//            return result;
//        }

//    }
//}