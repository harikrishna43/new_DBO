using DBO.App_Start;
using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using DBO.Data;
using DBO.Data.Models;
using DBO.Extensions;

namespace DBO
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            CofigureDI.Configure();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var resourceConfig = new ResourceConfig();
            resourceConfig.PopulateResources().Wait();

            ConfigureMapper.Configure();
        }

        void Application_Error(object sender, System.EventArgs e)
        {
            Exception exc = Server.GetLastError();
            while (exc.Message.EndsWith("See the inner exception for details."))
                exc = exc.InnerException;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Logs.Add(new LogItem {Time = DateTime.Now, Value = exc.Message});
                db.SaveChanges();
            }
        }
    }
}
