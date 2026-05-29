using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaMantenciones.Models;

namespace SistemaMantenciones.Pages.Vehiculos
{
    public class IndexModel : PageModel
    {
        private readonly ArriendosMantencionesDbContext _context;

        public IndexModel(ArriendosMantencionesDbContext context)
        {
            _context = context;
        }

        public IList<Vehiculo> Vehiculos { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Vehiculos != null)
            {
                Vehiculos = await _context.Vehiculos.ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            if (id == null || _context.Vehiculos == null)
            {
                return NotFound();
            }

            var vehiculo = await _context.Vehiculos.FindAsync(id);

            if (vehiculo != null)
            {
                _context.Vehiculos.Remove(vehiculo);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
