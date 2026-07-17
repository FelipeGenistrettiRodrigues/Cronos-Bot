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
            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro da Evolution API: Status {response.StatusCode} - Detalhes: {errorDetails}");
            }
            response.EnsureSuccessStatusCode();
        }
    }
}
