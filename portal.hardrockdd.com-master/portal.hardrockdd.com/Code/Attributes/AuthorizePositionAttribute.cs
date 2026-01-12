//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Caching;
//using System.Web;
//using System.Web.Mvc;
//using System.Web.Routing;

//namespace portal
//{
//    public class AuthorizePositionDNUAttribute : AuthorizeAttribute
//    {
//        // Custom property
//        public string PositionCodes { get; set; }

//        protected override bool AuthorizeCore(HttpContextBase httpContext)
//        {
//            if (httpContext == null)
//            {
//                throw new System.ArgumentNullException(nameof(httpContext));
//            }
//            var isAuthorized = base.AuthorizeCore(httpContext);

//            if (!isAuthorized)
//                return false;

//            var user = httpContext.User;
//            if (user.IsInRole("Admin"))
//            {
//                // Administrator => let him in
//                return true;
//            }
//            var userId = StaticFunctions.GetUserId();
//            var memKey = "User_" + userId;
//            if (!(MemoryCache.Default[memKey] is HRResource cacheEmp))
//            {
//                using var db = new VPContext();
//                var webUser = db.WebUsers.FirstOrDefault(f => f.Id == userId);
//                var emp = webUser.Employee.FirstOrDefault();
//                cacheEmp = emp;
//                ObjectCache systemCache = MemoryCache.Default;
//                CacheItemPolicy policy = new CacheItemPolicy
//                {
//                    AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(20)
//                };
//                if (cacheEmp != null)
//                {
//                    systemCache.Set(memKey, cacheEmp, policy);
//                }
//            }
//            if (cacheEmp != null)
//            {
//                if (cacheEmp.ActiveYN == "N")
//                {
//                    return false;
//                }
//                else if (PositionCodes == "*")
//                {
//                    return true;
//                }
//                else if (cacheEmp.PositionCode == "PRES")
//                {
//                    return true;
//                }
//                var codes = PositionCodes.Split(',').Select(s => s.Trim()).ToList();
//                return codes.Contains(cacheEmp.PositionCode);
//                //return cacheEmp.PositionCode == positionCode;
//            }
//            return false;
//        }

//        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
//        {
//            if (filterContext == null)
//            {
//                throw new System.ArgumentNullException(nameof(filterContext));
//            }
//            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
//            {
//                base.HandleUnauthorizedRequest(filterContext);
//            }
//            else
//            {
//                filterContext.Result = new RedirectToRouteResult(new
//                RouteValueDictionary(new { controller = "Error", action = "AccessDenied", area = "" }));
//            }
//        }
//    }
//}