namespace Communication.CronosBot.EvolutionWebHook.Request
{
    public record WebhookKey(string RemoteJid, bool FromMe, string Id);
}
