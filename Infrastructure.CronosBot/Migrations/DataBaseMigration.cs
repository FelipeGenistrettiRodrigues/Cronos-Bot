using Infrastructure.CronosBot.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.CronosBot.Migrations
{
    public class DataBaseMigration
    {
        public async static Task MigrateDatabase(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<CronosBotDbContext>();
            await dbContext.Database.MigrateAsync();
        }
    }
}
