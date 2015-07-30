using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Filters;

namespace WebApiProject
{
    public class ForceHttps : AuthorizationFilterAttribute
    {
        /**
        This authorization filter is responsible for examining if the URI scheme is secured. 
        If it is not, the filter will reject the call and send response back to the client 
        informing that the request should be transmitted over HTTPS.

        In case of a GET call over HTTP, the client (browser) is told to initiate 
        another GET request using the https scheme. In case of non GET requests, 
        filter returns a 404(Not Found) status code and small html message informing client to 
        send the request again over https.
        **/
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var request = actionContext.Request;

            if (request.RequestUri.Scheme != Uri.UriSchemeHttps)
            {

                dynamic jsonOutput = new JObject();
                jsonOutput.Message = "HTTPS is required";
                jsonOutput.MessageDetail = "Please make sure to use the correct URI Scheme";

                if (request.Method.Method == "GET")
                {
                    actionContext.Response = request.CreateResponse(HttpStatusCode.Found);
                    actionContext.Response.Content = new StringContent(jsonOutput.ToString(), Encoding.UTF8, "text/html");

                    UriBuilder httpsNewUri = new UriBuilder(request.RequestUri);
                    httpsNewUri.Scheme = Uri.UriSchemeHttps;
                    httpsNewUri.Port = 443;

                    actionContext.Response.Headers.Location = httpsNewUri.Uri;
                }
                else
                {
                    actionContext.Response = request.CreateResponse(HttpStatusCode.NotFound);
                    actionContext.Response.Content = new StringContent(jsonOutput.ToString(), Encoding.UTF8, "text/html");
                }

            }
        }
    }
}