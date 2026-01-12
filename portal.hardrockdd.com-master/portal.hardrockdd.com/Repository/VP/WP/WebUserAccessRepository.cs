using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Data.Entity;
using System.Web;
using portal.Models.Views.Dashboard;
using System.Web.Security;

namespace portal.Repository.VP.WP
{
    public static class WebUserAccessRepository
    {
        public static void AutoAddAccess(string ControllerName, string ActionName, DB.AccessLevelEnum AccessLevel, VPContext db)
        {

            //using var db = new VPContext();
            //var webUser = db.WebUsers.FirstOrDefault(f => f.Id == userId);//s.Include(x => x.ControllerAccess.Select(y => y.ControllerAction))
            var employee = StaticFunctions.GetCurrentHREmployee();
            if (employee.PortalAccountActive == "N" || employee.ActiveYN == "N")
            {
                return;
            }
            var controller = CachedAccessList().FirstOrDefault(f => f.ControllerAction.ControllerName.ToLower() == ControllerName.ToLower() && f.ControllerAction.ActionName.ToLower() == ActionName.ToLower());
            if (controller == null)
            {
                var controllerAction = db.WebControllers.FirstOrDefault(f => f.ControllerName.ToLower() == ControllerName.ToLower() && f.ActionName.ToLower() == ActionName.ToLower());
                if (controllerAction == null)
                {
                    controllerAction = new WebController
                    {
                        Id = db.WebControllers.DefaultIfEmpty().Max(f => f == null ? 0 : f.Id) + 1,
                        ControllerName = ControllerName,
                        ActionName = ActionName,
                        //Path = url
                    };
                    db.WebControllers.Add(controllerAction);
                }
                if (controllerAction != null)
                {
                    var access = new WebUserAccess
                    {
                        UserId = StaticFunctions.GetUserId(),
                        ControllerActionId = controllerAction.Id,
                        AccessLevel = (byte)AccessLevel,
                        ControllerAction = controllerAction
                    };
                    controllerAction.Users.Add(access);
                    db.BulkSaveChanges();

                    //Update the cached User Access
                    CachedAccessList(true);
                }
            }
            else
            {
                var userId = StaticFunctions.GetUserId();
                if (AccessLevel == DB.AccessLevelEnum.Denied)
                {
                    var controllerAction = db.WebUserAccesses.FirstOrDefault(f => f.ControllerActionId == controller.ControllerActionId && f.UserId == userId);
                    controllerAction.AccessLevel = (byte)AccessLevel;
                    db.BulkSaveChanges();
                    CachedAccessList(true);
                }
                else if (controller.AccessLevel < (byte)AccessLevel)
                {
                    var controllerAction = db.WebUserAccesses.FirstOrDefault(f => f.ControllerActionId == controller.ControllerActionId && f.UserId == userId);
                    controllerAction.AccessLevel = (byte)AccessLevel;
                    db.BulkSaveChanges();
                    CachedAccessList(true);
                }
            }


        }

        public static List<WebUserAccess> CachedAccessList(bool flushCache = false)
        {
            var userId = StaticFunctions.GetUserId();
            var memKey = "UserForms_" + userId;
            if (!(MemoryCache.Default[memKey] is List<WebUserAccess> cacheAccess) || flushCache)
            {
                using var db = new VPContext();
                ObjectCache systemCache = MemoryCache.Default;
                //var webUser = db.WebUsers.Include(x => x.ControllerAccess.Select(y => y.ControllerAction)).FirstOrDefault(f => f.Id == userId);
                var webUser = db.WebUsers
                    .Include("ControllerAccess")
                    .Include("ControllerAccess.ControllerAction")
                    .Include("Employee")
                    .FirstOrDefault(f => f.Id == userId);
                if (webUser?.Employee == null)
                {
                    cacheAccess = new List<WebUserAccess>();
                    return cacheAccess;
                }
                if ((webUser.Employee.FirstOrDefault().PortalAccountActive == "N" || webUser.Employee.FirstOrDefault().ActiveYN == "N") && webUser.Employee.FirstOrDefault().HRRef != 100798)
                {
                    cacheAccess = new List<WebUserAccess>();
                    return cacheAccess;
                }
                var list = webUser.ControllerAccess.ToList();
                cacheAccess = list;
                CacheItemPolicy policy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(10)
                };
                systemCache.Set(memKey, cacheAccess, policy);
            }

