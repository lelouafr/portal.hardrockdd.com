using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Applicant.Controllers
{
    [RouteArea("Applicant")]
    public class ApplicantController : portal.Controllers.BaseController
    {
        #region Applicant List
        [HttpGet]
        [Route("Applicants")]
        public ActionResult Index()
        {
            using var db = new VPContext();
            var applications = db.WebApplicants
                .Include("Applications")
                .Include("Applications.AppliedPositions")
                .Include("Applications.AppliedPositions.HRPositionCode")
                .ToList();
            var result = new Models.ApplicantListViewModel(applications);

            ViewBag.TableAction = "Table";
            ViewBag.DataAction = "AllData";
            return View("List/Index", result);
        }

        [HttpGet]
        public ActionResult Table()
        {
            using var db = new VPContext();
            var applications = db.WebApplicants
                .Include("Applications")
                .Include("Applications.AppliedPositions")
                .Include("Applications.AppliedPositions.HRPositionCode")
                .ToList();
            var result = new Models.ApplicantListViewModel(applications);

            ViewBag.TableAction = "Table";
            ViewBag.DataAction = "AllData";

            return PartialView("List/_Table", result);
        }

        [HttpGet]
        public ActionResult AllData()//string statusId
        {
            using var db = new VPContext();
            var applications = db.WebApplicants
                .Include("Applications")
                .Include("Applications.AppliedPositions")
                .Include("Applications.AppliedPositions.HRPositionCode")
                .ToList();
            var results = new Models.ApplicantListViewModel(applications);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;
            return result;
        }
        #endregion

        #region Open Applicant
        [HttpGet]
        [Route("Applicants/Open")]
        public ActionResult OpenIndex()
        {
            using var db = new VPContext();
            var applications = db.WebApplicants
                .Include("Applications")
                .Include("Applications.AppliedPositions")
                .Include("Applications.AppliedPositions.HRPositionCode")
                .Where(s => s.Applications.Any(a => a.tStatusId == (int)DB.WAApplicationStatusEnum.Submitted || a.tStatusId == (int)DB.WAApplicationStatusEnum.Approved))
                .ToList();
            var result = new Models.ApplicantListViewModel(applications);
            ViewBag.TableAction = "OpenTable";
            ViewBag.DataAction = "OpenData";
            return View("List/Index", result);
        }

        [HttpGet]
        public ActionResult OpenTable()
        {
            using var db = new VPContext();
            var applications = db.WebApplicants
                .Include("Applications")
                .Include("Applications.AppliedPositions")
                .Include("Applications.AppliedPositions.HRPositionCode")
                .Where(s => s.Applications.Any(a => a.tStatusId == (int)DB.WAApplicationStatusEnum.Submitted || a.tStatusId == (int)DB.WAApplicationStatusEnum.Approved))
                .ToList();
            var result = new Models.ApplicantListViewModel(applications);
            ViewBag.TableAction = "OpenTable";
            ViewBag.DataAction = "OpenData";

            return PartialView("List/_Table", result);
        }
        [HttpGet]
        public ActionResult OpenData()//string statusId
        {
            using var db = new VPContext();
            var applications = db.WebApplicants
                .Include("Applications")
                .Include("Applications.AppliedPositions")
                .Include("Applications.AppliedPositions.HRPositionCode")
                .Where(s => s.Applications.Any(a => a.tStatusId == (int)DB.WAApplicationStatusEnum.Submitted || a.tStatusId == (int)DB.WAApplicationStatusEnum.Approved))
                .ToList();
            var results = new Models.ApplicantListViewModel(applications);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;
            return result;
        }
		#endregion

		#region Web Applicant Form
		[HttpGet]
		[Route("Application/{applicantId}")]
		public ActionResult ApplicationIndex(int applicantId)
		{
			using var db = new VPContext();
			var result = db.WebApplicants.FirstOrDefault(f => f.ApplicantId == applicantId);

			var model = new Models.FormViewModel(result);
            ViewBag.ExpandAllPanels = true;
			return View("Form/Index", model);
		}

		[HttpGet]
		public ActionResult Panel(int applicantId)
		{
			using var db = new VPContext();
			var result = db.WebApplicants.FirstOrDefault(f => f.ApplicantId == applicantId);

			var model = new Models.FormViewModel(result);


			return PartialView("Form/_Panel", model);
		}

		[HttpGet]
        public ActionResult Form(int applicantId)
        {
            using var db = new VPContext();
            var result = db.WebApplicants.FirstOrDefault(f => f.ApplicantId == applicantId);

            var model = new Models.FormViewModel(result);


            return PartialView("Form/_Form", model);
        }
        #endregion

        #region Applicant Info
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult InfoUpdate(Models.ApplicantInfoViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EmergencyContactUpdate(Models.EmergencyContactViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult LicenseInfoUpdate(Models.LicenseInfoViewModel model)
		{
			if (model == null)
				throw new ArgumentNullException(nameof(model));

			using var db = new VPContext();
			model = model.ProcessUpdate(db, ModelState);

			return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult HRNoteUpdate(Models.NotesViewModel model)
		{
			if (model == null)
				throw new ArgumentNullException(nameof(model));

			using var db = new VPContext();
			model = model.ProcessUpdate(db, ModelState);

			return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
		}
		#endregion

		#region Application Forms

		[HttpGet]
        public ActionResult ApplicationTable(int applicantId)
        {
            using var db = new VPContext();
            var applicant = db.WebApplicants.FirstOrDefault(s => s.ApplicantId == applicantId);
            var result = new Models.ApplicationListViewModel(applicant);

            return PartialView("Application/List/_Table", result);
        }

        [HttpGet]
        public ActionResult ApplicationPopup(int applicantId)
        {
            //using var db = new VPContext();
            //var applicant = db.WebApplicants.FirstOrDefault(s => s.ApplicantId == applicantId);
            //var result = new Models.ApplicationListViewModel(applicant);
            //ViewBag.DisplayOnly = true;

            //return View("Form/Application/List/Table", result);

            using var db = new VPContext();
            var result = db.WebApplicants.FirstOrDefault(f => f.ApplicantId == applicantId);

            var model = new Models.FormViewModel(result);
            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";
            return PartialView("Form/Index", model);
        }

        [HttpGet]
        public ActionResult ApplicationPanel(int applicantId, int applicationId)
        {
            using var db = new VPContext();
            var applicant = db.WebApplications.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId);
            var result = new Models.ApplicationFormViewModel(applicant);

            return PartialView("Application/Form/_Panel", result);
        }

        [HttpGet]
        public ActionResult ApplicationForm(int applicantId, int applicationId)
        {
            using var db = new VPContext();
            var applicant = db.WebApplications.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId);
            var result = new Models.ApplicationFormViewModel(applicant);

            return PartialView("Application/Form/_Form", result);
        }



        #region Work History
        [HttpGet]
        public ActionResult WorkHistoryTable(int applicantId, int applicationId)
        {
            using var db = new VPContext();
            var applicant = db.WebApplications.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId);
            var result = new Models.WorkHistoryListViewModel(applicant);

            return PartialView("Application/Form/WorkHistory/_FormList", result);
        }

        [HttpGet]
        public ActionResult WorkHistoryAdd(int applicantId, int applicationId)
        {
            using var db = new VPContext();
            var applicant = db.WebApplications.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId);
            var result = new Models.WorkHistoryViewModel(applicant.AddWorkHistory());
            db.BulkSaveChanges();

            return PartialView("Application/Form/WorkHistory/_Form", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WorkHistoryDelete(int applicantId, int applicationId, int seqId)
        {
            using var db = new VPContext();
            var delObj = db.WAWorkExperiences.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId && s.SeqId == seqId);
            if (delObj != null)
            {
                try
                {
                    db.WAWorkExperiences.Remove(delObj);
                    db.BulkSaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Object could not be removed!");
                }
            }
            else
            {
                ModelState.AddModelError("", "Object doesn't exisit!");
            }
;
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WorkHistoryUpdate(Models.WorkHistoryViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult WorkHistoryValidate(Models.WorkHistoryViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Equipment History
        [HttpGet]
        public ActionResult EquipmentHistoryTable(int applicantId, int applicationId)
        {
            using var db = new VPContext();
            var applicant = db.WebApplications.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId);
            var result = new Models.EquipmentHistoryListViewModel(applicant);

            return PartialView("Application/Form/EquipmentHistory/_Table", result);
        }

        [HttpGet]
        public ActionResult EquipmentHistoryAdd(int applicantId, int applicationId)
        {
            using var db = new VPContext();
            var applicant = db.WebApplications.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId);
            var result = new Models.EquipmentHistoryViewModel(applicant.AddDrivingExperience());
            db.BulkSaveChanges();

            return PartialView("Application/Form/EquipmentHistory/_TableRow", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EquipmentHistoryDelete(int applicantId, int applicationId, int seqId)
        {
            using var db = new VPContext();
            var delObj = db.WADrivingExperiences.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId && s.SeqId == seqId);
            if (delObj != null)
            {
                try
                {
                    db.WADrivingExperiences.Remove(delObj);
                    db.BulkSaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Object could not be removed!");
                }
            }
            else
            {
                ModelState.AddModelError("", "Object doesn't exisit!");
            }
;
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EquipmentHistoryUpdate(Models.EquipmentHistoryViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult EquipmentHistoryValidate(Models.EquipmentHistoryViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Traffic Citation
        [HttpGet]
        public ActionResult TrafficCitationTable(int applicantId, int applicationId)
        {
            using var db = new VPContext();
            var applicant = db.WebApplications.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId);
            var result = new Models.TrafficCitationListViewModel(applicant);

            return PartialView("Application/Form/TrafficCitation/_Table", result);
        }

        [HttpGet]
        public ActionResult TrafficCitationAdd(int applicantId, int applicationId)
        {
            using var db = new VPContext();
            var applicant = db.WebApplications.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId);
            var result = new Models.TrafficCitationViewModel(applicant.AddTrafficTicket());
            db.BulkSaveChanges();

            return PartialView("Application/Form/TrafficCitation/_TableRow", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrafficCitationDelete(int applicantId, int applicationId, int seqId)
        {
            using var db = new VPContext();
            var delObj = db.WADrivingExperiences.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId && s.SeqId == seqId);
            if (delObj != null)
            {
                try
                {
                    db.WADrivingExperiences.Remove(delObj);
                    db.BulkSaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Object could not be removed!");
                }
            }
            else
            {
                ModelState.AddModelError("", "Object doesn't exisit!");
            }
;
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrafficCitationUpdate(Models.TrafficCitationViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult TrafficCitationValidate(Models.TrafficCitationViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Traffic Accident
        [HttpGet]
        public ActionResult TrafficAccidentValidateTable(int applicantId, int applicationId)
        {
            using var db = new VPContext();
            var applicant = db.WebApplications.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId);
            var result = new Models.TrafficAccidentListViewModel(applicant);

            return PartialView("Application/Form/TrafficAccident/_Table", result);
        }

        [HttpGet]
        public ActionResult TrafficAccidentValidateAdd(int applicantId, int applicationId)
        {
            using var db = new VPContext();
            var applicant = db.WebApplications.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId);
            var result = new Models.TrafficAccidentViewModel(applicant.AddTrafficAccident());
            db.BulkSaveChanges();

            return PartialView("Application/Form/TrafficAccident/_TableRow", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrafficAccidentValidateDelete(int applicantId, int applicationId, int seqId)
        {
            using var db = new VPContext();
            var delObj = db.WADrivingExperiences.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId && s.SeqId == seqId);
            if (delObj != null)
            {
                try
                {
                    db.WADrivingExperiences.Remove(delObj);
                    db.BulkSaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Object could not be removed!");
                }
            }
            else
            {
                ModelState.AddModelError("", "Object doesn't exisit!");
            }

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrafficAccidentValidateUpdate(Models.TrafficAccidentViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult TrafficAccidentValidate(Models.TrafficAccidentViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Applied Positions
        [HttpGet]
        public ActionResult AppliedPositionsPanel(int applicantId, int applicationId)
        {
            using var db = new VPContext();
            var applicant = db.WebApplications.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId);
            var result = new Models.PositionListViewModel(applicant);

            return PartialView("Application/Form/AppliedPosition/_Panel", result);
        }
        [HttpGet]
        public ActionResult AppliedPositionsTable(int applicantId, int applicationId)
        {
            using var db = new VPContext();
            var applicant = db.WebApplications.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId);
            var result = new Models.PositionListViewModel(applicant);

            return PartialView("Application/Form/AppliedPosition/_Table", result);
        }

        [HttpGet]
        public ActionResult AppliedPositionsAdd(int applicantId, int applicationId)
        {
            using var db = new VPContext();
            var applicant = db.WebApplications.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId);
            var result = new Models.PositionViewModel(applicant.AddPosition());
            db.BulkSaveChanges();
            ViewBag.TableRow = true;
            return PartialView("Application/Form/AppliedPosition/_TableRow", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AppliedPositionsDelete(int applicantId, int applicationId, int seqId)
        {
            using var db = new VPContext();
            var delObj = db.WAAppliedPositions.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId && s.SeqId == seqId);
            if (delObj != null)
            {
                try
                {
                    db.WAAppliedPositions.Remove(delObj);
                    db.BulkSaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Object could not be removed!");
                }
            }
            else
            {
                ModelState.AddModelError("", "Object doesn't exisit!");
            }
;
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AppliedPositionsUpdate(Models.PositionViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            using var db = new VPContext();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult AppliedPositionsValidate(Models.PositionViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #endregion

        #region Request Actionsx 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(int applicantId, int applicationId, int statusId)
        {
            using var db = new VPContext();

            var applicant = db.WebApplicants.FirstOrDefault(s => s.ApplicantId == applicantId);
            var application = applicant.Applications.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId);
            if (application != null)
            {
                var form = new Models.FormViewModel(applicant);
                form.Validate(ModelState);

                application.Status = (DB.WAApplicationStatusEnum)statusId;
                db.SaveChanges(ModelState);
            }

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }
        #endregion

        #region Reports
        #region By Position
        [HttpGet]
        public ActionResult PositionReportPanel()
        {
            var result = new Models.Report.SummaryByPositionListViewModel();
            result.Status = DB.WAApplicationStatusEnum.Submitted;
            ViewBag.TableAction = "PositionReportTable";
            ViewBag.DataAction = "PositionReportData";
            return PartialView("Report/Positions/_Panel", result);
        }

        [HttpGet]
        public ActionResult PositionReportTable()
        {
            var result = new Models.Report.SummaryByPositionListViewModel();
            result.Status = DB.WAApplicationStatusEnum.Submitted;
            ViewBag.TableAction = "PositionReportTable";
            ViewBag.DataAction = "PositionReportData";

            return PartialView("Report/Positions/_Table", result);
        }

        [HttpGet]
        public ActionResult PositionReportData(int? statusId = 2)//string statusId
        {
            var appStatus = (DB.WAApplicationStatusEnum)statusId;
            using var db = new VPContext();
            var applications = db.WebApplicants
                .Include("Applications")
                .Include("Applications.AppliedPositions")
                .Include("Applications.AppliedPositions.HRPositionCode")
                .Where(f => f.Applications.Any(a => a.tStatusId == (int)appStatus))
                .ToList();
            var results = new Models.Report.SummaryByPositionListViewModel(applications);
            results.Status = appStatus;

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;
            return result;
        }

        #endregion
        #endregion
    }
}