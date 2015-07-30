using System;
using System.Web.Http.Controllers;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http.Filters;
using System.Data;
using Newtonsoft.Json.Linq;


namespace WebApiProject
{
    public class Private : AuthorizationFilterAttribute
    {

        public override void OnAuthorization(HttpActionContext actionContext)
        {

            var authHeader = actionContext.Request.Headers.Authorization;

            if (!IsActionPrivate(authHeader))
            {
                HandleForbiddenRequest(actionContext);
            }
            
        }


        private bool IsActionPrivate(AuthenticationHeaderValue authHeader)
        {
            if (authHeader != null)
            {
                if (authHeader.Scheme.Equals("Basic", StringComparison.OrdinalIgnoreCase)
                        && !String.IsNullOrWhiteSpace(authHeader.Parameter))
                {
                    string[] credArray = GetCredentials(authHeader);
                    if (Helpers.TryConvertTo<int>(credArray[0])
                        && !String.IsNullOrWhiteSpace(credArray[1]))
                    {
                        int userId = int.Parse(credArray[0]);
                        string ticket = credArray[1];
                        if (ticket.Length != 36)
                        {
                            return false;
                        }
                        if (!ValidateCredentials(userId, ticket))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }


        private bool ValidateCredentials(int userId, string ticket)
        {

            bool valid = false;
            ADONET AdoNet = new ADONET();

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
            finally
            {
                AdoNet.SqlDisconnect();
            }

            return valid;
        }


        private string[] GetCredentials(AuthenticationHeaderValue authHeader)
        {
            var rawCred = authHeader.Parameter;
            var encoding = Encoding.GetEncoding("iso-8859-1"); /** encodes ASCII exactly as UTF8 **/
            var cred = encoding.GetString(Convert.FromBase64String(rawCred));
            var credArray = cred.Split(':');

            return credArray;
        }


        private void HandleForbiddenRequest(HttpActionContext actionContext)
        {
            dynamic jsonOutput = new JObject();
            jsonOutput.Message = "Login Required!";
            jsonOutput.MessageDetail = "Please sign-in to view this page.";

            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);/** .Unauthorized forces browser to ask for user and password **/
            actionContext.Response.Headers.Add("WWW-Authenticate", "Basic");
            actionContext.Response.Content = new StringContent(jsonOutput.ToString(), Encoding.UTF8, "application/json");
        }

    }
}