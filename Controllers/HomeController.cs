using IBNB.Models;
using IBNB.Services;
using IBNB.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IBNB.Controllers;

public class HomeController : Controller
{
    private readonly VersiculoDoDiaService _versiculoDoDia;

    public HomeController(VersiculoDoDiaService versiculoDoDia)
    {
        _versiculoDoDia = versiculoDoDia;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var versiculo = await _versiculoDoDia.ObterAsync(cancellationToken);

        var modelo = new HomeViewModel
        {
            Versiculo = versiculo is null ? null : new VersiculoDoDiaViewModel
            {
                Texto = versiculo.Texto,
                Referencia = versiculo.Referencia,
                Autor = versiculo.Autor,
                Traducao = versiculo.Traducao
            }
        };

        return View(modelo);
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
