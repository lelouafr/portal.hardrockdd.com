using portal.Code.Data.VP;
using portal.Models.Views.HR.Resource;
using portal.Models.Views.PR.Employee.Form;
using portal.Repository.VP.HQ;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using portal.Code;

namespace portal.Controllers.View.HR
{
    [ControllerAccess]
    [ControllerAuthorize]
    public class HREmployeeController : BaseController
    {
        [HttpGet]
        [Route("HR/Employees")]
        public ActionResult Index()
        {
            using var db = new Code.Data.VP.VPEntities();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var emp = user.Employee.FirstOrDefault();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emp.HRCo);

            var results = new ResourceListViewModel();
            results.List.Add(new ResourceViewModel());

            results.HRCo = company.HQCo;
            ViewBag.DataController = "HREmployee";
            ViewBag.DataAction = "Data";
            return View("../HR/Employee/Summary/Index", results);
        }

        [HttpGet]
        public ActionResult Table()
        {
            using var db = new Code.Data.VP.VPEntities();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var emp = user.Employee.FirstOrDefault();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emp.HRCo);


            var results = new ResourceListViewModel();
            results.List.Add(new ResourceViewModel());

            results.HRCo = company.HQCo;
            ViewBag.DataController = "HREmployee";
            ViewBag.DataAction = "Data";
            return PartialView("../HR/Employee/Summary/List/Table", results);
        }

        [HttpGet]
        public ActionResult Data()//string statusId
        {
            using var db = new Code.Data.VP.VPEntities();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var emp = user.Employee.FirstOrDefault();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emp.HRCo);
            ResourceListViewModel results;
            results = new ResourceListViewModel(company);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);


            result.MaxJsonLength = int.MaxValue;
            return result;
        }

        [HttpPost]
        public ActionResult UploadProfilePicture(byte hrco, int employeeId)
        {
            using var db = new VPEntities();

            var employee = db.HRResources.FirstOrDefault(f => f.HRCo == hrco && f.HRRef == employeeId);
            if (employee != null)
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase fileUpload = Request.Files[fileName];
                    if (fileUpload.ContentType.Contains("image"))
                    {
                        var folder = employee.Attachment.GetRootFolder();
                        var attach = employee.Attachment.AddFile(fileUpload, 50005, folder.FolderId);
                    }
                }
                db.SaveChanges(ModelState);
            }

            return Json(new { Message = "Uploaded" });
        }



        [HttpGet]
        public ActionResult Termination(byte co, int employeeId)
        {
            using var db = new VPEntities();
            var entity = db.HRResources.First(f => f.HRCo == co && f.HRRef == employeeId);
            var results = new PayrollTerminationViewModel(entity);
            ViewBag.ViewOnly = false;
            return PartialView("../HR/Employee/Forms/Termination/Index", results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Termination(PayrollTerminationViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);
            if (ModelState.IsValid)
            {
                EmailTermedEmployee(model);
            }

            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }


        public void EmailTermedEmployee(PayrollTerminationViewModel model)
        {
            try
            {
                using var db = new VPEntities();
                var sendList = db.HRResources.Where(f => f.HRCo == model.Co && (f.PositionCode == "HR-MGR" || f.PositionCode == "HR-PRMGR" || f.PositionCode == "DIR-IT")).ToList();
                using MailMessage msg = new MailMessage()
                {
                    Body = EmailHelper.RenderViewToString(ControllerContext, "../HR/Employee/Forms/Termination/Email/Email", model, false),
                    IsBodyHtml = true,
                    Subject = string.Format(AppCultureInfo.CInfo(), "Termination Request for Employee  - {0}", model.EmployeeName),
                };

                foreach (var emp in sendList)
                {
                    msg.To.Add(new MailAddress(emp.CompanyEmail));
                }
                //msg.CC.Add(new MailAddress(model.RequestedUserEmail));

                EmailHelper.Send(msg);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.GetBaseException().ToString());
            }
        }

    }
}