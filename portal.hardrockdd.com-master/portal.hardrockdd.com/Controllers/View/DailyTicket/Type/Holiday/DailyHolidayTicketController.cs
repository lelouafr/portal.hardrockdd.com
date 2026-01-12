using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.DailyTicket.Form;
using portal.Repository.VP.DT;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View.DailyTicket
{
    public class DailyHolidayTicketController : BaseController
    {
        [HttpGet]
        public ActionResult Form(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var modelTicket = new TicketForm(ticket, true);
            var model = (HolidayTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.TicketForm = modelTicket;

            ViewBag.ViewOnly = model.Access == DB.SessionAccess.View ? true : false;
            ViewBag.UnSumbitAllowed = modelTicket.CanUnSubmit;
            return PartialView("../DT/Type/Holiday/Form/Form", model.Ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(Models.Views.DailyTicket.DailyHolidayTicketViewModel model)
        {
            using var db = new VPContext();
            var result = model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult Validate(Models.Views.DailyTicket.DailyHolidayTicketViewModel model)
        {
            //ModelState.AddModelError("", "test");
            //var ticket = (new DailyTicketRepository()).GetDailyTicket(model.Co, model.TicketId, "DailyShopTicket.*, DailyShopEmployees.*, DailyShopTask.*, DailyEquipments.*");
            var ticket = new VPContext().DailyTickets.Where(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId).FirstOrDefault();

            //using var repo = new DailyTicketRepository();
            var form = new Models.Views.DailyTicket.Form.HolidayTicketFormViewModel(ticket);
            model = new Models.Views.DailyTicket.DailyHolidayTicketViewModel(ticket);
            //form.Validate(ModelState);

            if (!ModelState.IsValid)
            {
                var errorModel = ModelState.ModelErrors();
                    return Json(new { success = ModelState.IsValidJson(), errorModel }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        }
    }
}