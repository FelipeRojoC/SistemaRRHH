using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaArriendos.Models;

namespace SistemaArriendos.Pages.Clientes
{
    public class CrearModel : PageModel
    {
        private readonly ArriendosMantencionesDbContext _context;

        public CrearModel(ArriendosMantencionesDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            Cliente = new Cliente();
            return Page();
        }

        [BindProperty]
        public Cliente Cliente { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Cliente.Arriendos");

            if (!ModelState.IsValid || Cliente == null)
            {
                return Page();
            }

            var existente = await _context.Clientes.FindAsync(Cliente.Rut);
            if (existente != null)
            {
                ModelState.AddModelError("Cliente.Rut", "Ya existe un cliente con este RUT.");
                return Page();
            }

            _context.Clientes.Add(Cliente);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Cliente {Cliente.Nombre} registrado correctamente.";
            return RedirectToPage("./Index");
        }
    }
}
