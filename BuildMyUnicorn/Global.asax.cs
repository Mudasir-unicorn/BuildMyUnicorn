﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace BuildMyUnicorn
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDM0MDU3QDMxMzkyZTMxMmUzMG5QZzM2U2l3OTh4YlBBbnBDK2ZtL2h5V1V3dlJIMWI5OUFQSU56S3hKYXc9");
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
           // BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
