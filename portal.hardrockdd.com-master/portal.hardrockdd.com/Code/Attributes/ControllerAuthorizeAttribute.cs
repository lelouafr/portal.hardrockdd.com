using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Data.Entity;
using portal.Repository.VP.WP;
using System.Net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Web.Security;

namespace portal
{
    public class ControllerAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)throw new System.ArgumentNullException(nameof(httpContext));

            var isAuthorized = base.AuthorizeCore(httpContext);

            if (!isAuthorized)
                return false;
            //return false;
            var userId = StaticFunctions.GetUserId();
            var memKey = "User_" + userId;
            var cacheEmp = StaticFunctions.GetCurrentHREmployee();

            //if (!(MemoryCache.Default[memKey] is HRResource cacheEmp))
            //{
            //    using var db = new VPContext();
            //    var webUser = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            //    if (webUser == null)
            //    {
            //        return false;
            //    }
            //    var emp = webUser.Employee.FirstOrDefault();
            //    if (emp == null)
            //    {
            //        var prEmp = db.Employees.FirstOrDefault(f => f.Email == webUser.Email);
            //        if (prEmp != null)
            //        {
            //            emp = prEmp.Resource.FirstOrDefault();
            //            if (emp == null)
            //            {
            //                emp = new HRResource();
            //                emp.ActiveYN = "N";
            //                emp.PortalAccountActive = "N";
            //            }
            //            else
            //            {
            //                emp.WebId = webUser.Id;
            //                db.SaveChanges();
            //            }
            //        }
            //        else
            //        {
            //            emp = new HRResource();
            //            emp.ActiveYN = "N";
            //            emp.PortalAccountActive = "N";
            //        }
            //    }
            //    if (emp.ActiveYN == "N")
            //    {
            //        webUser.ControllerAccess.ToList().ForEach(e => {
            //            webUser.ControllerAccess.Remove(e);
            //        });

            //        db.BulkSaveChanges();
            //    }
            //    cacheEmp = emp;
            //    if (emp.ActiveYN == "Y")
            //    {
            //        ObjectCache systemCache = MemoryCache.Default;
            //        CacheItemPolicy policy = new CacheItemPolicy
            //        {
            //            AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(20)
            //        };
            //        if (cacheEmp != null && cacheEmp?.HRRef != 0)
            //        {
            //            systemCache.Set(memKey, cacheEmp, policy);
            //        }

            //    }
            //}
            if (cacheEmp == null)
                return false;
            if ((cacheEmp.ActiveYN == "N" || cacheEmp.PortalAccountActive == "N" ) && cacheEmp.HRRef != 100798)
            {
                return false;
            }

            /**If Ajax Request Authorize the request**/
            if (httpContext.Request.IsAjaxRequest()) return true;

            var cacheAccess = WebUserAccessRepository.CachedAccessList();
            var user = httpContext.User;
            if (cacheAccess != null)
            {
                var request = httpContext.Request;
                string controller = request.RequestContext.RouteData.Values["controller"].ToString();
                string action = request.RequestContext.RouteData.Values["action"].ToString();
                string controllerTokenID = request.Headers["controllertokenid"];

                var access = cacheAccess.FirstOrDefault(f => f.ControllerAction.ControllerName.ToLower() == controller.ToLower() && 
                                                             f.ControllerAction.ActionName.ToLower() == action.ToLower());
                if (access != null)
                {                    
                    if ((DB.AccessLevelEnum)access.AccessLevel == DB.AccessLevelEnum.Denied)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

            }
            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
            else
            {

                string controllerName = filterContext.HttpContext.Request.RequestContext.RouteData.Values["controller"].ToString();
                string actionName = filterContext.HttpContext.Request.RequestContext.RouteData.Values["action"].ToString();
                if ((filterContext.HttpContext.User.IsInRole("Admin") || filterContext.HttpContext.User.IsInPosition("IT-DIR")) && 
                    (StaticFunctions.GetCurrentEmployee()?.ActiveYN == "Y"))
                {
                    try
                    {
                        using var db = new VPContext();
                        WebUserAccessRepository.AutoAddAccess(controllerName, actionName, DB.AccessLevelEnum.Full, db);

                    }
                    catch (Exception)
                    {
                        FormsAuthentication.SignOut();
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "AccessDenied", area = "" }));
                        return;
                    }
                }
                WebUserAccessRepository.AddDefaultSecurity(true);
                var cacheAccess = WebUserAccessRepository.CachedAccessList();
                var access = cacheAccess.FirstOrDefault(f => f.ControllerAction.ControllerName.ToLower() == controllerName.ToLower() &&
                                                             f.ControllerAction.ActionName.ToLower() == actionName.ToLower());
                if (access != null)
                {
                    if ((DB.AccessLevelEnum)access.AccessLevel == DB.AccessLevelEnum.Denied)
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "AccessDenied", area = "" }));
                    else
                        return;
                }
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "AccessDenied", area = "" }));
            }
        }
    }
}