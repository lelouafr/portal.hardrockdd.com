using portal.Models.Views.Equipment;
using System.Linq;
using System.Web.Mvc;


namespace portal.Controllers.View.Equipment
{
    [ControllerAuthorize]
    public class EMInspectionController : BaseController
    {

        [HttpGet]
        [Route("Equipment/Inspections")]
        public ActionResult Index()
        {
            var results = new EquipmentListViewModel();
            results.List.Add(new EquipmentViewModel());
            ViewBag.Title = "Pending Inspections";
            ViewBag.DataController = "EMInspection";
            return View("../EM/Equipment/Inspection/Index", results);
        }

        [HttpGet]
        public ActionResult Table()
        {
            ViewBag.DataController = "EMInspection";
            var results = new EquipmentListViewModel();
            results.List.Add(new EquipmentViewModel());
            return PartialView("../EM/Equipment/Inspection/List/Table", results);
        }

        [HttpGet]
        public ActionResult Data()
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var comp = StaticFunctions.GetCurrentCompany();
            var company = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == comp.HQCo);
			//var date = DateTime.Now.AddMonths(1);
			//var eqpList = company.Equipments.Where(f => f.InspectionExpiration <= date && f.Status == "A").ToList();

			var eqpList = db.Equipments.Where(f => f.EMCo != 2 && f.EMCo != 3 && f.Status == "A").ToList();
			//var eqpList = company.EMCompanyParm.Equipments.Where(f => f.Status == "A").ToList();

            var results = new EquipmentListViewModel(company, eqpList);
            JsonResult result = Json(new { data = results.List.ToArray() }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }

    }
}