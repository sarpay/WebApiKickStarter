using System.Web.Http;

namespace WebApiProject
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configuration
                .ParameterBindingRules
                .Insert(0, PostParameterBinding.HookupParameterBinding);

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}