            return cacheAccess;
        }


        public static void AddDefaultSecurity(bool force = false)
        {

            var userId = StaticFunctions.GetUserId();

            ObjectCache systemCache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(60)
            };
            var memKey = "DefaultSecurityChecked_" + userId;
            if (!(MemoryCache.Default[memKey] is bool cacheDefaultSecurityChecked))
            {
                cacheDefaultSecurityChecked = false;
            }
            using var db = new VPContext();
            var webUser = db.WebUsers
                   .Include("ControllerAccess")
                   .Include("ControllerAccess.ControllerAction")
                   .Include("Employee")
                   .Include("DivisionLinks")
                   .Include("Employee.PREmployee")
                   .FirstOrDefault(f => f.Id == userId);
            var emp = webUser?.Employee.FirstOrDefault();
            if (emp == null)
            {
                cacheDefaultSecurityChecked = true;
                systemCache.Set(memKey, cacheDefaultSecurityChecked, policy);
                return;
            }
            else if((emp.ActiveYN == "N" || emp.PortalAccountActive == "N") && emp.HRRef != 100798)
            {
                cacheDefaultSecurityChecked = true;
                systemCache.Set(memKey, cacheDefaultSecurityChecked, policy);
                return;
            }
            if (!webUser.DivisionLinks.Any())
            {
                webUser.DivisionLinks.Add(new WPUserDivisionLink() { 
                    UserId = webUser.Id,
                    DivisionId = emp.PREmployee.DivisionId,
                    Active = true
                });

                db.SaveChanges();
            }
            if (!cacheDefaultSecurityChecked || force)
            {

                var model = new HomeViewModel();
                var user = HttpContext.Current.User;


                if (user.IsInPosition("IT-DIR") ||
                    user.IsInPosition("FIN-CTRL") ||
                    user.IsInPosition("FIN-APMGR") ||
                    user.IsInPosition("FIN-ARMGR") ||
                    user.IsInPosition("OP-DM") ||
                    user.IsInPosition("OP-GM") ||
                    user.IsInPosition("PRES") ||
                    user.IsInPosition("CFO") ||
                    user.IsInPosition("COO") ||
                    user.IsInPosition("HR-MGR") ||
                    user.IsInPosition("AUDIT"))
                {
                    var divisions = db.CompanyDivisions.Where(f => f.IsActive == true).ToList();

                    foreach (var division in divisions)
                    {
                        var userDiv = webUser.DivisionLinks.FirstOrDefault(f => f.DivisionId == division.DivisionId);
                        if (userDiv == null)
                        {
                            webUser.DivisionLinks.Add(new WPUserDivisionLink()
                            {
                                UserId = webUser.Id,
                                DivisionId = division.DivisionId,
                                Active = true
                            });

                            db.BulkSaveChanges();
                        }
                    }
                }

                //WebUserAccessRepository.CachedAccessList(true);
                if (model.DirectReportCnt > 0)
                {
                    WebUserAccessRepository.AutoAddAccess("HRUserEmployee", "Index", DB.AccessLevelEnum.Read, db);
                    WebUserAccessRepository.AutoAddAccess("HRUserEmployee", "Data", DB.AccessLevelEnum.Read, db);
                }
                else
                {
                    WebUserAccessRepository.AutoAddAccess("HRUserEmployee", "Index", DB.AccessLevelEnum.Denied, db);
                    WebUserAccessRepository.AutoAddAccess("HRUserEmployee", "Data", DB.AccessLevelEnum.Denied, db);
                }
                if (model.CMSupervisorNeedingApproval > 0)
                {
                    WebUserAccessRepository.AutoAddAccess("APCreditCardSupervisorTransaction", "Index", DB.AccessLevelEnum.Write, db);
                }
                if (model.CMEmployeeAmount > 0 || model.CMEmployeePriorAmount > 0 || model.CMEmployeePriorMissingPictureAmount > 0)
                {
                    WebUserAccessRepository.AutoAddAccess("APCreditCardEmployeeTransaction", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("APCreditCardTransaction", "Index", DB.AccessLevelEnum.Write, db);
                }
                if (model.POApprovalCount > 0)
                {
                    WebUserAccessRepository.AutoAddAccess("PORequestApproval", "Index", DB.AccessLevelEnum.Write, db);

                }

                if (user.HasAccess("PORequest") || user.IsInPosition("FLD-CL") || user.IsInPosition("OP-PM"))
                {
                    WebUserAccessRepository.AutoAddAccess("UserPO", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("UserPORequest", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("UserPORequest", "PendingIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("PORequestApproval", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("PORequest", "Add", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("PORequest", "Create", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("PORequest", "Index", DB.AccessLevelEnum.Write, db);

                    //WebUserAccessRepository.AutoAddAccess("PORequest", "Create", DB.AccessLevelEnum.Write, db);
                }
                WebUserAccessRepository.AutoAddAccess("DailyTicket", "Index", DB.AccessLevelEnum.Write, db);
                if (user.HasAccess("DailyTicket"))
                {
                    WebUserAccessRepository.AutoAddAccess("DailyTicketForm", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("DailyTicketForm", "PartialIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("DailyTicketForm", "PopupForm", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("DailyTicketForm", "Create", DB.AccessLevelEnum.Write, db);

                    WebUserAccessRepository.AutoAddAccess("DailyTicket", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("DailyTicket", "PopupForm", DB.AccessLevelEnum.Write, db);


                    WebUserAccessRepository.AutoAddAccess("DailyTicketForm", "JobTicketIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("DailyTicketForm", "JobTicketPopupForm", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("DailyTicketForm", "JobTicketForm", DB.AccessLevelEnum.Write, db);

                    WebUserAccessRepository.AutoAddAccess("DailyTicketForm", "ShopTicketIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("DailyTicketForm", "ShopTicketPopupForm", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("DailyTicketForm", "ShopTicketForm", DB.AccessLevelEnum.Write, db);

                    WebUserAccessRepository.AutoAddAccess("DailyTicketForm", "TruckTicketIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("DailyTicketForm", "TruckTicketPopupForm", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("DailyTicketForm", "TruckTicketForm", DB.AccessLevelEnum.Write, db);


                    WebUserAccessRepository.AutoAddAccess("DailyTicketForm", "CrewTicketIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("DailyTicketForm", "CrewTicketPopupForm", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("DailyTicketForm", "CrewTicketForm", DB.AccessLevelEnum.Write, db);

                    WebUserAccessRepository.AutoAddAccess("DailyEmployeeTicketForm", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("DailyEmployeeTicketForm", "PopupForm", DB.AccessLevelEnum.Write, db);

                    WebUserAccessRepository.AutoAddAccess("DailyGeneralTicketForm", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("DailyGeneralTicketForm", "PopupForm", DB.AccessLevelEnum.Write, db);

                    WebUserAccessRepository.AutoAddAccess("DailyEmployeeTimeOffTicketForm", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("DailyEmployeeTimeOffTicketForm", "PopupForm", DB.AccessLevelEnum.Write, db);

                    WebUserAccessRepository.AutoAddAccess("DailyHolidayTicketForm", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("DailyHolidayTicketForm", "PopupForm", DB.AccessLevelEnum.Write, db);


                    WebUserAccessRepository.AutoAddAccess("UserSummary", "Index", DB.AccessLevelEnum.Write, db);
                }
                if (user.IsInPosition("FIN-AUDIT"))
				{
					WebUserAccessRepository.AutoAddAccess("APInvoice", "Index", DB.AccessLevelEnum.Read, db);
					WebUserAccessRepository.AutoAddAccess("AllPO", "Index", DB.AccessLevelEnum.Read, db);
				}
                if (user.HasAccess("Vendor"))
                {
                    //WebUserAccessRepository.AutoAddAccess("Vendor", "Table", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("VendorForm", "PopupForm", DB.AccessLevelEnum.Read, db);
                    WebUserAccessRepository.AutoAddAccess("Vendor", "XData", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Vendor", "XForm", DB.AccessLevelEnum.Write, db);
                }
                if (user.HasAccess("Merchant"))
                {
                    //WebUserAccessRepository.AutoAddAccess("Merchant", "Table", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Merchant", "XData", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Merchant", "XForm", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("MerchantTransaction", "XData", DB.AccessLevelEnum.Read, db);
                }
                if (user.IsInPosition("IT-DIR") ||
                    user.HasAccess("Customer"))
                {
                    WebUserAccessRepository.AutoAddAccess("Customer", "Index", DB.AccessLevelEnum.Read, db);
                    WebUserAccessRepository.AutoAddAccess("Customer", "Data", DB.AccessLevelEnum.Read, db);
                    WebUserAccessRepository.AutoAddAccess("Customer", "PopupForm", DB.AccessLevelEnum.Read, db);
                }
                if (user.IsInPosition("OP-GM") ||
                    user.IsInPosition("OP-DM") ||
                    user.IsInPosition("FIN-CTRL") ||
                    user.IsInPosition("FIN-APMGR") ||
                    user.IsInPosition("FIN-AP") ||
					user.IsInPosition("PRES") ||
					user.IsInPosition("COO") ||
					user.IsInPosition("CEO"))
                {
                    WebUserAccessRepository.AutoAddAccess("AllPORequest", "Index", DB.AccessLevelEnum.Read, db);
                    WebUserAccessRepository.AutoAddAccess("AllPORequest", "Table", DB.AccessLevelEnum.Read, db);
                }
                if (user.IsInPosition("IT-DIR") ||
                    user.IsInPosition("HR-MGR") ||
                    user.IsInPosition("HR-PRMGR") ||
                    user.IsInPosition("PRES") ||
                    user.IsInPosition("CFO") ||
                    user.IsInPosition("FIN-CTRL") ||
                    user.IsInPosition("COO") ||
					user.IsInPosition("CEO"))
                {
                    WebUserAccessRepository.AutoAddAccess("Resource", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Resource", "Data", DB.AccessLevelEnum.Write, db);

                    WebUserAccessRepository.AutoAddAccess("Resource", "ActiveIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Resource", "ActiveData", DB.AccessLevelEnum.Write, db);

                    WebUserAccessRepository.AutoAddAccess("Resource", "TermedIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Resource", "TermedData", DB.AccessLevelEnum.Write, db);
                    
                    WebUserAccessRepository.AutoAddAccess("Resource", "DirectIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Resource", "DirectData", DB.AccessLevelEnum.Write, db);

                    WebUserAccessRepository.AutoAddAccess("Resource", "PopupForm", DB.AccessLevelEnum.Write, db);


					WebUserAccessRepository.AutoAddAccess("PayrollReview", "Index", DB.AccessLevelEnum.Write, db);
					WebUserAccessRepository.AutoAddAccess("Batch", "BatchSourceListIndex", DB.AccessLevelEnum.Write, db);
					WebUserAccessRepository.AutoAddAccess("PRPayrate", "Index", DB.AccessLevelEnum.Write, db);
					WebUserAccessRepository.AutoAddAccess("Crew", "Index", DB.AccessLevelEnum.Write, db);


				}
				if (user.HasAccess("PMProjectGantt") ||
                    user.HasAccess("PMProjectSchedule") ||
                    user.IsInPosition("IT-DIR"))
                {
                    WebUserAccessRepository.AutoAddAccess("PMProjectSchedule", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("PMProjectScheduleForm", "TreeData", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("PMProjectScheduleForm", "TimeLineData", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("PMProjectScheduleForm", "GanttData", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("PMProjectGantt", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("PMProjectGantt", "Data", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("PMProjectGantt", "PopupForm", DB.AccessLevelEnum.Write, db);
                }
                if (model.EMAuditCount > 0 || user.IsInPosition("FLD-CL"))
                {
                    WebUserAccessRepository.AutoAddAccess("EMAuditUser", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("EMAuditForm", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("EMAuditMeterForm", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("EMAuditInventoryForm", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Equipment", "UserIndex", DB.AccessLevelEnum.Read, db);
                    WebUserAccessRepository.AutoAddAccess("EMInspection", "Index", DB.AccessLevelEnum.Write, db);
                }

                if (model.HRTermRequestCount >0 )
                {
                    WebUserAccessRepository.AutoAddAccess("HRTermRequestForm", "UserIndex", DB.AccessLevelEnum.Write, db);
                }

                if (db.GetCurrentEmployee().DirectReports.Any() ||
                    user.IsInPosition("HR-PRMGR") || 
                    user.IsInPosition("HR-MGR") || 
                    user.IsInPosition("IT-DIR") ||
                    user.HasAccess("PositionRequest", "UserIndex")
                    )
                {
                    WebUserAccessRepository.AutoAddAccess("TermRequest", "Create", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("TermRequest", "CreateForm", DB.AccessLevelEnum.Write, db);

                    WebUserAccessRepository.AutoAddAccess("PositionRequest", "Create", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("PositionRequest", "CreateForm", DB.AccessLevelEnum.Write, db);

                    WebUserAccessRepository.AutoAddAccess("PositionRequest", "UserIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("TermRequest", "UserIndex", DB.AccessLevelEnum.Write, db);
                }

                if (user.HasAccess("Equipment"))
                {
                    WebUserAccessRepository.AutoAddAccess("EMInspection", "Index", DB.AccessLevelEnum.Write, db);
                }

                if (model.EMAuditApprovalCount > 0 || user.HasAccess("EMAudit", "ApprovalIndex") || user.IsInPosition("IT-DIR"))
                {
                    WebUserAccessRepository.AutoAddAccess("EMAudit", "ApprovalIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("EquipmentForm", "PopupForm", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Equipment", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("EMAuditSubmissionReport", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("EMAuditSubmissionReport", "Table", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("EMAuditWeeklySubmissionReport", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("EMAuditWeeklySubmissionReport", "Table", DB.AccessLevelEnum.Write, db);
                }

                if (model.EMAuditProcessCount > 0 || user.IsInPosition("IT-DIR"))
                {
                    WebUserAccessRepository.AutoAddAccess("EMAudit", "ProcessIndex", DB.AccessLevelEnum.Write, db);
                }


                if (user.IsInPosition("OP-DM"))
                {
                    WebUserAccessRepository.AutoAddAccess("EMAuditSubmissionReport", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("EMAuditSubmissionReport", "Table", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("EMAuditWeeklySubmissionReport", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("EMAuditWeeklySubmissionReport", "Table", DB.AccessLevelEnum.Write, db);
                }

                if (user.IsInPosition("OP-DM") ||
                    user.IsInPosition("OP-ENGD") ||
                    user.IsInPosition("OP-GM") ||
                    user.IsInPosition("OP-PM") ||
                    user.IsInPosition("OP-SF") ||
                    user.IsInPosition("OP-SLS") ||
                    user.IsInPosition("OP-SLSMGR") ||
                    user.IsInPosition("OP-SUP") ||
                    user.IsInPosition("FIN-AP") ||
                    user.IsInPosition("FIN-APMGR") ||
                    user.IsInPosition("FIN-AR") ||
                    user.IsInPosition("FIN-ARMGR") ||
                    user.IsInPosition("FIN-CTRL") ||
                    user.IsInPosition("IT-DIR") ||
                    user.IsInPosition("CFO") ||
                    user.IsInPosition("COO"))
                {
                    WebUserAccessRepository.AutoAddAccess("Job", "OpenIndex", DB.AccessLevelEnum.Read, db);
                    WebUserAccessRepository.AutoAddAccess("Job", "AllIndex", DB.AccessLevelEnum.Read, db);

                    WebUserAccessRepository.AutoAddAccess("Locate", "LocateIndex", DB.AccessLevelEnum.Read, db);
                    WebUserAccessRepository.AutoAddAccess("Locate", "LocateOpenIndex", DB.AccessLevelEnum.Read, db);
                    WebUserAccessRepository.AutoAddAccess("Locate", "LocateExpiredIndex", DB.AccessLevelEnum.Read, db);

                    WebUserAccessRepository.AutoAddAccess("Locate", "Create", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Locate", "CreateFormBid", DB.AccessLevelEnum.Write, db);

                    WebUserAccessRepository.AutoAddAccess("Locate", "LocateImportIndex", DB.AccessLevelEnum.Read, db);
                    WebUserAccessRepository.AutoAddAccess("Locate", "RequestImportIndex", DB.AccessLevelEnum.Read, db);
                }

                if (user.IsInPosition("OP-DM") ||
                    user.IsInPosition("OP-ENGD") ||
                    user.IsInPosition("OP-SF") ||
                    user.IsInPosition("CFO") ||
                    user.IsInPosition("COO") ||
                    user.IsInPosition("IT-DIR"))
                {

                    WebUserAccessRepository.AutoAddAccess("Locate", "LocateIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Locate", "LocateOpenIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Locate", "LocateExpiredIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Locate", "RequestIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Locate", "RequestOpenIndex", DB.AccessLevelEnum.Write, db);

                    WebUserAccessRepository.AutoAddAccess("Locate", "LocateImportIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Locate", "RequestImportIndex", DB.AccessLevelEnum.Write, db);
                }

                if (db.GetCurrentEmployee().DirectReports.Any() || 
                    user.IsInPosition("CFO") ||
                    user.IsInPosition("HR-MGR") ||
                    user.IsInPosition("HR-PRMGR") ||
                    user.IsInPosition("IT-DIR"))
                {

                    WebUserAccessRepository.AutoAddAccess("PositionRequest", "AllIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("PositionRequest", "OpenIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("PositionRequest", "UserIndex", DB.AccessLevelEnum.Write, db);

                    WebUserAccessRepository.AutoAddAccess("TermRequest", "AllIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("TermRequest", "OpenIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("TermRequest", "SupervisorIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("TermRequest", "UserIndex", DB.AccessLevelEnum.Write, db);
                }

                if (user.IsInPosition("HR-MGR") ||
                    user.IsInPosition("HR-PRMGR") ||
                    user.IsInPosition("IT-DIR"))
                {

                    WebUserAccessRepository.AutoAddAccess("Position", "PositionListIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Position", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Position", "PopupForm", DB.AccessLevelEnum.Write, db);
                }
                if (user.IsInPosition("HR-MGR") ||
                    user.IsInPosition("HR-PRMGR") ||
                    user.IsInPosition("IT-DIR"))
                {

                    WebUserAccessRepository.AutoAddAccess("Applicant", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Applicant", "AllData", DB.AccessLevelEnum.Write, db);

                    WebUserAccessRepository.AutoAddAccess("Applicant", "OpenIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Applicant", "OpenData", DB.AccessLevelEnum.Write, db);

                    WebUserAccessRepository.AutoAddAccess("Applicant", "ApplicationPopup", DB.AccessLevelEnum.Write, db);
                }


                if (user.IsInPosition("IT-DIR") ||
                    user.IsInPosition("FIN-CTRL") ||
                    user.IsInPosition("FIN-ARMGR") ||
                    user.IsInPosition("OP-DM") ||
                    user.IsInPosition("OP-SLS") ||
                    user.IsInPosition("OP-SLSMGR") ||
                    user.IsInPosition("OP-GM") ||
                    user.IsInPosition("PRES") ||
                    user.IsInPosition("CFO") ||
                    user.IsInPosition("COO") ||
					user.IsInPosition("CEO"))
                { 
                    WebUserAccessRepository.AutoAddAccess("CustomerAging", "Index", DB.AccessLevelEnum.Read, db);
                    WebUserAccessRepository.AutoAddAccess("CustomerAging", "Data", DB.AccessLevelEnum.Read, db);
                    WebUserAccessRepository.AutoAddAccess("APInvoice", "PopupForm", DB.AccessLevelEnum.Read, db);
                    WebUserAccessRepository.AutoAddAccess("ServiceRequest", "ServiceEquipmentAllListIndex", DB.AccessLevelEnum.Read, db);
                    WebUserAccessRepository.AutoAddAccess("Job", "PopupForm", DB.AccessLevelEnum.Write, db);
                }

                if (model.YourInvoicePendingCount > 0)
                {
                    WebUserAccessRepository.AutoAddAccess("APDocument", "RequestedInfoIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Document", "RequestedInfoIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("APInvoice", "PopupForm", DB.AccessLevelEnum.Read, db);
                }
                if (model.BidReviewCount > 0 || user.HasAccess("Bid"))
                {
                    WebUserAccessRepository.AutoAddAccess("Bid", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Bid", "Tracker", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Bid", "UserBids", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Bid", "Popup", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Bid", "UpdateStatus", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Bid", "Create", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Bid", "BidProposalPDF", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Bid", "BidProposalPDFPreview", DB.AccessLevelEnum.Write, db);

                    WebUserAccessRepository.AutoAddAccess("Customer", "Add", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Contact", "Add", DB.AccessLevelEnum.Write, db);

                    WebUserAccessRepository.AutoAddAccess("BidProposal", "Panel", DB.AccessLevelEnum.Write, db);

                    WebUserAccessRepository.AutoAddAccess("JCIndustry", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("JCMarket", "Index", DB.AccessLevelEnum.Write, db);
                }

                if (user.HasAccess("APDocument") ||
                    user.HasAccess("Document"))
                {
                    WebUserAccessRepository.AutoAddAccess("APDocument", "Open", DB.AccessLevelEnum.Write, db);
                }

                if (user.HasAccess("APPayment", "BatchListIndex"))
                {
                    WebUserAccessRepository.AutoAddAccess("APPayment", "DownloadZionCSV", DB.AccessLevelEnum.Write, db);
                }

                if (user.HasAccess("APDocument", "FiledIndex") ||
                    user.HasAccess("Document", "FiledIndex"))// || user.HasAccess("Equipment")
                {
                    WebUserAccessRepository.AutoAddAccess("APDocument", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("APDocument", "RequestedInfoIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("APDocument", "NewIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("APDocument", "FiledIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("APDocument", "LineTablePanel", DB.AccessLevelEnum.Write, db);


                    WebUserAccessRepository.AutoAddAccess("Document", "AllIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Document", "RequestedInfoIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Document", "UploadIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Document", "FiledIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("Document", "LineTablePanel", DB.AccessLevelEnum.Write, db);
                }

                if (model.EMServiceRequestLineCount > 0 ||
                    user.IsInPosition("SHP-MECH") ||
                    user.IsInPosition("SHP-MGR") ||
                    user.IsInPosition("SHP-SUP") ||
                    user.IsInPosition("OP-EQMGR"))
                {
                    WebUserAccessRepository.AutoAddAccess("ServiceRequest", "CreateServiceRequest", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("ServiceRequest", "ServiceRequestIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("ServiceRequest", "ServiceEquipmentRequestListIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("ServiceRequest", "ServiceEquipmentIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("ServiceRequest", "RequestUpdateStatus", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("ServiceRequest", "ServiceEquipmentAllListIndex", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("ServiceRequest", "ServiceRequestUserListIndex", DB.AccessLevelEnum.Write, db);                    
                }

                if (model.ApprovalCount > 0)
                {
                    WebUserAccessRepository.AutoAddAccess("DailyTicketApproval", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("DailyTicketReject", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("CrewSubmission", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("TicketPendingSummary", "Index", DB.AccessLevelEnum.Write, db);
                    WebUserAccessRepository.AutoAddAccess("TicketSummary", "Index", DB.AccessLevelEnum.Write, db);
                }
                else
                {
                    //WebUserAccessRepository.AutoAddAccess("DailyTicketApproval", "Index", DB.AccessLevelEnum.Denied);
                    //WebUserAccessRepository.AutoAddAccess("DailyTicketReject", "Index", DB.AccessLevelEnum.Denied);
                    //WebUserAccessRepository.AutoAddAccess("CrewSubmission", "Index", DB.AccessLevelEnum.Denied);
                    //WebUserAccessRepository.AutoAddAccess("TicketPendingSummary", "Index", DB.AccessLevelEnum.Denied);
                    //WebUserAccessRepository.AutoAddAccess("TicketSummary", "Index", DB.AccessLevelEnum.Denied);

                }

                if (model.LeaveReviewCount > 0)
                {
                    WebUserAccessRepository.AutoAddAccess("LeaveRequestApproval", "Index", DB.AccessLevelEnum.Write, db);
                }

                //Standard Access
                WebUserAccessRepository.AutoAddAccess("Document", "Index", DB.AccessLevelEnum.Write, db);
                WebUserAccessRepository.AutoAddAccess("ServiceRequest", "CreateServiceRequest", DB.AccessLevelEnum.Write, db);
                WebUserAccessRepository.AutoAddAccess("ServiceRequest", "ServiceRequestIndex", DB.AccessLevelEnum.Write, db);
                WebUserAccessRepository.AutoAddAccess("ServiceRequest", "ServiceRequestLineUserListIndex", DB.AccessLevelEnum.Write, db);

				WebUserAccessRepository.AutoAddAccess("Explorer", "Download", DB.AccessLevelEnum.Read, db);
				WebUserAccessRepository.AutoAddAccess("SQLReports", "Index", DB.AccessLevelEnum.Read, db);
				WebUserAccessRepository.AutoAddAccess("Attachment", "DownloadFolder", DB.AccessLevelEnum.Read, db);
                WebUserAccessRepository.AutoAddAccess("EquipmentForm", "PopupForm", DB.AccessLevelEnum.Read, db);
                WebUserAccessRepository.AutoAddAccess("Resource", "PopupForm", DB.AccessLevelEnum.Read, db);
                WebUserAccessRepository.AutoAddAccess("Job", "PopupForm", DB.AccessLevelEnum.Read, db);
                WebUserAccessRepository.AutoAddAccess("VendorForm", "PopupForm", DB.AccessLevelEnum.Read, db);
                WebUserAccessRepository.AutoAddAccess("APInvoice", "PopupForm", DB.AccessLevelEnum.Read, db);
                WebUserAccessRepository.AutoAddAccess("APDocument", "Open", DB.AccessLevelEnum.Write, db);
                WebUserAccessRepository.AutoAddAccess("LeaveRequestUserList", "Index", DB.AccessLevelEnum.Write, db);
                WebUserAccessRepository.AutoAddAccess("LeaveRequest", "Index", DB.AccessLevelEnum.Write, db);
                WebUserAccessRepository.AutoAddAccess("LeaveRequest", "Create", DB.AccessLevelEnum.Write, db);
                WebUserAccessRepository.AutoAddAccess("WeeklyEmployeeSummary", "Index", DB.AccessLevelEnum.Read, db);
                WebUserAccessRepository.AutoAddAccess("UserSummary", "Index", DB.AccessLevelEnum.Read, db);
                WebUserAccessRepository.AutoAddAccess("Home", "Index", DB.AccessLevelEnum.Read, db);

                WebUserAccessRepository.AutoAddAccess("Video", "Pearsall", DB.AccessLevelEnum.Read, db);
                WebUserAccessRepository.AutoAddAccess("Video", "Monahans", DB.AccessLevelEnum.Read, db);
                WebUserAccessRepository.AutoAddAccess("Video", "SaShop", DB.AccessLevelEnum.Read, db);
                WebUserAccessRepository.AutoAddAccess("APIRegister", "Index", DB.AccessLevelEnum.Read, db);

                WebUserAccessRepository.AutoAddAccess("Documents", "Index", DB.AccessLevelEnum.Read, db);



                cacheDefaultSecurityChecked = true;
                systemCache.Set(memKey, cacheDefaultSecurityChecked, policy);
            }

        }

    }
}