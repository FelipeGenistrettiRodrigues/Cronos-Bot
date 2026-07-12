namespace Communication.CronosBot.EvolutionWebHook.Request
{
    public record WebhookMessage(string? Conversation, MediaMessageDetail? ImageMessage, MediaMessageDetail? DocumentMessage);
}
