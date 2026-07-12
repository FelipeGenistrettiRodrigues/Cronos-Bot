namespace Application.CronosBot.UseCases.CallApiEvolution
{
    public interface IWhatsappProvider
    {
        Task SendTextMessage(string fromNumber, string message, string instanceName);
    }
}
