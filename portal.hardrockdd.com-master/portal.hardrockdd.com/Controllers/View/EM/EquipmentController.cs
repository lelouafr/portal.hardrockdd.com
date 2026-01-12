using portal.Models.Views.Equipment;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.Equipment
{
    [ControllerAccess]
    [ControllerAuthorize]
    public class EquipmentController : BaseController
    {
        [HttpGet]
        [Route("Equipment")]
        public ActionResult Index()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var emp = user.Employee.FirstOrDefault();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emp.HRCo);


            var results = new EquipmentListViewModel();
            results.List.Add(new EquipmentViewModel());
            //results.StatusId = "A";
            //results.AssignedStatus = null; 
            results.Co = company.HQCo;
            ViewBag.DataController = "Equipment";
            ViewBag.DataAction = "Data";
            return View("../EM/Equipment/Summary/Index", results);
        }

        [HttpGet]
        [Route("Equipment/User")]
        public ActionResult UserIndex()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var emp = user.Employee.FirstOrDefault();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emp.HRCo);


            var results = new EquipmentListViewModel();
            results.List.Add(new EquipmentViewModel());
            results.Co = company.HQCo;
            ViewBag.DataController = "Equipment";
            ViewBag.DataAction = "UserData";
            return View("../EM/Equipment/User/Summary/Index", results);
        }

        [HttpGet]
        public ActionResult Table()
        {
            //var results = new EquipmentListViewModel();
            //results.List.Add(new EquipmentViewModel());
            //results.StatusId = "A";

            //ViewBag.DataController = "Equipment";
            //ViewBag.DataAction = "Data";

            //return PartialView("../EM/Equipment/Summary/List/Table", results);


            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var emp = user.Employee.FirstOrDefault();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emp.HRCo);


            var results = new EquipmentListViewModel();
            results.List.Add(new EquipmentViewModel());
            //results.StatusId = "A";
            //results.AssignedStatus = null; 
            results.Co = company.HQCo;
            ViewBag.DataController = "Equipment";
            ViewBag.DataAction = "Data";
            return PartialView("../EM/Equipment/Summary/List/Table", results);
        }

        [HttpGet]
        public ActionResult Data()//string statusId
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            //var userId = StaticFunctions.GetUserId();
            //var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            //var emp = user.Employee.FirstOrDefault();
            //var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emp.HRCo);

            var equipments = db.Equipments.Where(f => f.EMCo != 2 && f.EMCo != 3).ToList();
            var results = new EquipmentListViewModel(equipments);

            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);

            //result = Json(jsonData, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }

        [HttpGet]
        public ActionResult UserData()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var emp = user.Employee.FirstOrDefault().PREmployee;
            //var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emp.HRCo);
            EquipmentListViewModel results;
            if (true)
            {
                results = new EquipmentListViewModel(emp);
            }
            //var jsonData = JsonConvert.SerializeObject(new { data = results.List.ToArray() } , Formatting.Indented);
            //var data = new { data = jsonData };
            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);

            //result = Json(jsonData, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }
    }
}