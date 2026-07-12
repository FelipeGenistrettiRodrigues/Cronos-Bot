using Domain.CronosBot.Repositories;
using Infrastructure.CronosBot.DataAccess;
using Infrastructure.CronosBot.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.CronosBot
{
    public static class DependencyInjectionInfrastructure
    {

        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            AddDbContext(services, configuration);
            AddRepositories(services);
        }

        private static void AddDbContext( IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("Connection");
            services.AddDbContext<CronosBotDbContext>(config => config.UseNpgsql(connectionString));
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IChatSessionRepository, ChatSessionRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
