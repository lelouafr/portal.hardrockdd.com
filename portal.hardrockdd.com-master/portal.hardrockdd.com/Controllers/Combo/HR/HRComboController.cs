using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Employee;
using portal.Repository.VP.PR;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Runtime.Caching;

namespace portal.Controllers.VP.HR
{
    [Authorize]
    public class HRComboController : BaseController
    {

        //SupervisorTermReasonCodeCombo
        [HttpGet]
        public JsonResult SupervisorTermReasonCodeCombo(byte hrco)
        {

            using var db = new VPContext();
            var list = db.HRCodes
                        .Where(f => f.HRCo == hrco && f.TypeId == "N" && f.PortalTermCode == "Y")
                        .OrderBy(f => f.Description)
                        .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Code",
                }
            };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.CodeId,
                Text = s.Description,
                Selected = false,

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DotStatusCodesCombo()
        {

            using var db = new VPContext();
            var result = db.HRResources.GroupBy(g => g.udFMCSAPHMSA)
                        .Select(s => new SelectListItem()
                        {
                            Text = s.Key == null ? "" : s.Key,
                            Value = s.Key,
                        }).OrderBy(o => o.Value).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult HistoryCodeTypeCombo()
        {

            var list = StaticFunctions.GetComboValues("HRHistCodeType");
            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Type"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.DatabaseValue,
                Text = s.DisplayValue
            }).ToList()
            );

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult TermCodeTypeCombo()
        {

            var list = StaticFunctions.GetComboValues("HRHistCodeType");
            list = list.Where(f => f.DisplayValue.ToLower().Contains("term")).ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Type"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.DatabaseValue,
                Text = s.DisplayValue
            }).ToList()
            );

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult HRTestCodesTypeCombo(byte hrco, string type)
        {

            var memKey = string.Format("HRTestCodesTypeCombo_{0}_{1}", hrco, type);
            
            if (MemoryCache.Default[memKey] is List<SelectListItem> result)
                return Json(result, JsonRequestBehavior.AllowGet);

            using var db = new VPContext();
            var list = db.HRCodes.Where(f => f.HRCo == hrco && f.TypeId == type).ToList();

            result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Type"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
                {
                    Value = s.CodeId,
                    Text = s.Description
                }).ToList()
            );

            ObjectCache systemCache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy
            {
                SlidingExpiration = System.TimeSpan.FromMinutes(60)
            };
            systemCache.Set(memKey, result, policy);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult PositionCodeCombo(byte hrco)
        {
            using var db = new VPContext();

            var list = db.HRPositions
                        .Where(f => f.HRCo == hrco)
                        .OrderBy(f => f.Description)
                        .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Position",
                }
            };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.PositionCodeId,
                Text = s.Description,
                Selected = false,

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult HireTaskCombo()
        {
            using var db = new VPContext();

            var list = db.HRTasks
                        .Where(f => f.TaskTypeId == 1)
                        .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Task",
                }
            };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.TaskId.ToString(AppCultureInfo.CInfo()),
                Text = s.Description,
                Selected = false,

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult EmployeeCombo()
        {
            using var db = new VPContext();

            var list = db.HRResources
                        .Where(f => f.HRRef <= 200000)
                        .ToList()
                        .OrderByDescending(o => o.ActiveYN)
                        .ThenBy(f => f.FullName())
                        .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Employee",
                }
            };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.HRRef.ToString(AppCultureInfo.CInfo()),
                Text = s.FullName(),
                Selected = false,
                Group = new SelectListGroup
                {
                    Name = s.ActiveYN == "Y" ? "Active" : "Inactive",
                    Disabled = s.ActiveYN != "Y"
                },

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult TermEmployeeCombo()
        {
            using var db = new VPContext();
            var emp = db.GetCurrentHREmployee();
            var list = db.HRResources
                        .Where(f => f.HRRef <= 200000 && f.ActiveYN == "Y")
                        .ToList()
                        .Where(f => f.IsSupervisor(emp.HRRef))
                        .OrderByDescending(o => o.ActiveYN)
                        .ThenBy(f => f.FullName(false))
                        .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Employee",
                }
            };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.HRRef.ToString(AppCultureInfo.CInfo()),
                Text = s.FullName(false),
                Selected = false,
                //Group = new SelectListGroup
                //{
                //    Name = s.ActiveYN == "Y" ? "Active" : "Inactive",
                //    Disabled = s.ActiveYN != "Y"
                //},

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult PositionRequestApplicants(byte hrco, string positionCodeId)
        {
            using var db = new VPContext();
            var position = db.HRPositions.FirstOrDefault(f => f.HRCo == hrco && f.PositionCodeId == positionCodeId);
            var list = position.WAApplications.Where(f => f.Application.tIsActive).Select(s => s.Application.Applicant).Distinct();
            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Applicant",
                }
            };
            result.AddRange(list.Select(s => s.SelectListItem()).OrderBy(o => o.Text).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult WAAppicantPositionCombo(string positionCodeId)
        {
            using var db = new VPContext();
            var list = db.WebApplications.Where(f => f.tIsActive && f.AppliedPositions.Any(f => f.tPositionCodeId == positionCodeId)).Select(s => s.Applicant).Distinct();
            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Applicant",
                }
            };

            result.AddRange(list.Select(s => s.SelectListItem()).OrderBy(o => o.Text).ToList());


            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult AssetCategoryCombo()
        {
            using var db = new VPContext();

            var list = db.HRCompanyAssets
                        .GroupBy(g => g.AssetCategory)
                        .OrderBy(f => f.Key)
                        .Select(s => new { Label = s.Key, Text = s.Key })
                        .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Category",
                }
            };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.Label,
                Text = s.Text,
                Selected = false,

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}