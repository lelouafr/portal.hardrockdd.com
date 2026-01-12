using Newtonsoft.Json;
using portal.Code.Data.VP;
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

namespace portal.Controllers
{

    public class ApplicationController : BaseController
    {
        [HttpGet]
        [Route("Web/Applicants")]
        public ActionResult Index()
        {
            using var db = new VPEntities();
            var applications = db.WebApplicants
                .Include("Applications")
                .Include("Applications.AppliedPositions")
                .Include("Applications.AppliedPositions.HRPositionCode")
                .ToList();
            var result = new Models.Views.WA.Applicant.ApplicantListViewModel(applications);

            ViewBag.TableAction = "Table";
            ViewBag.DataAction = "AllData";
            return View("../HR/Applicant/Index", result);
        }

        [HttpGet]
        public ActionResult Table()
        {
            using var db = new VPEntities();
            var applications = db.WebApplicants
                .Include("Applications")
                .Include("Applications.AppliedPositions")
                .Include("Applications.AppliedPositions.HRPositionCode")
                .ToList();
            var result = new Models.Views.WA.Applicant.ApplicantListViewModel(applications);

            ViewBag.TableAction = "Table";
            ViewBag.DataAction = "AllData";

            return PartialView("../HR/Applicant/Table", result);
        }

        [HttpGet]
        public ActionResult AllData()//string statusId
        {
            using var db = new VPEntities();
            var applications = db.WebApplicants
                .Include("Applications")
                .Include("Applications.AppliedPositions")
                .Include("Applications.AppliedPositions.HRPositionCode")
                .ToList();
            var results = new Models.Views.WA.Applicant.ApplicantListViewModel(applications);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;
            return result;
        }

        [HttpGet]
        [Route("Web/Applicants/Open")]
        public ActionResult OpenIndex()
        {
            using var db = new VPEntities();
            var applications = db.WebApplicants
                .Include("Applications")
                .Include("Applications.AppliedPositions")
                .Include("Applications.AppliedPositions.HRPositionCode")
                .Where(s => s.Applications.Any(a => a.tStatusId == (int)WAApplicationStatusEnum.Submitted || a.tStatusId == (int)WAApplicationStatusEnum.Approved))
                .ToList();
            var result = new Models.Views.WA.Applicant.ApplicantListViewModel(applications);
            ViewBag.TableAction = "OpenTable";
            ViewBag.DataAction = "OpenData";
            return View("../HR/Applicant/Index", result);
        }

        [HttpGet]
        public ActionResult OpenTable()
        {
            using var db = new VPEntities();
            var applications = db.WebApplicants
                .Include("Applications")
                .Include("Applications.AppliedPositions")
                .Include("Applications.AppliedPositions.HRPositionCode")
                .Where(s => s.Applications.Any(a => a.tStatusId == (int)WAApplicationStatusEnum.Submitted || a.tStatusId == (int)WAApplicationStatusEnum.Approved))
                .ToList();
            var result = new Models.Views.WA.Applicant.ApplicantListViewModel(applications);
            ViewBag.TableAction = "OpenTable";
            ViewBag.DataAction = "OpenData";

            return PartialView("../HR/Applicant/Table", result);
        }
        [HttpGet]
        public ActionResult OpenData()//string statusId
        {
            using var db = new VPEntities();
            var applications = db.WebApplicants
                .Include("Applications")
                .Include("Applications.AppliedPositions")
                .Include("Applications.AppliedPositions.HRPositionCode")
                .Where(s => s.Applications.Any(a => a.tStatusId == (int)WAApplicationStatusEnum.Submitted || a.tStatusId == (int)WAApplicationStatusEnum.Approved))
                .ToList();
            var results = new Models.Views.WA.Applicant.ApplicantListViewModel(applications);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;
            return result;
        }

        #region Web Applicant Form

        [HttpGet]
        public ActionResult Applicant(int applicantId)
        {
            using var db = new VPEntities();
            var result = db.WebApplicants.FirstOrDefault(f => f.ApplicantId == applicantId);

            var model = new Models.Views.WA.Applicant.FormViewModel(result);


            return PartialView("../HR/Applicant/Form/Panel", model);
        }

