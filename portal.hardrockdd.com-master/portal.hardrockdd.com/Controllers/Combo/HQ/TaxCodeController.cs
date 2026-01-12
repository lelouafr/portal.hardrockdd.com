//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Repository.VP.HR;
//using portal.Repository.VP.PM;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Controllers.VP.PM
//{
//    [Authorize]
//    public class TaxCodeController : BaseController
//    {
        
//        [HttpGet]
//        public JsonResult Combo(byte co, string selected)
//        {
//            using var db = new VPContext();
//            var results = db.TaxCodes.Where(f => f.TaxGroup == co).ToList();

//            var result = new List<SelectListItem>
//            {
//                new SelectListItem
//                {
//                    Text = "Select TaxCode"
//                }
//            };
//            var list = results.OrderBy(o => o.TaxCodeId)
//                           .Select(s => new SelectListItem
//                           {
//                               Value = s.TaxCodeId,
//                               Text = s.Description,
//                           }).ToList();
//            result.AddRange(list);

//            return Json(result, JsonRequestBehavior.AllowGet);
//        }
//    }
//}