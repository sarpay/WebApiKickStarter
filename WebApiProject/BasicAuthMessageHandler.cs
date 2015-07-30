using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace WebApiProject
{
    public class BasicAuthMessageHandler : DelegatingHandler
    {
        /**
            sample request header for Authorization:
            In Base64 Text: Basic MTAxMjoxMkI4MEU2QS04RkY0LTRDNzMtOTU2Mi03RDlFM0M4NzM5MzQ=
            In Plain Text : Basic 1201:12B80E6A-8FF4-4C73-9562-7D9E3C873934
        **/


        public IProvidePrincipal PrincipalProvider { get; set; }


        protected override async System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {

            AuthenticationHeaderValue authValue = request.Headers.Authorization;
            
            if (authValue != null && !String.IsNullOrWhiteSpace(authValue.Parameter))
            {
                Credentials parsedCredentials = ParseAuthHeader(authValue.Parameter);
                if (parsedCredentials != null)
                {
                    /** this is what [Authenticate] is looking for **/
                    request.GetRequestContext().Principal = PrincipalProvider
                        .CreatePrincipal(parsedCredentials.UserId, parsedCredentials.Ticket);
                }
            }

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                dynamic jsonOutput = new JObject();
                jsonOutput.Message = "Login Required!";
                jsonOutput.MessageDetail = "Please sign-in to view this page.";
                response.Content = new StringContent(jsonOutput.ToString(), Encoding.UTF8, "application/json");

                /**
                when status response code is Forbidden (403),
                browser forced login dialog does NOT show no matter what.
                **/
                //response = request.CreateResponse(HttpStatusCode.Forbidden);
                
                /** 
                when status response code is Unauthorized (401), 
                code below causes browser to show the standard login dilaog 
                **/
                //response.Headers.Add("WWW-Authenticate", "Basic");
            }

            return response;
        }


        private Credentials ParseAuthHeader(string authParams)
        {
            string[] credentials;
            try
            {
                Encoding encoding = Encoding.UTF8; /** -- or -- Encoding.GetEncoding("iso-8859-1") **/
                string creds = encoding.GetString(Convert.FromBase64String(authParams));
                credentials = creds.Split(':');
                /** --- or --- **/
                //credentials = Encoding.ASCII.GetString(Convert.FromBase64String(authParams)).Split(new[] { ':' });

                if (credentials.Length == 2
                        && Helpers.TryConvertStringToInt(credentials[0])
                        && !String.IsNullOrWhiteSpace(credentials[1]))
                {
                    int userId = int.Parse(credentials[0]);
                    string ticket = credentials[1];
                    Guid guidOutput;
                    if (!Guid.TryParse(ticket, out guidOutput))
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
            
            return new Credentials()
            {
                UserId = credentials[0],
                Ticket = credentials[1],
            };
        }
    }


    public class Credentials
    {
        public string UserId { get; set; }
        public string Ticket { get; set; }
    }


    public interface IProvidePrincipal
    {
        IPrincipal CreatePrincipal(string userId, string ticket);
    }


    public class PrincipalProvider : IProvidePrincipal
    {

        public IPrincipal CreatePrincipal(string userId, string ticket)
        {
            ADONET AdoNet = new ADONET();
            bool valid = false;

            try
            {
                /** query the database to authenticate private request **/
                AdoNet.SqlConnect();

                /** specify the stored procedure **/
                AdoNet.SqlNewCommand("dbo.authUser", "sp");
                /** INs **/
                AdoNet.SqlNewParam("Input", "@UserID", userId, SqlDbType.Int, 0);
                AdoNet.SqlNewParam("Input", "@Ticket", ticket, SqlDbType.Char, 36);
                /** OUTs **/
                AdoNet.SqlNewParam("Output", "@Authenticated", null, SqlDbType.Bit, 0);
                /** Execute SP **/
                AdoNet.SqlExecuteCommand();

                /** Obtain output params' values **/
                valid = Convert.ToBoolean(AdoNet.SqlOutputParamValue("@Authenticated"));
            }
            catch
            {
                return null;
            }
            finally
            {
                AdoNet.SqlDisconnect();
            }

            if (valid) {
                var identity = new GenericIdentity(userId);
                IPrincipal principal = new GenericPrincipal(identity, new[] { "User" });
                return principal;
            }
            else
            {
                return null;
            }
        }

    }

}