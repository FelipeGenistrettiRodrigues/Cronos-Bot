using System.Net.Http.Json;

namespace Application.CronosBot.UseCases.CallApiEvolution
{
    public class EvolutionApiProvider : IWhatsappProvider
    {
        private readonly HttpClient _httpClient;

        public EvolutionApiProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;    
        }

        public async Task SendTextMessage(string fromNumber, string message, string instanceName)
        {
            string url = $"/message/sendText/{instanceName}";

            var payload = new
            {
                number = fromNumber,
                options = new { delay = 1200, presence = "composing" },
                text = message,
            };

            var response = await _httpClient.PostAsJsonAsync(url, payload);
            response.EnsureSuccessStatusCode();
        }
    }
}
