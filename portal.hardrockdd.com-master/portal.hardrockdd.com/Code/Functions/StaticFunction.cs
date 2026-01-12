using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Text.RegularExpressions;
using System;
using System.Runtime.Caching;
using DB.Infrastructure.ViewPointDB.Data;
using System.Linq;

namespace portal
{
    
    public static partial class StaticFunctions
    {
        public static bool HasAttachments(Guid? uniqueAttchID)
        {
            if (uniqueAttchID == null)
            {
                return false;
            }
            using var db = new VPContext();
            return db.HQAttachmentFiles.Any(f => f.UniqueAttchID == uniqueAttchID);
        }

        public static string GetUserId()
        {
            if (HttpContext.Current != null)
            {
                var curentUserId = HttpContext.Current.User.Identity.GetUserId();
                var memKey = "UserIdOverRide_" + curentUserId;

                if ((MemoryCache.Default[memKey] is string OverrideUserId))
                {
                    return OverrideUserId;
                }
                return HttpContext.Current.User.Identity.GetUserId();
            }
            else
            {
                return "caac7817-4e69-48a9-b8a8-795ac8e32867";
            }
        }

        public static WebUser GetCurrentUser()
        {
            var userId = GetUserId();

            var memKey = "GetCurrentUser_" + userId;
            if (MemoryCache.Default[memKey] is WebUser cacheUser)
            {
                return cacheUser;
            }

            using var db = new VPContext();
            cacheUser = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            var emp = cacheUser.PREmployee;

            ObjectCache systemCache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(60)
            };
            systemCache.Set(memKey, cacheUser, policy);
            return cacheUser;
        }

