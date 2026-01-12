using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Code;
using portal.Models.Views.Payroll.Leave;
using portal.Repository.VP.PR;
using System;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;

namespace portal.Controllers.View.PurchasOrders
{
    public class LeaveRequestController : BaseController
    {

        [HttpGet] 
        public PartialViewResult Add()
        {
            using var db = new VPContext();
            var userId = StaticFunctions.GetUserId();
            var emp = db.WebUsers.FirstOrDefault(f => f.Id == userId).Employee.FirstOrDefault();
            //var result = db.LeaveRequests.FirstOrDefault(f => f.CreatedBy == userId && f.Status == 0);

            var result = RequestRepository.Init();
            result.RequestId = RequestRepository.NextId(result);
            result.EmployeeId = emp.HRRef;
            db.LeaveRequests.Add(result);
            result.StatusLogs.Add(RequestStatusLogRepository.Init(result));
            db.SaveChanges(ModelState);

            RequestWorkFlowRepository.GenerateWorkFlow(result);
            if (result.Lines.Count == 0)
            {
                var line = RequestLineRepository.Init(result);
                line.LineId = RequestLineRepository.NextId(line);
                result.Lines.Add(line);
                db.SaveChanges(ModelState);
            }
            var results = new LeaveRequestSummaryViewModel(DateTime.Now.Year, result);
            ViewBag.tableRow = "True";
            return PartialView("TableRow", results);
        }

        [HttpGet]
        [Route("Leave/Request/Create")]
        public ActionResult Create()
        {
            using var db = new VPContext();
            var userId = StaticFunctions.GetUserId();
            var emp = db.WebUsers.FirstOrDefault(f => f.Id == userId).Employee.FirstOrDefault();
            //var result = db.LeaveRequests.FirstOrDefault(f => f.CreatedBy == userId && f.Status == 0);

            var result = db.LeaveRequests.FirstOrDefault(f => f.CreatedBy == userId && f.Status == 0);
            if (result == null)
            {
                result = RequestRepository.Init();
                result.RequestId = RequestRepository.NextId(result);
            }
            result.EmployeeId = emp.HRRef;
            db.LeaveRequests.Add(result);
            result.StatusLogs.Add(RequestStatusLogRepository.Init(result));
            db.SaveChanges(ModelState);

            RequestWorkFlowRepository.GenerateWorkFlow(result);
            if (result.Lines.Count == 0)
            {
                var line = RequestLineRepository.Init(result);
                line.LineId = RequestLineRepository.NextId(line);
                result.Lines.Add(line);
                db.SaveChanges(ModelState);
            }
            return RedirectToAction("Index",  new { result.PRCo, result.RequestId });
        }

        [HttpGet]
        [Route("Leave/Request/{requestId}-{prco}")]
        public ActionResult Index(byte prco, int requestId)
        {
            using var db = new VPContext();
            var userId = StaticFunctions.GetUserId();
            var result = db.LeaveRequests.FirstOrDefault(f => f.PRCo == prco && f.RequestId == requestId);
            
            if (result.Lines.Count == 0)
            {
                var line = RequestLineRepository.Init(result);
                line.LineId = RequestLineRepository.NextId(line);
                result.Lines.Add(line);
                db.SaveChanges(ModelState);
            }
            var model = new LeaveRequstFormViewModel(result);
            ViewBag.ViewOnly = model.Action.Access == DB.SessionAccess.Edit ? "False" : "True";
            ViewBag.Partial = false;
            return View("Form/Index", model);
        }

        [HttpGet]
        public ActionResult PartialIndex(byte prco, int requestId)
        {
            using var db = new VPContext();
            var userId = StaticFunctions.GetUserId();
            var result = db.LeaveRequests.FirstOrDefault(f => f.PRCo == prco && f.RequestId == requestId);

            if (result.Lines.Count == 0)
            {
                var line = RequestLineRepository.Init(result);
                line.LineId = RequestLineRepository.NextId(line);
                result.Lines.Add(line);
                db.SaveChanges(ModelState);
            }
            var model = new LeaveRequstFormViewModel(result);
            ViewBag.ViewOnly = model.Action.Access == DB.SessionAccess.Edit ? "False" : "True";
            ViewBag.Partial = true;
            return PartialView("Form/PartialIndex", model);
        }
        
