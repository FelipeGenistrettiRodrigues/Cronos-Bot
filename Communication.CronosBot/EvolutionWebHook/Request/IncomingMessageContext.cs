namespace Communication.CronosBot.EvolutionWebHook.Request
{
    public record IncomingMessageContext(
        string FromNumber,
        string MessageText,
        string InstanceName,
        string PushName,
        string MessageType,
        string? MediaUrl
       );
}
