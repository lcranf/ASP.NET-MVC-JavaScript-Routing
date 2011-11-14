using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using JsRouting.Core;

namespace $rootnamespace$
{
    // TODO: add the following line to your Application_Start method in your Global.asax
    // new WebRoutes().Routes.AddTo(RouteTable.Routes);

    public class WebRoutes : RouteSource
    {
        protected override void Map(IList<Tuple<string, Route>> routes)
        {
            routes.MapRoute("Default", 
                            "{controller}/{action}/{id}", 
                            new { controller = "Home", action = "Index", id = "" });
        }
    }
}