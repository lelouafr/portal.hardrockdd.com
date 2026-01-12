using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Safety.Controllers
{
    [RouteArea("Safety")]
    public class SafetyController : portal.Controllers.BaseController
    {
        #region Employee List
        [HttpGet]
        [Route("Employees")]
        public ActionResult EmployeeIndex()
        {
            var results = new Models.Safety.EmployeeListViewModel();

            ViewBag.Title = "Employee Listing";
            return View("List/Employee/Index");
        }

        [HttpGet]
        public ActionResult EmployeeTable()
        {
            var results = new Models.Safety.EmployeeListViewModel();

            return PartialView("List/Employee/_Table", results);
        }

        [HttpGet]
        public ActionResult EmployeeData()
        {
            using var db = new VPContext();
            var results = new Models.Safety.EmployeeListViewModel(db);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);

            result.MaxJsonLength = int.MaxValue;
            return result;
        }
        #endregion

        #region Safety Form

        [HttpGet]
        public ActionResult EmployeeSafetyPanel(byte hrco, int hrref)
        {

            using var db = new VPContext();
            var entity = db.HRResources.FirstOrDefault(f => f.PRCo == hrco && f.HRRef == hrref);
            var result = new Models.Safety.FormViewModel(entity);

            return PartialView("Forms/_Panel", result);
        }

        [HttpGet]
        public ActionResult EmployeeSafetyForm(byte hrco, int hrref)
        {
            using var db = new VPContext();
            var entity = db.HRResources.FirstOrDefault(f => f.PRCo == hrco && f.HRRef == hrref);
            var result = new Models.Safety.FormViewModel(entity);

            return PartialView("Forms/_Form", result);
        }

        #endregion

        #region Safety Employee Info

        [HttpGet]
        public ActionResult EmployeeInfoForm(byte hrco, int hrref)
        {
            using var db = new VPContext();
            var entity = db.HRResources.FirstOrDefault(f => f.PRCo == hrco && f.HRRef == hrref);
            var result = new Models.Safety.EmployeeInfoViewModel(entity);

            return PartialView("Forms/Info/_Form", result);
        }

        #endregion

    }
}