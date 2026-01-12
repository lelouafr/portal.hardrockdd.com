//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Repository.VP.HR;
//using portal.Repository.VP.PM;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Controllers.VP.PM
//{
//    [Authorize]
//    public class UnitofMeasureController : BaseController
//    {
//        [HttpGet]
//        public JsonResult Combo(string selected)
//        {
//            using var db = new VPContext();

//            var list = db.UnitofMeasures.Select(s => new SelectListItem
//            {
//                Value = s.UM,
//                Text = s.UM,
//                Selected = s.UM == selected ? true : false
//            }).ToList();

//            var result = new List<SelectListItem>
//            {
//                new SelectListItem
//                {
//                    Text = "Select UM"
//                }
//            };
//            result.AddRange(list);

//            return Json(result, JsonRequestBehavior.AllowGet);
//        }
//    }
//}