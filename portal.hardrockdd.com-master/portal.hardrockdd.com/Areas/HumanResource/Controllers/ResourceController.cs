using portal.Areas.HumanResource.Models.Resource;
using DB.Infrastructure.ViewPointDB.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Areas.HumanResource.Controllers
{
    [RouteArea("HumanResource")]
    public class ResourceController : portal.Controllers.BaseController
    {
        #region All Employees
        [HttpGet]
        [Route("Employees")]
        public ActionResult Index()
        {
            using var db = new VPContext();
            var company = db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo);

            var results = new ResourceListViewModel();
            results.List.Add(new ResourceViewModel());

            results.HRCo = company.HQCo;
            ViewBag.DataController = "Resource";
            ViewBag.DataAction = "Data";
            ViewBag.TableAction = "Table";
            return View("List/Index", results);
        }

        [HttpGet]
        public PartialViewResult Table()
        {
            using var db = new VPContext();
            var company = db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo);

            var results = new ResourceListViewModel();
            results.List.Add(new ResourceViewModel());

            results.HRCo = company.HQCo;
            ViewBag.DataController = "Resource";
            ViewBag.DataAction = "Data";
            ViewBag.TableAction = "Table";
            return PartialView("List/_Table", results);
        }

        [HttpGet]
        public JsonResult Data()
        {
            using var db = new VPContext();
            var results = new ResourceListViewModel(db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo));
            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }
        #endregion

        #region Active Employees
        [HttpGet]
        [Route("Employees/Active")]
        public ActionResult ActiveIndex()
        {
            using var db = new VPContext();
            var company = db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo);

            var results = new ResourceListViewModel();
            results.List.Add(new ResourceViewModel());

            results.HRCo = company.HQCo;
            ViewBag.DataController = "Resource";
            ViewBag.DataAction = "ActiveData";
            ViewBag.TableAction = "ActiveTable";
            return View("List/Index", results);
        }

        [HttpGet]
        public PartialViewResult ActiveTable()
        {
            using var db = new VPContext();
            var company = db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo);

            var results = new ResourceListViewModel();
            results.List.Add(new ResourceViewModel());

            results.HRCo = company.HQCo;
            ViewBag.DataController = "Resource";
            ViewBag.DataAction = "ActiveData";
            ViewBag.TableAction = "ActiveTable";
            return PartialView("List/_Table", results);
        }

        [HttpGet]
        public JsonResult ActiveData()
        {
            using var db = new VPContext();
            var results = new ResourceListViewModel(db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo), "Y");
            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }
        #endregion

        #region Termed Employees
        [HttpGet]
        [Route("Employees/Termed")]
        public ActionResult TermedIndex()
        {
            using var db = new VPContext();
            var company = db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo);

            var results = new ResourceListViewModel();
            results.List.Add(new ResourceViewModel());

            results.HRCo = company.HQCo;
            ViewBag.DataController = "Resource";
            ViewBag.DataAction = "TermedData";
            ViewBag.TableAction = "TermedTable";
            return View("List/Index", results);
        }

        [HttpGet]
        public PartialViewResult TermedTable()
        {
            using var db = new VPContext();
            var company = db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo);

            var results = new ResourceListViewModel();
            results.List.Add(new ResourceViewModel());

            results.HRCo = company.HQCo;
            ViewBag.DataController = "Resource";
            ViewBag.DataAction = "TermedData";
            ViewBag.TableAction = "TermedTable";
            return PartialView("List/_Table", results);
        }

        [HttpGet]
        public JsonResult TermedData()
        {
            using var db = new VPContext();
            var results = new ResourceListViewModel(db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo), "N");
            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }
        #endregion

        #region Direct Report Employees
        [HttpGet]
        //[Route("Employees/Direct")]
        public ActionResult DirectIndex()
        {
            using var db = new VPContext();
            var company = db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo);

            var results = new ResourceListViewModel();
            results.List.Add(new ResourceViewModel());

            results.HRCo = company.HQCo;
            ViewBag.DataController = "Resource";
            ViewBag.DataAction = "DirectData";
            return View("Index", results);
        }

        [HttpGet]
        public PartialViewResult DirectTable()
        {
            using var db = new VPContext();
            var company = db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo);

            var results = new ResourceListViewModel();
            results.List.Add(new ResourceViewModel());

            results.HRCo = company.HQCo;
            ViewBag.DataController = "Resource";
            ViewBag.DataAction = "DirectData";
            return PartialView("List/_Table", results);
        }

        [HttpGet]
        public JsonResult DirectData()
        {
            using var db = new VPContext();
            var results = new ResourceListViewModel(db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo), db.GetCurrentHREmployee());
            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }
        #endregion

        #region Resource Form
        [HttpGet]
        public PartialViewResult Panel(byte hrco, int employeeId)
        {
            using var db = new VPContext();
            var entity = db.HRResources.FirstOrDefault(f => f.HRCo == hrco && f.HRRef == employeeId);
            if (!entity.Attachment.GetRootFolder().SubFolders.Any())
            {
                entity.Attachment.BuildDefaultFolders();
                db.BulkSaveChanges();
            }

            var result = new Models.Resource.Form.FormViewModel(entity);

            return PartialView("Forms/_Panel", result);
        }

        [HttpGet]
        public PartialViewResult Form(byte hrco, int employeeId)
        {
            using var db = new VPContext();
            var entity = db.HRResources.FirstOrDefault(f => f.HRCo == hrco && f.HRRef == employeeId);
            var result = new Models.Resource.Form.FormViewModel(entity);

            return PartialView("Forms/_Form", result);
        }


        [HttpGet]
        public ActionResult PopupForm(byte hrco, int employeeId)
        {

            using var db = new VPContext();
            var entity = db.HRResources.FirstOrDefault(f => f.HRCo == hrco && f.HRRef == employeeId);
            var result = new Models.Resource.Form.FormViewModel(entity);

            ViewBag.LayOut = "~/Views/Shared/_LayoutPopup.cshtml";
            return View("Forms/Index", result);
        }

        [HttpGet]
        public JsonResult ResourceValidate(Models.Resource.Form.FormViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Info Form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult InfoUpdate(Models.Resource.Form.InfoViewModel model)
        {
            if (model != null)
            {
                using var db = new VPContext();
                model = model.ProcessUpdate(ModelState, db);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult InfoValidate(Models.Resource.Form.InfoViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region AD Info Form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ADInfoUpdate(Models.Resource.Form.ADInfoViewModel model)
        {
            if (model != null)
            {
                using var db = new VPContext();
                model = model.ProcessUpdate(ModelState, db);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult ADInfoValidate(Models.Resource.Form.ADInfoViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Assignment Form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AssignmentUpdate(Models.Resource.Form.AssignmentViewModel model)
        {
            if (model != null)
            {
                using var db = new VPContext();
                model = model.ProcessUpdate(ModelState, db);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult AssignmentValidate(Models.Resource.Form.AssignmentViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Driving Form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DrivingInfoUpdate(Models.Resource.Form.DrivingInfoViewModel model)
        {
            if (model != null)
            {
                using var db = new VPContext();
                model = model.ProcessUpdate(ModelState, db);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult DrivingInfoValidate(Models.Resource.Form.DrivingInfoViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Pay History Form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult PayrollHistoryUpdate(Models.Resource.Form.PayrollHistoryViewModel model)
        {
            if (model != null)
            {
                using var db = new VPContext();
                model = model.ProcessUpdate(ModelState, db);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult PayrollHistoryValidate(Models.Resource.Form.PayrollHistoryViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Pay Info Form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult PayInfoUpdate(Models.Resource.Form.PayInfoViewModel model)
        {
            if (model != null)
            {
                using var db = new VPContext();
                model = model.ProcessUpdate(ModelState, db);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult PayInfoValidate(Models.Resource.Form.PayInfoViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Personal Info Form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult PersonalInfoUpdate(Models.Resource.Form.PersonalInfoViewModel model)
        {
            if (model != null)
            {
                using var db = new VPContext();
                model = model.ProcessUpdate(ModelState, db);
            }
            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }
        
        [HttpGet]
        public JsonResult PersonalInfoValidate(Models.Resource.Form.PersonalInfoViewModel model)
        {
            return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UploadProfilePicture(byte hrco, int employeeId)
        {
            using var db = new VPContext();

            var employee = db.HRResources.FirstOrDefault(f => f.HRCo == hrco && f.HRRef == employeeId);
            if (employee != null)
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase fileUpload = Request.Files[fileName];
                    if (fileUpload.ContentType.Contains("image"))
                    {
                        employee.Attachment.BuildDefaultFolders();
                        var folder = employee.Attachment.Folders.FirstOrDefault(f => f.AttachmentTypeId == 50005);
                        if (folder == null)
                        {
                            folder = employee.Attachment.GetRootFolder();
                        }
                        var attach = folder.AddFile(fileUpload);
                    }
                }
                db.SaveChanges(ModelState);
            }

            return Json(new { Message = "Uploaded" });
        }
    }
}