        [HttpGet]
        public ActionResult ApplicantForm(int applicantId)
        {
            using var db = new VPEntities();
            var result = db.WebApplicants.FirstOrDefault(f => f.ApplicantId == applicantId);

            var model = new Models.Views.WA.Applicant.FormViewModel(result);


            return PartialView("../HR/Applicant/Form/Form", model);
        }

        [HttpGet]
        public ActionResult ApplicantInfoForm(int applicantId)
        {
            using var db = new VPEntities();
            var result = db.WebApplicants.FirstOrDefault(f => f.ApplicantId == applicantId);

            var model = new Models.Views.WA.Applicant.ApplicantInfoViewModel(result);


            return PartialView("../HR/Applicant/Form/Info/Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ApplicantInfoUpdate(Models.Views.WA.Applicant.ApplicantInfoViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            using var db = new VPEntities();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EmergencyContactUpdate(Models.Views.WA.Applicant.EmergencyContactViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            using var db = new VPEntities();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult LicenseInfoUpdate(Models.Views.WA.Applicant.LicenseInfoViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            using var db = new VPEntities();
            model = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        #endregion

        #region Application Forms

        [HttpGet]
        public ActionResult ApplicationTable(int applicantId)
        {
            using var db = new VPEntities();
            var applicant = db.WebApplicants.FirstOrDefault(s => s.ApplicantId == applicantId);
            var result = new Models.Views.WA.Application.ApplicationListViewModel(applicant);

            return PartialView("../HR/Applicant/Form/Application/List/Table", result);
        }

        [HttpGet]
        //[Route("Web/Application/Popup/{applicantId}")]
        public ActionResult ApplicationPopup(int applicantId)
        {
            //using var db = new VPEntities();
            //var applicant = db.WebApplicants.FirstOrDefault(s => s.ApplicantId == applicantId);
            //var result = new Models.Views.WA.Application.ApplicationListViewModel(applicant);
            //ViewBag.DisplayOnly = true;

            //return View("../HR/Applicant/Form/Application/List/Table", result);

            using var db = new VPEntities();
            var result = db.WebApplicants.FirstOrDefault(f => f.ApplicantId == applicantId);

            var model = new Models.Views.WA.Applicant.FormViewModel(result);
            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";
            return PartialView("../HR/Applicant/Form/Index", model);
        }

        [HttpGet]
        public ActionResult ApplicationPanel(int applicantId, int applicationId)
        {
            using var db = new VPEntities();
            var applicant = db.WebApplications.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId);
            var result = new Models.Views.WA.Application.FormViewModel(applicant);

            return PartialView("../HR/Applicant/Form/Application/Form/Panel", result);
        }

        [HttpGet]
        public ActionResult ApplicationForm(int applicantId, int applicationId)
        {
            using var db = new VPEntities();
            var applicant = db.WebApplications.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId);
            var result = new Models.Views.WA.Application.FormViewModel(applicant);

            return PartialView("../HR/Applicant/Form/Application/Form/Form", result);
        }



        #region WorkHistory
        [HttpGet]
        public ActionResult ApplicationWorkHistoryTable(int applicantId, int applicationId)
        {
            using var db = new VPEntities();
            var applicant = db.WebApplications.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId);
            var result = new Models.Views.WA.Application.WorkHistoryListViewModel(applicant);

            return PartialView("../HR/Applicant/Form/Application/Form/WorkHistory/FormList", result);
        }

        [HttpGet]
        public ActionResult ApplicationWorkHistoryAdd(int applicantId, int applicationId)
        {
            using var db = new VPEntities();
            var applicant = db.WebApplications.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId);
            var result = new Models.Views.WA.Application.WorkHistoryViewModel(applicant.AddWorkHistory());
            db.BulkSaveChanges();

