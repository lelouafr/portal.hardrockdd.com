using DB.Infrastructure.ViewPointDB.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.JC
{
    [Authorize]
    public class PhaseMasterCostTypeController : BaseController
    {

        [HttpGet]
        public JsonResult Combo(byte? phaseGroupId, string phaseId)
        {
            if (phaseGroupId == null || string.IsNullOrEmpty(phaseId))
            {
                var result = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "Phase Group Id/Phase Id missing",
                    }
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {

                using var db = new VPContext();
                var list = db.PhaseMasterCosts.Where(w => w.PhaseGroupId == phaseGroupId && w.PhaseId == phaseId).ToList();

                var listSelect = list.Select(s => new SelectListItem
                {
                    Value = s.CostTypeId.ToString(AppCultureInfo.CInfo()),
                    Text = s.CostType.Description,
                }).ToList();

                var result = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "Select Cost Type"
                    }
                };
                result.AddRange(listSelect);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult CCCombo(byte? phaseGroupId, string phaseId, string selected)
        {
            if (phaseGroupId == null || string.IsNullOrEmpty(phaseId))
            {
                var result = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "Phase Group Id/Phase Id missing",
                    }
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                using var db = new VPContext();
                var list = db.PhaseMasterCosts.Where(w => w.PhaseGroupId == phaseGroupId &&
                                                          w.PhaseId == phaseId &&
                                                          w.CostType.POCostType == "Y")
                                              .ToList();
                var result = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "Select Cost Type"
                    }
                };
                result.AddRange(list.Select(s => new SelectListItem
                {
                    Value = s.CostTypeId.ToString(AppCultureInfo.CInfo()),
                    Text = s.CostType.Description,
                    Selected = s.CostTypeId.ToString(AppCultureInfo.CInfo()) == selected ? true : false,
                    Group = new SelectListGroup
                    {
                        Name = s.CostType.JBCostTypeCategory != "L" &&
                               s.CostType.JBCostTypeCategory != "B" ? "" : "Disabled",
                        Disabled = s.CostType.JBCostTypeCategory != "L" &&
                                   s.CostType.JBCostTypeCategory != "B" ? false : true
                    },

                }).OrderBy(o => string.IsNullOrEmpty(o.Group.Name) ? 0 :
                                o.Group.Name == "Disabled" ? 1 : 2)
                 .ThenBy(o => o.Value).ToList());

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult APCombo(byte phaseGroupId, string selected)
        {
            using var db = new VPContext();
            var list = db.CostTypes.Where(w => w.PhaseGroupId == phaseGroupId).ToList();

            var listSelect = list.Select(s => new SelectListItem
            {
                Value = s.CostTypeId.ToString(AppCultureInfo.CInfo()),
                Text = string.Format(AppCultureInfo.CInfo(), "{0} - {1}", s.CostTypeId, s.Description),
                Selected = s.CostTypeId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
            }).ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Cost Type"
                }
            };
            result.AddRange(listSelect);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public JsonResult POCombo(byte? phaseGroupId, string phaseId)
        {
            if (phaseGroupId == null || string.IsNullOrEmpty(phaseId))
            {
                var result = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "Phase Group Id/Phase Id missing",
                    }
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                using var db = new VPContext();
                var list = db.PhaseMasterCosts.Where(w => w.PhaseGroupId == phaseGroupId && w.PhaseId == phaseId && w.CostType.POCostType == "Y" && w.CostType.JBCostTypeCategory != "L" && w.CostType.JBCostTypeCategory != "B").ToList();

                var listSelect = list.Select(s => new SelectListItem
                {
                    Value = s.CostTypeId.ToString(AppCultureInfo.CInfo()),
                    Text = s.CostType.Description,
                }).ToList();

                var result = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "Select Cost Type"
                    }
                };
                result.AddRange(listSelect);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

    }
}