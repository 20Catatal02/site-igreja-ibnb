using System.Text.Json;

namespace IBNB.Services
{
    public class FeriadoDto
    {
        public string Date { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
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
                if (resposta.IsSuccessStatusCode)
                {
                    var json = await resposta.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<FeriadoDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                }
            }
            catch
            {
                // Tratamento de falha caso esteja sem conexão com a internet
            }
            return new List<FeriadoDto>();
        }
    }
}