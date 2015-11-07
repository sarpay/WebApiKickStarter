using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace WebApiProject
{
    public class ApiKeyHandler : DelegatingHandler
    {
        /**
        This handler is enforced in WebApiConfig.cs file.
        This handler looks for the API key in the URI query string. 
        For this example, Api Key is also defined in global.js and sent with each request. 
        A real implementation would probably use more complex validation.
        If the query string contains the key, the handler passes the request to the inner handler.
        If the request does not have a valid key, the handler creates a response message with status 403, Forbidden. 
        In this case, the handler does not call base.SendAsync, so the inner handler never receives the request, 
        nor does the controller. Therefore, the controller can assume that all incoming requests have a valid API key.
        **/

        string Key = "AV3xqDcx3txaGAkN";

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            bool good = ValidateKey(request);
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            if (!good)
            {
                response = request.CreateResponse(HttpStatusCode.Forbidden);

                dynamic jsonOutput = new JObject();
                jsonOutput.Message = "API Key Required!";
                jsonOutput.MessageDetail = "Please make sure you specify an API key.";
                response.Content = new StringContent(jsonOutput.ToString(), Encoding.UTF8, "application/json");
            }

            /** continue with the inner handler **/
            return response;
        }


        private bool ValidateKey(HttpRequestMessage request)
        {
            var query = request.RequestUri.ParseQueryString(); // message.RequestUri.ParseQueryString();
            string key = query["key"];
            return (key == Key);
            //return parsedCredentials.Username == Key;
        }

    }
}