﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
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
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
          
           
            //HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            // BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

    }
}











































































































































































































































































































































































































































































































































































































































































































































































































































































