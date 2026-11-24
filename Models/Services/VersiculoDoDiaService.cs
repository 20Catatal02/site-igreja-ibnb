using IBNB.Data;
using IBNB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace IBNB.Services
{
    public class VersiculoDoDiaService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly TimeProvider _timeProvider;
        private readonly ILogger<VersiculoDoDiaService> _logger;

        public VersiculoDoDiaService(
            AppDbContext context,
            IMemoryCache cache,
            TimeProvider timeProvider,
            ILogger<VersiculoDoDiaService> logger)
        {
            _context = context;
            _cache = cache;
            _timeProvider = timeProvider;
            _logger = logger;
        }

        public async Task<Versiculo?> ObterAsync(CancellationToken cancellationToken = default)
        {
            var fuso = ObterFusoHorario();
            var agora = TimeZoneInfo.ConvertTime(_timeProvider.GetUtcNow(), fuso);
            var hoje = DateOnly.FromDateTime(agora.DateTime);
            var chave = $"versiculo-do-dia:{hoje:yyyy-MM-dd}";

            if (_cache.TryGetValue(chave, out Versiculo? emCache) && emCache is not null)
            {
                return emCache;
            }

            try
            {
                var registro = await _context.VersiculosDoDia
                    .AsNoTracking()
                    .Include(item => item.Versiculo)
                    .FirstOrDefaultAsync(item => item.Data == hoje, cancellationToken);

                if (registro?.Versiculo is not null)
                {
                    Guardar(chave, registro.Versiculo, hoje);
                    return registro.Versiculo;
                }

                // Escolhe o versículo que nunca apareceu ou que apareceu há mais tempo.
                var escolhido = await _context.Versiculos
                    .Where(versiculo => versiculo.Ativo)
                    .OrderBy(versiculo => versiculo.UltimaExibicao == null ? 0 : 1)
                    .ThenBy(versiculo => versiculo.UltimaExibicao)
                    .ThenBy(versiculo => EF.Functions.Random())
                    .FirstOrDefaultAsync(cancellationToken);

                if (escolhido is null)
                {
                    return null;
                }

                escolhido.UltimaExibicao = hoje;
                _context.VersiculosDoDia.Add(new VersiculoDoDia
                {
                    Data = hoje,
                    VersiculoId = escolhido.Id
                });

                try
                {
                    await _context.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateException)
                {
                    // Duas requisições ao mesmo tempo: uma delas já gravou o dia.
                    _context.ChangeTracker.Clear();

                    var existente = await _context.VersiculosDoDia
                        .AsNoTracking()
                        .Include(item => item.Versiculo)
                        .FirstOrDefaultAsync(item => item.Data == hoje, cancellationToken);

                    if (existente?.Versiculo is null)
                    {
                        throw;
                    }

                    Guardar(chave, existente.Versiculo, hoje);
                    return existente.Versiculo;
                }

                Guardar(chave, escolhido, hoje);
                return escolhido;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Não foi possível obter o versículo do dia.");
                return null;
            }
        }

        private void Guardar(string chave, Versiculo versiculo, DateOnly dia)
        {
            var fuso = ObterFusoHorario();
            var agora = TimeZoneInfo.ConvertTime(_timeProvider.GetUtcNow(), fuso);
            var expira = new DateTimeOffset(dia.AddDays(1).ToDateTime(TimeOnly.MinValue), agora.Offset);

            _cache.Set(chave, versiculo, expira);
        }

        private static TimeZoneInfo ObterFusoHorario()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
            }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.Local;
            }
            catch (InvalidTimeZoneException)
            {
                return TimeZoneInfo.Local;
            }
        }
    }
}
