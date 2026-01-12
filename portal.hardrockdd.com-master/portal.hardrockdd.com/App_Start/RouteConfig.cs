using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;

namespace portal
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
            AreaRegistration.RegisterAllAreas();
            routes.MapRoute(
                "Errors",
                "Error/{action}/{code}",
                new
                {
                    controller = "Error",
                    action = "BadRequest",
                    code = RouteParameter.Optional
                });
            
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
