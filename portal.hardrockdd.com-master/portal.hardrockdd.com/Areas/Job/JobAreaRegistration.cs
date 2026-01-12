using System.Web.Mvc;

namespace portal.Areas.Job
{
    public class JobAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Job";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            if (context == null)
                return;

            context.MapRoute(
                "Job_default",
                "Job/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}