using DB.Infrastructure.ViewPointDB.Data;
using portal.Repository.VP.PM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers.VP.PM
{
    [Authorize]
    public class BudgetCodeController : BaseController
    {
        private readonly ProjectBudgetCodeRepository repo = new ProjectBudgetCodeRepository();

        [HttpGet]
        public JsonResult SizeCombo(byte pmco, string selected)
        {
            using var db = new VPContext();
            
            var codes = db.ProjectBudgetCodes.Where(f => f.PMCo == pmco).ToList();
            var list = codes.Where(w => w.Radius != null && w.PhaseId == "   005-  -")
                            .GroupBy(g => g.Radius)
                            .Select(s => new SelectListItem
                            {
                                Text = s.Key.ToString(),
                                Value = s.Key?.ToString("G29", AppCultureInfo.CInfo()),
                                Selected = s.Key.ToString() == selected ? true : false
                            })
                            .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text =  "Bit/Reamer Size"
                }
            };
            result.AddRange(list);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult PassSizeCombo(byte phaseGroupId, string phaseId)
        {
            using var db = new VPContext();

            var codes = db.ProjectBudgetCodes
                        .Where(f => f.PhaseGroupId == phaseGroupId &&
                                    f.PhaseId == phaseId &&
                                    f.Radius != null)
                        .OrderBy(g => g.Radius)
                        .GroupBy(g => g.Radius)
                        .Select( s=> new { 
                            Radius = s.Key
                        })
                        .ToList();
            var list = codes.Select(s => new SelectListItem
                            {
                                Text = s.Radius.ToString(),
                                Value = s.Radius?.ToString("G29", AppCultureInfo.CInfo()),
                            })
                            .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Bit/Reamer Size"
                }
            };
            result.AddRange(list);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RentedCombo(byte pmco, string selected)
        {
            var codes = repo.GetProjectBudgetCodes(pmco);
            var list = codes.Where(w => w.BudgetCodeId.StartsWith("RE-", StringComparison.Ordinal))
                            .Select(s => new SelectListItem
                            {
                                Text = s.Description.ToString(AppCultureInfo.CInfo()),
                                Value = s.BudgetCodeId.ToString(AppCultureInfo.CInfo()),
                                Selected = s.BudgetCodeId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
                            })
                            .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Rental"
                }
            };
            result.AddRange(list);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Combo(byte pmco, string selected)
        {
            var codes = repo.GetProjectBudgetCodes(pmco);
            var list = codes.Select(s => new SelectListItem
                            {
                                Text = s.Description.ToString(AppCultureInfo.CInfo()),
                                Value = s.BudgetCodeId.ToString(AppCultureInfo.CInfo()),
                                Selected = s.BudgetCodeId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
                            })
                            .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select"
                }
            };
            result.AddRange(list);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CostItemCombo(byte pmco, string selected)
        {
            var codes = repo.GetProjectBudgetCodes(pmco);
            var list = codes.Where(w => w.BudgetCodeId.StartsWith("CI-", StringComparison.Ordinal))
                            .Select(s => new SelectListItem
                            {
                                Text = s.Description.ToString(AppCultureInfo.CInfo()),
                                Value = s.BudgetCodeId.ToString(AppCultureInfo.CInfo()),
                                Selected = s.BudgetCodeId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
                            })
                            .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Item"
                }
            };
            result.AddRange(list);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult HardnessCombo(byte pmco, string selected)
        {
            var codes = repo.GetProjectBudgetCodes(pmco);
            var list = codes.Where(w => w.PhaseId == "   005-  -")
                            .GroupBy(g => g.Hardness)
                            .Select(s => new SelectListItem
                            {
                                Text = s.Key,
                                Value = s.Key,
                                Selected = s.Key == selected ? true : false
                            })
                            .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Hardness"
                }
            };
            result.AddRange(list);

            return Json(result, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public JsonResult PMCostItemCombo(byte pmco, string budgetCategory, string selected)
        {
            var codes = repo.GetProjectBudgetCodes(pmco);
            var list = codes.Where(w => w.BudgetCodeId.StartsWith(budgetCategory, StringComparison.Ordinal))
                            .Select(s => new SelectListItem
                            {
                                Text = s.Description.ToString(AppCultureInfo.CInfo()),
                                Value = s.BudgetCodeId.ToString(AppCultureInfo.CInfo()),
                                Selected = s.BudgetCodeId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
                            })
                            .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Item"
                }
            };
            result.AddRange(list);

            return Json(result, JsonRequestBehavior.AllowGet);
        }


    }
}