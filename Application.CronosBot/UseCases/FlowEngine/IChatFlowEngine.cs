using Communication.CronosBot.EvolutionWebHook.Request;

namespace Application.CronosBot.UseCases.FlowEngine
{
    public interface IChatFlowEngine
    {
        Task ProcessIncomingMessage(IncomingMessageContext context);
    }
}
