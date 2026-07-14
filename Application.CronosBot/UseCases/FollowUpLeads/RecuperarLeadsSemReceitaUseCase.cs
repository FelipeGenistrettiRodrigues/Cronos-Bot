using Application.CronosBot.UseCases.CallApiEvolution;
using Domain.CronosBot.Models;
using Domain.CronosBot.Models.Enums;
using Domain.CronosBot.Repositories;

namespace Application.CronosBot.UseCases.FollowUpLeads
{
    public class RecuperarLeadsSemReceitaUseCase : IRecuperarLeadsSemReceitaUseCase
    {
        private readonly IChatSessionRepository _sessionRepository;
        private readonly IWhatsappProvider _whatsappProvider;
        private readonly IUnitOfWork _unitOfWork;

        public RecuperarLeadsSemReceitaUseCase(
            IChatSessionRepository sessionRepository,
            IWhatsappProvider whatsappProvider,
            IUnitOfWork unitOfWork)
        {
            _sessionRepository = sessionRepository;
            _whatsappProvider = whatsappProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecutarLembreteAutomaticoAsync(TipoLembreteReceita tipoLembrete)
        {
            var regra = DefinirTipoLembrete(tipoLembrete);

            var dataLimite = DateTime.UtcNow.AddDays(-regra.DiasAtraso);

            List<ChatSession> sessoesEsquecidas = await _sessionRepository.GetSessionsStuckInStep(
                ChatStep.AguardandoReceitaMedica,
                dataLimite,
                regra.EstagioRequerido            
            );

            foreach (var sessao in sessoesEsquecidas)
            {
                try
                {
                    string msgLembrete = regra.MensagemFactory(sessao.User.Name);

                    await _whatsappProvider.SendTextMessage(sessao.User.PhoneNumber, msgLembrete, sessao.PhoneId);

                    regra.AcaoAtualizar(sessao);
                    await _sessionRepository.Update(sessao);

                    await _unitOfWork.Commit();

                    await Task.Delay(Random.Shared.Next(5000, 12000));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Falha ao lembrar lead {sessao.User.PhoneNumber}: {ex.Message}");
                }
            }

            
        }

        private (int DiasAtraso, EstagioLembreteReceita EstagioRequerido, Func<string, string> MensagemFactory, Action<ChatSession> AcaoAtualizar) DefinirTipoLembrete(TipoLembreteReceita tipoLembrete)
        {
            return tipoLembrete switch
            {
                TipoLembreteReceita.CincoDias => (
                    5,
                    EstagioLembreteReceita.Nenhum,
                    (string nome) => $"Oi {nome}! Conseguiu agendar sua consulta para adquirir sua receita? Se precisar de ajuda, me avise!",
                    (ChatSession s) => s.SetLembreteCincoDiasEnviado()
                ),
                TipoLembreteReceita.QuatorzeDias => (
                    14,
                    EstagioLembreteReceita.LembreteCincoDiasEnviado,
                    (string nome) => $"Olá {nome}! Passando para lembrar da foto da sua receita médica. 👓\n\nSe já tiver com ela, é só mandar aqui!",
                    (ChatSession s) => s.SetLembreteQuatorzeDiasEnviado()
                ),
                _ => throw new ArgumentException("Tipo de lembrete não suportado.")
            };
        }
    }
}