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
    public class HRAssetController : BaseController
    {
        [HttpGet]
        [Route("HR/Asset")]
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
        public ActionResult UploadAssetPicture(byte hrco, int employeeId)
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



    }
}