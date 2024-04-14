using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace PeopleApp.Api.Attributes
{
    [AttributeUsage(validOn: AttributeTargets.Class)]
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string APIKEYNAME = "ApiKey";
        private readonly IConfiguration appSettings;

        public ApiKeyAttribute(IConfiguration configuration)
        {
            appSettings = configuration;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try 
            { 
                if (!context.HttpContext.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
                {
                    context.Result = new ContentResult()
                    {
                        StatusCode = 401,
                        Content = "Api Key is missing"
                    };
                    return;
                }


                //var appSettings = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>(); 
                if (appSettings == null) 
                { 
                    context.Result = new ContentResult()
                    {
                        StatusCode = 401,
                        Content = "Appsettings not found"
                    }; 
                    return; 
                }

                var apiKey = appSettings.GetValue<string>(APIKEYNAME); 
                if (apiKey == null) 
                { 
                    context.Result = new ContentResult()
                    {
                        StatusCode = 401,
                        Content = "Appsettings - ApiKey - not found"
                    }; 
                    return; 
                }

                if (!apiKey.Equals(extractedApiKey))
                {
                    context.Result = new ContentResult()
                    {
                        StatusCode = 401,
                        Content = "Api Key is not valid"
                    };
                    return;
                }

                await next();
            }
            catch (Exception ex)    
            { 
                context.Result = new ContentResult()
                {
                    StatusCode = 500,
                    Content = ex.Message
                };
            }
        }
    }

}
