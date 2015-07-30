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
        This handler looks for the API key in the URI query string. 
        For this example, we assume that the key is a static string. 
        A real implementation would probably use more complex validation.
        If the query string contains the key, the handler passes the request to the inner handler.
        If the request does not have a valid key, the handler creates a response message with status 403, Forbidden. 
        In this case, the handler does not call base.SendAsync, so the inner handler never receives the request, 
        nor does the controller. Therefore, the controller can assume that all incoming requests have a valid API key.
        **/

        //public IProvidePrincipal PrincipalProvider { get; set; }
        //public string Key { get; set; }

        //public ApiKeyHandler(string key)
        //{
        //    this.Key = key;
        //}
        string Key = "AV3xqDcx3txaGAkN";

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            bool good = false;

            AuthenticationHeaderValue authValue = request.Headers.Authorization;
            if (authValue != null && !String.IsNullOrWhiteSpace(authValue.Parameter))
            {
                Credentials parsedCredentials = ParseAuthorizationHeader(authValue.Parameter);
                good = ValidateKey(parsedCredentials);
            }

            if (!good)
            {
                var response = new HttpResponseMessage(HttpStatusCode.Forbidden);//Unauthorized
                var tsc = new TaskCompletionSource<HttpResponseMessage>();
                tsc.SetResult(response);
                return tsc.Task;
            }

            /** continue with the inner handler **/
            //Thread.CurrentPrincipal = PrincipalProvider.CreatePrincipal(parsedCredentials.Username, parsedCredentials.Password);
            return base.SendAsync(request, cancellationToken);
        }


        private bool ValidateKey(Credentials parsedCredentials)
        {
            if (parsedCredentials == null)
            {
                return false;
            }

            //var query = message.RequestUri.ParseQueryString();
            //string key = query["key"];
            //return (key == Key);
            return parsedCredentials.Username == Key;
        }




        private Credentials ParseAuthorizationHeader(string authHeader)
        {
            string[] credentials = Encoding.ASCII.GetString(Convert
                                                            .FromBase64String(authHeader))
                                                            .Split(
                                                            new[] { ':' });
            if (credentials.Length != 2 || string.IsNullOrEmpty(credentials[0]) || string.IsNullOrEmpty(credentials[1]))
            { 
                return null;
            }
            return new Credentials()
            {
                Username = credentials[0],
                Password = credentials[1]
            };
        }
        public class Credentials
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
        //public interface IProvidePrincipal
        //{
        //    IPrincipal CreatePrincipal(string username, string password);
        //}
        //public class DummyPrincipalProvider : IProvidePrincipal
        //{
        //    private const string Username = "sarpay@gmail.com";
        //    private const string Password = "1q2w3e";

        //    public IPrincipal CreatePrincipal(string username, string password)
        //    {
        //        if (username != Username || password != Password)
        //        {
        //            return null;
        //        }

        //        var identity = new GenericIdentity(Username);
        //        IPrincipal principal = new GenericPrincipal(identity, new[] { "User" });
        //        return principal;
        //    }
        //}
    }
}