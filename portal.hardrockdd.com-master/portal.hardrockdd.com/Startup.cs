using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(portal.Startup))]
namespace portal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
