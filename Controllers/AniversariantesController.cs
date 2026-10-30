using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IBNB.Data;
using IBNB.Models;
using IBNB.ViewModels;

namespace IBNB.Controllers
{
    [Authorize]
    public class AniversariantesController : Controller
    {
        private readonly AppDbContext _context;

        public AniversariantesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Aniversariantes
        public async Task<IActionResult> Index()
        {
            var aniversariantes = await _context.Aniversariantes
                .OrderBy(a => a.DataNascimento.Month)
                .ThenBy(a => a.DataNascimento.Day)
                .ToListAsync();
            return View(aniversariantes);
        }

        // GET: Retorna o HTML do Modal (Create/Edit)
        [HttpGet]
        public async Task<IActionResult> FormModal(int? id)
        {
            AniversarianteViewModel vm;

            if (id.HasValue)
            {
                var aniversariante = await _context.Aniversariantes.FindAsync(id.Value);
                if (aniversariante == null) return NotFound();

                vm = new AniversarianteViewModel
                {
                    Id = aniversariante.Id,
                    Nome = aniversariante.Nome,
                    DataNascimento = aniversariante.DataNascimento
                };
            }
            else
            {
                vm = new AniversarianteViewModel();
            }

            return PartialView("_FormModal", vm);
        }

        // POST: Salvar (Create ou Edit)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Salvar(AniversarianteViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // Retorna o modal com os erros de validação
                return PartialView("_FormModal", vm); 
            }

            if (vm.Id == 0)
            {
                // Create
                var novo = new Aniversariante
                {
                    Nome = vm.Nome,
                    DataNascimento = vm.DataNascimento
                };
                _context.Add(novo);
            }
            else
            {
                // Edit
                var existente = await _context.Aniversariantes.FindAsync(vm.Id);
                if (existente == null) return NotFound();

                existente.Nome = vm.Nome;
                existente.DataNascimento = vm.DataNascimento;
                _context.Update(existente);
            }

            await _context.SaveChangesAsync();

            // Retorna 200 OK com JSON para o front-end saber que deu certo
            return Json(new { success = true });
        }

        // POST: Excluir (Soft Delete)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Excluir(int id)
        {
            var aniversariante = await _context.Aniversariantes.FindAsync(id);
            if (aniversariante == null) return NotFound();

            _context.Aniversariantes.Remove(aniversariante); // O AppDbContext transforma isso em Soft Delete
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}