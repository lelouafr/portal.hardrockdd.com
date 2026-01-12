
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http.Controllers;
using System.Web;
using System.Web.Mvc;
using DB.Infrastructure.ViewPointDB.Data;
using System.Runtime.Caching;
using System.Web.Routing;

namespace portal
{
    public class FilterIPAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// List of denied IPs
        /// </summary>
        IPList deniedIPListToCheck = new IPList();
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                return true;
            }
            string userIpAddress = httpContext.Request.UserHostName;
            //userIpAddress = "192.168.5.83";
            bool ipDenied = CheckDeniedIPs(userIpAddress);
            return !ipDenied;
        }


        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new
                RouteValueDictionary(new { controller = "Error", action = "AccessDenied", area = "" }));
            }
        }

        /// <summary>
        /// Checks the denied IPs.
        /// </summary>
        /// <param name="userIpAddress">The user ip address.</param>
        /// <returns></returns>
        private bool CheckDeniedIPs(string userIpAddress)
        {
            var deniedIPList = DBDeniedIpList();            
            foreach (var ip in deniedIPList)
            {
                deniedIPListToCheck.Add(ip);
            }
            return deniedIPListToCheck.CheckNumber(userIpAddress);
        }

        private static List<string> DBDeniedIpList()
        {
            var memKey = "deniedIPList";

            if (!(MemoryCache.Default[memKey] is List<string> deniedIPList))
            {
                using var db = new VPContext();
                deniedIPList = db.WebIPAddresses.Where(f => f.Allowed == false).Select(s => s.IPAddress).ToList();
                ObjectCache systemCache = MemoryCache.Default;
                CacheItemPolicy policy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(60)
                };
                systemCache.Set(memKey, deniedIPList, policy);
                return deniedIPList;
            }
            return deniedIPList;
        }

    }
}