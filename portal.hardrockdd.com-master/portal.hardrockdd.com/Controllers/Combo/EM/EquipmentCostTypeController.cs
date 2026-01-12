using DB.Infrastructure.ViewPointDB.Data;
using portal.Repository.VP.EM;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.EM
{
    [Authorize]
    public class EquipmentCostTypeController : BaseController
    {
        [HttpGet]
        public JsonResult Combo(byte emGroupId, string costCodeId, string selected)
        {
            using var db = new VPContext();
            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Cost Type"
                }
            };
            var emCostCode = db.EMCostCodes.FirstOrDefault(f => f.EMGroupId == emGroupId && f.CostCodeId == costCodeId);
            if (emCostCode != null)
            {
                var list = emCostCode.CostTypes.Select(s => s.CostType).ToList();
                //var list = db.EMCostTypes.Where(w => w.EMGroup == co).ToList();
                var listSelect = list.Select(s => new SelectListItem
                {
                    Value = s.CostTypeId.ToString(AppCultureInfo.CInfo()),
                    Text = s.Description,
                    Selected = s.CostTypeId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
                }).ToList();

                result.AddRange(listSelect);
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult APCombo(byte emGroupId, string selected)
        {
            using var db = new VPContext();
            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Cost Type"
                }
            };
            
            var list = db.EMCostTypes.Where(f => f.EMGroupId == emGroupId).OrderBy(o => o.CostTypeId).ToList();
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.CostTypeId.ToString(AppCultureInfo.CInfo()),
                Text = string.Format(AppCultureInfo.CInfo(), "{0} - {1}", s.CostTypeId, s.Description),
                Selected = s.CostTypeId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
            }).ToList());


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult POCombo(byte? emGroupId, string costCodeId)
        {
            if (emGroupId == null || string.IsNullOrEmpty(costCodeId))
            {
                var result = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "EM Group Id/Cost Code Id missing",
                    }
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {

                using var db = new VPContext();
                var result = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "Select Cost Type"
                    }
                };
                var emCostCode = db.EMCostCodes.FirstOrDefault(f => f.EMGroupId == emGroupId && f.CostCodeId == costCodeId);
                if (emCostCode != null)
                {
                    var list = emCostCode.CostTypes.Where(f => f.CostTypeId != 1 && f.CostTypeId != 5 && f.CostTypeId != 10).Select(s => s.CostType).ToList();
                    //var list = db.EMCostTypes.Where(w => w.EMGroup == co).ToList();
                    var listSelect = list.Select(s => new SelectListItem
                    {
                        Value = s.CostTypeId.ToString(AppCultureInfo.CInfo()),
                        Text = s.Description,
                    }).ToList();

                    result.AddRange(listSelect);
                }


                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

    }
}