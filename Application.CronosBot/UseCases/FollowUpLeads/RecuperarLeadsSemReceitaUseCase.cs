using Application.CronosBot.UseCases.CallApiEvolution;
using Application.CronosBot.UseCases.FollowUpLeads;
using Domain.CronosBot.Models;
using Domain.CronosBot.Models.Enums;
using Domain.CronosBot.Repositories;

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
    public async Task ExecutarLembreteAutomaticoAsync()
    {
        var dataLimite = DateTime.UtcNow.AddDays(-14);
        List<ChatSession> sessoesEsquecidas = await _sessionRepository.GetSessionsStuckInStep(ChatStep.AguardandoReceitaMedica, dataLimite);

        var random = new Random();

        foreach (var sessao in sessoesEsquecidas)
        {
            try
            {
                string msgLembrete = $"Olá {sessao.User.Name}! Passando para lembrar que ainda não recebemos a foto da sua receita médica. 👓\n\nSe tiver ela aí, é só mandar por aqui para liberarmos seu orçamento! Se não estiver com ela, digite *0* para falar com um atendente.";

                await _whatsappProvider.SendTextMessage(sessao.User.PhoneNumber, msgLembrete, sessao.PhoneId);

                sessao.MarcarLembreteEnviado();
                await _sessionRepository.Update(sessao);

                await Task.Delay(random.Next(5000, 12000));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Falha ao lembrar lead {sessao.User.PhoneNumber}: {ex.Message}");
            }
        }
        await _unitOfWork.Commit();
    }
}