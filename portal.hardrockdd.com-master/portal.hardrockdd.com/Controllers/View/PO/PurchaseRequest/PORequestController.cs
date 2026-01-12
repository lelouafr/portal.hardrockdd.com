using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Purchase.Request;
using portal.Repository.VP.PO;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using portal.Code;

namespace portal.Controllers.View.PurchasOrders
{
    //[AuthorizePosition(PositionCodes = "CFO,COO,PRES,FIN-AP,IT-DIR,FIN-APMGR,FIN-AR,FIN-CTRL,FLD-CL,HR-MGR,IT-DIR,OP-DM,OP-EQADM,OP-EQMGR,OP-GM,OP-SFMGR,SHP-MGR,SHP-SUP")]
    [ControllerAuthorize]
    public class PORequestController : BaseController
    {

        [HttpGet] 
        public PartialViewResult Add()
        {
            using var db = new VPContext();
            var userId = StaticFunctions.GetUserId();
            //var result = db.PORequests.FirstOrDefault(f => f.CreatedBy == userId && f.Status == 0);
            var comp = StaticFunctions.GetCurrentCompany();

            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);
            var result = RequestRepository.Init(company);
            result.AddLine();
            db.PORequests.Add(result);
            db.SaveChanges(ModelState);

            var results = new PORequestSummaryViewModel(result);
            ViewBag.tableRow = "True";
            return PartialView("../PO/Request/Summary/List/TableRow", results);
        }

