using System.Text.Json;
using System.Text.Json.Serialization;

namespace IBNB.Services
{
    public class FeriadoDto
    {
        [JsonPropertyName("date")]
        public string Date { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
    }

    // A BrasilAPI responde com um objeto { "value": [ ... ] }
    public class FeriadoRespostaDto
    {
        [JsonPropertyName("value")]
        public List<FeriadoDto> Value { get; set; } = new();
    }

    public class FeriadoService
    {
        private readonly HttpClient _httpClient;

        public FeriadoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<FeriadoDto>> ObterFeriadosAsync(int ano)
        {
            try
            {
                string url = $"https://brasilapi.com.br/api/feriados/v1/{ano}";
                var resposta = await _httpClient.GetAsync(url);
                if (!resposta.IsSuccessStatusCode)
                    return new List<FeriadoDto>();

                var json = await resposta.Content.ReadAsStringAsync();
                var opcoes = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                // Formato atual: { "value": [ ... ] }
                if (json.TrimStart().StartsWith("{"))
                {
                    var envelope = JsonSerializer.Deserialize<FeriadoRespostaDto>(json, opcoes);
                    return envelope?.Value ?? new List<FeriadoDto>();
                }

                // Formato antigo: [ ... ]
                return JsonSerializer.Deserialize<List<FeriadoDto>>(json, opcoes) ?? new List<FeriadoDto>();
            }
            catch
            {
                // Tratamento de falha caso esteja sem conexão com a internet
                return new List<FeriadoDto>();
            }
        }
    }
}
