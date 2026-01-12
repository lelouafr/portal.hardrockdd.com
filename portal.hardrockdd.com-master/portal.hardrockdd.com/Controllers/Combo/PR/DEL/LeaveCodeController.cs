//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Repository.VP.JC;
//using portal.Repository.VP.PR;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Controllers.VP.PR
//{
//    [Authorize]
//    public class LeaveCodeController : BaseController
//    {
//        [HttpGet]
//        public JsonResult Combo(byte co)
//        {
//            using var db = new VPContext();

//            var list = db.LeaveCodes.Where(f => f.PRCo == co).ToList();
//            var result = new List<SelectListItem>
//            {
//                new SelectListItem
//                {
//                    Text = "Select"
//                }
//            };

//            result.AddRange(list.OrderBy(o => o.LeaveCodeId)
//                          .Select(s => new SelectListItem
//                          {
//                              Value = s.LeaveCodeId.ToString(AppCultureInfo.CInfo()),
//                              Text = string.Format(AppCultureInfo.CInfo(), "{0}", s.Description)
//                          }).ToList());

//            return Json(result, JsonRequestBehavior.AllowGet);
//        }

//    }
//}