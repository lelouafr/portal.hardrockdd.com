using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.PhaseMaster;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.JV
{
    [Authorize]
    public class PhaseMasterController : BaseController
    {
        [HttpGet]
        public PartialViewResult Search(byte phaseGroupId, int vendorId)
        {
            using var db = new VPContext();
            var comp = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == phaseGroupId);
            var result = new PhaseMasterListViewModel(comp, vendorId);

            return PartialView("../JC/PhaseMaster/Search/Panel", result);
        }

        [HttpGet]
        public PartialViewResult SearchTable(byte phaseGroupId, int vendorId)
        {
            using var db = new VPContext();
            var comp = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == phaseGroupId);
            var result = new PhaseMasterListViewModel(comp, vendorId);

            return PartialView("../JC/PhaseMaster/Search/Table", result);
        }

        [HttpPost]
        public JsonResult SearchReturn(PhaseMasterViewModel model)
        {
            if (model == null) throw new System.ArgumentNullException(nameof(model));
            using var db = new VPContext();

            if (model.VendorId != null)
            {
                var vendor = db.APVendors.FirstOrDefault(f => f.VendorId == model.VendorId);

                if (!db.VendorPhases.Any(f => f.VendorGroupId == vendor.VendorGroupId && f.VendorId == model.VendorId && f.PhaseId == model.PhaseId))
                {
                    db.VendorPhases.Add(new VendorPhase {
                        VendorGroupId = vendor.VendorGroupId,
                        VendorId = (int)model.VendorId,
                        PhaseId = model.PhaseId,
                        Seq = db.VendorPhases .Where(f => f.VendorGroupId == vendor.VendorGroupId && f.VendorId == model.VendorId).DefaultIfEmpty().Max(f => f == null ? 0 : f.Seq) + 1
                    });
                    db.SaveChanges(ModelState);
                }
            }
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Json(new { success = "true", value = model.PhaseId, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult Combo(byte? phaseGroupId)
        {

            if (phaseGroupId == null)
            {
                var result = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "Phase Group Id is missing",
                    }
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                using var db = new VPContext();

                var list = db.PhaseMasters.Where(w => w.PhaseGroupId == phaseGroupId).OrderBy(o => o.Description).ToList();
                var result = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "Select Phase",
                    }
                };

                result.AddRange(list.Select(s => new SelectListItem
                {
                    Value = s.PhaseId?.ToString(AppCultureInfo.CInfo()),
                    Text = s.Description,
                    Selected = false,
                    Group = new SelectListGroup
                    {
                        Name = s.ActiveYN == "Y" ? "Active" : "Inactive",
                        Disabled = s.ActiveYN != "Y"
                    },

                }).OrderBy(o => o.Group.Name).ToList());

                return Json(result, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpGet]
        public JsonResult CCCombo(byte phaseGroupId, string selected)
        {
            using var db = new VPContext();
            var list = db.PhaseMasters.Where(w => w.PhaseGroupId == phaseGroupId).OrderBy(o => o.Description).ToList();
            //var list = repo.GetPhaseMasters(co);

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Phase"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.PhaseId.ToString(AppCultureInfo.CInfo()),
                Text = s.Description,
                Selected = s.PhaseId.ToString(AppCultureInfo.CInfo()) == selected ? true : false,
                Group = new SelectListGroup
                {
                    Name = s.PhaseType,
                    Disabled = s.PhaseType == "Allocation"
                },

            }).OrderBy(o => o.Group.Name == "Cost" ? 0 :
                            o.Group.Name == "Production" ? 1 :
                            o.Group.Name == "Allocation" ? 2 :
                            string.IsNullOrEmpty(o.Group.Name) ? 3 : 4)
             .ThenBy(o => o.Value).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult APCombo(byte phaseGroupId, string selected)
        {
            using var db = new VPContext();
            var list = db.PhaseMasters.Where(w => w.PhaseGroupId == phaseGroupId && w.ActiveYN == "Y" ).OrderBy(o => o.PhaseId).ToList();
            //var list = repo.GetPhaseMasters(co);

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Phase"
                }
            };
            var selectList = list.Select(s => new SelectListItem
            {
                Value = s.PhaseId,
                Text = string.Format(AppCultureInfo.CInfo(), "{0} {1}", s.PhaseId.Trim(), s.Description),
                Selected = s.PhaseId == selected ? true : false
            }).ToList();

            result.AddRange(selectList);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult VendorCombo(byte? phaseGroupId, int? vendorId)
        {
            if (phaseGroupId == null || vendorId == null)
            {
                var result = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "Phase Group Id/Vendor Id missing",
                    }
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                using var db = new VPContext();

                var list = db.VendorPhases.Where(w => w.VendorGroupId == phaseGroupId && w.VendorId == vendorId &&
                                                      w.Phase.Costs.Any(f => f.CostType.JBCostTypeCategory != "L" && f.CostType.JBCostTypeCategory != "B"))
                                          .Select(s => s.Phase)
                                          .OrderBy(o => o.PhaseId)
                                          .ToList();

                if (list.Count == 0)
                {
                    list = db.PhaseMasters.Where(w => w.PhaseGroupId == phaseGroupId && w.ActiveYN == "Y" &&
                                                        w.Costs.Any(f => f.CostType.JBCostTypeCategory != "L" && f.CostType.JBCostTypeCategory != "B")
                                            ).ToList();

                    //list = db.PhaseMasters.Where(w => w.PhaseGroup == co)
                    //                      .ToList();
                }

                var selectList = list.Select(s => new SelectListItem
                {
                    Value = s.PhaseId,
                    Text = string.Format(AppCultureInfo.CInfo(), "{0} {1}", s.PhaseId.Trim(), s.Description),
                }).ToList();

                var result = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "Select Phase"
                    }
                };

                result.AddRange(selectList);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ProductionCombo(byte phaseGroupId, string selected)
        {
            using var db = new VPContext();
            var list = db.PhaseMasters.Where(w => w.PhaseGroupId == phaseGroupId &&
                                                  w.ActiveYN == "Y" &&
                                                  w.PhaseType == "Production" 
                                                  //&&
                                                  //w.SubPhases.Any(c => c.ActiveYN == "Y") == false
                                                  )
                                                  .OrderBy(o => o.PhaseId)
                                                  .ToList()
                                                  ;

            var sList = list.Select(s => new SelectListItem
            {
                Value = s.PhaseId.ToString(AppCultureInfo.CInfo()),
                Text = s.Description
            }).ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Phase"
                }
            };
            result.AddRange(sList);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult BoreCombo(byte bdco, string selected)
        {
            using var db = new VPContext();            
            var bidParms = db.BDCompanyParms.FirstOrDefault(f => f.BDCo == bdco); 
            
            var list = db.PhaseMasters.Where(w => w.PhaseGroupId == bidParms.HQCompanyParm.PhaseGroupId && 
                                                  w.PhaseType == "Production" &&
                                                               (w.PhaseId == bidParms.PilotPhaseId ||
                                                                w.PhaseId == bidParms.ReamPhaseId ||
                                                                w.PhaseId == bidParms.SwabPhaseId ||
                                                                w.PhaseId == bidParms.PullPipePhaseId ||
                                                                w.PhaseId == bidParms.TripInOutPhaseId)).ToList();

            var sList = list.Select(s => new SelectListItem
            {
                Value = s.PhaseId.ToString(AppCultureInfo.CInfo()),
                Text = s.Description
            }).ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Phase"
                }
            };
            result.AddRange(sList);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}