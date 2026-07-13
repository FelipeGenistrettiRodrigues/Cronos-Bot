using Domain.CronosBot.Models;
using Domain.CronosBot.Models.Enums;

namespace Domain.CronosBot.Repositories
{
    public interface IChatSessionRepository
    {
        Task Create(ChatSession chatSession);
        Task<ChatSession?> GetActiveSessionByUserId(string userId);

        Task Update(ChatSession chatSession);

        Task<List<ChatSession>> GetSessionsStuckInStep(ChatStep step, DateTime dataLimite, EstagioLembreteReceita estagioRequerido);
    }
}