        [HttpGet]
        public ActionResult Form(byte prco, int requestId)
        {
            using var db = new VPContext();
            var result = db.LeaveRequests.FirstOrDefault(f => f.PRCo == prco && f.RequestId == requestId);
            //var model = new LeaveRequestViewModel(result);
            var model = new LeaveRequstFormViewModel(result);
            var userId = StaticFunctions.GetUserId();
            ViewBag.ViewOnly = model.Action.Access == DB.SessionAccess.Edit ? "False" : "True";
            return PartialView("Header/Form", model.Request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(LeaveRequestViewModel model)
        {
            var result = RequestLineRepository.ProcessUpdate(model, ModelState);
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(byte prco, int requestId)
        {
            using var db = new VPContext();
            var delObj = db.LeaveRequests.FirstOrDefault(f => f.PRCo == prco && f.RequestId == requestId);
            delObj.Status = (int)DB.LeaveRequestStatusEnum.Canceled;
            db.SaveChanges(ModelState);
            var model = new LeaveRequstFormViewModel(delObj);

            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult Validate(LeaveRequestViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            ModelState.Clear();
            model.Validate(ModelState);
            using var db = new VPContext();
            var obj = db.LeaveRequests.Where(f => f.PRCo == model.PRCo && f.RequestId == model.RequestId).FirstOrDefault();
            if (obj != null)
            {
                var form = new LeaveRequstFormViewModel(obj);
                //TryValidateModel(form);
                TryValidateModelRecursive(form);
                if (obj.Lines.Count == 0)
                {
                    ModelState.AddModelError("", "No Items listed");
                }
            }

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Submit(byte prco, int requestId)
        {           
            using var db = new VPContext();
            var result = db.LeaveRequests.Where(f => f.PRCo == prco && f.RequestId == requestId).FirstOrDefault();
            var model = new LeaveRequstFormViewModel(result);
            var curEmp = StaticFunctions.GetCurrentEmployee();

            ModelState.Clear();
            TryValidateModelRecursive(model);
            var sendLeaveEmail = false;
            if (ModelState.IsValid && model.Action.CanSubmit)
            {
                
                result.Status = (int)DB.LeaveRequestStatusEnum.Submitted;
                result.StatusLogs.Add(RequestStatusLogRepository.Init(result));
                db.SaveChanges(ModelState);
                if ((User.IsInPosition("HR-MGR") || User.IsInPosition("HR-PRMGR") || curEmp.EmployeeId == result.Employee.Supervisor.EmployeeId) && result.EmployeeId != curEmp.EmployeeId)
                {
                    result.Status = (int)DB.LeaveRequestStatusEnum.Approved;
                    var log = RequestStatusLogRepository.Init(result);
                    log.Comments = string.Format(AppCultureInfo.CInfo(), "Auto Approved");
                    result.StatusLogs.Add(log);
                }

                db.SaveChanges(ModelState);
                if (ModelState.IsValid)
                {
                    RequestWorkFlowRepository.GenerateWorkFlow(result);
                    db.SaveChanges(ModelState);
                    EmailStatusUpdate(result);
                    if (sendLeaveEmail)
                    {
                        EmailLeaveUpdate(result);
                    }
                }
            }
            model = new LeaveRequstFormViewModel(result);
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(byte prco, int requestId)
        {
            using var db = new VPContext();
            using var empRepo = new Repository.VP.WP.WebUserRepository();

            var result = db.LeaveRequests.Where(f => f.PRCo == prco && f.RequestId == requestId).FirstOrDefault();
            var model = new LeaveRequstFormViewModel(result);
            var userId = StaticFunctions.GetUserId();
            var curUser = empRepo.GetUser(StaticFunctions.GetUserId());
            var emp = curUser.Employee.FirstOrDefault();
            ModelState.Clear();
            TryValidateModelRecursive(model);
            var sendLeaveEmail = false;
            if (ModelState.IsValid && model.Action.CanApprove)
            {                
                result.Status = (int)DB.LeaveRequestStatusEnum.Approved;
                result.StatusLogs.Add(RequestStatusLogRepository.Init(result));
                db.SaveChanges(ModelState);
                if (ModelState.IsValid)
                {
                    RequestWorkFlowRepository.GenerateWorkFlow(result);
                    db.SaveChanges(ModelState);

                    EmailStatusUpdate(result);
                    if (sendLeaveEmail)
                    {
                        EmailLeaveUpdate(result);
                    }
                }
            }
            model = new LeaveRequstFormViewModel(result);
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
            var requests = user.AssignedLeaveRequests.Where(f => f.Status == (int)DB.LeaveRequestStatusEnum.Submitted && f.Active == "Y").Select( s=> s.Request).ToList();
            foreach (var result in requests)
            {                
                var model = new LeaveRequstFormViewModel(result);

                ModelState.Clear();
                TryValidateModelRecursive(model);

                if (ModelState.IsValid && model.Action.CanApprove)
                {
                    result.Status = (int)DB.LeaveRequestStatusEnum.Approved;
                    result.StatusLogs.Add(RequestStatusLogRepository.Init(result));
                    db.SaveChanges(ModelState);
                }
                if (ModelState.IsValid)
                {
                    RequestWorkFlowRepository.GenerateWorkFlow(result);
                    db.SaveChanges(ModelState);
                    //EmailStatusUpdate(result);
                }

            }

            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnSubmit(byte prco, int requestId)
        {
            using var db = new VPContext();
            var result = db.LeaveRequests.Where(f => f.PRCo == prco && f.RequestId == requestId).FirstOrDefault();
            var model = new LeaveRequstFormViewModel(result);

            ModelState.Clear();
            TryValidateModelRecursive(model);

            if (ModelState.IsValid && model.Action.CanUnSubmit)
            {
                result.Status = (int)DB.LeaveRequestStatusEnum.Open;
                result.StatusLogs.Add(RequestStatusLogRepository.Init(result));
                db.SaveChanges(ModelState);
                if (ModelState.IsValid)
                {
                    RequestWorkFlowRepository.GenerateWorkFlow(result);
                    db.SaveChanges(ModelState);
                    //EmailStatusUpdate(result);
                }
            }

            model = new LeaveRequstFormViewModel(result);
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnDelete(byte prco, int requestId)
        {
            using var db = new VPContext();
            var result = db.LeaveRequests.Where(f => f.PRCo == prco && f.RequestId == requestId).FirstOrDefault();
            var model = new LeaveRequstFormViewModel(result);

            ModelState.Clear();
            TryValidateModelRecursive(model);

            if (ModelState.IsValid && model.Action.CanUnDelete)
            {
                result.Status = (int)DB.LeaveRequestStatusEnum.Open;
                result.StatusLogs.Add(RequestStatusLogRepository.Init(result));
                db.SaveChanges(ModelState);
                if (ModelState.IsValid)
                {
                    RequestWorkFlowRepository.GenerateWorkFlow(result);
                    db.SaveChanges(ModelState);
                    //EmailStatusUpdate(result);
                }
            }

            model = new LeaveRequstFormViewModel(result);
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel(byte prco, int requestId)
        {
            using var db = new VPContext();
            var result = db.LeaveRequests.Where(f => f.PRCo == prco && f.RequestId == requestId).FirstOrDefault();
            var model = new LeaveRequstFormViewModel(result);

            if (model.Action.CanCancel)
            {
                result.Status = (int)DB.LeaveRequestStatusEnum.Canceled;
                result.StatusLogs.Add(RequestStatusLogRepository.Init(result));
                db.SaveChanges(ModelState);
                if (ModelState.IsValid)
                {
                    RequestWorkFlowRepository.GenerateWorkFlow(result);
                    db.SaveChanges(ModelState);
                    //EmailStatusUpdate(result);
                }
            }


            model = new LeaveRequstFormViewModel(result);
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reject(LeaveRequestRejectViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            using var db = new VPContext();
            var result = db.LeaveRequests.Where(f => f.PRCo == model.PRCo && f.RequestId == model.RequestId).FirstOrDefault();
            var formModel = new LeaveRequstFormViewModel(result);
            ModelState.Clear();
            TryValidateModelRecursive(model);

            if (ModelState.IsValid && formModel.Action.CanReject)
            {
                result.Status = (int)DB.LeaveRequestStatusEnum.Rejected;
                var statusLog = RequestStatusLogRepository.Init(result);
                statusLog.Comments = model.Comments;
                result.StatusLogs.Add(statusLog);
                db.SaveChanges(ModelState);
                if (ModelState.IsValid)
                {
                    RequestWorkFlowRepository.GenerateWorkFlow(result);
                    db.SaveChanges(ModelState);
                    //EmailStatusUpdate(result);
                }
            }
            formModel = new LeaveRequstFormViewModel(result);
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Json(new { success = ModelState.IsValidJson(), model = formModel, errorModel = ModelState.ModelErrors() });
        }


        public void EmailStatusUpdate(LeaveRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            using var db = new VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var emp = user.Employee.FirstOrDefault();
            var model = new LeaveRequstFormViewModel(request);
            try
            {
                using MailMessage msg = new MailMessage()
                {
                    Body = EmailHelper.RenderViewToString(ControllerContext, "EmailStatusChange", model, false),
                    IsBodyHtml = true,
                    Subject = string.Format(AppCultureInfo.CInfo(), "Time Off Request {0} By {1} {2}", ((DB.LeaveRequestStatusEnum)request.Status), emp.FirstName, emp.LastName),
                };
                foreach (var workflow in request.WorkFlows.Where(f => f.Active == "Y").ToList())
                {
                    if (workflow.AssignedUser == null)
                    {
                        var assUser = db.WebUsers.FirstOrDefault(f => f.Id == workflow.AssignedTo);
                        msg.To.Add(new MailAddress(assUser.Email));
                    }
                    else
                    {
                        msg.To.Add(new MailAddress(workflow.AssignedUser.Email));
                    }
                }
                msg.CC.Add(new MailAddress(request.CreatedUser.Email));

                EmailHelper.Send(msg);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.GetBaseException().ToString());
            }
        }
        
        public void EmailLeaveUpdate(LeaveRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            using var db = new VPContext();
            var model = new LeaveRequstFormViewModel(request);
            try
            {
                using MailMessage msg = new MailMessage()
                {
                    Body = EmailHelper.RenderViewToString(ControllerContext, "EmailLeaveUpdate", model, false),
                    IsBodyHtml = true,
                    Subject = string.Format(AppCultureInfo.CInfo(), "Time Off Request Approved"),
                };
                msg.CC.Add(new MailAddress(request.CreatedUser.Email));
                EmailHelper.Send(msg);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.GetBaseException().ToString());
            }
        }
    }
}