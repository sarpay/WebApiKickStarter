using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Dispatcher;

namespace WebApiProject
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            /**
            Require / Force HTTPS when client tries to access resources over HTTP
            Can also be used at the controller level:
            [ForceHttps()]
            **/
            //config.Filters.Add(new ForceHttps());


            /**
            Applies basic authorization filter on all the calls.
            Since sign-in page needs to be public, global setting is disabled.
            Filtering is done per-action on the controller level : [Private()]
            **/
            //config.Filters.Add(new Private());


            /**
            Enable CORS (Cross Origin Resource Sharing)
            Uses the NuGet Package : Microsoft.AspNet.WebApi.Cors
            Can be also used at the controller level: 
            [EnableCors(origins: "*", headers: "*", methods: "GET,POST,PUT,DELETE")]
            **/
            EnableCorsAttribute corsPolicy = new EnableCorsAttribute(
                origins: "*",
                headers: "*",
                methods: "GET,PUT,POST,DELETE"
            );
            config.EnableCors(corsPolicy);


            /** Enables attribute routing. Routing is how Web API matches a URI to an action. **/
            config.MapHttpAttributeRoutes();


            /** Configure Template Routing **/
            config.Routes.MapHttpRoute(
                name: "Simple-GET-Route",/* name can be anything - not referred */
                routeTemplate: "{action}/{id}",/*{controller}*/
                defaults: new
                {
                    controller = "Home",
                    id = RouteParameter.Optional
                }
            );


            //config.Routes.MapHttpRoute(
            //    name: "Default-Public-Route",/* name can be anything - not referred */
            //    routeTemplate: "public/{action}/{id}",/*{controller}*/
            //    defaults: new
            //    {
            //        controller = "Home",
            //        id = RouteParameter.Optional
            //    },
            //    constraints: null,
            //    handler: HttpClientFactory.CreatePipeline(
            //        new HttpControllerDispatcher(config),
            //        new DelegatingHandler[] { new ApiKeyHandler() }) /** per-route message handler **/
            //);

            //config.Routes.MapHttpRoute(
            //    name: "Private-Route",/* name can be anything - not referred */
            //    routeTemplate: "private/{action}/{id}",/*{controller}*/
            //    defaults: new
            //    {
            //        controller = "Home",
            //        id = RouteParameter.Optional
            //    },
            //    constraints: null,
            //    handler: HttpClientFactory.CreatePipeline(
            //        new HttpControllerDispatcher(config),
            //        new DelegatingHandler[] { new ApiKeyHandler() }) /** per-route message handler **/
            //);


            /**
            Message handlers are called in the same order that they appear in MessageHandlers collection.
            Because they are nested, the response message travels in the other direction.
            That is, the last handler is the first to get the response message.
            **/

            config.MessageHandlers.Add(new BasicAuthMessageHandler() /** works in conjuction w/ [Authorize] **/
            {
                PrincipalProvider = new PrincipalProvider()
            });

            config.MessageHandlers.Add(new ApiKeyHandler()); /* Global handler - applicable to all the requests */

            /** this line of code makes sure that server responds as json instead of xml using Chrome **/
            /** it can be safely removed **/
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
        }
    }
}