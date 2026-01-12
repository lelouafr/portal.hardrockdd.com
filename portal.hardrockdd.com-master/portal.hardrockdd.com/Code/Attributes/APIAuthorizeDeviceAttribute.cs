using DB.Infrastructure.ViewPointDB.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace portal
{
    public class APIAuthorizeDeviceAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (!AuthorizeRequest(actionContext))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
            }
            base.OnAuthorization(actionContext);
        }
        private bool AuthorizeRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (!actionContext.Request.Headers.Any(f => f.Key == "DeviceId"))
            {
                return false;
            }
            var deviceHeader = actionContext.Request.Headers.GetValues("DeviceId");
            var deviceId = deviceHeader.FirstOrDefault();

            using var db = new VPContext();
            var apiAuth = db.WPUserDeviceRegistries.FirstOrDefault(f => f.DeviceId == deviceId);
            if (apiAuth != null)
            {
                if (apiAuth.IsDisabled)
                {
                    return false;
                }
                if (!apiAuth.User.Active)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}