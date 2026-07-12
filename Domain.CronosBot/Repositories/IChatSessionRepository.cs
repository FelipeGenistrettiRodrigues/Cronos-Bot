using Domain.CronosBot.Models;

namespace Domain.CronosBot.Repositories
{
    public interface IChatSessionRepository
    {
        Task Create(ChatSession chatSession);
        Task<ChatSession?> GetActiveSessionByUserId(string userId);

        Task Update(ChatSession chatSession);
    }
}
