using Application.CronosBot.AutoMapper;
using Application.CronosBot.UseCases.CallApiEvolution;
using Application.CronosBot.UseCases.FlowEngine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.CronosBot
{
    public static class DependencyInjectionApplication
    {
        public static void AddApplication(this IServiceCollection service)
        {
            AddUseCase(service);
            AddAutoMapper(service);
        }

        private static void AddUseCase(IServiceCollection service)
        {
            service.AddScoped<IChatFlowEngine, ChatFlowEngine>();
        }

        private static void AddAutoMapper(IServiceCollection service)
        {
            service.AddAutoMapper(config => config.AddProfile<WebhookMappingProfile>());
        }
    }
}
