using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Equipment;
using portal.Models.Views.Equipment.Audit;
using portal.Models.Views.Equipment.Category;
using portal.Repository.VP.EM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace portal.Controllers.VP.EM
{
    public class EMComboController : BaseController
    {

        [HttpGet]
        public PartialViewResult Search(byte emco)
        {
            using var db = new VPContext();
            var comp = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emco);
            var result = new EquipmentListViewModel(comp, "A", null);

            return PartialView("../EM/Equipment/Search/Panel", result);
        }

        [HttpGet]
        public PartialViewResult SearchTable(byte emco)
        {
            using var db = new VPContext();
            var comp = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == emco);
            var result = new EquipmentListViewModel(comp, "A", null);

            return PartialView("../EM/Equipment/Search/Table", result);
        }

        [HttpPost]
        public JsonResult SearchReturn(EquipmentViewModel model)
        {
            if (model == null)
            {
                model = new EquipmentViewModel();
            }
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Json(new { success = "true", value = model.EquipmentId, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult Combo(byte? emco)
        {
            if (emco == null)
            {
                emco = 0;
            }

            using var db = new VPContext();
            List<Equipment> list = null;
            if (emco == 0)
            {
                list = db.Equipments
                        .Where(f => f.EMCo != 2 && f.EMCo != 3 && f.EMCo != 101)
                        .OrderBy(o => o.Status)
                        .ThenBy(o => o.EquipmentId)
                        .ToList();
            }
            else
            {
                list = db.Equipments
                        .Where(f => f.EMCo != 2 && f.EMCo != 3 && f.EMCo != 101)
                        .OrderBy(o => o.Status)
                        .ThenBy(o => o.EquipmentId)
                        .ToList();
            }

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Equipment"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.EquipmentId.ToString(AppCultureInfo.CInfo()),
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.EquipmentId, s.Description),
                Group = new SelectListGroup
                {
                    Name = s.Status == "A" ? "Active" : "Inactive",
                    Disabled = s.Status != "A"
                },

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult AssingedEquipmentCombo(byte emco, int employeeId, string selected)
        {
            using var db = new VPContext();
            var employee = db.Employees.FirstOrDefault(f => f.EmployeeId == employeeId);
            var employeeList = new EquipmentListViewModel(employee);
            List<Equipment> list = null;
            if (emco == 0)
            {
                list = db.Equipments
                        .Where(f => f.EMCo != 2 && f.EMCo != 3 && f.EMCo != 101)
                        .OrderBy(o => o.Status)
                        .ThenBy(o => o.EquipmentId)
                        .ToList();
            }
            else
            {
                list = db.Equipments
                        .Where(f => f.EMCo != 2 && f.EMCo != 3 && f.EMCo != 101)
                        .OrderBy(o => o.Status)
                        .ThenBy(o => o.EquipmentId)
                        .ToList();
            }

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Equipment"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.EquipmentId.ToString(AppCultureInfo.CInfo()),
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.EquipmentId, s.Description),
                Selected = s.EquipmentId.ToString(AppCultureInfo.CInfo()) == selected,
                Group = new SelectListGroup
                {
                    Name = employeeList.List.Any(f => f.EquipmentId == s.EquipmentId) ? " Your Equipment" : s.Status == "A" ? "Active" : "Inactive",
                    Disabled = s.Status != "A",
                },
            }).ToList());

            result = result.OrderBy(o => o.Group?.Name == null ? 0 : o.Group?.Name == " Your Equipment" ? 1 : 2)
                   .ThenBy(o => o.Group?.Name).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CCCombo(byte ccco, long transId, string selected)
        {
            using var db = new VPContext();
            var list = db.Equipments
                        .Where(f => f.EMCo != 2 && f.EMCo != 3 && f.EMCo != 101)
                        .OrderBy(o => o.Status)
                        .ThenBy(o => o.EquipmentId)
                        .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Equipment"
                }
            };

            var tran = db.CreditTransactions.FirstOrDefault(f => f.CCCo == ccco && f.TransId == transId);
            var options = new EquipmentAuditCreateViewModel();
            options.AuditType = DB.EMAuditTypeEnum.EmployeeAudit;
            options.EmployeeId = tran.EmployeeId;
            options.IncludeSubEquipment = true;
            options.IncludCrewLeaderEquipment = true;
            options.IncludeDirectReportEmployeeEquipment = true;
            var equipmentList = new EquipmentListViewModel(options);

            var minDate = tran.TransDate.AddDays(-3);
            var maxDate = tran.TransDate.AddDays(1);
            var currentEqpList = db.DTPayrollHours.Where(f => f.EmployeeId == tran.EmployeeId && f.WorkDate >= minDate && f.WorkDate <= maxDate)
                                                .GroupBy(g => g.EquipmentId)
                                                .Select(s => s.Key)
                                                .ToList();
            currentEqpList.AddRange(equipmentList.List.Select(s => s.EquipmentId));
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.EquipmentId.ToString(AppCultureInfo.CInfo()),
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.EquipmentId, s.Description),
                Selected = s.EquipmentId.ToString(AppCultureInfo.CInfo()) == selected,
                Group = new SelectListGroup
                {
                    Name = currentEqpList.Any(f => f == s.EquipmentId) ? "Current Equip(s)" : s.Status == "A" ? "Active" : "Inactive",
                    Disabled = s.Status != "A"
                },

            }).OrderBy(o => o.Group.Name == "Current Equip(s)" ? 0 :
                            o.Group.Name == "Active" ? 1 :
                            o.Group.Name == "Inactive" ? 2 : 3)
             .ThenBy(o => o.Value).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RigCombo(byte emco, string selected)
        {
            using var db = new VPContext();
            var list = db.Equipments
                        .Where(f => (f.EMCo != 2 &&
                                     f.EMCo != 3 &&
                                     f.EMCo != 101) &&
                                    f.Status == "A" &&
                                    f.Category.Description.Contains("Rig")
                                    //&& f.RevenueCodes.Any(r => r.RevCode == (f.RevenueCodeId ?? "2"))
                                    )
                        .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Equipment"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.EquipmentId.ToString(AppCultureInfo.CInfo()),
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.EquipmentId, s.Description),
                Group = new SelectListGroup
                {
                    Name = s.RevenueCodes.Any(r => r.RevCode == (s.RevenueCodeId ?? "2")) ? "Active" : "No Rev Setup (Disabled)",
                    Disabled = !s.RevenueCodes.Any(r => r.RevCode == (s.RevenueCodeId ?? "2"))
                },
            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult CategoryEquipmentCombo(string categoryId)
        {
            using var db = new VPContext();
            var list = db.Equipments
                        .Where(f => (f.EMCo != 2 && f.EMCo != 3 && f.EMCo != 101) && f.Status == "A" && f.CategoryId == categoryId)
                        .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Equipment"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.EquipmentId.ToString(AppCultureInfo.CInfo()),
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.EquipmentId, s.Description),
            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CrewRigCombo(byte prco, string crewId, string selected)
        {
            using var db = new VPContext();
            var crew = db.Crews.FirstOrDefault(f => f.PRCo == prco && f.CrewId == crewId);

            var list = db.Equipments
                        .Where(f => (f.EMCo != 2 &&
                                     f.EMCo != 3 &&
                                     f.EMCo != 101) &&
                                    f.Status == "A" &&
                                    f.Category.Description.Contains("Rig")
                                    //&& f.RevenueCodes.Any(r => r.RevCode == (f.RevenueCodeId ?? "2"))
                                    )
                        .ToList();
            if (crew?.RigEquipmentId != null)
            {
                var newList = list.Where(f => f.EquipmentId == crew.RigEquipmentId).ToList();
                if (newList.Count > 0)
                {
                    list = newList;
                }
            }
            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Equipment"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.EquipmentId.ToString(AppCultureInfo.CInfo()),
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.EquipmentId, s.Description),
                Group = new SelectListGroup
                {
                    Name = s.RevenueCodes.Any(r => r.RevCode == (s.RevenueCodeId ?? "2")) ? "Active" : "No Rev Setup (Disabled)",
                    Disabled = !s.RevenueCodes.Any(r => r.RevCode == (s.RevenueCodeId ?? "2"))
                },
            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RigCatCombo(byte emco, string selected)
        {
            //    using var repo = new EquipmentCategoryRepository();
            //    var list = repo.GetEquipmentCategories(co).Where(f => f.Description.Contains("Rig")).ToList();

            using var db = new VPContext();
            var riglist = db.EquipmentCategories.Where(f => f.EMCo != 2 && f.EMCo != 3 && f.EMCo != 101 && f.Description.Contains("Rig")).OrderBy(o => o.CategoryId).ToList();
            var list = riglist.Select(s => new
            {
                CategoryId = s.CategoryId,
                Description = s.Description,
                Active = db.Equipments.Any(f => f.CategoryId == s.CategoryId && f.EMCo != 2 && f.EMCo != 3 && f.EMCo != 101 && f.Status == "A"),
                Count = db.Equipments.Where(f => f.CategoryId == s.CategoryId && f.EMCo != 2 && f.EMCo != 3 && f.EMCo != 101 && f.Status == "A").Count(),
            }).Distinct().OrderBy(o => o.Description).ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Equipment"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.CategoryId.ToString(AppCultureInfo.CInfo()),
                Text = s.Description,
                Group = new SelectListGroup
                {
                    Name = s.Active ? "Active" : "Inactive",
                    Disabled = !s.Active
                },
            }).ToList().Distinct());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CategoryCombo(byte emco)
        {
            using var db = new VPContext();
            var list = db.EquipmentCategories.Where(f => f.EMCo == emco).ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Category"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.CategoryId.ToString(AppCultureInfo.CInfo()),
                Text = s.Description
            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult CategorySearch(byte emco)
        {
            using var db = new VPContext();
            var comp = db.EMCompanyParms.FirstOrDefault(f => f.EMCo == emco);
            var result = new EquipmentCategoryListViewModel(comp);

            return PartialView("../EM/Category/Search/Panel", result);
        }

        [HttpGet]
        public PartialViewResult CategorySearchTable(byte emco)
        {
            using var db = new VPContext();
            var comp = db.EMCompanyParms.FirstOrDefault(f => f.EMCo == emco);
            var result = new EquipmentCategoryListViewModel(comp);

            return PartialView("../EM/Category/Search/Table", result);
        }

        [HttpPost]
        public JsonResult CategorySearchReturn(EquipmentCategoryViewModel model)
        {
            if (model == null)
            {
                model = new EquipmentCategoryViewModel();
            }
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Json(new { success = "true", value = model.CategoryId, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult AssignmentTypeCombo()
        {
            var result = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select Type" },
                new SelectListItem { Text = "Equipment", Value = "1" },
                new SelectListItem { Text = "Crew", Value = "2" },
                new SelectListItem { Text = "Employee", Value = "3" }
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RevCodeCombo(byte emGroupId, string selected)
        {
            using var db = new VPContext();
            var list = db.EMRevCodes.Where(f => f.EMGroup == emGroupId).ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Code"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.RevCode.ToString(AppCultureInfo.CInfo()),
                Text = s.Description,
                Selected = s.RevCode.ToString(AppCultureInfo.CInfo()) == selected ? true : false
            }).ToList()
            );

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult StatusCombo()
        {

            var list = StaticFunctions.GetComboValues("EMEquipStatus");
            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Status"
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
        public JsonResult OwnershipCombo()
        {

            var list = StaticFunctions.GetComboValues("EMOwnerStatus");
            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Status"
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
        public JsonResult TypeCombo()
        {
            var list = StaticFunctions.GetComboValues("EMEquipType");
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
        public JsonResult EMCostCodeCombo(byte? emGroupId)
        {
            if (emGroupId == null)
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
                var list = db.EMCostCodes.Where(w => w.EMGroupId == emGroupId).ToList();
                var listSelect = list.Select(s => new SelectListItem
                {
                    Value = s.CostCodeId,
                    Text = s.Description,
                }).ToList();

                var result = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "Select Cost Code"
                    }
                };
                result.AddRange(listSelect);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult EMCostCodeAPCombo(byte? emGroupId)
        {
            if (emGroupId == null)
            {
                var result = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "EM Group Id missing",
                    }
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                using var db = new VPContext();
                var list = db.EMCostCodes.Where(w => w.EMGroupId == emGroupId).ToList();
                var listSelect = list.Select(s => new SelectListItem
                {
                    Value = s.CostCodeId,
                    Text = string.Format(AppCultureInfo.CInfo(), "{0} - {1}", s.CostCodeId, s.Description),
                }).ToList();

                var result = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "Select Cost Code"
                    }
                };
                result.AddRange(listSelect);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult EMCostTypeCombo(byte? emGroupId, string costCodeId)
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
                    var list = emCostCode.CostTypes.Select(s => s.CostType).ToList();
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

        public JsonResult EMCostTypeAPCombo(byte emGroupId, string selected)
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
        public JsonResult EMServiceCombo(byte emco)
        {
            using var db = new VPContext();
            var list = db.EMServiceItems.Where(f => f.EMCo == emco).ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Service"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.ServiceItemId.ToString(AppCultureInfo.CInfo()),
                Text = s.Description
            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ShopCombo(byte shopGroupId)
        {
            using var db = new VPContext();
            var list = db.EMShops
                        .Where(f => f.ShopGroupId == shopGroupId)
                        .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Shop"
                }
            };
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.ShopId.ToString(AppCultureInfo.CInfo()),
                Text = s.Description,

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult MechanicCombo(byte prco, string shopId)
        {
            using var db = new VPContext();

            // Get employees with Position = "Mechanic"
            var list = db.HRResources
                .Where(f => f.HRCo == prco &&
                            f.ActiveYN == "Y" &&
                            f.Position != null &&
                            f.Position.Description == "Mechanic")
                .OrderBy(o => o.LastName)
                .ThenBy(o => o.FirstName)
                .ToList();

            var result = new List<SelectListItem>
    {
        new SelectListItem
        {
            Text = "Select Mechanic"
        }
    };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.HRRef.ToString(AppCultureInfo.CInfo()),
                Text = string.Format(AppCultureInfo.CInfo(), "{0} {1}", s.FirstName, s.LastName)
            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }



    }
}