using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Equipment;
using portal.Models.Views.Equipment.Audit;
using portal.Repository.VP.HQ;
using portal.Repository.VP.WF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;

namespace portal.Repository.VP.EM
{
    public static class EquipmentAuditRepository
    {
     
        public static void AutoGenerateUserEquipmentAudit()
        {
            using var db = new VPContext();
            var userId = StaticFunctions.GetUserId();

            ObjectCache systemCache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(480)
            };
            var memKey = "EMAuditChecked_" + userId;
            if (!(MemoryCache.Default[memKey] is bool cacheEMAuditChecked))
            {
                cacheEMAuditChecked = false;
            }


            if (!cacheEMAuditChecked)
            {
                var empHr = StaticFunctions.GetCurrentEmployee();
                var emp = db.Employees.FirstOrDefault(f => f.PRCo == empHr.PRCo && f.EmployeeId == empHr.EmployeeId);
                var model = new EquipmentAuditCreateViewModel();
                model.EMCo = emp.PRCo;
                model.AuditForm = DB.EMAuditFormEnum.Meter;
                if (emp.Crew?.CrewLeaderId == emp.EmployeeId && emp.Crew?.udCrewType == "Crew" && emp.Crew?.CrewStatus == "ACTIVE")
                {
                    model.AuditType = DB.EMAuditTypeEnum.CrewAudit;
                    model.CrewId = emp.CrewId;
                    model.IncludeDirectReportEmployeeEquipment = true;
                    model.IncludCrewLeaderEquipment = true;
                    model.IncludeSubEquipment = true;
                }
                else
                {
                    model.AuditType = DB.EMAuditTypeEnum.EmployeeAudit;
                    model.IncludeDirectReportEmployeeEquipment = false;
                    model.IncludCrewLeaderEquipment = false;
                    model.IncludeSubEquipment = true;
                    model.EmployeeId = emp.EmployeeId;
                }
                model.AssignedTo = StaticFunctions.GetUserId();
                var currentDate = DateTime.Now.Date;
                var cal = db.Calendars.FirstOrDefault(f => f.Date == currentDate);
                var currentWeek = db.Calendars.Where(f => f.Week == cal.Week);

                var userAudits = db.EMAudits.Where(f => f.AuditTypeId == (int)model.AuditType &&
                                                          f.AuditFormId == (int)model.AuditForm &&
                                                          f.ParmCrewId == model.CrewId &&
                                                          f.AssignedTo == model.AssignedTo).ToList();

                var currentAudit = userAudits.Where(f =>((f.CreatedOn >= currentWeek.Min(min => min.Date) && f.CreatedOn <= currentWeek.Max(max => max.Date)) ||
                                                           (f.CreatedOn < currentWeek.Min(min => min.Date) &&
                                                            (f.Status == DB.EMAuditStatusEnum.New || 
                                                             f.Status == DB.EMAuditStatusEnum.Started || 
                                                             f.Status == DB.EMAuditStatusEnum.Rejected
                                                            )
                                                           )
                                                          )) 
                                              .ToList();

                cacheEMAuditChecked = true;
                systemCache.Set(memKey, cacheEMAuditChecked, policy);

                if (!currentAudit.Any() && currentDate.DayOfWeek != DayOfWeek.Saturday && currentDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    var eqpList = new EquipmentListViewModel(model);
                    if (eqpList.List.Count > 0)
                    {
                        var audit = Create(model, db);
                        db.EMAudits.Add(audit);
                        db.SaveChanges();
                    }
                }
            }

        }

        public static EMAudit Reject(EquipmentAuditRejectViewModel model, VPContext db)
        {
            if (model == null) throw new System.ArgumentNullException(nameof(model));
            if (db == null) throw new System.ArgumentNullException(nameof(db));


            var audit = db.EMAudits.FirstOrDefault(f => f.EMCo == model.EMCo && f.AuditId == model.AuditId);
            audit.Status = DB.EMAuditStatusEnum.Rejected;
            audit.WorkFlow.CurrentSequence().Comments = model.Comments;
            var log = audit.StatusLogs.OrderByDescending(f => f.LineNum).FirstOrDefault();
            log.Comments = model.Comments;
            return audit;
        }

