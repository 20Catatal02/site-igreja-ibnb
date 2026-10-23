using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IBNB.Data;
using IBNB.Models;
using IBNB.Services;
using IBNB.ViewModels;

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
            var eventos = await _context.Eventos.ToListAsync();
            return View(eventos);
        }

        // GET: Eventos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Eventos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Evento evento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(evento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(evento);
        }

        // GET: Eventos/Calendario (Calendário Unificado)
        public async Task<IActionResult> Calendario()
        {
            int anoAtual = DateTime.Now.Year;

            // 1. Buscar feriados da API externa
            var feriadosDto = await _feriadoService.ObterFeriadosAsync(anoAtual);

            // 2. Buscar eventos da base de dados
            var eventos = await _context.Eventos.ToListAsync();

            // 3. Buscar aniversariantes da base de dados
            var aniversariantes = await _context.Aniversariantes.ToListAsync();

            var itensCalendario = new List<CalendarioItemViewModel>();

            // Mapear feriados
            foreach (var f in feriadosDto)
            {
                if (DateTime.TryParse(f.Date, out var dataFeriado))
                {
                    itensCalendario.Add(new CalendarioItemViewModel
                    {
                        Titulo = f.Name,
                        Data = dataFeriado,
                        Categoria = "Feriado",
                        Local = "Nacional"
                    });
                }
            }

            // Mapear eventos
            foreach (var ev in eventos)
            {
                itensCalendario.Add(new CalendarioItemViewModel
                {
                    Titulo = ev.Titulo,
                    Data = ev.DataHora,
                    Categoria = "Evento da Igreja",
                    Local = ev.Local
                });
            }

            // Mapear aniversariantes para o ano atual
            foreach (var aniv in aniversariantes)
            {
                var dataAniv = new DateTime(anoAtual, aniv.DataNascimento.Month, aniv.DataNascimento.Day);
                itensCalendario.Add(new CalendarioItemViewModel
                {
                    Titulo = $"Aniversário: {aniv.Nome}",
                    Data = dataAniv,
                    Categoria = "Aniversário",
                    Local = "-"
                });
            }

            // Ordenar cronologicamente
            itensCalendario = itensCalendario.OrderBy(x => x.Data).ToList();

            return View(itensCalendario);
        }
    }
}