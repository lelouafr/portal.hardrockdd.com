using DB.Infrastructure.ViewPointDB.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Payroll.Controllers
{
    [RouteArea("Payroll")]

    public class TicketController : portal.Controllers.BaseController
    {
		#region List
		#region All Tickets
		[Route("Tickets/All")]
		[HttpGet]
		public ActionResult Index()
		{
			ViewBag.Title = "All Tickets";

			ViewBag.DataController = "Ticket";
			ViewBag.TableAction = "AllTable";
			ViewBag.DataAction = "AllData";

			var results = new Models.Ticket.TicketListViewModel();
			return View("List/Index", results);
		}

		[HttpGet]
		public ActionResult AllTable(DB.TimeSelectionEnum timeSelection)
		{
			ViewBag.DataController = "Ticket";
			ViewBag.DataAction = "AllData";
			var results = new Models.Ticket.TicketListViewModel(timeSelection);
			return PartialView("List/_Table", results);
		}

		[HttpGet]
		public ActionResult AllData(DB.TimeSelectionEnum timeSelection)
		{
			using var db = new VPContext();
			var results = new Models.Ticket.TicketListViewModel(db, timeSelection);

			JsonResult result = Json(new
			{
				data = results.List.ToArray()
			}, JsonRequestBehavior.AllowGet);
			result.MaxJsonLength = int.MaxValue;
			return result;
		}
		#endregion

		#region Pending Tickets
		[Route("Tickets/Pending")]
		[HttpGet]
		public ActionResult PendingIndex()
		{
			ViewBag.Title = "Pending Tickets";

			ViewBag.TableAction = "PendingTable";
			ViewBag.DataController = "Ticket";
			ViewBag.DataAction = "PendingData";

			var results = new Models.Ticket.TicketListViewModel();
			results.TimeSelection = DB.TimeSelectionEnum.All;
			return View("List/Index", results);
		}

		[HttpGet]
		public ActionResult PendingTable(DB.TimeSelectionEnum timeSelection)
		{
			ViewBag.DataController = "Ticket";
			ViewBag.DataAction = "PendingData";
			var results = new Models.Ticket.TicketListViewModel(timeSelection);
			return PartialView("List/_Table", results);
		}

		[HttpGet]
		public ActionResult PendingData(DB.TimeSelectionEnum timeSelection)
		{
			using var db = new VPContext();
			var statusList = new List<DB.DailyTicketStatusEnum>
			{
				DB.DailyTicketStatusEnum.Draft,
				DB.DailyTicketStatusEnum.Submitted,
				DB.DailyTicketStatusEnum.Rejected,
				DB.DailyTicketStatusEnum.UnPosted
			};

			var results = new Models.Ticket.TicketListViewModel(db, timeSelection, statusList);

			JsonResult result = Json(new
			{
				data = results.List.ToArray()
			}, JsonRequestBehavior.AllowGet);
			result.MaxJsonLength = int.MaxValue;
			return result;
		}
		#endregion

		#region User Tickets
		[Route("Tickets/User")]
		[HttpGet]
		public ActionResult UserIndex()
		{
			ViewBag.Title = "User Tickets";
			ViewBag.TableAction = "UserTable";
			ViewBag.DataController = "Ticket";
			ViewBag.DataAction = "UserData";
			var results = new Models.Ticket.TicketListViewModel();
			return View("List/Index", results);
		}

		[HttpGet]
		public ActionResult UserTable(DB.TimeSelectionEnum timeSelection)
		{
			ViewBag.DataController = "Ticket";
			ViewBag.DataAction = "UserData";

			var results = new Models.Ticket.TicketListViewModel(timeSelection);
			return PartialView("List/_Table", results);
		}

		[HttpGet]
		public ActionResult UserData(DB.TimeSelectionEnum timeSelection)
		{
			using var db = new VPContext();

			var results = new Models.Ticket.TicketListViewModel(db, timeSelection);

			JsonResult result = Json(new
			{
				data = results.List.ToArray()
			}, JsonRequestBehavior.AllowGet);
			result.MaxJsonLength = int.MaxValue;
			return result;
		}
		#endregion
		#endregion

		#region Ticket Form
		[HttpGet]
		[Route("Ticket/{ticketId}-{dtco}")]
		public ActionResult TicketIndex(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
			var model = new Models.Ticket.FormViewModel(ticket);

			return View("Form/Index", model);
		}

		[HttpGet]
		public ActionResult TicketPopup(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
			var model = new Models.Ticket.FormViewModel(ticket);

			return View("Form/Index", model);
		}

		[HttpGet]
		public ActionResult TicketPanel(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
			var model = new Models.Ticket.FormViewModel(ticket);

			return PartialView("Form/_Panel", model);
		}

		[HttpGet]
		public ActionResult TicketForm(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
			var model = new Models.Ticket.FormViewModel(ticket);

			return PartialView("Form/_Form", model);
		}
		#endregion

		#region Ticket Status Update
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult UpdateStatus(byte dtco, int ticketId, int statusId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
			ticket?.Validate().ToList().ForEach(x => ModelState.AddModelError(x.Key, x.Value));

			if (ticket != null && ModelState.IsValid)
			{
				ticket.Status = (DB.DailyTicketStatusEnum)statusId;
				db.SaveChanges(ModelState);
			}

			return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() });
		}
		#endregion

		#region Status Log
		[HttpGet]
		public ActionResult StatusLogPanel(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
			var model = new Models.Ticket.StatusLogListViewModel(ticket);
			return PartialView("Form/Log/_Panel", model);
		}

		[HttpGet]
		public ActionResult StatusLogTable(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
			var model = new Models.Ticket.StatusLogListViewModel(ticket);
			return PartialView("Form/Log/_Table", model);
		}
		#endregion

		#region Job Ticket
		#region Job Info Form
		[HttpGet]
		public ActionResult JobInfoForm(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
			var model = new Models.Ticket.FormViewModel(ticket);

			return View("Form/Job/_Form", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult JobInfoUpdate(Models.Ticket.Job.TicketViewModel model)
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
		public ActionResult JobInfoValidate(Models.Ticket.Job.TicketViewModel model)
		{
			if (model == null)
				return Json(new { success = "false" });

			ModelState.Clear();
			TryValidateModel(model);

			//using var db = new VPContext();
			//var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId);
			//var valModel = new Models.Ticket.Job.FormViewModel(ticket);

			//ModelState.Clear();
			//TryValidateModel(model);
			//valModel.Validate(ModelState);
			return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Employees
		[HttpGet]
		public ActionResult JobEmployeePanel(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

			var model = new Models.Ticket.Job.EmployeeListViewModel(ticket);
			return PartialView("Form/Job/Employee/_Panel", model);
		}

		[HttpGet]
		public ActionResult JobEmployeeTable(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

			var model = new Models.Ticket.Job.EmployeeListViewModel(ticket);
			return PartialView("Form/Job/Employee/_Table", model);
		}

		[HttpGet]
		public PartialViewResult JobEmployeeAdd(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			Models.Ticket.Job.EmployeeViewModel model = null;
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
			if (ticket != null)
			{
				var emp = ticket.DailyJobTicket.AddEmployee();
				db.SaveChanges(ModelState);
				model = new Models.Ticket.Job.EmployeeViewModel(emp);
			}

			ViewBag.tableRow = "True";

			return PartialView("Form/Job/Employee/_TableRow", model);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult JobEmployeeDelete(byte dtco, int ticketId, int lineNum)
		{
			using var db = new VPContext();
			var updObj = db.DailyJobEmployees.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId && f.LineNum == lineNum);
			if (updObj != null)
			{
				db.DailyJobEmployees.Remove(updObj);
				db.SaveChanges(ModelState);
			}
			return Json(new { success = ModelState.IsValidJson() });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult JobEmployeeUpdate(Models.Ticket.Job.EmployeeViewModel model)
		{
			if (model == null)
				return Json(new { success = "false" });
			ModelState.Clear();
			using var db = new VPContext();
			model = model.ProcessUpdate(db, ModelState);

			return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
		}

		[HttpGet]
		public JsonResult JobEmployeeValidate(Models.Ticket.Job.EmployeeViewModel model)
		{
			if (model == null)
				throw new System.ArgumentNullException(nameof(model));


			ModelState.Clear();
			TryValidateModel(model);

			return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region Phases (Tasks)
		[HttpGet]
		public ActionResult JobPhasePanel(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
			var model = new Models.Ticket.Job.JobPhaseListViewModel(ticket.DailyJobTicket);
			return PartialView("Form/Job/Task/_Panel", model);
		}

		[HttpGet]
		public ActionResult JobPhaseTable(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
			var model = new Models.Ticket.Job.JobPhaseListViewModel(ticket.DailyJobTicket);
			return PartialView("Form/Job/Task/_Table", model);
		}

		[HttpGet]
		public ActionResult JobPhaseAdd(byte dtco, int ticketId, string phaseId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
			var task = ticket.DailyJobTicket.AddJobPhase(phaseId);
			db.SaveChanges(ModelState);

			var model = new Models.Ticket.Job.JobPhaseViewModel(task);

			ViewBag.tableRow = "True";
			return PartialView("Form/Job/Task/_TablePhaseRow", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult JobPhaseDelete(byte dtco, int ticketId, int taskId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
			var delObj = db.DTJobPhases.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId && f.TaskId == taskId);
			if (delObj != null)
			{
				ticket.DailyJobTicket.DeleteJobPhase(delObj);
				db.SaveChanges(ModelState);
			}
			return Json(new { success = ModelState.IsValidJson() });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult JobPhaseUpdate(Models.Ticket.Job.JobPhaseCostViewModel model)
		{
			if (model == null)
				throw new System.ArgumentNullException(nameof(model));
			using var db = new VPContext();
			model = model.ProcessUpdate(db, ModelState);
			db.SaveChanges(ModelState);

			return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
		}

		[HttpGet]
		public JsonResult JobPhaseValidate(Models.Ticket.Job.JobPhaseCostViewModel model)
		{
			using var db = new VPContext();
			var entity = db.DTJobPhaseCosts.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId && f.TaskId == model.TaskId && f.LineId == model.LineId);
			if (entity != null)
			{
				model = new Models.Ticket.Job.JobPhaseCostViewModel(entity);
				ModelState.Clear();
				this.TryValidateModel(model);

			}
			return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
		}
		#endregion
		#endregion

		#region Shop Ticket
		#region Shop Info Form
		[HttpGet]
		public ActionResult ShopInfoForm(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
			var model = new Models.Ticket.FormViewModel(ticket);

			return View("Form/Shop/_Form", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult ShopInfoUpdate(Models.Ticket.Shop.TicketViewModel model)
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
		public ActionResult ShopInfoValidate(Models.Ticket.Shop.TicketViewModel model)
		{
			if (model == null)
				return Json(new { success = "false" });

			ModelState.Clear();
			TryValidateModel(model);

			//using var db = new VPContext();
			//var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId);
			//var valModel = new Models.Ticket.Shop.FormViewModel(ticket);

			//ModelState.Clear();
			//TryValidateModel(model);
			//valModel.Validate(ModelState);
			return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Employees
		[HttpGet]
		public ActionResult ShopEmployeePanel(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

			var model = new Models.Ticket.EmployeeEntryListViewModel(ticket);
			return PartialView("Form/Shop/Employees/_Panel", model);
		}

		[HttpGet]
		public ActionResult ShopEmployeeTable(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

			var model = new Models.Ticket.EmployeeEntryListViewModel(ticket);
			return PartialView("Form/Shop/Employees/_Table", model);
		}

		[HttpGet]
		public PartialViewResult ShopEmployeeAdd(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			Models.Ticket.EmployeeEntryViewModel model = null;
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
			if (ticket != null)
			{
				var emp = ticket.AddHoursEntry();
				db.SaveChanges(ModelState);
				model = new Models.Ticket.EmployeeEntryViewModel(emp);
			}

			ViewBag.tableRow = "True";

			return PartialView("Form/Shop/Employees/_TableRow", model);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ShopEmployeeDelete(byte dtco, int ticketId, int lineNum)
		{
			using var db = new VPContext();
			var updObj = db.DailyEmployeeEntries.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId && f.LineNum == lineNum);
			if (updObj != null)
			{
				db.DailyEmployeeEntries.Remove(updObj);
				db.SaveChanges(ModelState);
			}
			return Json(new { success = ModelState.IsValidJson() });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult ShopEmployeeUpdate(Models.Ticket.EmployeeEntryViewModel model)
		{
			if (model == null)
				return Json(new { success = "false" });
			ModelState.Clear();
			using var db = new VPContext();
			model = model.ProcessUpdate(db, ModelState);

			return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
		}

		[HttpGet]
		public JsonResult ShopEmployeeValidate(Models.Ticket.EmployeeEntryViewModel model)
		{
			if (model == null)
				throw new System.ArgumentNullException(nameof(model));


			ModelState.Clear();
			TryValidateModel(model);

			return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
		}
		#endregion
		#endregion

		#region Crew Ticket
		#region Crew Info Form
		[HttpGet]
		public ActionResult CrewInfoForm(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
			var model = new Models.Ticket.FormViewModel(ticket);

			return View("Form/Crew/_Form", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult CrewInfoUpdate(Models.Ticket.Crew.TicketViewModel model)
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
		public ActionResult CrewInfoValidate(Models.Ticket.Crew.TicketViewModel model)
		{
			if (model == null)
				return Json(new { success = "false" });

			ModelState.Clear();
			TryValidateModel(model);

			//using var db = new VPContext();
			//var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId);
			//var valModel = new Models.Ticket.Crew.FormViewModel(ticket);

			//ModelState.Clear();
			//TryValidateModel(model);
			//valModel.Validate(ModelState);
			return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Employees
		[HttpGet]
		public ActionResult CrewEmployeePanel(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

			var model = new Models.Ticket.EmployeeEntryListViewModel(ticket);
			return PartialView("Form/Crew/Employees/_Panel", model);
		}

		[HttpGet]
		public ActionResult CrewEmployeeTable(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

			var model = new Models.Ticket.EmployeeEntryListViewModel(ticket);
			return PartialView("Form/Crew/Employees/_Table", model);
		}

		[HttpGet]
		public PartialViewResult CrewEmployeeAdd(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			Models.Ticket.EmployeeEntryViewModel model = null;
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
			if (ticket != null)
			{
				var emp = ticket.AddHoursEntry();
				db.SaveChanges(ModelState);
				model = new Models.Ticket.EmployeeEntryViewModel(emp);
			}

			ViewBag.tableRow = "True";

			return PartialView("Form/Crew/Employees/_TableRow", model);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CrewEmployeeDelete(byte dtco, int ticketId, int lineNum)
		{
			using var db = new VPContext();
			var updObj = db.DailyEmployeeEntries.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId && f.LineNum == lineNum);
			if (updObj != null)
			{
				db.DailyEmployeeEntries.Remove(updObj);
				db.SaveChanges(ModelState);
			}
			return Json(new { success = ModelState.IsValidJson() });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult CrewEmployeeUpdate(Models.Ticket.EmployeeEntryViewModel model)
		{
			if (model == null)
				return Json(new { success = "false" });
			ModelState.Clear();
			using var db = new VPContext();
			model = model.ProcessUpdate(db, ModelState);

			return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
		}

		[HttpGet]
		public JsonResult CrewEmployeeValidate(Models.Ticket.EmployeeEntryViewModel model)
		{
			if (model == null)
				throw new System.ArgumentNullException(nameof(model));


			ModelState.Clear();
			TryValidateModel(model);

			return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
		}
		#endregion
		#endregion

		#region Truck Ticket
		#region Truck Info Form
		[HttpGet]
		public ActionResult TruckInfoForm(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
			var model = new Models.Ticket.FormViewModel(ticket);

			return View("Form/Truck/_Form", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult TruckInfoUpdate(Models.Ticket.Truck.TicketViewModel model)
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
		public ActionResult TruckInfoValidate(Models.Ticket.Truck.TicketViewModel model)
		{
			if (model == null)
				return Json(new { success = "false" });

			ModelState.Clear();
			TryValidateModel(model);

			//using var db = new VPContext();
			//var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId);
			//var valModel = new Models.Ticket.Truck.FormViewModel(ticket);

			//ModelState.Clear();
			//TryValidateModel(model);
			//valModel.Validate(ModelState);
			return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Employees
		[HttpGet]
		public ActionResult TruckEmployeePanel(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

			var model = new Models.Ticket.EmployeeEntryListViewModel(ticket);
			return PartialView("Form/Truck/Employees/_Panel", model);
		}

		[HttpGet]
		public ActionResult TruckEmployeeTable(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

			var model = new Models.Ticket.EmployeeEntryListViewModel(ticket);
			return PartialView("Form/Truck/Employees/_Table", model);
		}

		[HttpGet]
		public PartialViewResult TruckEmployeeAdd(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			Models.Ticket.EmployeeEntryViewModel model = null;
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
			if (ticket != null)
			{
				var emp = ticket.AddHoursEntry();
				db.SaveChanges(ModelState);
				model = new Models.Ticket.EmployeeEntryViewModel(emp);
			}

			ViewBag.tableRow = "True";

			return PartialView("Form/Truck/Employees/_TableRow", model);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult TruckEmployeeDelete(byte dtco, int ticketId, int lineNum)
		{
			using var db = new VPContext();
			var updObj = db.DailyEmployeeEntries.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId && f.LineNum == lineNum);
			if (updObj != null)
			{
				db.DailyEmployeeEntries.Remove(updObj);
				db.SaveChanges(ModelState);
			}
			return Json(new { success = ModelState.IsValidJson() });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult TruckEmployeeUpdate(Models.Ticket.EmployeeEntryViewModel model)
		{
			if (model == null)
				return Json(new { success = "false" });
			ModelState.Clear();
			using var db = new VPContext();
			model = model.ProcessUpdate(db, ModelState);

			return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
		}

		[HttpGet]
		public JsonResult TruckEmployeeValidate(Models.Ticket.EmployeeEntryViewModel model)
		{
			if (model == null)
				throw new System.ArgumentNullException(nameof(model));


			ModelState.Clear();
			TryValidateModel(model);

			return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
		}
		#endregion
		#endregion

		#region Employee Ticket
		#region Employee Info Form
		[HttpGet]
		public ActionResult EmployeeInfoForm(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
			var model = new Models.Ticket.Employee.FormViewModel(ticket);

			return View("Form/Employee/_Form", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult EmployeeInfoUpdate(Models.Ticket.Employee.TicketViewModel model)
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
		public ActionResult EmployeeInfoValidate(Models.Ticket.Employee.TicketViewModel model)
		{
			if (model == null)
				return Json(new { success = "false" });

			ModelState.Clear();
			TryValidateModel(model);

			//using var db = new VPContext();
			//var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId);
			//var valModel = new Models.Ticket.Employee.FormViewModel(ticket);

			//ModelState.Clear();
			//TryValidateModel(model);
			//valModel.Validate(ModelState);
			return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Employees
		[HttpGet]
		public ActionResult EmployeeEntryPanel(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

			var model = new Models.Ticket.EmployeeEntryListViewModel(ticket);
			return PartialView("Form/Employee/Entry/_Panel", model);
		}

		[HttpGet]
		public ActionResult EmployeeEntryTable(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

			var model = new Models.Ticket.EmployeeEntryListViewModel(ticket);
			return PartialView("Form/Employee/Entry/_Table", model);
		}

		[HttpGet]
		public PartialViewResult EmployeeEntryAdd(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			Models.Ticket.EmployeeEntryViewModel model = null;
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
			if (ticket != null)
			{
				var emp = ticket.AddHoursEntry();
				db.SaveChanges(ModelState);
				model = new Models.Ticket.EmployeeEntryViewModel(emp);
			}

			ViewBag.tableRow = "True";

			return PartialView("Form/Employee/Entry/_TableRow", model);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult EmployeeEntryDelete(byte dtco, int ticketId, int lineNum)
		{
			using var db = new VPContext();
			var updObj = db.DailyEmployeeEntries.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId && f.LineNum == lineNum);
			if (updObj != null)
			{
				db.DailyEmployeeEntries.Remove(updObj);
				db.SaveChanges(ModelState);
			}
			return Json(new { success = ModelState.IsValidJson() });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult EmployeeEntryUpdate(Models.Ticket.EmployeeEntryViewModel model)
		{
			if (model == null)
				return Json(new { success = "false" });
			ModelState.Clear();
			using var db = new VPContext();
			model = model.ProcessUpdate(db, ModelState);

			return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
		}

		[HttpGet]
		public JsonResult EmployeeEntryValidate(Models.Ticket.EmployeeEntryViewModel model)
		{
			if (model == null)
				throw new System.ArgumentNullException(nameof(model));


			ModelState.Clear();
			TryValidateModel(model);

			return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
		}
		#endregion
		#endregion

		#region Holiday Ticket
		#region Holiday Info Form
		[HttpGet]
		public ActionResult HolidayInfoForm(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
			var model = new Models.Ticket.FormViewModel(ticket);

			return View("Form/Holiday/_Form", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult HolidayInfoUpdate(Models.Ticket.Holiday.TicketViewModel model)
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
		public ActionResult HolidayInfoValidate(Models.Ticket.Holiday.TicketViewModel model)
		{
			if (model == null)
				return Json(new { success = "false" });

			ModelState.Clear();
			TryValidateModel(model);

			//using var db = new VPContext();
			//var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId);
			//var valModel = new Models.Ticket.Holiday.FormViewModel(ticket);

			//ModelState.Clear();
			//TryValidateModel(model);
			//valModel.Validate(ModelState);
			return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Employees
		[HttpGet]
		public ActionResult HolidayEmployeePanel(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

			var model = new Models.Ticket.EmployeeEntryListViewModel(ticket);
			return PartialView("Form/Holiday/Employees/_Panel", model);
		}

		[HttpGet]
		public ActionResult HolidayEmployeeTable(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

			var model = new Models.Ticket.EmployeeEntryListViewModel(ticket);
			return PartialView("Form/Holiday/Employees/_Table", model);
		}

		[HttpGet]
		public PartialViewResult HolidayEmployeeAdd(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			Models.Ticket.EmployeeEntryViewModel model = null;
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
			if (ticket != null)
			{
				var emp = ticket.AddHoursEntry();
				db.SaveChanges(ModelState);
				model = new Models.Ticket.EmployeeEntryViewModel(emp);
			}

			ViewBag.tableRow = "True";

			return PartialView("Form/Holiday/Employees/_TableRow", model);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult HolidayEmployeeDelete(byte dtco, int ticketId, int lineNum)
		{
			using var db = new VPContext();
			var updObj = db.DailyEmployeeEntries.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId && f.LineNum == lineNum);
			if (updObj != null)
			{
				db.DailyEmployeeEntries.Remove(updObj);
				db.SaveChanges(ModelState);
			}
			return Json(new { success = ModelState.IsValidJson() });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult HolidayEmployeeUpdate(Models.Ticket.EmployeeEntryViewModel model)
		{
			if (model == null)
				return Json(new { success = "false" });
			ModelState.Clear();
			using var db = new VPContext();
			model = model.ProcessUpdate(db, ModelState);

			return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
		}

		[HttpGet]
		public JsonResult HolidayEmployeeValidate(Models.Ticket.EmployeeEntryViewModel model)
		{
			if (model == null)
				throw new System.ArgumentNullException(nameof(model));


			ModelState.Clear();
			TryValidateModel(model);

			return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
		}
		#endregion
		#endregion

		#region TimeOff Ticket
		#region TimeOff Info Form
		[HttpGet]
		public ActionResult TimeOffInfoForm(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
			var model = new Models.Ticket.FormViewModel(ticket);

			return View("Form/TimeOff/_Form", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult TimeOffInfoUpdate(Models.Ticket.TimeOff.TicketViewModel model)
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
		public ActionResult TimeOffInfoValidate(Models.Ticket.TimeOff.TicketViewModel model)
		{
			if (model == null)
				return Json(new { success = "false" });

			ModelState.Clear();
			TryValidateModel(model);

			//using var db = new VPContext();
			//var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId);
			//var valModel = new Models.Ticket.TimeOff.FormViewModel(ticket);

			//ModelState.Clear();
			//TryValidateModel(model);
			//valModel.Validate(ModelState);
			return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Employees
		[HttpGet]
		public ActionResult TimeOffEmployeePanel(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

			var model = new Models.Ticket.EmployeeEntryListViewModel(ticket);
			return PartialView("Form/TimeOff/Employees/_Panel", model);
		}

		[HttpGet]
		public ActionResult TimeOffEmployeeTable(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);

			var model = new Models.Ticket.EmployeeEntryListViewModel(ticket);
			return PartialView("Form/TimeOff/Employees/_Table", model);
		}

		[HttpGet]
		public PartialViewResult TimeOffEmployeeAdd(byte dtco, int ticketId)
		{
			using var db = new VPContext();
			Models.Ticket.EmployeeEntryViewModel model = null;
			var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
			if (ticket != null)
			{
				var emp = ticket.AddHoursEntry();
				db.SaveChanges(ModelState);
				model = new Models.Ticket.EmployeeEntryViewModel(emp);
			}

			ViewBag.tableRow = "True";

			return PartialView("Form/TimeOff/Employees/_TableRow", model);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult TimeOffEmployeeDelete(byte dtco, int ticketId, int lineNum)
		{
			using var db = new VPContext();
			var updObj = db.DailyEmployeeEntries.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId && f.LineNum == lineNum);
			if (updObj != null)
			{
				db.DailyEmployeeEntries.Remove(updObj);
				db.SaveChanges(ModelState);
			}
			return Json(new { success = ModelState.IsValidJson() });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult TimeOffEmployeeUpdate(Models.Ticket.EmployeeEntryViewModel model)
		{
			if (model == null)
				return Json(new { success = "false" });
			ModelState.Clear();
			using var db = new VPContext();
			model = model.ProcessUpdate(db, ModelState);

			return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
		}

		[HttpGet]
		public JsonResult TimeOffEmployeeValidate(Models.Ticket.EmployeeEntryViewModel model)
		{
			if (model == null)
				throw new System.ArgumentNullException(nameof(model));


			ModelState.Clear();
			TryValidateModel(model);

			return Json(new { success = ModelState.IsValidJson(), errorModel = ModelState.ModelErrors() }, JsonRequestBehavior.AllowGet);
		}
		#endregion
		#endregion
	}
}