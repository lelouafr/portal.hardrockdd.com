//using portal.Models.Views.DailyTicket;
//using System.Linq;
//using System.Web.Mvc;


//namespace portal.Controllers.View.DailyTicket
//{
//    public class CreateTicketController : BaseController
//    {
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create(DailyTicketCreateViewModel model)
//        {
//            using var repo = new Repository.VP.DT.DailyTicketRepository();

//            if (ModelState.IsValid)
//            {
//                var ticket = Repository.VP.DT.DailyTicketRepository.Init(model);
//                _ = repo.Create(ticket);
//            }
//            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
//        }

//        [HttpGet]
//        public JsonResult Validate(DailyTicketCreateViewModel model)
//        {
//            if (model == null)
//            {
//                throw new System.ArgumentNullException(nameof(model));
//            }
//            ModelState.Clear();
//            TryValidateModel(model);
//            if (!ModelState.IsValid)
//            {
//                var errorModel = ModelState.ModelErrors();
//                    return Json(new { success = ModelState.IsValidJson(), errorModel }, JsonRequestBehavior.AllowGet);
//            }
//            return Json(new { success = ModelState.IsValidJson() }, JsonRequestBehavior.AllowGet);
//        }

//    }
//}