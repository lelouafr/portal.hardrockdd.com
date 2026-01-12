using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace portal.Controllers.VP.JV
{
    [Authorize]
    public class JCComboController : BaseController
    {
        [HttpGet]
        public PartialViewResult Search(byte jcco)
        {
            using var db = new VPContext();
            var comp = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == jcco);
            var result = new portal.Areas.Project.Models.Job.JobListViewModel(comp, 1, db);

            return PartialView("../JC/Job/Search/Panel", result);
        }

        [HttpGet]
        public PartialViewResult SearchTable(byte jcco)
        {
            using var db = new VPContext();
            var comp = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == jcco);
            var result = new portal.Areas.Project.Models.Job.JobListViewModel(comp, 1, db);

            return PartialView("../JC/Job/Search/Table", result);
        }

        [HttpPost]
#pragma warning disable CA3147 // Mark Verb Handlers With Validate Antiforgery Token
        public JsonResult SearchReturn(portal.Areas.Project.Models.Job.JobViewModel model)
#pragma warning restore CA3147 // Mark Verb Handlers With Validate Antiforgery Token
        {
            if (model == null)
            {
                model = new portal.Areas.Project.Models.Job.JobViewModel();
            }
            //var jsonModelState = JsonConvert.SerializeObject(ModelState, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Json(new { success = "true", value = model.JobId, errorModel = ModelState.ModelErrors() });
        }

        [HttpGet]
        public JsonResult Combo(byte? jcco)
        {
            if (jcco == null)
            {
                jcco = 0;
            }
            using var db = new VPContext();
            var jobTypeStr = ((int)DB.JCJobTypeEnum.Job).ToString();
            var list = db.Jobs
                        .Where(f => f.JCCo == jcco)// && f.JobStatus <= 2 && f.JobTypeId == jobTypeStr
                        .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Job"
                }
            };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.JobId,
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.ContractId, s.Description),
                Group = new SelectListGroup
                {
                    Name = s.JobStatus == 1 ? "Open" : s.JobStatus == 2 ? "Soft Closed" : "Closed",
                    Disabled = s.JobStatus >= 2
                },

            }).ToList());


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult AwardCombo(byte jcco)
        {
           // var list = JobRepository.GetJobs(jcco, "Jobs", true);

            using var db = new VPContext();
            var list = db.Jobs.Where(f => (f.JobTypeId == ((int)DB.JCJobTypeEnum.Job).ToString() ||
                                          f.JobTypeId == ((int)DB.JCJobTypeEnum.ContractJob).ToString()) &&
                                          f.JobStatus == 1).ToList();
            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Job",
                }
            };
            result.Add(new SelectListItem
            {
                Text = "Create New Job",
                Value = "New"
            });
            //result.AddRange(JobRepository.GetSelectList(list, selected));

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.JobId,
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.ContractId, s.Description),
                Group = new SelectListGroup
                {
                    Name = s.JobStatus == 1 ? "Open" : s.JobStatus == 2 ? "Soft Closed" : "Closed",
                    Disabled = s.JobStatus >= 2
                },

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CrewCombo(byte jcco, string crewId)
        {
            using var db = new VPContext();
            var list = db.Jobs.Where(f => (f.JobTypeId == ((int)DB.JCJobTypeEnum.Job).ToString() ||
                                           f.JobTypeId == ((int)DB.JCJobTypeEnum.ContractJob).ToString())
                                           && f.JobStatus == 1);
            //var list = JobRepository.GetJobs(jcco, "Jobs", true);
            //var newList = list.Where(f => f.CrewId == crewId).ToList();
            //if (newList.Count > 0)
            //{
            //    list = newList;
            //}
            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Job"
                }
            };
            //result.AddRange(JobRepository.GetSelectList(list, selected));
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.JobId,
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.ContractId, s.Description),
                Group = new SelectListGroup
                {
                    Name = s.JobStatus == 1 ? "Open" : s.JobStatus == 2 ? "Soft Closed" : "Closed",
                    Disabled = s.JobStatus >= 2
                },

            }).ToList());


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult JobTicketCombo(byte dtco, int ticketId)
        {
            using var db = new VPContext();
            var ticket = db.DailyJobTickets.FirstOrDefault(f => f.DTCo == dtco && f.TicketId == ticketId);
            var list = db.Jobs.Where(f => (f.JobTypeId == ((int)DB.JCJobTypeEnum.Job).ToString() ||
                                           f.JobTypeId == ((int)DB.JCJobTypeEnum.ContractJob).ToString())
                                           && (f.JobStatus == 1 || f.JobId == ticket.JobId)).ToList();
            var sList =  list.Select(s => new SelectListItem
            {
                Value = s.JobId,
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.JobId, s.Description),
            }).ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Job"
                }
            };
            result.AddRange(sList);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult BidCombo(byte bdco, int bidId, int packageId)
        {
            using var db = new VPContext();
            var package = db.BidPackages.FirstOrDefault(f => f.BDCo == bdco && f.BidId == bidId && f.PackageId == packageId);

            var list = new List<Job>();
            if (package.Project != null)
            {
                list = package.Project.SubJobs.ToList();
            }
            var sList = list.Select(s => new SelectListItem
            {
                Value = s.JobId,
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.JobId, s.Description),
            }).ToList();
            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Job"
                }
            };
            result.Add(new SelectListItem { Text = "New Job", Value = "00-0000-00" });


            result.AddRange(sList);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SubJobCombo(byte jcco, string parentJobId)
        {
            using var db = new VPContext();

            var list = db.Jobs.Where(f => 
                                          f.ParentJobId == parentJobId &&
                                          f.JobId != parentJobId &&
                                          f.JobStatus <= 3
                                          ).ToList();//f.JCCo == co &&

            var sList = list.Select(s => new SelectListItem
            {
                Value = s.JobId,
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.JobId, s.Description),
            }).ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Job"
                }
            };
            result.Add(new SelectListItem { Text = "New Job", Value = jcco == 1 ? "00-0000-00" : "00-0000-10" });


            result.AddRange(sList);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult BidProjectCombo(byte jcco, int bidId)
        {
            using var db = new VPContext();
            var bid = db.Bids.FirstOrDefault(f => f.BidId == bidId);
            var JobIds = bid.Packages.Select(s => s.JobId).ToList();
            var list = db.Jobs.Where(f => 
                                        (f.JobTypeId == ((int)DB.JCJobTypeEnum.Project).ToString() && 
                                         f.StatusId != ((int)DB.JCJobStatusEnum.Invoiced).ToString()) || 
                                         JobIds.Contains(f.JobId)
                                        )
                                .ToList();
            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "New Project",
                    Value = jcco == 1 ? "00-0000-00": "00-0000-10"
                }
            };
            var selectList = list.Select(s => new SelectListItem
            {
                Value = s.JobId,
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.JobId, s.Description),
            }).ToList();
            result.AddRange(selectList);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ShopCrewJobCombo(byte jcco, string crewId)
        {

            using var db = new VPContext();
            var list = db.Jobs.Where(f => f.JobTypeId == ((int)DB.JCJobTypeEnum.ShopYard).ToString() && f.CrewId == crewId).ToList();
            //var list = JobRepository.GetJobs(jcco, "Shop", true).Where(f => f.CrewId == crewId).ToList(); 

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Job"
                }
            };
            //result.AddRange(JobRepository.GetSelectList(list, selected));
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.JobId,
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.ContractId, s.Description),
                Group = new SelectListGroup
                {
                    Name = s.JobStatus == 1 ? "Open" : s.JobStatus == 2 ? "Soft Closed" : "Closed",
                    Disabled = s.JobStatus >= 2
                },

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ActiveCombo(byte? jcco)
        {
            if (jcco == null)
            {
                jcco = 0;
            }
            var memKey = string.Format("JCCombo_ActiveCombo_JobList_{0}", jcco);
            ObjectCache systemCache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(5)
            };

            if (!(MemoryCache.Default[memKey] is List<Job> list))
            {
                using var db = new VPContext();
                list = db.Jobs
                            .Where(f => f.JobStatus <= 1)
                            .OrderBy(f => f.JobStatus)
                            .ThenBy(o => o.JobId)
                            .ToList();//f.JCCo == co && 

                systemCache.Set(memKey, list, policy);
            }

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Job",
                }
            };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.JobId,
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.JobId, s.Description),
                Group = new SelectListGroup
                {
                    Name = s.JobStatus <= 1 ? "Open" : "Closed",
                    Disabled = s.JobStatus > 1
                },

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult POCombo(byte? jcco)
        {
            var memKey = "JCCombo_POCombo_JobList";
            if (jcco == null)
            {
                jcco = 0;
            }
            ObjectCache systemCache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(5)
            };

            if (!(MemoryCache.Default[memKey] is List<Job> list))
            {
                using var db = new VPContext();
                list = db.Jobs
                            .Where(f => f.JobStatus <= 1 && f.JobTypeId != ((int)DB.JCJobTypeEnum.ShopYard).ToString())
                            .OrderBy(f => f.JobStatus)
                            .ThenBy(o => o.JobId)
                            .ToList();//f.JCCo == co && 

                systemCache.Set(memKey, list, policy);
            }

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Job",
                }
            };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.JobId,
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.JobId, s.Description),
                Group = new SelectListGroup
                {
                    Name = s.JobStatus <= 1 ? "Open" : "Closed",
                    Disabled = s.JobStatus > 1
                },

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult APCombo(byte jcco)
        {
            using var db = new VPContext();
            var list = db.Jobs
                        .Where(f =>f.JobStatus <= 2)
                        .OrderBy(f => f.JobStatus)
                        .ThenBy(o => o.JobId)
                        .ToList();// f.JCCo == co && 

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Job",
                }
            };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.JobId,
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.JobId, s.Description),
                Group = new SelectListGroup
                {
                    Name = s.JobStatus == 1 ? "Open" : s.JobStatus == 2 ? "Soft Closed" : "Closed",
                    Disabled = s.JobStatus > 2
                },

            }).ToList());
            // result.AddRange(JobRepository.GetSelectList(list, selected));

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CCCombo(byte ccco, long transId)
        {
            using var db = new VPContext();
            var list = db.Jobs
                        .Where(f =>
                                    f.JobStatus <= 2 &&
                                    f.JobTypeId != ((int)DB.JCJobTypeEnum.ShopYard).ToString())
                        .OrderBy(f => f.JobStatus)
                        .ThenBy(o => o.JobId)
                        .ToList();
            var result = new List<SelectListItem>();

            var tran = db.CreditTransactions.FirstOrDefault(f => f.CCCo == ccco && f.TransId == transId);
            var minDate = tran.TransDate.AddDays(-3);
            //var maxDate = tran.TransDate.AddDays(1);
            var currentJobList = tran.Employee.JobList(minDate);

            result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Job",
                }
            };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.JobId,
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.JobId, s.Description),
                Group = new SelectListGroup
                {
                    Name = currentJobList.Any(f => f.JobId == s.JobId) ? "Current Job(s)" :  
                                            s.JobStatus == 1 ? "Open" : 
                                            s.JobStatus == 2 ? "Soft Closed" : 
                                            "Closed",
                    Disabled = s.JobStatus > 2
                },

            }).OrderBy(o => o.Group.Name == "Current Job(s)" ? 0 : 
                            o.Group.Name == "Open" ? 1 :
                            o.Group.Name == "Soft Closed" ? 2 :
                            o.Group.Name == "Closed" ? 3 : 4)
             .ThenBy(o => o.Value).ToList());

            //systemCache.Set(memKeyTrans, result, policy);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult TruckCombo(byte jcco)
        {
            //var list = JobRepository.GetJobs(jcco, "Jobs", true);
            //list.AddRange(JobRepository.GetJobs(jcco, "Shop", true));

            using var db = new VPContext();
            var list = db.Jobs.Where(f => (f.JobTypeId == ((int)DB.JCJobTypeEnum.Job).ToString() ||
                                          f.JobTypeId == ((int)DB.JCJobTypeEnum.ContractJob).ToString() ||
                                          f.JobTypeId == ((int)DB.JCJobTypeEnum.ShopYard).ToString()
                                          ) && f.JobStatus == 1
                                          ).ToList();
            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Job"
                }
            };
            //result.AddRange(JobRepository.GetSelectList(list, selected));
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.JobId,
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.ContractId, s.Description),
                Group = new SelectListGroup
                {
                    Name = s.JobStatus == 1 ? "Open" : s.JobStatus == 2 ? "Soft Closed" : "Closed",
                    Disabled = s.JobStatus >= 2
                },

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ShopJobCombo(byte jcco)
        {
            //var list = JobRepository.GetJobs(jcco, "Shop", true);

            using var db = new VPContext();
            var list = db.Jobs.Where(f => f.JobTypeId == ((int)DB.JCJobTypeEnum.ShopYard).ToString() && f.JobStatus == 1).ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Job"
                }
            };
            //result.AddRange(JobRepository.GetSelectList(list, selected));
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.JobId,
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.ContractId, s.Description),
                Group = new SelectListGroup
                {
                    Name = s.JobStatus == 1 ? "Open" : s.JobStatus == 2 ? "Soft Closed" : "Closed",
                    Disabled = s.JobStatus >= 2
                },

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CostJobCombo(byte jcco)
        {
            //var list = JobRepository.GetJobs(jcco, "JOB COST", true);

            using var db = new VPContext();
            var list = db.Jobs.Where(f => f.JobTypeId == ((int)DB.JCJobTypeEnum.CrewJobCost).ToString() && f.JobStatus == 1).ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Job"
                }
            };
            //result.AddRange(JobRepository.GetSelectList(list, selected));
            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.JobId,
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.ContractId, s.Description),
                Group = new SelectListGroup
                {
                    Name = s.JobStatus == 1 ? "Open" : s.JobStatus == 2 ? "Soft Closed" : "Closed",
                    Disabled = s.JobStatus >= 2
                },

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult POJobCombo(byte jcco)
        {
            using var db = new VPContext();
            var list = db.Jobs.Where(f => f.JobStatus == 1).ToList();
            //var list = JobRepository.GetJobs(jcco, true);

            var sList = list.Select(s => new SelectListItem
            {
                Value = s.JobId,
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.JobId, s.Description),
            }).ToList();
            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Job"
                }
            };
            result.AddRange(sList);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult StatusCombo(string selected)
        {
            using var db = new VPContext();
            var list = db.JobStatuses.ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Status"
                }
            };
            result.AddRange(list.OrderBy(o => o.Id)
                         .Select(s => new SelectListItem
                         {
                             Value = s.Id.ToString(AppCultureInfo.CInfo()),
                             Text = string.Format(AppCultureInfo.CInfo(), "{0}", s.Description),
                             Selected = s.Id.ToString(AppCultureInfo.CInfo()) == selected ? true : false
                         }).ToList());


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult IndustryCombo(byte jcco)
        {
            using var db = new VPContext();

            var list = db.JCIndustries
                        .Where(f => f.JCCo == jcco)
                        .OrderBy(f => f.Description)
                        .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Industry",
                }
            };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.IndustryId.ToString(AppCultureInfo.CInfo()),
                Text = s.Description,
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
        public JsonResult IndustryMarketCombo(byte jcco, int industryId)
        {
            using var db = new VPContext();

            var list = db.JCIndustryMarkets
                        .Where(f => f.JCCo == jcco && f.IndustryId == industryId && f.MarketId != null)
                        .OrderBy(f => f.Market.Description)
                        .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Industry",
                }
            };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.MarketId?.ToString(AppCultureInfo.CInfo()),
                Text = s.Market.Description,
                Selected = false,
                Group = new SelectListGroup
                {
                    Name = s.Market.ActiveYN == "Y" ? "Active" : "Inactive",
                    Disabled = s.Market.ActiveYN != "Y"
                },

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult JCMarketCombo(byte jcco)
        {
            using var db = new VPContext();

            var list = db.JCMarkets
                        .Where(f => f.JCCo == jcco)
                        .OrderBy(f => f.Description)
                        .ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Market",
                }
            };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.MarketId.ToString(AppCultureInfo.CInfo()),
                Text = s.Description,
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
        public JsonResult ContractCombo(byte jcco)
        {
            using var db = new VPContext();
            var list = db.JCContracts.Where(f => f.JCCo == jcco).ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Contract"
                }
            };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.ContractId,
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.ContractId, s.Description),
                Group = new SelectListGroup
                {
                    Name = s.ContractStatus == 1 ? "Open" : s.ContractStatus == 2 ? "Soft Closed" : "Closed",
                    Disabled = s.ContractStatus >= 2
                },

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ContractItemsCombo(byte jcco, string contractId)
        {
            using var db = new VPContext();
            var list = db.ContractItems.Where(f => f.JCCo == jcco && f.ContractId == contractId).ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Item"
                }
            };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.Item,
                Text = string.Format(AppCultureInfo.CInfo(), "{0}", s.Description),

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult DepartmentCombo(byte jcco)
        {
            using var db = new VPContext();
            var list = db.JobDepartments.Where(f => f.JCCo == jcco).ToList();

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Department"
                }
            };

            result.AddRange(list.Select(s => new SelectListItem
            {
                Value = s.DepartmentId,
                Text = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", s.DepartmentId, s.Description),
                Group = new SelectListGroup
                {
                    Name = s.Active == "Y" ? "Active" : "Disabled",
                    Disabled = s.Active != "Y"
                },

            }).ToList());

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult JCJobPhaseCombo(byte? jcco, string? jobId)
        {
            if (jcco == null || jobId == null)
            {
                var result = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "JCCo/Job Id missing",
                    }
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                using var db = new VPContext();
                var list = db.JobPhases
                                .Where(f => f.JCCo == jcco && f.JobId == jobId)
                                .ToList();
                var result = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "Select Phase"
                    }
                };

                result.AddRange(list.Select(s => new SelectListItem
                {
                    Value = s.PhaseId,
                    Text = s.Description,
                    Group = new SelectListGroup
                    {
                        Name = s.ActiveYN == "Y" ? "Active" : "Disabled",
                        Disabled = s.ActiveYN != "Y"
                    },

                }).ToList());


                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

    }
}