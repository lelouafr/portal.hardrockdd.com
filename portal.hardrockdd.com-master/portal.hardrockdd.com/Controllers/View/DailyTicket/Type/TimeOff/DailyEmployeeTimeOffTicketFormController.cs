using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.DailyTicket.Form;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View.DailyTicket
{
    public class DailyEmployeeTimeOffTicketFormController : BaseController
    {
        [HttpGet]
        [Route("Ticket/EmployeeTimeOff/{ticketId}-{dtco}")]
        public ActionResult Index(byte dtco, int ticketId, bool partialView = false)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var modelTicket = new TicketForm(ticket, true);
            var model = (EmployeeTimeOffTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.TicketForm = modelTicket;
            ViewBag.ViewOnly = model.Access == DB.SessionAccess.View ? true : false;
            ViewBag.UnSumbitAllowed = modelTicket.CanUnSubmit;
            ViewBag.Partial = partialView;
            ViewBag.StartDate = ticket.WorkDate.Value.WeekStartDate(System.DayOfWeek.Saturday);
            ViewBag.EndDate = ticket.WorkDate.Value.WeekEndDate(System.DayOfWeek.Saturday);

            if (model.Access == DB.SessionAccess.Denied)
            {
                return RedirectToAction("Denied", "Error");
            }
            if (!partialView)
                return View("../DT/Type/TimeOff/Index", model);
            else
                return PartialView("../DT/Type/TimeOff/PartialIndex", model);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Update(Models.Views.DailyTicket.DailyEmployeeTimeOffTicketViewModel model)
        //{
        //    using var repo = new DailyGeneralTicketRepository();
        //    var result = repo.ProcessUpdate(model, ModelState);

        //    //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

        //    return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
        //}

        //[HttpGet]
        //public JsonResult ValidateFieldTicket(Models.Views.DailyTicket.DailyShopTicketViewModel model)
        //{
        //    TryValidateModel(model);
        //    if (!ModelState.IsValid)
        //    {
        //        var errorModel = ModelState.ModelErrors();
        //        return Json(new { success = ModelState.IsValidJson(), errorModel }, JsonRequestBehavior.AllowGet);
        //        ;
        //    }
        //    return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        //}
    }
}