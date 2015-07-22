using System.Web.Http;

namespace WebApiProject
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            /* Activates HttpPost manipulation (PostParameterBinding.cs) for accepting multiple params */
            GlobalConfiguration.Configuration
                .ParameterBindingRules
                .Insert(0, PostParameterBinding.HookupParameterBinding);
            /*******************************************************************************************/

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}