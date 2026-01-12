using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Employee;
using portal.Repository.VP.PR;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Runtime.Caching;

namespace portal.Controllers.VP.PR
{
    [Authorize]
    public class PRComboController : BaseController
    {
        [HttpGet]
        public JsonResult ActiveEmployeeCombo(byte prco)
        {
            using var db = new VPContext();

            var list = db.Employees
                        .Where(f => f.PRCo == prco && f.EmployeeId <= 200000)
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
                Value = s.EmployeeId.ToString(AppCultureInfo.CInfo()),
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
        public JsonResult EmployeeCombo()
        {
            using var db = new VPContext();

            var list = db.Employees
                        .Where(f => f.EmployeeId <= 200000)
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
                Value = s.EmployeeId.ToString(AppCultureInfo.CInfo()),
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
        public JsonResult ProjectManagerCombo(byte prco)
        {
            using var db = new VPContext();
            string[] posList = { "OP-DM", "OP-GM", "OP-SUP", "OP-PM" };

            var list = db.HRResources
                        .Where(f => f.HRCo == prco && f.HRRef <= 200000 && posList.Contains(f.PositionCode))
                        .ToList()
                        .OrderByDescending(o => o.ActiveYN)
                        .ThenBy(f => f.FullName())
                        .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Prj Manger",
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
        public JsonResult CrewCombo(byte prco)
        {
            using var db = new VPContext();
            var list = db.Crews.Where(f => f.PRCo == prco && f.CrewStatus == "ACTIVE").ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Crew"
                }
            };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.CrewId,
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.CrewId, s.Description),
                Selected = false,               

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ShopTypeCrewCombo(byte prco)
        {
            using var db = new VPContext();
            var list = db.Crews.Where(f => f.PRCo == prco && f.CrewStatus == "ACTIVE").ToList()
                               .Where(f => f.Description.ToLower(AppCultureInfo.CInfo()).Contains("shop")).ToList();
            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Crew"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.CrewId,
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.CrewId, s.Description),
                Selected = false,

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult TruckTypeCrewCombo(byte prco)
        {
            using var db = new VPContext();
            var list = db.Crews.Where(f => f.PRCo == prco && f.CrewStatus == "ACTIVE").ToList()
                               .Where(f => f.Description.ToLower(AppCultureInfo.CInfo()).Contains("truck")).ToList();
            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Crew"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.CrewId,
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.CrewId, s.Description),
                Selected = false,

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CrewTypeCrewCombo(byte prco)
        {
            using var db = new VPContext();
            var list = db.Crews.Where(f => f.PRCo == prco && f.CrewStatus == "ACTIVE").ToList()
                               .Where(f => f.Description.ToLower(AppCultureInfo.CInfo()).Contains("crew") ||
                                           f.Description.ToLower(AppCultureInfo.CInfo()).Contains("truck")).ToList();
            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Crew"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.CrewId,
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.CrewId, s.Description),
                Selected = false,

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult JobTypeCrewCombo()
        {
            using var db = new VPContext();
            var list = db.Crews.Where(f => f.CrewStatus == "ACTIVE").ToList()//f.PRCo == co && 
                               .Where(f => f.Description.ToLower(AppCultureInfo.CInfo()).Contains("crew")).ToList();
            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Crew"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.CrewId,
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.CrewId, s.Description),
                Selected = false,
                //Group = new SelectListGroup
                //{
                //    Name = s.PRCompany.Company.Name,
                //    Disabled = s.CrewStatus == "ACTIVE" ? false : true
                //},

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ARCrewCombo()
        {
            using var db = new VPContext();
            var list = db.Crews.Where(f => (f.CrewStatus == "ACTIVE" && f.udCrewType == "Crew") || (f.udCrewType == "Sub"))
                               .ToList();
            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Crew"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.CrewId,
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.CrewId, s.Description),
                Selected = false,
                Group = new SelectListGroup
                {
                    Name = s.udCrewType,
                    Disabled = !(s.CrewStatus == "ACTIVE" || s.udCrewType == "Sub")
                },

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult HouseTypeCombo(byte prco, string selected)
        {
            using var db = new VPContext();
            var list = db.EmployeeHouseTypes.Where(f => f.Co == prco).ToList();
           

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "..."
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
                {
                    Value = s.HouseTypeId.ToString(AppCultureInfo.CInfo()),
                    Text = s.Description,
                    Selected = s.HouseTypeId.ToString(AppCultureInfo.CInfo()) == selected ? true : false
                }).ToList()
            );
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult TimeOffEarnCodeCombo(byte prco, int employeeEarnCodeId, string selected)
        {
            using var repo = new EarnCodeRepository();

            var list = repo.GetEarnCodes(prco)
                           .Where(w => w.Method == (employeeEarnCodeId == 1 ? "H" : "A") &&
                            w.JCCostType == 1 &&
                            w.OTCalcs == "N" &&
                            w.PortalIsHidden == "N"
                           ).ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Type"
                }
            };
            result.AddRange(EarnCodeRepository.GetSelectList(list, selected));

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult EarnCodeCombo(byte prco)
        {
            using var db = new VPContext();

            var list = db.EarnCodes.Where(f => f.PRCo == prco).ToList();


            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Earn Code"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.EarnCodeId.ToString(AppCultureInfo.CInfo()),
                Text = s.PortalLabel,
                Selected = false,

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DefaultEarnCodeCombo(byte prco)
        {
            using var db = new VPContext();

            var list = db.EarnCodes.Where(f => f.PRCo == prco && (f.EarnCodeId == 1 || f.EarnCodeId == 4)).ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Earn Code"
                }
            };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.EarnCodeId.ToString(AppCultureInfo.CInfo()),
                Text = string.Format("{0}: {1}", s.EarnCodeId, s.Description),

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult SalaryTypeCombo(byte prco)
        {
            var list = StaticFunctions.GetComboValues("HRSalaryType");
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
        public JsonResult EmployeeEarnCodeCombo(byte prco, int EmployeeId, string selected)
        {
            using var repo = new EarnCodeRepository();
            using var db = new VPContext();

            var emp = db.Employees.FirstOrDefault(f => f.PRCo == prco && f.EmployeeId == EmployeeId);
            var list = repo.GetEarnCodes(prco)
                           .Where(w => (w.Method == (emp.EarnCodeId == 1 ? "H" : "A") &&
                            w.JCCostType == 1) || w.EarnCodeId == 999
                           ).ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Type"
                }
            };
            result.AddRange(EarnCodeRepository.GetSelectList(list, selected));

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult LeaveCodeCombo(byte prco)
        {
            using var db = new VPContext();

            var list = db.LeaveCodes.Where(f => f.PRCo == prco).ToList();
            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select"
                }
            };

            result.AddRange(list.OrderBy(o => o.LeaveCodeId)
                          .Select(s => new SelectListItem
                          {
                              Value = s.LeaveCodeId.ToString(AppCultureInfo.CInfo()),
                              Text = string.Format(AppCultureInfo.CInfo(), "{0}", s.Description)
                          }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        //
        [HttpGet]
        public JsonResult PRTCTypes()
        {

            var list = StaticFunctions.GetComboValues("PRTCTypes");
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
        public JsonResult PRDepartmentCode(byte prco)
        {
            using var db = new VPContext();

            var list = db.PRDepartments
                        .Where(f => f.PRCo == prco)
                        .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Department",
                }
            };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.PRDept,
                Text = s.Description,
            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }




        [HttpGet]
        public JsonResult PRInsuranceCode(byte prco)
        {
            using var db = new VPContext();

            var list = db.PRInsuranceCodes
                        .Where(f => f.PRCo == prco)
                        .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Insureance",
                }
            };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.InsCode,
                Text = s.Description,
            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}