using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.CronosBot.FIlters
{
    public class ApiKeyAuthFilter : IAsyncActionFilter
    {

        private readonly IConfiguration _configuration;
        public ApiKeyAuthFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var expectedApiKey = _configuration["EvolutionApi:ApiKey"];

            if (!context.HttpContext.Request.Headers.TryGetValue("apikey", out var extractedApiKey) ||
            extractedApiKey != expectedApiKey)
            {
                context.Result = new UnauthorizedObjectResult("API Key inválida ou ausente.");
                return; 
            }

            await next();

        }
    }
}
