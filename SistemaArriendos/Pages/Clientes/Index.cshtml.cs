using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaArriendos.Models;

namespace SistemaArriendos.Pages.Clientes
{
    public class IndexModel : PageModel
    {
        private readonly ArriendosMantencionesDbContext _context;

        public IndexModel(ArriendosMantencionesDbContext context)
        {
            _context = context;
        }

        public IList<Cliente> Clientes { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Clientes = await _context.Clientes
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .Include(c => c.Arriendos)
                .FirstOrDefaultAsync(c => c.Rut == id);

            if (cliente == null)
            {
                return NotFound();
            }

            // No permitir eliminar clientes con arriendos vigentes
            if (cliente.Arriendos.Any(a => a.FechaFin >= DateTime.Now))
            {
                TempData["ErrorMessage"] = $"No se puede eliminar al cliente {cliente.Nombre} porque tiene arriendos vigentes.";
                return RedirectToPage();
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Cliente {cliente.Nombre} eliminado correctamente.";
            return RedirectToPage();
        }
    }
}
