using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.DailyTicket.Form;
using portal.Repository.VP.DT;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View.DailyTicket
{
    public class DailyEmployeeTimeOffTicketController : BaseController
    {
        [HttpGet]
        public ActionResult Form(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var modelTicket = new TicketForm(ticket, true);
            var model = (EmployeeTimeOffTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.TicketForm = modelTicket;

            ViewBag.ViewOnly = model.Access == DB.SessionAccess.View ? true : false;
            ViewBag.UnSumbitAllowed = modelTicket.CanUnSubmit;
            ViewBag.ViewOnly = true;
            return PartialView("../DT/Type/TimeOff/Form/Form", model.Ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(Models.Views.DailyTicket.DailyEmployeeTimeOffTicketViewModel model)
        {
            using var db = new VPContext();
            var result = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult Validate(Models.Views.DailyTicket.DailyEmployeeTimeOffTicketViewModel model)
        {
            //ModelState.AddModelError("", "test");
            //var ticket = (new DailyTicketRepository()).GetDailyTicket(model.Co, model.TicketId, "DailyShopTicket.*, DailyShopEmployees.*, DailyShopTask.*, DailyEquipments.*");
            using var db = new VPContext();
            var ticket = db.DailyTickets.Where(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId).FirstOrDefault();

            //using var repo = new DailyTicketRepository();
            var form = new Models.Views.DailyTicket.Form.EmployeeTimeOffTicketFormViewModel(ticket);
            model = new Models.Views.DailyTicket.DailyEmployeeTimeOffTicketViewModel(ticket);
            form.Validate(ModelState);
            
            if (!ModelState.IsValid)
            {
                var errorModel = ModelState.ModelErrors();
                    return Json(new { success = ModelState.IsValidJson(), errorModel }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        }
    }
}