        public static EMAudit Create(EquipmentAuditCreateViewModel model, VPContext db)
        {
            if (model == null) throw new System.ArgumentNullException(nameof(model));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            var userId = StaticFunctions.GetUserId();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var assignedUser = db.WebUsers.FirstOrDefault(f => f.Id == model.AssignedTo);
            var emParms = db.EMCompanyParms.FirstOrDefault(f => f.EMCo == model.EMCo);
            var audit = new EMAudit();
            if (!string.IsNullOrEmpty(model.Description))
            {
                model.Description += " - ";
            }
            else
            {
                model.Description = "";
            }
            switch (model.AuditType)
            {
                case DB.EMAuditTypeEnum.CrewAudit:
                    var crew = db.Crews.FirstOrDefault(f => f.PRCo == model.EMCo && f.CrewId == model.CrewId);
                    model.Description +=  crew.Description;
                    audit.ParmCrewId = model.CrewId;
                    break;
                case DB.EMAuditTypeEnum.EmployeeAudit:
                    var emp = db.Employees.FirstOrDefault(f => f.PRCo == model.EMCo && f.EmployeeId == model.EmployeeId);
                    model.Description += string.Format(AppCultureInfo.CInfo(), "{0} {1}", emp.FirstName, emp.LastName);
                    audit.ParmEmployeeId = model.EmployeeId;
                    break;
                case DB.EMAuditTypeEnum.LocationAudit:
                    var Loc = db.EMLocations.FirstOrDefault(f => f.EMCo == model.EMCo && f.LocationId == model.LocationId);
                    model.Description += string.Format(AppCultureInfo.CInfo(), "{0}", Loc.Description);
                    audit.ParmLocationId = model.LocationId;
                    break;
                default:
                    break;
            }

            audit.EMParameter = emParms;
            audit.EMCo = model.EMCo;
            audit.AuditId = db.EMAudits.DefaultIfEmpty().Max(f => f == null ? 0 : f.AuditId) + 1;
            audit.AssignedTo = model.AssignedTo;
            audit.AssignedUser = assignedUser;
            audit.CreatedBy = StaticFunctions.GetUserId();
            audit.CreatedOn = DateTime.Now;
            audit.CreatedUser = user;
            audit.AuditTypeId = (byte)model.AuditType;
            audit.AuditFormId = (byte)model.AuditForm;
            audit.Status = DB.EMAuditStatusEnum.New;
            audit.Description = model.Description;
            audit.ParmIncludeCrewLeaderEquipment = model.IncludCrewLeaderEquipment;
            audit.ParmIncludeDirectReports = model.IncludeDirectReportEmployeeEquipment;
            audit.ParmIncludeSubEquipment = model.IncludeSubEquipment;
           
            var eqpList = new EquipmentListViewModel(model);
            var seqId = 1;
            foreach (var eqp in eqpList.List)
            {
                var line = new EMAuditLine
                {
                    Completed = false,
                    EMCo = audit.EMCo,
                    AuditId = audit.AuditId,
                    EquipmentId = eqp.EquipmentId,
                    SeqId = seqId,
                    ActionId = (byte)DB.EMAuditLineActionEnum.Update,
                    MeterTypeId = eqp.MeterType == null ? ((byte)DB.EMMeterTypeEnum.Both).ToString(AppCultureInfo.CInfo()) : ((byte)eqp.MeterType).ToString(AppCultureInfo.CInfo())
                };
                audit.Lines.Add(line);
                seqId++;
            }
            //GenerateWorkFlow(audit, db);
            //audit.StatusLogs.Add(EquipmentAuditStatusLogRepository.Init(audit));

            return audit;
        }
        
        public static void UpdateStatus(EMAudit audit, byte status, VPContext db)
        {
            if (audit == null) throw new System.ArgumentNullException(nameof(audit));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            if (audit != null)
            {
                audit.Status = (DB.EMAuditStatusEnum)status;
                
            }
        }

    }
}