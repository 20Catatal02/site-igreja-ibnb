using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IBNB.Data;
using IBNB.Models;
using IBNB.Services;
using IBNB.ViewModels;
using Microsoft.AspNetCore.Authorization; 

namespace IBNB.Controllers
{
    
    public class EventosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly FeriadoService _feriadoService;

        public EventosController(AppDbContext context, FeriadoService feriadoService)
        {
            _context = context;
            _feriadoService = feriadoService;
        }

        // GET: Eventos
        public async Task<IActionResult> Index()
        {
            var eventos = await _context.Eventos
                .OrderBy(e => e.DataHora)
                .ToListAsync();
            return View(eventos);
        }

        [Authorize]
        // GET: Eventos/Create
        public IActionResult Create()
        {
            return View(new EventoViewModel());
        }

        [Authorize]
        // POST: Eventos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var evento = new Evento
                {
                    Titulo = model.Titulo,
                    Descricao = model.Descricao,
                    DataHora = model.DataHora,
                    Local = model.Local,
                    IsRecorrente = model.IsRecorrente,
                    TipoRecorrencia = model.IsRecorrente ? (model.TipoRecorrencia ?? "Nunca") : "Nunca",
                    FrequenciaPersonalizada = model.FrequenciaPersonalizada,
                    IntervaloRecorrencia = model.IntervaloRecorrencia <= 0 ? 1 : model.IntervaloRecorrencia,
                    IndiceSemana = model.IndiceSemana,
                    DiaDaSemana = model.DiaDaSemana
                };

