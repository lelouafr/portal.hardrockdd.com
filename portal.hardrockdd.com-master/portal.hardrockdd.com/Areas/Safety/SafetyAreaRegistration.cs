using System.Web.Mvc;

namespace portal.Areas.Safety
{
    public class SafetyAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Safety";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            if (context == null)
                return;

            context.MapRoute(
                "Safety_default",
                "Safety/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}