using Application.CronosBot.UseCases.FollowUpLeads; 
using Domain.CronosBot.Models.Enums;
using Hangfire;

namespace Api.CronosBot.Jobs 
{
    public class LembreteJobs : ILembreteJobs
    {
        private readonly IRecuperarLeadsSemReceitaUseCase _useCase;

        public LembreteJobs(IRecuperarLeadsSemReceitaUseCase useCase)
        {
            _useCase = useCase;
        }

        [DisableConcurrentExecution(3600)]
        public async Task ExecutarLembreteCincoDiasAsync()
        {
            await _useCase.ExecutarLembreteAutomaticoAsync(TipoLembreteReceita.CincoDias);
        }

        [DisableConcurrentExecution(3600)]
        public async Task ExecutarLembreteQuatorzeDiasAsync()
        {
            await _useCase.ExecutarLembreteAutomaticoAsync(TipoLembreteReceita.QuatorzeDias);
        }
    }
}