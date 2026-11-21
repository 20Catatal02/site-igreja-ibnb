using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IBNB.Data;
using IBNB.Models;
using System.Diagnostics;

namespace IBNB.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Busca os próximos 3 eventos a partir de hoje
        var proximosEventos = await _context.Eventos
            .Where(e => e.DataHora >= DateTime.Today)
            .OrderBy(e => e.DataHora)
            .Take(3)
            .ToListAsync();

        return View(proximosEventos);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}