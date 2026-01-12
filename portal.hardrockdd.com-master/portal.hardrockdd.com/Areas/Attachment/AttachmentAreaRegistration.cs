using System.Web.Mvc;

namespace portal.Areas.Attachment
{
    public class AttachmentAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Attachment";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            if (context == null)
                return;

            context.MapRoute(
                "Attachment_default",
                "Attachment/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}