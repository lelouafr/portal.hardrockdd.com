using Newtonsoft.Json;
using portal.Code.Data.VP;
using portal.Models.Views.Equipment;
using portal.Models.Views.HR.Resource;
using portal.Models.Views.JC.Job;
using portal.Models.Views.Purchase.Order;
using portal.Repository.VP.EM;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.HR
{
    [ControllerAccess]
    [ControllerAuthorize]
    public class HRUserEmployeeController : BaseController
    {
        [HttpGet]
        [Route("HR/Employees/DirectReports")]
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
            ViewBag.DataController = "HRUserEmployee";
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
            ViewBag.DataController = "HRUserEmployee";
            ViewBag.DataAction = "Data";
            return PartialView("../HR/Employee/Summary/Table", results);
        }

        [HttpGet]
        public ActionResult Data()//string statusId
        {
            using var db = new Code.Data.VP.VPEntities();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var emp = user.Employee.FirstOrDefault();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emp.HRCo);
            ResourceListViewModel results = new ResourceListViewModel(company, emp);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);


            result.MaxJsonLength = int.MaxValue;
            return result;
        }

    }
}