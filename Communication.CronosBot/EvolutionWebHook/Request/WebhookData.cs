namespace Communication.CronosBot.EvolutionWebHook.Request
{
    public record WebhookData(WebhookKey Key, string PushName, WebhookMessage Message, string MessageType);
}
