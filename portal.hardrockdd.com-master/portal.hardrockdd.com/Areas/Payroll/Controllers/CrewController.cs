using DB.Infrastructure.ViewPointDB.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Payroll.Controllers
{
    [RouteArea("Payroll")]

    public class CrewController : portal.Controllers.BaseController
    {
		#region List
		#region All Crews
		[Route("Crews")]
		[HttpGet]
		public ActionResult Index()
		{
			ViewBag.Title = "All Crews";

			ViewBag.DataController = "Crew";
			ViewBag.TableAction = "AllTable";
			ViewBag.DataAction = "AllData";

			var results = new Models.Crew.CrewListViewModel();
			return View("List/Index", results);
		}

		[HttpGet]
		public ActionResult CrewTable()
		{
			ViewBag.DataController = "Crew";
			ViewBag.DataAction = "AllData";
			var results = new Models.Crew.CrewListViewModel();
			return PartialView("List/_Table", results);
		}

		[HttpGet]
		public ActionResult CrewData()
		{
			using var db = new VPContext();
			var results = new Models.Crew.CrewListViewModel(db);

			JsonResult result = Json(new
			{
				data = results.List.ToArray()
			}, JsonRequestBehavior.AllowGet);
			result.MaxJsonLength = int.MaxValue;
			return result;
		}
		#endregion

		#endregion

		#region Crew Form
		[HttpGet]
		[Route("Crew/{crewId}-{prco}")]
		public ActionResult CrewIndex(byte prco, string crewId)
		{
			using var db = new VPContext();
			var Crew = db.Crews.FirstOrDefault(f => f.PRCo == prco && f.CrewId == crewId);
			var model = new Models.Crew.CrewViewModel(Crew);

			return View("Form/Index", model);
		}

		[HttpGet]
		public ActionResult CrewPopup(byte prco, string crewId)
		{
			using var db = new VPContext();
			var Crew = db.Crews.FirstOrDefault(f => f.PRCo == prco && f.CrewId == crewId);
			var model = new Models.Crew.CrewViewModel(Crew);

			return View("Form/Index", model);
		}

		[HttpGet]
		public ActionResult CrewPanel(byte prco, string crewId)
		{
			using var db = new VPContext();
			var Crew = db.Crews.FirstOrDefault(f => f.PRCo == prco && f.CrewId == crewId);
			var model = new Models.Crew.CrewViewModel(Crew);

			return PartialView("Form/_Panel", model);
		}

		[HttpGet]
		public ActionResult CrewForm(byte prco, string crewId)
		{
			using var db = new VPContext();
			var Crew = db.Crews.FirstOrDefault(f => f.PRCo == prco && f.CrewId == crewId);
			var model = new Models.Crew.CrewViewModel(Crew);

			return PartialView("Form/_Form", model);
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult CrewUpdate(Models.Crew.CrewViewModel model)
		{
			if (model == null)
				return Json(new { success = "false" });
			ModelState.Clear();
			using var db = new VPContext();
			model = model.ProcessUpdate(db, ModelState);

			return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CrewValidate(Models.Crew.CrewViewModel model)
		{
			if (model == null)
				return Json(new { success = "false" });

			ModelState.Clear();
			TryValidateModel(model);
			return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
		}
		#endregion

	}
}