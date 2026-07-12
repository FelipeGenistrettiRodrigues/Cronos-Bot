using Domain.CronosBot.Models;
using Domain.CronosBot.Repositories;
using Infrastructure.CronosBot.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace Infrastructure.CronosBot.Repositories
{
    internal class UserRepository : IUserRepository
    {

        private readonly CronosBotDbContext _dbContext;

        public UserRepository(CronosBotDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<User?> GetUserByPhone(string phone)
        {
            return await _dbContext.Users
                .Include(u => u.Sessions)
                .FirstOrDefaultAsync(u => u.PhoneNumber == phone);
        }

        public async Task Create(User user)
        {
            await _dbContext.AddAsync(user);
        }
    }
}
