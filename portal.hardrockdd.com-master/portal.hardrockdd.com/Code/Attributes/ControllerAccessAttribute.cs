using DB.Infrastructure.ViewPointDB.Data;
using Microsoft.AspNet.Identity;
using portal.Models.Views.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;

namespace portal
{
    public class ControllerAccessAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext == null) throw new System.ArgumentNullException(nameof(filterContext));

            string actionName = filterContext.RouteData.Values["action"].ToString();
            string controllerName = filterContext.RouteData.Values["controller"].ToString();
            var systemCache = MemoryCache.Default;

            if (filterContext.Result is ViewResultBase view)
            {
                var user = HttpContext.Current.User;
                var userId = StaticFunctions.GetUserId();

                var formAccess = user.FormAccess(controllerName, actionName);
                if (filterContext.Result is PartialViewResult partialView)
                {
                    var httpContext = filterContext.HttpContext;
                    var controllerToken = httpContext.Request.Headers["controllerTokenId"];
                    if (Guid.TryParse(controllerToken, out Guid controllerTolkenIdOut))
                    {
                        using var db = new VPContext();
                        var currentSession = db.WebUserLogs.FirstOrDefault(f => f.SessionGuid == controllerTolkenIdOut);

                        if (currentSession != null)
                        {
                            if (formAccess < (DB.AccessLevelEnum)currentSession.AccessLevel)
                            {
                                formAccess = (DB.AccessLevelEnum)currentSession.AccessLevel;
                            }
                            
                        }
                    }
                    else
                    {
                    }
                }
                if (filterContext.HttpContext.User.IsInRole("Admin") && userId == filterContext.HttpContext.User.Identity.GetUserId())
                    formAccess = DB.AccessLevelEnum.Full;

                switch (formAccess)
                {
                    case DB.AccessLevelEnum.Denied:
                        view.ViewBag.ViewOnly = true;
                        break;
                    case DB.AccessLevelEnum.Read:
                        view.ViewBag.ViewOnly = view.ViewBag.ViewOnly ?? true;
                        break;
                    case DB.AccessLevelEnum.Write:
                        view.ViewBag.ViewOnly = view.ViewBag.ViewOnly ?? false;
                        break;
                    case DB.AccessLevelEnum.Full:
                        view.ViewBag.ViewOnly = false;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}