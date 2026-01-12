using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;

namespace portal.Controllers.View.DailyTicket
{
    public class DailyTicketFormController : BaseController
    {
        #region Main form
        [HttpGet]
        [Route("Ticket/Form/{ticketId}-{dtco}")]
        public ActionResult Index(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            ticket.CreateReviewLog();

            switch (ticket.FormType)
            {
                case DB.DTFormEnum.JobFieldTicket:
                    return RedirectToAction("JobTicketIndex", "DailyTicketForm", new { Area = "",dtco, ticketId });
                case DB.DTFormEnum.ProjectManager:
                    break;
                case DB.DTFormEnum.TruckingTicket:
                    return RedirectToAction("TruckTicketIndex", "DailyTicketForm", new { Area = "",dtco, ticketId });
                case DB.DTFormEnum.EmployeeDetailTicket:
                    break;
                case DB.DTFormEnum.EmployeeTicket:
                    break;
                case DB.DTFormEnum.ShopTicket:
                    return RedirectToAction("ShopTicketIndex", "DailyTicketForm", new { Area = "",dtco, ticketId });
                case DB.DTFormEnum.SubContractorTicket:
                    break;
                case DB.DTFormEnum.CrewTicket:
                    return RedirectToAction("CrewTicketIndex", "DailyTicketForm", new { Area = "",dtco, ticketId });
                case DB.DTFormEnum.TimeOff:
                    break;
                case DB.DTFormEnum.PayrollEntriesTicket:
                    break;
                case DB.DTFormEnum.HolidayTicket:
                    break;
                default:
                    break;
            }
            return RedirectToAction("Index", ticket.Form.ControllerName, new { dtco, ticketId });
        }

        [HttpGet]
        public ActionResult PartialIndex(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            ticket.CreateReviewLog();
            switch (ticket.FormType)
            {
                case DB.DTFormEnum.JobFieldTicket:
                    return RedirectToAction("JobTicketIndex", "DailyTicketForm", new { Area = "",dtco, ticketId, partialView = true });
                case DB.DTFormEnum.ProjectManager:
                    break;
                case DB.DTFormEnum.TruckingTicket:
                    return RedirectToAction("TruckTicketIndex", "DailyTicketForm", new { Area = "",dtco, ticketId, partialView = true });
                case DB.DTFormEnum.EmployeeDetailTicket:
                    break;
                case DB.DTFormEnum.EmployeeTicket:
                    break;
                case DB.DTFormEnum.ShopTicket:
                    return RedirectToAction("ShopTicketIndex", "DailyTicketForm", new { Area = "",dtco, ticketId, partialView = true });
                case DB.DTFormEnum.SubContractorTicket:
                    break;
                case DB.DTFormEnum.CrewTicket:
                    return RedirectToAction("CrewTicketIndex", "DailyTicketForm", new { Area = "",dtco, ticketId, partialView = true });
                case DB.DTFormEnum.TimeOff:
                    break;
                case DB.DTFormEnum.PayrollEntriesTicket:
                    break;
                case DB.DTFormEnum.HolidayTicket:
                    break;
                default:
                    break;
            }
            return RedirectToAction("Index", ticket.Form.ControllerName, new { dtco, ticketId, partialView = true });
        }

        [HttpGet]
        public ActionResult PopupForm(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            ticket.CreateReviewLog();
            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";
            switch (ticket.FormType)
            {
                case DB.DTFormEnum.JobFieldTicket:
                    return RedirectToAction("JobTicketPopupForm", "DailyTicketForm", new { Area = "",dtco, ticketId });
                case DB.DTFormEnum.ProjectManager:
                    break;
                case DB.DTFormEnum.TruckingTicket:
                    return RedirectToAction("TruckTicketPopupForm", "DailyTicketForm", new { Area = "",dtco, ticketId });
                case DB.DTFormEnum.EmployeeDetailTicket:
                    break;
                case DB.DTFormEnum.EmployeeTicket:
                    break;
                case DB.DTFormEnum.ShopTicket:
                    return RedirectToAction("ShopTicketPopupForm", "DailyTicketForm", new { Area = "",dtco, ticketId });
                case DB.DTFormEnum.SubContractorTicket:
                    break;
                case DB.DTFormEnum.CrewTicket:
                    return RedirectToAction("CrewTicketPopupForm", "DailyTicketForm", new { Area = "",dtco, ticketId });
                case DB.DTFormEnum.TimeOff:
                    break;
                case DB.DTFormEnum.PayrollEntriesTicket:
                    break;
                case DB.DTFormEnum.HolidayTicket:
                    break;
                default:
                    break;
            }
            return RedirectToAction("PopupForm", ticket.Form.ControllerName, new { dtco, ticketId, partialView = true });
        }
        #endregion

