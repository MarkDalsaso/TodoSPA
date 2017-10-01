using System;
using System.Web.Http;
using System.Net.Http.Formatting;

namespace TodoApi.App_Start
{
    public class WebApiConfig
    {
        public static void Configure(HttpConfiguration config)
        {
            // Hierarchical resources like “…/api/users/{userId}/todos/{todoId}”
            //   are best handled via "Attribute" routing, defined within the 
            //   Controller, i.e. [RoutePrefix("api/users/{userId:int}/todos")] and 
            //   further decorated in the action method, [Route("{todoId:int?}")]        
            config.MapHttpAttributeRoutes();

            //   The following approuch could be used to load the SPA, as 
            //   an alternative to using defaultDocument in the Web.config
            /*
             * 
            //check for a default SPA route (the todoSpaUi.html UI)
            config.Routes.MapHttpRoute(
                name: "Index",
                routeTemplate: "{index}.html",
                defaults: new { index = "todoSpaUi" });
            */

            // The following "Convention" routing configuration, as a default,
            //   gets added automatically by Visual Studio, via a Web API template(s).
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

        }
    }
}