                _context.Add(evento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Eventos/Calendario (CalendÃ¡rio pÃºblico - sem login)
        public async Task<IActionResult> Calendario(int? mes, int? ano)
        {
            var hoje = DateTime.Today;
            var usuarioLogado = User?.Identity?.IsAuthenticated ?? false;

            var mesAtual = mes is >= 1 and <= 12 ? mes.Value : hoje.Month;
            var anoAtual = ano is >= 2000 and <= 2100 ? ano.Value : hoje.Year;

            var primeiroDia = new DateTime(anoAtual, mesAtual, 1);
            var ultimoDia = primeiroDia.AddMonths(1).AddDays(-1);

            // Grade do mÃªs: do dia 1 ao Ãºltimo dia do mÃªs (somente o mÃªs exibido)
            var inicioGrade = primeiroDia;
            var fimGrade = ultimoDia;

            // 1. Feriados nacionais do ano exibido
            var feriados = new List<CalendarioItemViewModel>();
            var feriadosDto = await _feriadoService.ObterFeriadosAsync(anoAtual);
            foreach (var f in feriadosDto)
            {
                if (DateTime.TryParse(f.Date, out var dataFeriado)
                    && dataFeriado.Month == mesAtual
                    && dataFeriado.Year == anoAtual)
                {
                    feriados.Add(new CalendarioItemViewModel
                    {
                        Titulo = f.Name,
                        Data = dataFeriado.Date,
                        Categoria = "Feriado",
                        Local = "Nacional"
                    });
                }
            }

            // 2. Eventos do mÃªs (inclui as agendas recorrentes: EBD, cultos e ceia)
            var eventos = await _context.Eventos.ToListAsync();
            var itensEventos = new List<CalendarioItemViewModel>();
            foreach (var ev in eventos)
            {
                foreach (var data in OcorrenciasNoPeriodo(ev, inicioGrade, fimGrade))
                {
                    var tipo = TipoOcorrencia(ev, data);
                    var ehCeia = tipo is "Ceia" or "Virada";
                    itensEventos.Add(new CalendarioItemViewModel
                    {
                        // No dia 31/12 o tÃtulo Ã© normalizado para "Culto da Virada + Ceia do Senhor"
                        Titulo = tipo == "Virada" ? "Culto da Virada + Ceia do Senhor" : ev.Titulo,
                        Data = data,
                        Categoria = "Evento",
                        Local = ev.Local,
                        Subtipo = tipo,
                        Destaque = ehCeia
                    });
                }
            }

            // Remove o culto noturno comum quando no mesmo dia hÃ¡ o Culto de Ceia
            var diasComCeia = itensEventos
                .Where(i => i.Destaque)
                .Select(i => i.Data.Date)
                .Distinct()
                .ToList();

            itensEventos = itensEventos
                .Where(i => i.Destaque
                            || !(diasComCeia.Contains(i.Data.Date) && EhCultoNoturnoDeDomingo(i)))
                .ToList();

            // 3. Aniversariantes (somente para usuÃ¡rios autenticados)
            var itensAniversario = new List<CalendarioItemViewModel>();
            if (usuarioLogado)
            {
                var aniversariantes = await _context.Aniversariantes.ToListAsync();
                foreach (var aniv in aniversariantes)
                {
                    if (aniv.DataNascimento.Month != mesAtual)
                        continue;

                    itensAniversario.Add(new CalendarioItemViewModel
                    {
                        Titulo = aniv.Nome,
                        Data = new DateTime(anoAtual, aniv.DataNascimento.Month, aniv.DataNascimento.Day),
                        Categoria = "AniversÃ¡rio",
                        Local = "-"
                    });
                }
            }

            var todos = itensEventos.Concat(feriados).Concat(itensAniversario).ToList();

            // Monta a grade de dias
            var dias = new List<CalendarioDiaViewModel>();
            for (var d = inicioGrade; d <= fimGrade; d = d.AddDays(1))
            {
                dias.Add(new CalendarioDiaViewModel
                {
                    Data = d,
                    NoMesAtual = true,
                    EhHoje = d == hoje,
                    DiaSemana = d.ToString("ddd", new System.Globalization.CultureInfo("pt-BR")),
                    Itens = todos.Where(i => i.Data.Date == d)
                                 .OrderBy(i => i.Categoria == "Feriado" ? 0 : i.Categoria == "Evento" ? 1 : 2)
                                 .ThenByDescending(i => i.Destaque)
                                 .ThenBy(i => i.Data)
                                 .ToList()
                });
            }

            // PrÃ³ximo evento da igreja a partir de hoje
            var buscaProximoAte = hoje.AddMonths(6);
            var proximoEvento = eventos
                .SelectMany(ev => OcorrenciasNoPeriodo(ev, hoje, buscaProximoAte)
                    .Select(data => new CalendarioItemViewModel
                    {
                        Titulo = TipoOcorrencia(ev, data) == "Virada" ? "Culto da Virada + Ceia do Senhor" : ev.Titulo,
                        Data = data,
                        Categoria = "Evento",
                        Local = ev.Local,
                        Subtipo = TipoOcorrencia(ev, data),
                        Destaque = TipoOcorrencia(ev, data) is "Ceia" or "Virada"
                    }))
                .OrderBy(i => i.Data)
                .FirstOrDefault();

            var vm = new CalendarioViewModel
            {
                Hoje = DateTime.Now,
                Mes = mesAtual,
                Ano = anoAtual,
                NomeMes = primeiroDia.ToString("MMMM 'de' yyyy", new System.Globalization.CultureInfo("pt-BR")),
                Dias = dias,
                EventosHoje = itensEventos.Where(i => i.Data.Date == hoje).OrderBy(i => i.Data).ToList(),
                FeriadosHoje = feriados.Where(i => i.Data.Date == hoje).ToList(),
                AniversariantesHoje = itensAniversario.Where(i => i.Data.Date == hoje).ToList(),
                ProximoEvento = proximoEvento,
                MostrarAniversariantes = usuarioLogado,
                TotalEventos = itensEventos.Count,
                TotalCultos = itensEventos.Count(i => i.Subtipo is "EBD" or "CultoNoturno" or "Ceia" or "Virada"),
                TotalFeriados = feriados.Count,
                TotalAniversariantes = itensAniversario.Count
            };

            return View(vm);
        }

        // Classifica a ocorrÃªncia: "Ceia", "EBD", "CultoNoturno" ou "Evento"
        private static string TipoOcorrencia(Evento ev, DateTime data)
        {
            var titulo = (ev.Titulo ?? string.Empty).ToLowerInvariant();

            if (titulo.Contains("ceia"))
                return "Ceia";

            // Virada de ano: 31/12 (mesma tarja de destaque da Ceia)
            if ((data.Month == 12 && data.Day == 31) || titulo.Contains("virada") || titulo.Contains("ano novo"))
                return "Virada";

            if (titulo.Contains("ebd") || titulo.Contains("escola bÃ­blica") || titulo.Contains("escola biblica"))
                return "EBD";

            if (data.DayOfWeek == DayOfWeek.Sunday && data.Hour >= 18)
                return "CultoNoturno";

            return "Evento";
        }

        // O culto noturno de domingo pode ser substituÃ­do pelo Culto de Ceia no 1Âº domingo
        private static bool EhCultoNoturnoDeDomingo(CalendarioItemViewModel item)
            => item.Subtipo == "CultoNoturno";

        // Calcula as datas de ocorrÃªncia de um evento dentro de um perÃ­odo,
        // considerando recorrÃªncia semanal, mensal (com Ã­ndice da semana), anual,
        // dias Ãºteis e frequÃªncia personalizada.
        private static IEnumerable<DateTime> OcorrenciasNoPeriodo(Evento ev, DateTime inicio, DateTime fim)
        {
            var baseData = ev.DataHora;
            if (!ev.IsRecorrente)
            {
                if (baseData.Date >= inicio.Date && baseData.Date <= fim.Date)
                    yield return baseData;
                yield break;
            }

            var passo = ev.IntervaloRecorrencia <= 0 ? 1 : ev.IntervaloRecorrencia;
            var tipo = (ev.TipoRecorrencia ?? "Nunca").Trim();

            for (var data = baseData.Date; data <= fim.Date; )
            {
                if (data >= inicio.Date)
                {
                    yield return data.Date.Add(baseData.TimeOfDay);
                }

                switch (tipo)
                {
                    case "Diario":
                        data = data.AddDays(passo);
                        break;
                    case "SegundaSexta":
                        data = data.AddDays(1);
                        while (data.DayOfWeek == DayOfWeek.Saturday || data.DayOfWeek == DayOfWeek.Sunday)
                            data = data.AddDays(1);
                        break;
                    case "Semanal":
                        data = data.AddDays(7 * passo);
                        break;
                    case "Mensal":
                        // Ex.: IndiceSemana = 1 => primeira semana do mÃªs (1Âº domingo, 1Âª quarta...)
                        if (ev.IndiceSemana.HasValue && ev.DiaDaSemana.HasValue)
                        {
                            var alvo = (DayOfWeek)ev.DiaDaSemana.Value;
                            var proximoMes = new DateTime(data.Year, data.Month, 1).AddMonths(passo);
                            data = EnesimoDiaDaSemana(proximoMes.Year, proximoMes.Month, alvo, ev.IndiceSemana.Value);
                        }
                        else
                        {
                            data = data.AddMonths(passo);
                        }
                        break;
                    case "Anual":
                        data = data.AddYears(passo);
                        break;
                    case "Personalizado":
                        data = (ev.FrequenciaPersonalizada ?? "semana").ToLowerInvariant() switch
                        {
                            "dia" => data.AddDays(passo),
                            "mes" => data.AddMonths(passo),
                            "ano" => data.AddYears(passo),
                            _ => data.AddDays(7 * passo)
                        };
                        break;
                    default:
                        yield break; // "Nunca"
                }
            }
        }

        // Retorna o enÃ©simo dia da semana do mÃªs (1 = primeiro)
        private static DateTime EnesimoDiaDaSemana(int ano, int mes, DayOfWeek diaSemana, int indice)
        {
            var n = indice <= 0 ? 1 : indice;
            var primeiro = new DateTime(ano, mes, 1);
            var offset = ((int)diaSemana - (int)primeiro.DayOfWeek + 7) % 7;
            var data = primeiro.AddDays(offset + 7 * (n - 1));

            // Se estourar o mÃªs, usa a Ãºltima ocorrÃªncia
            if (data.Month != mes)
                data = data.AddDays(-7);

            return data;
        }
    }
}



