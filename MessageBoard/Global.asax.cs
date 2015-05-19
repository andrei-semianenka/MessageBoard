using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MessageBoard
{
    /// <summary>
    /// The purpose of this class is to pass compile-time directives values to Razor views (#define,#if directives does not work there)
    /// </summary>
    public static class Defines
    {
#if USE_DICTIONARY
        public static readonly bool USE_DICTIONARY = true;
#else
        public static readonly bool USE_DICTIONARY = false;
#endif
    }

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Session_Start(Object sender, EventArgs e)
        {
            Session["init"] = 0;
        }
    }
}
