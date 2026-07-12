using Domain.CronosBot.Repositories;

namespace Infrastructure.CronosBot.DataAccess
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly CronosBotDbContext _dbContext;

        public UnitOfWork(CronosBotDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Commit()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
