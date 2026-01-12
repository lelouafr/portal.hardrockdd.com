using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
namespace portal
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            try
            {
                //Telerik.Reporting.Services.WebApi.ReportsControllerConfiguration.RegisterRoutes(System.Web.Http.GlobalConfiguration.Configuration);
                //AreaRegistration.RegisterAllAreas();
                GlobalConfiguration.Configure(WebApiConfig.Register);
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                // AreaRegistration.RegisterAllAreas();
                BundleConfig.RegisterBundles(BundleTable.Bundles);

                ModelMetadataProviders.Current = new MetadataProvider();
                Z.EntityFramework.Extensions.EntityFrameworkManager.BulkOperationBuilder = builder => { builder.ForceUpdateUnmodifiedValues = false; };
            }
            catch (Exception ex)
            {
                string path = @"C:\Users\LarbiElouafrassi\Desktop\startup_error.txt";
                System.IO.File.WriteAllText(path, ex.ToString());
                throw;
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            string path = @"C:\Users\LarbiElouafrassi\Desktop\app_error.txt";
            System.IO.File.WriteAllText(path, DateTime.Now + "\n" + ex?.ToString());
        }
    }
}
