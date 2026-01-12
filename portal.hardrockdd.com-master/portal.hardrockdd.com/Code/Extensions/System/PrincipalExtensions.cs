using Microsoft.AspNet.Identity;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace portal
{
    public static class PrincipalExtensions
    {
        
        public static DB.AccessLevelEnum FormAccess(this IPrincipal principal, string Controller)
        {
            if (principal == null)
            {
                throw new System.ArgumentNullException(nameof(principal));
            }
            
            if (!principal.Identity.IsAuthenticated)
                return DB.AccessLevelEnum.Denied;
            var user = HttpContext.Current.User;
            var userId = StaticFunctions.GetUserId();
            if ((user.IsInRole("Admin") || user.IsInPosition("IT-DIR")) && userId == user.Identity.GetUserId())
            {
                // Administrator => let him in
                return DB.AccessLevelEnum.Full;
            }

            var cacheAccess = Repository.VP.WP.WebUserAccessRepository.CachedAccessList();
            //var userId = StaticFunctions.GetUserId();
            //var memKey = "UserForms_" + userId;
            //List<WebUserAccess> cacheAccess;
            //if (!(MemoryCache.Default[memKey] is List<WebUserAccess> cacheAccess))
            //{
            //    using var db = new VPContext();
            //    var webUser = db.WebUsers.Include(x => x.ControllerAccess.Select(y => y.ControllerAction)).FirstOrDefault(f => f.Id == userId);
            //    var list = webUser.ControllerAccess.ToList();
            //    cacheAccess = list;
            //    ObjectCache systemCache = MemoryCache.Default;
            //    CacheItemPolicy policy = new CacheItemPolicy
            //    {
            //        AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(10)
            //    };
            //    systemCache.Set(memKey, cacheAccess, policy);
            //}

            if (cacheAccess != null)
            {
                if (cacheAccess.Any(f => f.ControllerAction.ControllerName == Controller &&
                                        (DB.AccessLevelEnum)f.AccessLevel != DB.AccessLevelEnum.Denied))
                {

                    return (DB.AccessLevelEnum)(cacheAccess.Where(f => f.ControllerAction.ControllerName == Controller).Max(f => f.AccessLevel));
                }
            }
            return DB.AccessLevelEnum.Denied;
        }

        public static DB.AccessLevelEnum FormAccess(this IPrincipal principal, string Controller, string Action)
        {
            if (principal == null)
            {
                throw new System.ArgumentNullException(nameof(principal));
            }
            if (!principal.Identity.IsAuthenticated)
                return DB.AccessLevelEnum.Denied;
            var user = HttpContext.Current.User;
            //if (user.IsInRole("Admin") || user.IsInPosition("IT-DIR"))
            //{
            //    // Administrator => let him in
            //    return DB.AccessLevelEnum.Full;
            //}

            var cacheAccess = Repository.VP.WP.WebUserAccessRepository.CachedAccessList();
            //var userId = StaticFunctions.GetUserId();
            //var memKey = "UserForms_" + userId;
            //if (!(MemoryCache.Default[memKey] is List<WebUserAccess> cacheAccess))
            //{
            //    using var db = new VPContext();
            //    var webUser = db.WebUsers.Include(x => x.ControllerAccess.Select(y => y.ControllerAction)).FirstOrDefault(f => f.Id == userId);
            //    var list = webUser.ControllerAccess.ToList();
            //    cacheAccess = list;
            //    ObjectCache systemCache = MemoryCache.Default;
            //    CacheItemPolicy policy = new CacheItemPolicy
            //    {
            //        AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(10)
            //    };
            //    systemCache.Set(memKey, cacheAccess, policy);
            //}

            if (cacheAccess != null)
            {
                if (cacheAccess.Any(f => f.ControllerAction.ControllerName == Controller &&
                                        f.ControllerAction.ActionName == Action &&
                                        (DB.AccessLevelEnum)f.AccessLevel != DB.AccessLevelEnum.Denied))
                {

                    return (DB.AccessLevelEnum)(cacheAccess.FirstOrDefault(f => f.ControllerAction.ControllerName == Controller &&
                                            f.ControllerAction.ActionName == Action).AccessLevel);
                }
            }
            return DB.AccessLevelEnum.Denied;
        }

        public static bool IsInPosition(this IPrincipal principal, string positionCode)
        {
            if (principal == null)
            {
                throw new System.ArgumentNullException(nameof(principal));
            }
            if (positionCode == null)
            {
                throw new System.ArgumentNullException(nameof(positionCode));
            }
            if (!principal.Identity.IsAuthenticated)
                return false;

            var userId = StaticFunctions.GetUserId();
            var memKey = "User_" + userId;

            var cacheEmp = StaticFunctions.GetCurrentHREmployee();

            //if (!(MemoryCache.Default[memKey] is HRResource cacheEmp))
            //{
            //    using var db = new VPContext();
            //    var webUser = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            //    var emp = webUser.Employee.FirstOrDefault();
            //    cacheEmp = emp;
            //    ObjectCache systemCache = MemoryCache.Default;
            //    CacheItemPolicy policy = new CacheItemPolicy
            //    {
            //        AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(20)
            //    };
            //    if (cacheEmp != null)
            //    {
            //        systemCache.Set(memKey, cacheEmp, policy);
            //    }
            //}
            if (cacheEmp != null)
            {
                if ((cacheEmp.ActiveYN == "N" || cacheEmp.PortalAccountActive == "N") && cacheEmp.HRRef != 100798)
                {
                    return false;
                }
                if (positionCode == "*")
                {
                    return true;
                }
                var codes = positionCode.Split(',').Select(s => s.Trim()).ToList();
                return codes.Contains(cacheEmp.PositionCode);
                //return cacheEmp.PositionCode == positionCode;
            }
            return false;
        }

        public static bool HasAccess(this IPrincipal principal, string Controller)
        {
            if (principal == null)
            {
                throw new System.ArgumentNullException(nameof(principal));
            }
            if (!principal.Identity.IsAuthenticated)
                return false;
            var user = HttpContext.Current.User;
            var cacheEmp = StaticFunctions.GetCurrentHREmployee();
            if ((cacheEmp.PortalAccountActive == "N" || cacheEmp.ActiveYN == "N") && cacheEmp.HRRef != 100798)
            {
                return false;
            }

            var userId = StaticFunctions.GetUserId();
            if ((user.IsInRole("Admin") || user.IsInPosition("IT-DIR")) && userId == user.Identity.GetUserId())
            {
                // Administrator => let him in
                //return true;
            }
            var cacheAccess = Repository.VP.WP.WebUserAccessRepository.CachedAccessList();
            if (cacheAccess != null)
            {
                if (cacheAccess.Any(f => f.ControllerAction.ControllerName == Controller && (DB.AccessLevelEnum)f.AccessLevel != DB.AccessLevelEnum.Denied))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool HasAccess(this IPrincipal principal, string Controller, string Action)
        {
            if (principal == null)
            {
                throw new System.ArgumentNullException(nameof(principal));
            }

            if (!principal.Identity.IsAuthenticated)
                return false;
            var user = HttpContext.Current.User;
            var cacheEmp = StaticFunctions.GetCurrentHREmployee();
            if ((cacheEmp.PortalAccountActive == "N" || cacheEmp.ActiveYN == "N") && cacheEmp.HRRef != 100798)
            {
                return false;
            }
            if (user.IsInRole("Admin") || user.IsInPosition("IT-DIR"))
            {
                // Administrator => let him in
                //return true;
            }
            var cacheAccess = Repository.VP.WP.WebUserAccessRepository.CachedAccessList();

            if (cacheAccess != null)
            {
                if (cacheAccess.Any(f => f.ControllerAction.ControllerName == Controller &&
                                        f.ControllerAction.ActionName == Action &&
                                        (DB.AccessLevelEnum)f.AccessLevel != DB.AccessLevelEnum.Denied))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool HasFullAccess(this IPrincipal principal, string Controller, string Action)
        {
            if (principal == null)
            {
                throw new System.ArgumentNullException(nameof(principal));
            }

            if (!principal.Identity.IsAuthenticated)
                return false;

            var cacheEmp = StaticFunctions.GetCurrentHREmployee();
            if ((cacheEmp.PortalAccountActive == "N" || cacheEmp.ActiveYN == "N") && cacheEmp.HRRef != 100798)
            {
                return false;
            }
            var user = HttpContext.Current.User;
            if (user.IsInRole("Admin") || user.IsInPosition("IT-DIR"))
            {
                // Administrator => let him in
                //return true;
            }
            var cacheAccess = Repository.VP.WP.WebUserAccessRepository.CachedAccessList();

            if (cacheAccess != null)
            {
                if (cacheAccess.Any(f => f.ControllerAction.ControllerName == Controller &&
                                        f.ControllerAction.ActionName == Action &&
                                        (DB.AccessLevelEnum)f.AccessLevel == DB.AccessLevelEnum.Full))
                {
                    return true;
                }
            }
            return false;
        }
    }
}