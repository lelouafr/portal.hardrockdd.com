//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Repository.VP.EM;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Controllers.VP.EM
//{
//    [Authorize]
//    public class EquipmentCostCodeController : BaseController
//    {
//        [HttpGet]
//        public JsonResult Combo(byte emGroup, string selected)
//        {
//            using var db = new VPContext();
//            var list = db.EMCostCodes.Where(w => w.EMGroup == emGroup).ToList();
//            var listSelect = list.Select(s => new SelectListItem
//            {
//                Value = s.CostCodeId,
//                Text = s.Description,
//                Selected = s.CostCodeId == selected ? true : false
//            }).ToList();

//            var result = new List<SelectListItem>
//            {
//                new SelectListItem
//                {
//                    Text = "Select Cost Code"
//                }
//            };
//            result.AddRange(listSelect);

//            return Json(result, JsonRequestBehavior.AllowGet);
//        }
        
//        [HttpGet]
//        public JsonResult APCombo(byte emGroup, string selected)
//        {
//            using var db = new VPContext();
//            var list = db.EMCostCodes.Where(w => w.EMGroup == emGroup).ToList();
//            var listSelect = list.Select(s => new SelectListItem
//            {
//                Value = s.CostCodeId,
//                Text = string.Format(AppCultureInfo.CInfo(),"{0} - {1}", s.CostCodeId, s.Description),
//                Selected = s.CostCodeId == selected ? true : false
//            }).ToList();

//            var result = new List<SelectListItem>
//            {
//                new SelectListItem
//                {
//                    Text = "Select Cost Code"
//                }
//            };
//            result.AddRange(listSelect);

//            return Json(result, JsonRequestBehavior.AllowGet);
//        }

//    }
//}