        [HttpGet]
        [Route("PO/Request/Create")]
        public ActionResult Create()
        {
            using var db = new VPContext();
            var userId = StaticFunctions.GetUserId();
            var result = db.PORequests.FirstOrDefault(f => f.CreatedBy == userId && f.tStatusId == 0);
            if (result == null)
            {
                var comp = StaticFunctions.GetCurrentCompany();
                var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);
                result = RequestRepository.Init(company);
                result.AddLine();
                db.PORequests.Add(result);
                db.SaveChanges(ModelState);
            }
            if (result.Lines.Count == 0)
            {
                result.AddLine();
                db.SaveChanges(ModelState);
            }
            return RedirectToAction("Index",  new { result.POCo, result.RequestId });
        }

        [HttpGet]
        [Route("PO/Request/{requestId}-{poco}")]
        public ActionResult Index(byte poco, int requestId)
        {
            using var db = new VPContext();
            var userId = StaticFunctions.GetUserId();
            var result = db.PORequests.FirstOrDefault(f => f.POCo == poco && f.RequestId == requestId);
            
            if (result.Lines.Count == 0)
            {
                var line = RequestLineRepository.Init(result);
                result.Lines.Add(line);
                db.SaveChanges(ModelState);
            }
            var model = new PORequstFormViewModel(result);
            ViewBag.ViewOnly = model.Action.Access == DB.SessionAccess.Edit ? "False" : "True";
            ViewBag.Partial = false;
            return View("../PO/Request/Form/Index", model);
        }

        [HttpGet]
        public ActionResult PartialIndex(byte poco, int requestId)
        {
            using var db = new VPContext();
            var userId = StaticFunctions.GetUserId();
            var result = db.PORequests.FirstOrDefault(f => f.POCo == poco && f.RequestId == requestId);

            if (result.Lines.Count == 0)
            {
                var line = RequestLineRepository.Init(result);
                result.Lines.Add(line);
                db.SaveChanges(ModelState);
            }
            var model = new PORequstFormViewModel(result);
            ViewBag.ViewOnly = model.Action.Access == DB.SessionAccess.Edit ? "False" : "True";
            ViewBag.Partial = true;
            return PartialView("../PO/Request/Form/PartialIndex", model);
        }
        
        [HttpGet]
        public ActionResult Form(byte poco, int requestId)
        {
            using var db = new VPContext();
            var result = db.PORequests.FirstOrDefault(f => f.POCo == poco && f.RequestId == requestId);
            //var model = new PORequestViewModel(result);
            var model = new PORequstFormViewModel(result);
            var userId = StaticFunctions.GetUserId();
            ViewBag.ViewOnly = model.Action.Access == DB.SessionAccess.Edit ? "False" : "True";
            return PartialView("../PO/Request/Form/Header/Form", model.Request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(PORequestViewModel model)
        {
            var result = RequestRepository.ProcessUpdate(model, ModelState);
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(byte poco, int requestId)
        {
            using var db = new VPContext();
            var delObj = db.PORequests.FirstOrDefault(f => f.POCo == poco && f.RequestId == requestId);
            delObj.Status = DB.PORequestStatusEnum.Canceled;
            db.SaveChanges(ModelState);

            var model = new PORequstFormViewModel(delObj);
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult Validate(PORequestViewModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            ModelState.Clear();
            model.Validate(ModelState);
            using var db = new VPContext();
            var obj = db.PORequests.Where(f => f.POCo == model.POCo && f.RequestId == model.RequestId).FirstOrDefault();
            if (obj != null)
            {
                var form = new PORequstFormViewModel(obj);
                //TryValidateModel(form);
                TryValidateModelRecursive(form);
                if (obj.Lines.Count == 0)
                {
                    ModelState.AddModelError("", "No Items listed");
                }
            }
            if (!ModelState.IsValid)
            {
                var errorModel = ModelState.ModelErrors();
                    return Json(new { success = ModelState.IsValidJson(), errorModel }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Submit(byte poco, int requestId)
        {           
            using var db = new VPContext();
            var result = db.PORequests.FirstOrDefault(f => f.POCo == poco && f.RequestId == requestId);
            var model = new PORequstFormViewModel(result);
            
            ModelState.Clear();
            TryValidateModelRecursive(model);
            if (ModelState.IsValid && model.Action.CanSubmit)
            {
                
                result.Status = DB.PORequestStatusEnum.Submitted;               
                db.SaveChanges(ModelState);                
                if (result.PO != null)
                {
                    result.Status = DB.PORequestStatusEnum.Approved;
                    db.SaveChanges(ModelState);
                }
            }
            model = new PORequstFormViewModel(result);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(byte poco, int requestId)
        {
            using var db = new VPContext();
            using var empRepo = new Repository.VP.WP.WebUserRepository();

            var result = db.PORequests.FirstOrDefault(f => f.POCo == poco && f.RequestId == requestId);
            var model = new PORequstFormViewModel(result);

            ModelState.Clear();
            TryValidateModelRecursive(model);

            if (ModelState.IsValid && model.Action.CanApprove)
            {

                result.Status = DB.PORequestStatusEnum.Approved;
                //if (result.PO == null)
                //{
                //    result.Status = DB.PORequestStatusEnum.Submitted;
                //}
                db.SaveChanges(ModelState);

            }
            model = new PORequstFormViewModel(result);
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetPO(byte poco, int requestId)
        {
            using var db = new VPContext();
            using var empRepo = new Repository.VP.WP.WebUserRepository();

            var result = db.PORequests.FirstOrDefault(f => f.POCo == poco && f.RequestId == requestId);
            var model = new PORequstFormViewModel(result);

            ModelState.Clear();
            if (model.Action.CanCreatePO)
            {
                if (result.PO == null)
                {
                    result.GetPO();
                    //result.EmailStatusPOUpdate();
                }
                db.SaveChanges(ModelState);

            }
            model = new PORequstFormViewModel(result);
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveAll()
        {
            using var db = new VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var requests = user.AssignedPORequests.Where(f => f.Status == (int)DB.PORequestStatusEnum.Submitted && f.Active == "Y").Select( s=> s.Request).ToList();
            foreach (var result in requests)
            {                
                var model = new PORequstFormViewModel(result);

                ModelState.Clear();
                TryValidateModelRecursive(model);

                if (ModelState.IsValid && model.Action.CanApprove)
                {
                    result.Status = DB.PORequestStatusEnum.Approved;
                }
            }
            db.SaveChanges(ModelState);

            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnSubmit(byte poco, int requestId)
        {
            using var db = new VPContext();
            var result = db.PORequests.FirstOrDefault(f => f.POCo == poco && f.RequestId == requestId);
            var model = new PORequstFormViewModel(result);

            ModelState.Clear();
            TryValidateModelRecursive(model);

            if (ModelState.IsValid && model.Action.CanUnSubmit)
            {
                result.Status = DB.PORequestStatusEnum.Open;
                db.SaveChanges(ModelState);
            }

            model = new PORequstFormViewModel(result);
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnDelete(byte poco, int requestId)
        {
            using var db = new VPContext();
            var result = db.PORequests.FirstOrDefault(f => f.POCo == poco && f.RequestId == requestId);
            var model = new PORequstFormViewModel(result);

            ModelState.Clear();
            TryValidateModelRecursive(model);

            if (ModelState.IsValid && model.Action.CanUnDelete)
            {
                result.Status = (int)DB.PORequestStatusEnum.Open;
                db.SaveChanges(ModelState);
            }

            model = new PORequstFormViewModel(result);
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel(byte poco, int requestId)
        {
            using var db = new VPContext();
            var result = db.PORequests.FirstOrDefault(f => f.POCo == poco && f.RequestId == requestId);
            var model = new PORequstFormViewModel(result);

            if (model.Action.CanCancel)
            {
                result.Status = DB.PORequestStatusEnum.Canceled;
                db.SaveChanges(ModelState);
            }

            model = new PORequstFormViewModel(result);
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reject(PORequestRejectViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            using var db = new VPContext();
            var result = db.PORequests.FirstOrDefault(f => f.POCo == model.POCo && f.RequestId == model.RequestId);
            var formModel = new PORequstFormViewModel(result);
            ModelState.Clear();
            TryValidateModelRecursive(model);

            if (ModelState.IsValid && formModel.Action.CanReject)
            {

                result.StatusComments = model.Comments;
                result.Status = DB.PORequestStatusEnum.Rejected;                
                db.SaveChanges(ModelState);
            }
            formModel = new PORequstFormViewModel(result);
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), model = formModel, errorModel = ModelState.ModelErrors() });
        }

    }
}