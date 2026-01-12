using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.DailyTicket.Form;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View.DailyTicket
{
    public class DailyGeneralTicketFormController : BaseController
    {
        [HttpGet]
        [Route("Ticket/General/{ticketId}-{dtco}")]
        public ActionResult Index(byte dtco, int ticketId, bool partialView = false)
        {
            using var db = new VPContext();
            //var model = new ShopTicketFormViewModel(db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId));

            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var modelTicket = new TicketForm(ticket, true);
            var model = (ShopTicketFormViewModel)modelTicket.DynaimicTicket;

            ViewBag.ViewOnly = model.Access == DB.SessionAccess.View ? true : false;
            ViewBag.TicketForm = modelTicket;
            ViewBag.Partial = partialView;

            if (model.Access == DB.SessionAccess.Denied)
            {
                return RedirectToAction("Denied", "Error");
            }
            if (!partialView)
                return View("~/Views/DailyTicket/Form/Shop/Index.cshtml", model);
            else
                return PartialView("~/Views/DailyTicket/Form/Shop/PartialIndex.cshtml", model);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Update(Models.Views.DailyTicket.DailyShopTicketViewModel model)
        //{
        //    using var repo = new DailyShopTicketRepository();
        //    var result = repo.ProcessUpdate(model, ModelState);

        //    //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

        //    return Json(new { success = ModelState.IsValidJson(), model = result, errorModel = ModelState.ModelErrors() });
        //}

        //[HttpGet]
        //public JsonResult ValidateFieldTicket(Models.Views.DailyTicket.DailyShopTicketViewModel model)
        //{
        //    if (model == null)
        //    {
        //        throw new System.ArgumentNullException(nameof(model));
        //    }
        //    ModelState.Clear();
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