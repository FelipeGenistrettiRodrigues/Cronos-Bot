using Domain.CronosBot.Models.Enums;

namespace Application.CronosBot.UseCases.FollowUpLeads
{
    public interface IRecuperarLeadsSemReceitaUseCase
    {
        Task ExecutarLembreteAutomaticoAsync(TipoLembreteReceita tipoLembrete);
    }
}
