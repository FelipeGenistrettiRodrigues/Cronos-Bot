using Domain.CronosBot.Models;
using Domain.CronosBot.Models.Enums;
using Domain.CronosBot.Repositories;
using Infrastructure.CronosBot.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.CronosBot.Repositories
{
    internal class ChatSessionRepository : IChatSessionRepository
    {
        private readonly CronosBotDbContext _dbContext;

        public ChatSessionRepository(CronosBotDbContext dbContext)
        {
            _dbContext = dbContext;   
        }

        public async Task Create(ChatSession chatSession)
        {
            await _dbContext.Sessions.AddAsync(chatSession);
        }

        public async Task<ChatSession?> GetActiveSessionByUserId(string userId)
        {
            return await _dbContext.Sessions.FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive);
        }

        public async Task Update(ChatSession chatSession)
        {
            _dbContext.Sessions.Update(chatSession);
        }

        public async Task<List<ChatSession>> GetSessionsStuckInStep(ChatStep step, DateTime dataLimite)
        {
            return await _dbContext.Sessions.Include(s => s.User).
                Where(s => s.IsActive && !s.hasPrescription && s.ReminderSentAt == null && s.CurrentStep == step && s.LastInteraction <= dataLimite).ToListAsync();
        }
    }
}
