using System.Text.Json;
using IBNB.Data;
using IBNB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IBNB.Services
{
    public class VersiculosSeeder
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _ambiente;
        private readonly ILogger<VersiculosSeeder> _logger;

        public VersiculosSeeder(
            AppDbContext context,
            IWebHostEnvironment ambiente,
            ILogger<VersiculosSeeder> logger)
        {
            _context = context;
            _ambiente = ambiente;
            _logger = logger;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            if (await _context.Versiculos.AnyAsync(cancellationToken))
            {
                return;
            }

            var caminho = Path.Combine(_ambiente.ContentRootPath, "Data", "versiculos_seed.json");
            if (!File.Exists(caminho))
            {
                _logger.LogWarning("Arquivo de seed de versículos não encontrado em {Caminho}.", caminho);
                return;
            }

            await using var stream = File.OpenRead(caminho);
            var itens = await JsonSerializer.DeserializeAsync<List<VersiculoSeed>>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                cancellationToken);

            if (itens is null || itens.Count == 0)
            {
                _logger.LogWarning("O seed de versículos está vazio.");
                return;
            }

            _context.Versiculos.AddRange(itens.Select(item => new Versiculo
            {
                Referencia = item.Referencia,
                Texto = item.Texto,
                Traducao = item.Traducao,
                Autor = item.Autor
            }));

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Versículos carregados: {Total}.", itens.Count);
        }
    }

    public sealed class VersiculoSeed
    {
        public string Referencia { get; set; } = string.Empty;

        public string Texto { get; set; } = string.Empty;

        public string Traducao { get; set; } = "Almeida 1911";

        public string? Autor { get; set; }
    }
}
