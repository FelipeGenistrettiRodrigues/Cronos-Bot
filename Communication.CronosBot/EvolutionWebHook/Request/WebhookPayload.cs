namespace Communication.CronosBot.EvolutionWebHook.Request
{
    public record WebhookPayload(string Event, string Instance, WebhookData Data);
}
