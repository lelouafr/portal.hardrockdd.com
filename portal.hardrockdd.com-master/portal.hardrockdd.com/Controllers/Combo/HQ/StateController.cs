//using portal.Repository.VP.HR;
//using portal.Repository.VP.PM;
//using System.Collections.Generic;
//using System.Web.Mvc;

//namespace portal.Controllers.VP.PM
//{
//    public class StateController : BaseController
//    {
//        private readonly StateRepository repo = new StateRepository();

//        [HttpGet]
//        public JsonResult Combo(string selected)
//        {
//            var list = repo.GetStates();
//            var result = new List<SelectListItem>
//            {
//                new SelectListItem
//                {
//                    Text = "Select State"
//                }
//            };
//            result.AddRange(StateRepository.GetSelectList(list, selected));

//            return Json(result, JsonRequestBehavior.AllowGet);
//        }
//    }
//}