using System.Web.Mvc;

namespace portal.Areas.AccountsPayable
{
    public class AccountsPayableAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "AccountsPayable";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            if (context == null)
                return;

            context.MapRoute(
                "AccountsPayable_default",
                "AccountsPayable/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}