        #region Report Views
        [HttpGet]
        public ActionResult DailyTicketCrewList(byte prco, string crewId, int weekId)
        {
            using var db = new VPContext();

            var dates = db.Calendars.Where(f => f.Week == weekId).Select(s => s.Date).ToList();
            var tickets = db.vDailyTickets.Where(f => f.WorkDate >= dates.Min() && f.WorkDate <= dates.Max() && f.Status <= 4 && f.CrewId == crewId).ToList();//f.DTCo == dtco && 

            return PartialView("../DT/Summary/TabList/Panel", tickets);
        }

        #endregion 

        #region Ticket Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Submit(byte dtco, int ticketId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var ticketForm = new Models.Views.DailyTicket.Form.TicketForm(ticket, true);

            ModelState.Clear();
            this.TryValidateModelRecursive(ticketForm.DynaimicTicket);
            if (ModelState.IsValid && ticketForm.CanSubmit)
            {
                ticket.Status = DB.DailyTicketStatusEnum.Submitted;
                db.SaveChanges(ModelState);
            }

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(byte dtco, int ticketId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var ticketForm = new Models.Views.DailyTicket.Form.TicketForm(ticket, true);

            ModelState.Clear();
            this.TryValidateModelRecursive(ticketForm.DynaimicTicket);
            if (ModelState.IsValid && ticketForm.CanApprove)
            { 
                ticket.Status = DB.DailyTicketStatusEnum.Approved;
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }
       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnSubmit(byte dtco, int ticketId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            ticket.Status = DB.DailyTicketStatusEnum.Draft;
            db.SaveChanges(ModelState);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnDelete(byte dtco, int ticketId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            ticket.Status = DB.DailyTicketStatusEnum.Draft;
            db.SaveChanges(ModelState);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnPost(byte dtco, int ticketId)
        {            
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            ticket.Status = DB.DailyTicketStatusEnum.UnPosted;
            try
            {
                db.SaveChanges(ModelState);
            }
            catch (Exception)
            {

            }

            return Json(new { success = "true", errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel(byte dtco, int ticketId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            ticket.Status = DB.DailyTicketStatusEnum.Deleted;
            db.SaveChanges(ModelState);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReGenearateTempPost(byte dtco, int ticketId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

            ticket.RemoveDTPayrollEntries();
            db.BulkSaveChanges();
            ticket.GenerateDTPayrollEntries();
            db.BulkSaveChanges();

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        #region Reject Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reject(Models.Views.DailyTicket.DailyTicketRejectViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId);
            if (ticket != null)
            {
                ticket.StatusComments = model.Comments;
                ticket.Status = DB.DailyTicketStatusEnum.Rejected;
                db.SaveChanges(ModelState);
            }
            model.Url = Url.Action("Index", "DailyTicket", new { Area = "", dtco = model.DTCo, model.TicketId });

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }
        
        [HttpGet]
        public ActionResult RejectIndex(byte dtco, int ticketId)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var model = new Models.Views.DailyTicket.DailyTicketRejectViewModel(ticket);

            return PartialView("../DT/Reject/Model", model);
        }

        [HttpGet]
        public ActionResult RejectValidate(Models.Views.DailyTicket.DailyTicketRejectViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion

        #region Create Ticket
        [HttpGet]
        public JsonResult ValidateCreate(Models.Views.DailyTicket.DailyTicketCreateViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(Models.Views.DailyTicket.DailyTicketCreateViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);
            if (ModelState.IsValid)
            {
                using var db = new VPContext();
                var division = StaticFunctions.GetCurrentDivision();
                
				division = db.CompanyDivisions.FirstOrDefault(f => f.DivisionId == division.DivisionId);
                var ticket = division.AddDailyTicket(model.WorkDate, (DB.DTFormEnum)model.FormId);
				db.SaveChanges(ModelState);

                var url = Url.Action("Index", new { Area = "", dtco = ticket.DTCo, ticketId = ticket.TicketId });
                return Json(new { success = ModelState.IsValidJson(), url });
            }

            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Job Ticket Forms

        #region Main Form
        [HttpGet]
        [Route("Ticket/Field/{ticketId}-{dtco}/")]
        public ActionResult JobTicketIndex(byte dtco, int ticketId, bool partialView = false)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var modelTicket = new Models.Views.DailyTicket.Form.TicketForm(ticket, true);
            var model = (Models.Views.DailyTicket.Form.JobTicketFormViewModel)modelTicket.DynaimicTicket;

            //var model = new JobTicketFormViewModel(ticket);
            ViewBag.TicketForm = modelTicket;
            ViewBag.ViewOnly = model.Access == DB.SessionAccess.View ? true : false;
            ViewBag.Partial = partialView;

            if (model.Access == DB.SessionAccess.Denied)
            {
                return RedirectToAction("Denied", "Error");
            }

            if (!partialView)
                return View("../DT/Type/Job/Index", model);
            else
                return PartialView("../DT/Type/Job/PartialIndex", model);
        }

        [HttpGet]
        public ActionResult JobTicketPopupForm(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var modelTicket = new Models.Views.DailyTicket.Form.TicketForm(ticket, true);
            var model = (Models.Views.DailyTicket.Form.JobTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.ViewOnly = model.Access == DB.SessionAccess.View ? true : false;
            ViewBag.TicketForm = modelTicket;
            ViewBag.Partial = false;
            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";
            return PartialView("../DT/Type/Job/Index", model);
        }

        [HttpGet]
        public ActionResult JobTicketForm(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var modelTicket = new Models.Views.DailyTicket.Form.TicketForm(ticket, true);
            var model = (Models.Views.DailyTicket.Form.JobTicketFormViewModel)modelTicket.DynaimicTicket;
            ViewBag.ViewOnly = model.Access == DB.SessionAccess.View;
            ViewBag.TicketForm = modelTicket;
            return PartialView("../DT/Type/Job/Form/Form", model.Ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult JobTicketUpdate(Models.Views.DailyTicket.DailyJobTicketViewModel model)
        {
            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            db.SaveChanges(ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult JobTicketValidate(Models.Views.DailyTicket.DailyJobTicketViewModel model)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId);
            var form = new Models.Views.DailyTicket.Form.JobTicketFormViewModel(ticket);            
            form.Validate(ModelState);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Daily Phases (tasks)
        [HttpGet]
        public ActionResult DTJobPhasePanel(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var model = new portal.Models.Views.DailyTicket.Job.DailyJobPhaseListViewModel(ticket.DailyJobTicket);
            return PartialView("../DT/Type/Job/Phases/List/Panel", model);
        }

        [HttpGet]
        public ActionResult DTJobPhaseTable(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var model = new portal.Models.Views.DailyTicket.Job.DailyJobPhaseListViewModel(ticket.DailyJobTicket);
            return PartialView("../DT/Type/Job/Phases/List/Table", model);
        }

        [HttpGet]
        public ActionResult DTJobPhaseAdd(byte dtco, int ticketId, string phaseId)
        {
            var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var task = ticket.DailyJobTicket.AddJobPhase(phaseId);
            db.SaveChanges(ModelState);

            var result = new Models.Views.DailyTicket.Job.DailyJobPhaseViewModel(task);

            ViewBag.tableRow = "True";
            db.Dispose();
            return PartialView("../DT/Type/Job/Phases/List/Row/TablePhaseRow", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DTJobPhaseUpdate(Models.Views.DailyTicket.Job.DailyJobPhaseCostViewModel model)
        {
            if (model == null)
                throw new System.ArgumentNullException(nameof(model));
            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            db.SaveChanges(ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DTJobPhaseDelete(byte dtco, int ticketId, int taskId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var delObj = db.DTJobPhases.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId && f.TaskId == taskId);
            if (delObj != null)
            {
                ticket.DailyJobTicket.DeleteJobPhase(delObj);
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson() });
        }

        [HttpGet]
        public JsonResult DTJobPhaseValidate(Models.Views.DailyTicket.Job.DailyJobPhaseCostViewModel model)
        {
            using var db = new VPContext();
            var entity = db.DTJobPhaseCosts.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId && f.TaskId == model.TaskId && f.LineId == model.LineId);
            if (entity != null)
            {
                model = new Models.Views.DailyTicket.Job.DailyJobPhaseCostViewModel(entity);
                ModelState.Clear();
                this.TryValidateModel(model);

            }
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Job Employee
        [HttpGet]
        public ActionResult DailyJobEmployeePanel(byte dtco, int ticketId, bool forEdit)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.Where(f => f.DTCo == dtco && f.TicketId == ticketId).FirstOrDefault();

            var modelTicket = new Models.Views.DailyTicket.Form.TicketForm(ticket, true);
            var form = (Models.Views.DailyTicket.Form.JobTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.ViewOnly = form.Access == DB.SessionAccess.View ? true : false;
            var model = form.Employees;
            return PartialView("../DT/Type/Job/Employee/List/Panel", model);
        }

        [HttpGet]
        public ActionResult DailyJobEmployeeTable(byte dtco, int ticketId, bool forEdit)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.Where(f => f.DTCo == dtco && f.TicketId == ticketId).FirstOrDefault();

            var modelTicket = new Models.Views.DailyTicket.Form.TicketForm(ticket, true);
            var form = (Models.Views.DailyTicket.Form.JobTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.ViewOnly = form.Access == DB.SessionAccess.View ? true : false;
            var model = form.Employees;
            return PartialView("../DT/Type/Job/Employee/List/Table", model);
        }

        [HttpGet]
        public PartialViewResult DailyJobEmployeeAdd(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            Models.Views.DailyTicket.DailyJobEmployeeViewModel result = null;
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            if (ticket != null)
            {
                var emp = ticket.DailyJobTicket.AddEmployee();
                db.SaveChanges(ModelState);
                result = new Models.Views.DailyTicket.DailyJobEmployeeViewModel(emp);
            }

            ViewBag.tableRow = "True";

            return PartialView("../DT/Type/Job/Employee/List/TableRow", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DailyJobEmployeeUpdate(Models.Views.DailyTicket.DailyJobEmployeeViewModel model)
        {
            if (model == null)
                throw new System.ArgumentNullException(nameof(model));

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            db.SaveChanges(ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DailyJobEmployeeDelete(byte dtco, int ticketId, int lineNum)
        {
            using var db = new VPContext();
            var updObj = db.DailyJobEmployees.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId && f.LineNum == lineNum);
            if (updObj != null)
            {
                db.DailyJobEmployees.Remove(updObj);
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson() });
        }

        [HttpGet]
        public JsonResult DailyJobEmployeeValidate(Models.Views.DailyTicket.DailyJobEmployeeViewModel model)
        {
            if (model == null)
                throw new System.ArgumentNullException(nameof(model));

            //if (model.EmployeeId == null)
            //{
            //    using var db = new VPContext();
            //    var fieldEmployee = db.DailyJobEmployees.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId && f.LineNum == model.LineNum);
            //    if (fieldEmployee != null)
            //    {
            //        var obj = new DailyJobEmployeeViewModel(fieldEmployee)
            //        {
            //            Comments = model.Comments
            //        };
            //        model = obj;
            //    }
            //}

            ModelState.Clear();
            TryValidateModel(model);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Job PO Usage
        [HttpGet]
        public ActionResult POUsagePanel(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

            var modelTicket = new Models.Views.DailyTicket.Form.TicketForm(ticket, true);
            var form = (Models.Views.DailyTicket.Form.JobTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.ViewOnly = form.Access == DB.SessionAccess.View ? true : false;
            var model = form.POUsages;
            return PartialView("../DT/Type/Job/POUsage/List/Panel", model);
        }

        [HttpGet]
        public ActionResult POUsageTable(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

            var modelTicket = new Models.Views.DailyTicket.Form.TicketForm(ticket, true);
            var form = (Models.Views.DailyTicket.Form.JobTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.ViewOnly = form.Access == DB.SessionAccess.View ? true : false;
            var model = form.POUsages;
            return PartialView("../DT/Type/Job/POUsage/List/Table", model);
        }

        [HttpGet]
        public PartialViewResult POUsageAdd(byte dtco, int ticketId)
        {
            using var db = new VPContext();

            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var entity = ticket.DailyJobTicket.AddPOUsage();

            var result = new Models.Views.DailyTicket.DailyPOUsageViewModel(entity);
            ViewBag.tableRow = "True";
            db.SaveChanges(ModelState);
            return PartialView("../DT/Type/Job/POUsage/List/TableRow", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult POUsageUpdate(Models.Views.DailyTicket.DailyPOUsageViewModel model)
        {
            if (model == null)
                throw new System.ArgumentNullException(nameof(model));

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            db.SaveChanges(ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult POUsageDelete(byte dtco, int ticketId, int lineId)
        {
            using var db = new VPContext();
            var updObj = db.DailyPOUsages.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId && f.LineId == lineId);
            var model = new Models.Views.DailyTicket.DailyPOUsageViewModel(updObj);
            if (updObj != null)
            {
                db.DailyPOUsages.Remove(updObj);
                db.SaveChanges(ModelState);
            }
            return Json(new { success = ModelState.IsValidJson(), model });
        }

        [HttpGet]
        public JsonResult POUsageValidate(Models.Views.DailyTicket.DailyPOUsageViewModel model)
        {
            if (model == null)
                throw new System.ArgumentNullException(nameof(model));


            ModelState.Clear();
            TryValidateModel(model);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #endregion

        #region Crew Ticket Forms
        #region Main Form
        [HttpGet]
        [Route("Ticket/Crew/{ticketId}-{dtco}")]
        public ActionResult CrewTicketIndex(byte dtco, int ticketId, bool partialView = false)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var modelTicket = new Models.Views.DailyTicket.Form.TicketForm(ticket, true);
            var model = (Models.Views.DailyTicket.Form.CrewTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.TicketForm = modelTicket;
            ViewBag.ViewOnly = model.Access == DB.SessionAccess.View;
            ViewBag.Partial = partialView;

            if (model.Access == DB.SessionAccess.Denied)
            {
                return RedirectToAction("Denied", "Error");
            }
            if (!partialView)
                return View("../DT/Type/Crew/Index", model);
            else
                return PartialView("../DT/Type/Crew/PartialIndex", model);
        }

        [HttpGet]
        public ActionResult CrewTicketPopupForm(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var modelTicket = new Models.Views.DailyTicket.Form.TicketForm(ticket, true);
            var model = (Models.Views.DailyTicket.Form.CrewTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.ViewOnly = model.Access == DB.SessionAccess.View ? true : false;
            ViewBag.TicketForm = modelTicket;
            ViewBag.Partial = false;
            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";
            return PartialView("../DT/Type/Crew/Index", model);
        }
        #endregion
        #region Ticket Form
        [HttpGet]
        public ActionResult CrewTicketForm(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var modelTicket = new Models.Views.DailyTicket.Form.TicketForm(ticket, true);
            var model = (Models.Views.DailyTicket.Form.CrewTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.TicketForm = modelTicket;
            ViewBag.ViewOnly = model.Access == DB.SessionAccess.View;
            return PartialView("../DT/Type/Crew/Form/Form", model.Ticket);
        }

        [HttpGet]
        public ActionResult CrewTicketPanel(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var modelTicket = new Models.Views.DailyTicket.Form.TicketForm(ticket, true);
            var model = (Models.Views.DailyTicket.Form.CrewTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.TicketForm = modelTicket;
            ViewBag.ViewOnly = model.Access == DB.SessionAccess.View ? true : false;
            return PartialView("../DT/Type/Crew/Form/Panel", model.Ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrewTicketUpdate(Models.Views.DailyTicket.DailyCrewTicketViewModel model)
        {
            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            db.SaveChanges(ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult CrewTicketValidate(Models.Views.DailyTicket.DailyCrewTicketViewModel model)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId);

            var form = new Models.Views.DailyTicket.Form.CrewTicketFormViewModel(ticket);
            model = new Models.Views.DailyTicket.DailyCrewTicketViewModel(ticket);
            form.Validate(ModelState);

            if (!ModelState.IsValid)
            {
                var errorModel = ModelState.ModelErrors();
                return Json(new { success = ModelState.IsValidJson(), errorModel }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Crew Ticket Employee
        [HttpGet]
        public ActionResult CrewEmployeePanel(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.Where(f => f.DTCo == dtco && f.TicketId == ticketId).FirstOrDefault();

            var modelTicket = new Models.Views.DailyTicket.Form.TicketForm(ticket, true);
            var form = (Models.Views.DailyTicket.Form.CrewTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.ViewOnly = form.Access == DB.SessionAccess.View ? true : false;
            var model = form.Employees;
            return PartialView("../DT/Type/Crew/Employee/List/Panel", model);
        }

        [HttpGet]
        public ActionResult CrewEmployeeTable(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.Where(f => f.DTCo == dtco && f.TicketId == ticketId).FirstOrDefault();

            var modelTicket = new Models.Views.DailyTicket.Form.TicketForm(ticket, true);
            var form = (Models.Views.DailyTicket.Form.CrewTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.ViewOnly = form.Access == DB.SessionAccess.View ? true : false;
            var model = form.Employees;

            return PartialView("../DT/Type/Crew/Employee/List/Table", model);
        }

        [HttpGet]
        public PartialViewResult CrewEmployeeAdd(byte dtco, int ticketId)
        {
            ViewBag.tableRow = "True";
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

            var entry = ticket.DailyShopTicket.AddHoursEntry();
            var perdiem = ticket.DailyShopTicket.AddPerdiem();
            entry.EntryTypeId = 1;
            entry.JobId = ticket.DailyShopTicket.JobId;
            entry.PerdiemLineNum = perdiem.LineNum;

            db.SaveChanges(ModelState);

            var result = new Models.Views.DailyTicket.DailyCrewEmployeeViewModel(entry);
            var results = new List<Models.Views.DailyTicket.DailyCrewEmployeeViewModel>();
            results.Add(result);
            return PartialView("../DT/Type/Crew/Employee/List/Row/EmployeeRow", results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrewEmployeeUpdate(Models.Views.DailyTicket.DailyCrewEmployeeViewModel model)
        {
            if (model == null)
                throw new System.ArgumentNullException(nameof(model));

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            db.SaveChanges(ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrewEmployeeDelete(byte dtco, int ticketId, int lineNum)
        {
            using var db = new VPContext();
            var entries = db.DailyEmployeeEntries.Where(f => f.DTCo == dtco && f.TicketId == ticketId).ToList();
            var delObj = entries.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId && f.LineNum == lineNum);

            if (delObj != null)
            {
                var perdiemLineNum = delObj.PerdiemLineNum;
                var employeeId = delObj.tEmployeeId;
                entries.Remove(delObj);
                db.DailyEmployeeEntries.Remove(delObj);

                var perdiems = db.DailyEmployeePerdiems.Where(f => f.DTCo == dtco && f.TicketId == ticketId && (f.tEmployeeId == employeeId || f.tEmployeeId == null)).ToList();
                foreach (var perdiem in perdiems)
                {
                    var perdiemWithEntries = entries.Where(f => f.DTCo == dtco && f.TicketId == ticketId && f.PerdiemLineNum == perdiem.LineNum).ToList();
                    if (!perdiemWithEntries.Any())
                    {
                        db.DailyEmployeePerdiems.Remove(perdiem);
                    }
                }


                db.SaveChanges(ModelState);
            }
            else
            {
                ModelState.AddModelError("", "Could not delete, line not found.");
            }
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CrewEmployeeValidate(Models.Views.DailyTicket.DailyCrewEmployeeViewModel model)
        {
            using var db = new VPContext();
            var entry = db.DailyEmployeeEntries.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId && f.LineNum == model.LineNum);
            model = new Models.Views.DailyTicket.DailyCrewEmployeeViewModel(entry);

            ModelState.Clear();
            this.TryValidateModel(model);
            model.Validate(ModelState);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion

        #region Shop Ticket Form
        #region Main Form
        [HttpGet]
        [Route("Ticket/Shop/{ticketId}-{dtco}")]
        public ActionResult ShopTicketIndex(byte dtco, int ticketId, bool partialView = false)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var modelTicket = new Models.Views.DailyTicket.Form.TicketForm(ticket, true);
            var model = (Models.Views.DailyTicket.Form.ShopTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.TicketForm = modelTicket;
            ViewBag.ViewOnly = model.Access == DB.SessionAccess.View;
            ViewBag.Partial = partialView;

            if (model.Access == DB.SessionAccess.Denied)
            {
                return RedirectToAction("Denied", "Error");
            }
            if (!partialView)
                return View("../DT/Type/Shop/Index", model);
            else
                return PartialView("../DT/Type/Shop/PartialIndex", model);
        }

        [HttpGet]
        public ActionResult ShopTicketPopupForm(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var modelTicket = new Models.Views.DailyTicket.Form.TicketForm(ticket, true);
            var model = (Models.Views.DailyTicket.Form.ShopTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.ViewOnly = model.Access == DB.SessionAccess.View ? true : false;
            ViewBag.TicketForm = modelTicket;
            ViewBag.Partial = false;
            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";
            return PartialView("../DT/Type/Shop/Index", model);
        }
        #endregion
        #region Ticket Form
        [HttpGet]
        public ActionResult ShopTicketForm(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var modelTicket = new Models.Views.DailyTicket.Form.TicketForm(ticket, true);
            var model = (Models.Views.DailyTicket.Form.ShopTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.TicketForm = modelTicket;
            ViewBag.ViewOnly = model.Access == DB.SessionAccess.View;
            return PartialView("../DT/Type/Shop/Form/Form", model.Ticket);
        }

        [HttpGet]
        public ActionResult ShopTicketPanel(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var modelTicket = new Models.Views.DailyTicket.Form.TicketForm(ticket, true);
            var model = (Models.Views.DailyTicket.Form.ShopTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.TicketForm = modelTicket;
            ViewBag.ViewOnly = model.Access == DB.SessionAccess.View ? true : false;
            return PartialView("../DT/Type/Shop/Form/Panel", model.Ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ShopTicketUpdate(Models.Views.DailyTicket.DailyShopTicketViewModel model)
        {
            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            db.SaveChanges(ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult ShopTicketValidate(Models.Views.DailyTicket.DailyShopTicketViewModel model)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId);

            var form = new Models.Views.DailyTicket.Form.ShopTicketFormViewModel(ticket);
            model = new Models.Views.DailyTicket.DailyShopTicketViewModel(ticket);
            form.Validate(ModelState);

            if (!ModelState.IsValid)
            {
                var errorModel = ModelState.ModelErrors();
                return Json(new { success = ModelState.IsValidJson(), errorModel }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Crew Ticket Employee
        [HttpGet]
        public ActionResult ShopEmployeePanel(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.Where(f => f.DTCo == dtco && f.TicketId == ticketId).FirstOrDefault();

            var modelTicket = new Models.Views.DailyTicket.Form.TicketForm(ticket, true);
            var form = (Models.Views.DailyTicket.Form.ShopTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.ViewOnly = form.Access == DB.SessionAccess.View ? true : false;
            var model = form.Employees;
            return PartialView("../DT/Type/Shop/Employee/List/Panel", model);
        }

        [HttpGet]
        public ActionResult ShopEmployeeTable(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.Where(f => f.DTCo == dtco && f.TicketId == ticketId).FirstOrDefault();

            var modelTicket = new Models.Views.DailyTicket.Form.TicketForm(ticket, true);
            var form = (Models.Views.DailyTicket.Form.ShopTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.ViewOnly = form.Access == DB.SessionAccess.View ? true : false;
            var model = form.Employees;

            return PartialView("../DT/Type/Shop/Employee/List/Table", model);
        }

        [HttpGet]
        public PartialViewResult ShopEmployeeAdd(byte dtco, int ticketId)
        {
            ViewBag.tableRow = "True";
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

            var entry = ticket.DailyShopTicket.AddHoursEntry();
            var perdiem = ticket.DailyShopTicket.AddPerdiem();
            entry.EntryTypeId = 1;
            entry.JobId = ticket.DailyShopTicket.JobId;
            entry.PerdiemLineNum = perdiem.LineNum;

            db.SaveChanges(ModelState);

            var result = new Models.Views.DailyTicket.DailyShopEmployeeViewModel(entry);
            var results = new List<Models.Views.DailyTicket.DailyShopEmployeeViewModel>();
            results.Add(result);
            return PartialView("../DT/Type/Shop/Employee/List/Row/EmployeeRow", results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ShopEmployeeUpdate(Models.Views.DailyTicket.DailyShopEmployeeViewModel model)
        {
            if (model == null)
                throw new System.ArgumentNullException(nameof(model));

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ShopEmployeeDelete(byte dtco, int ticketId, int lineNum)
        {
            using var db = new VPContext();
            var entries = db.DailyEmployeeEntries.Where(f => f.DTCo == dtco && f.TicketId == ticketId).ToList();
            var delObj = entries.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId && f.LineNum == lineNum);

            if (delObj != null)
            {
                var perdiemLineNum = delObj.PerdiemLineNum;
                var employeeId = delObj.tEmployeeId;
                entries.Remove(delObj);
                db.DailyEmployeeEntries.Remove(delObj);

                var perdiems = db.DailyEmployeePerdiems.Where(f => f.DTCo == dtco && f.TicketId == ticketId && (f.tEmployeeId == employeeId || f.tEmployeeId == null)).ToList();
                foreach (var perdiem in perdiems)
                {
                    var perdiemWithEntries = entries.Where(f => f.DTCo == dtco && f.TicketId == ticketId && f.PerdiemLineNum == perdiem.LineNum).ToList();
                    if (!perdiemWithEntries.Any())
                    {
                        db.DailyEmployeePerdiems.Remove(perdiem);
                    }
                }


                db.SaveChanges(ModelState);
            }
            else
            {
                ModelState.AddModelError("", "Could not delete, line not found.");
            }
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ShopEmployeeValidate(Models.Views.DailyTicket.DailyShopEmployeeViewModel model)
        {
            using var db = new VPContext();
            var entry = db.DailyEmployeeEntries.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId && f.LineNum == model.LineNum);
            model = new Models.Views.DailyTicket.DailyShopEmployeeViewModel(entry);

            ModelState.Clear();
            this.TryValidateModel(model);
            model.Validate(ModelState);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion

        #region Truck Ticket Form
        #region Main Form
        [HttpGet]
        [Route("Ticket/Truck/{ticketId}-{dtco}")]
        public ActionResult TruckTicketIndex(byte dtco, int ticketId, bool partialView = false)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var modelTicket = new Models.Views.DailyTicket.Form.TicketForm(ticket, true);
            var model = (Models.Views.DailyTicket.Form.TruckTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.TicketForm = modelTicket;
            ViewBag.ViewOnly = model.Access == DB.SessionAccess.View;
            ViewBag.Partial = partialView;

            if (model.Access == DB.SessionAccess.Denied)
            {
                return RedirectToAction("Denied", "Error");
            }
            if (!partialView)
                return View("../DT/Type/Trucking/Index", model);
            else
                return PartialView("../DT/Type/Trucking/PartialIndex", model);
        }

        [HttpGet]
        public ActionResult TruckTicketPopupForm(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var modelTicket = new Models.Views.DailyTicket.Form.TicketForm(ticket, true);
            var model = (Models.Views.DailyTicket.Form.TruckTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.ViewOnly = model.Access == DB.SessionAccess.View ? true : false;
            ViewBag.TicketForm = modelTicket;
            ViewBag.Partial = false;
            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";
            return PartialView("../DT/Type/Trucking/Index", model);
        }
        #endregion
        #region Ticket Form
        [HttpGet]
        public ActionResult TruckTicketForm(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var modelTicket = new Models.Views.DailyTicket.Form.TicketForm(ticket, true);
            var model = (Models.Views.DailyTicket.Form.TruckTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.TicketForm = modelTicket;
            ViewBag.ViewOnly = model.Access == DB.SessionAccess.View;
            return PartialView("../DT/Type/Trucking/Form/Form", model.Ticket);
        }

        [HttpGet]
        public ActionResult TruckTicketPanel(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var modelTicket = new Models.Views.DailyTicket.Form.TicketForm(ticket, true);
            var model = (Models.Views.DailyTicket.Form.TruckTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.TicketForm = modelTicket;
            ViewBag.ViewOnly = model.Access == DB.SessionAccess.View ? true : false;
            return PartialView("../DT/Type/Trucking/Form/Panel", model.Ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TruckTicketUpdate(Models.Views.DailyTicket.DailyTruckTicketViewModel model)
        {
            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            db.SaveChanges(ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult TruckTicketValidate(Models.Views.DailyTicket.DailyTruckTicketViewModel model)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId);

            var form = new Models.Views.DailyTicket.Form.TruckTicketFormViewModel(ticket);
            model = new Models.Views.DailyTicket.DailyTruckTicketViewModel(ticket);
            form.Validate(ModelState);

            if (!ModelState.IsValid)
            {
                var errorModel = ModelState.ModelErrors();
                return Json(new { success = ModelState.IsValidJson(), errorModel }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Crew Ticket Employee
        [HttpGet]
        public ActionResult TruckEmployeePanel(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.Where(f => f.DTCo == dtco && f.TicketId == ticketId).FirstOrDefault();

            var modelTicket = new Models.Views.DailyTicket.Form.TicketForm(ticket, true);
            var form = (Models.Views.DailyTicket.Form.TruckTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.ViewOnly = form.Access == DB.SessionAccess.View ? true : false;
            var model = form.Employees;
            return PartialView("../DT/Type/Trucking/Employee/List/Panel", model);
        }

        [HttpGet]
        public ActionResult TruckEmployeeTable(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.Where(f => f.DTCo == dtco && f.TicketId == ticketId).FirstOrDefault();

            var modelTicket = new Models.Views.DailyTicket.Form.TicketForm(ticket, true);
            var form = (Models.Views.DailyTicket.Form.TruckTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.ViewOnly = form.Access == DB.SessionAccess.View ? true : false;
            var model = form.Employees;

            return PartialView("../DT/Type/Trucking/Employee/List/Table", model);
        }

        [HttpGet]
        public PartialViewResult TruckEmployeeAdd(byte dtco, int ticketId)
        {
            ViewBag.tableRow = "True";
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

            var entry = ticket.DailyTruckTicket.AddHoursEntry();
            var perdiem = ticket.DailyTruckTicket.AddPerdiem();
            entry.PerdiemLineNum = perdiem.LineNum;

            db.SaveChanges(ModelState);

            var result = new Models.Views.DailyTicket.DailyTruckEmployeeViewModel(entry);
            var results = new List<Models.Views.DailyTicket.DailyTruckEmployeeViewModel>();
            results.Add(result);
            return PartialView("../DT/Type/Trucking/Employee/List/Row/EmployeeRow", results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TruckEmployeeUpdate(Models.Views.DailyTicket.DailyTruckEmployeeViewModel model)
        {
            if (model == null)
                throw new System.ArgumentNullException(nameof(model));

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            db.SaveChanges(ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TruckEmployeeDelete(byte dtco, int ticketId, int lineNum)
        {
            using var db = new VPContext();
            var entries = db.DailyEmployeeEntries.Where(f => f.DTCo == dtco && f.TicketId == ticketId).ToList();
            var delObj = entries.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId && f.LineNum == lineNum);

            if (delObj != null)
            {
                var perdiemLineNum = delObj.PerdiemLineNum;
                var employeeId = delObj.tEmployeeId;
                entries.Remove(delObj);
                db.DailyEmployeeEntries.Remove(delObj);

                var perdiems = db.DailyEmployeePerdiems.Where(f => f.DTCo == dtco && f.TicketId == ticketId && (f.tEmployeeId == employeeId || f.tEmployeeId == null)).ToList();
                foreach (var perdiem in perdiems)
                {
                    var perdiemWithEntries = entries.Where(f => f.DTCo == dtco && f.TicketId == ticketId && f.PerdiemLineNum == perdiem.LineNum).ToList();
                    if (!perdiemWithEntries.Any())
                    {
                        db.DailyEmployeePerdiems.Remove(perdiem);
                    }
                }


                db.SaveChanges(ModelState);
            }
            else
            {
                ModelState.AddModelError("", "Could not delete, line not found.");
            }
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult TruckEmployeeValidate(Models.Views.DailyTicket.DailyTruckEmployeeViewModel model)
        {
            using var db = new VPContext();
            var entry = db.DailyEmployeeEntries.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId && f.LineNum == model.LineNum);
            model = new Models.Views.DailyTicket.DailyTruckEmployeeViewModel(entry);

            ModelState.Clear();
            this.TryValidateModel(model);
            model.Validate(ModelState);

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion

        [HttpGet]
        public ActionResult StatusLogTable(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

            var results = new Models.Views.DailyTicket.DailyStatusLogListViewModel(ticket);
            return PartialView("../DT/StatusLog/List/Table", results);
        }
    }
}