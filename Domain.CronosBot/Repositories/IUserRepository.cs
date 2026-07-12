using Domain.CronosBot.Models;

namespace Domain.CronosBot.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByPhone(string phone);
        Task Create(User user);
    }
}