        public static void SetCurrentCompany(byte companyId)
        {
            var userId = GetUserId();

            var memKey = "GetCurrentCompany_" + userId;

            using var db = new VPContext();
            var payEmp = GetCurrentEmployee();
            var cacheCompany = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == companyId);
            ObjectCache systemCache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(60)
            };
            systemCache.Set(memKey, cacheCompany, policy);

        }

        public static HQCompanyParm GetCurrentCompany()
        {
            var userId = GetUserId();

            var memKey = "GetCurrentCompany_" + userId;

            if (MemoryCache.Default[memKey] is DB.Infrastructure.ViewPointDB.Data.HQCompanyParm cacheCompany)
            {
                return cacheCompany;
            }
            using var db = new VPContext();
            var payEmp = GetCurrentEmployee();
            cacheCompany = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == payEmp.PortalCompanyCode);
            ObjectCache systemCache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(60)
            };
            systemCache.Set(memKey, cacheCompany, policy);
            return cacheCompany;
        }


        public static void SetCurrentDivision(int divisionId)
        {
            var userId = GetUserId();

            var memKey = "GetCurrentDivision_" + userId;

            using var db = new VPContext();
            var payEmp = GetCurrentEmployee();
            var cacheCompany = db.CompanyDivisions.FirstOrDefault(f => f.DivisionId == divisionId);
            ObjectCache systemCache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(60)
            };
            systemCache.Set(memKey, cacheCompany, policy);

        }

        public static CompanyDivision GetCurrentDivision()
        {
            var userId = GetUserId();

            var memKey = "GetCurrentDivision_" + userId;

            if (MemoryCache.Default[memKey] is DB.Infrastructure.ViewPointDB.Data.CompanyDivision cacheCompany)
            {
                if (cacheCompany.DivisionId == GetCurrentEmployee().DivisionId)
                {
                    return cacheCompany;
                }
            }
            using var db = new VPContext();
            var payEmp = GetCurrentEmployee();
            cacheCompany = db.CompanyDivisions.FirstOrDefault(f => f.DivisionId == (payEmp.DivisionId));
            if (cacheCompany == null)
            {
                cacheCompany = db.CompanyDivisions.FirstOrDefault(f => f.DivisionId == 1);
            }
            ObjectCache systemCache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(60)
            };
            systemCache.Set(memKey, cacheCompany, policy);
            return cacheCompany;
        }

        public static Employee GetCurrentEmployee()
        {
            var userId = GetUserId();

            var memKey = "GetCurrentEmployee_" + userId;
            if (MemoryCache.Default[memKey] is Employee cacheEmp)
            {
                return cacheEmp;
            }

            using var db = new VPContext();
            ObjectCache systemCache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(60)
            };

            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            if (user == null)
                return null;

            cacheEmp = user.Employee.FirstOrDefault()?.PREmployee;
            if (cacheEmp == null)
            {
                cacheEmp = new Employee();
                cacheEmp.ActiveYN = "N";
            }
            if (cacheEmp.ActiveYN == "Y" )
            {
                systemCache.Set(memKey, cacheEmp, policy);
            }
            return cacheEmp;
        }


        public static HRResource GetCurrentHREmployee()
        {
            var userId = GetUserId();
            var memKey = "GetCurrentHREmployee_" + userId;
            if (MemoryCache.Default[memKey] is HRResource cacheEmp)
            {
                return cacheEmp;
            }

            using var db = new VPContext();
            ObjectCache systemCache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(5)
            };

            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            if (user == null)
            {
                return null;
            }
            cacheEmp = new HRResource();
            if (user == null)
            {
                cacheEmp.ActiveYN = "N";
                cacheEmp.PortalAccountActive = "N";
            }

            cacheEmp = user?.Employee.FirstOrDefault();
            //Try to find the employee related to the account
            if (cacheEmp == null)
            {
                cacheEmp = db.HRResources.FirstOrDefault(f => f.Email == user.Email);
                if (cacheEmp == null)
                    cacheEmp = db.HRResources.FirstOrDefault(f => f.CompanyEmail == user.Email);

                if (cacheEmp != null)
                {
                    cacheEmp.WebId = user.Id;
                    db.BulkSaveChanges();
                }
            }

            //No employee found
            if (cacheEmp == null)
            {
                cacheEmp = new HRResource();
                cacheEmp.ActiveYN = "N";
                cacheEmp.PortalAccountActive = "N";
            }
            if (cacheEmp.ActiveYN == "Y" && cacheEmp.PortalAccountActive == "Y")
            {
                systemCache.Set(memKey, cacheEmp, policy);
            }

            if (cacheEmp.WebUser == null)
            {

            }
            return cacheEmp;
        }



        public static Models.Views.LoggedInUserViewModel GetLoggedInUser()
        {
            var userId = GetUserId();

            var memKey = "GetLoggedInUser_" + userId;
            if (MemoryCache.Default[memKey] is Models.Views.LoggedInUserViewModel cacheUser)
            {
                return cacheUser;
            }
            cacheUser = new Models.Views.LoggedInUserViewModel();
            using var db = new VPContext();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            if (user == null)
            {
                return cacheUser;
            }

            var emp = user.Employee.FirstOrDefault();
            ObjectCache systemCache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(60)
            };

            cacheUser = new portal.Models.Views.LoggedInUserViewModel(emp);
            systemCache.Set(memKey, cacheUser, policy);
            return cacheUser;
        }

        public static string GetPlainTextFromHtml(string htmlString)
        {
            string htmlTagPattern = "<.*?>";
            //var regexCss = new Regex("(\\<script(.+?)\\)|(\\<style(.+?)\\)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            //htmlString = regexCss.Replace(htmlString, string.Empty);
            htmlString = Regex.Replace(htmlString, htmlTagPattern, string.Empty);
            htmlString = Regex.Replace(htmlString, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
            htmlString = htmlString.Replace(" ", string.Empty);

            return htmlString;
        }

        public static IDictionary<string, object> GetHtmlAttributes(
            object fixedHtmlAttributes = null,
            IDictionary<string, object> dynamicHtmlAttributes = null
            )
        {
            var rvd = (fixedHtmlAttributes == null)
                ? new RouteValueDictionary()
                : HtmlHelper.AnonymousObjectToHtmlAttributes(fixedHtmlAttributes);
            if (dynamicHtmlAttributes != null)
            {
                foreach (KeyValuePair<string, object> kvp in dynamicHtmlAttributes)
                    rvd[kvp.Key] = kvp.Value;
            }
            return rvd;
        }

    }
}