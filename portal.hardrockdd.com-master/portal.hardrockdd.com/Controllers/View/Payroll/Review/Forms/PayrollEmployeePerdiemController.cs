using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.DailyTicket;
using portal.Models.Views.Payroll;
using portal.Repository.VP.DT;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.View.DailyTicket
{
    public class PayrollEmployeePerdiemController : BaseController
    {

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(PayrollPerdiemReviewViewModel model)
        {
            using var db = new VPContext();
            model.ProcessUpdate(db, ModelState);

            return Json(new { success = ModelState.IsValidJson(), model, errorModel = ModelState.ModelErrors() });
        }


        [HttpGet]
        public JsonResult Validate(DailyEmployeePerdiemViewModel model)
        {
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == model.DTCo && f.TicketId == model.TicketId);
            var entry = ticket.DailyEmployeePerdiems.FirstOrDefault(f => f.LineNum == model.LineNum);
            if (entry != null)            {

                model = new DailyEmployeePerdiemViewModel(entry);

                ModelState.Clear();
                this.TryValidateModel(model);

                if (!ModelState.IsValid)
            {
                var errorModel = ModelState.ModelErrors();
                return Json(new { success = ModelState.IsValidJson(), errorModel }, JsonRequestBehavior.AllowGet);
            }
            }
            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
        }
    }
}