            return PartialView("../HR/Applicant/Form/Application/Form/WorkHistory/Form", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApplicationWorkHistoryDelete(int applicantId, int applicationId, int seqId)
        {
            using var db = new VPEntities();
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
        public ActionResult ApplicationWorkHistoryUpdate(Models.Views.WA.Application.WorkHistoryViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            using var db = new VPEntities();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult ApplicationWorkHistoryValidate(Models.Views.WA.Application.WorkHistoryViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region EquipmentHistory
        [HttpGet]
        public ActionResult ApplicationEquipmentHistoryTable(int applicantId, int applicationId)
        {
            using var db = new VPEntities();
            var applicant = db.WebApplications.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId);
            var result = new Models.Views.WA.Application.EquipmentHistoryListViewModel(applicant);

            return PartialView("../HR/Applicant/Form/Application/Form/EquipmentHistory/Table", result);
        }

        [HttpGet]
        public ActionResult ApplicationEquipmentHistoryAdd(int applicantId, int applicationId)
        {
            using var db = new VPEntities();
            var applicant = db.WebApplications.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId);
            var result = new Models.Views.WA.Application.EquipmentHistoryViewModel(applicant.AddDrivingExperience());
            db.BulkSaveChanges();

            return PartialView("../HR/Applicant/Form/Application/Form/EquipmentHistory/TableRow", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApplicationEquipmentHistoryDelete(int applicantId, int applicationId, int seqId)
        {
            using var db = new VPEntities();
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
        public ActionResult ApplicationEquipmentHistoryUpdate(Models.Views.WA.Application.EquipmentHistoryViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            using var db = new VPEntities();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult ApplicationEquipmentHistoryValidate(Models.Views.WA.Application.EquipmentHistoryViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region TrafficCitation
        [HttpGet]
        public ActionResult ApplicationTrafficCitationTable(int applicantId, int applicationId)
        {
            using var db = new VPEntities();
            var applicant = db.WebApplications.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId);
            var result = new Models.Views.WA.Application.TrafficCitationListViewModel(applicant);

            return PartialView("../HR/Applicant/Form/Application/Form/TrafficCitation/Table", result);
        }

        [HttpGet]
        public ActionResult ApplicationTrafficCitationAdd(int applicantId, int applicationId)
        {
            using var db = new VPEntities();
            var applicant = db.WebApplications.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId);
            var result = new Models.Views.WA.Application.TrafficCitationViewModel(applicant.AddTrafficTicket());
            db.BulkSaveChanges();

            return PartialView("../HR/Applicant/Form/Application/Form/TrafficCitation/TableRow", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApplicationTrafficCitationDelete(int applicantId, int applicationId, int seqId)
        {
            using var db = new VPEntities();
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
        public ActionResult ApplicationTrafficCitationUpdate(Models.Views.WA.Application.TrafficCitationViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            using var db = new VPEntities();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult ApplicationTrafficCitationValidate(Models.Views.WA.Application.TrafficCitationViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region TrafficAccident
        [HttpGet]
        public ActionResult ApplicationTrafficAccidentTable(int applicantId, int applicationId)
        {
            using var db = new VPEntities();
            var applicant = db.WebApplications.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId);
            var result = new Models.Views.WA.Application.TrafficAccidentListViewModel(applicant);

            return PartialView("../HR/Applicant/Form/Application/Form/TrafficAccident/Table", result);
        }

        [HttpGet]
        public ActionResult ApplicationTrafficAccidentAdd(int applicantId, int applicationId)
        {
            using var db = new VPEntities();
            var applicant = db.WebApplications.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId);
            var result = new Models.Views.WA.Application.TrafficAccidentViewModel(applicant.AddTrafficAccident());
            db.BulkSaveChanges();

            return PartialView("../HR/Applicant/Form/Application/Form/TrafficAccident/TableRow", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApplicationTrafficAccidentDelete(int applicantId, int applicationId, int seqId)
        {
            using var db = new VPEntities();
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
        public ActionResult ApplicationTrafficAccidentUpdate(Models.Views.WA.Application.TrafficAccidentViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            using var db = new VPEntities();
            model = model.ProcessUpdate(db, ModelState);
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult ApplicationTrafficAccidentValidate(Models.Views.WA.Application.TrafficAccidentViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #endregion


        #region Request Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(int applicantId, int applicationId, int statusId)
        {
            using var db = new Code.Data.VP.VPEntities();

            var applicant = db.WebApplicants.FirstOrDefault(s => s.ApplicantId == applicantId);
            var application = applicant.Applications.FirstOrDefault(s => s.ApplicantId == applicantId && s.ApplicationId == applicationId);
            if (application != null)
            {
                var form = new Models.Views.WA.Applicant.FormViewModel(applicant);
                form.Validate(ModelState);

                application.Status = (WAApplicationStatusEnum)statusId;
                db.SaveChanges(ModelState);
            }

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
        }
        #endregion
    }
}