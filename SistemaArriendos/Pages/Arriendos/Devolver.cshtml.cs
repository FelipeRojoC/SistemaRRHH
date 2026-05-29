using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaArriendos.Models;

namespace SistemaArriendos.Pages.Arriendos
{
    public class DevolverModel : PageModel
    {
        private readonly ArriendosMantencionesDbContext _context;

        public DevolverModel(ArriendosMantencionesDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            // Solo se opera via POST desde la lista de arriendos
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var arriendo = await _context.Arriendos
                .Include(a => a.CodigoVehiculoNavigation)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (arriendo == null)
            {
                TempData["ErrorMessage"] = "No se encontró el arriendo indicado.";
                return RedirectToPage("./Index");
            }

            var vehiculo = arriendo.CodigoVehiculoNavigation;

            if (vehiculo.Estado != "Arrendado")
            {
                TempData["ErrorMessage"] = $"No se puede devolver: el vehículo no está en estado 'Arrendado' (estado actual: {vehiculo.Estado}).";
                return RedirectToPage("./Index");
            }

            vehiculo.Estado = "Activo";
            _context.Attach(vehiculo).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Vehículo {vehiculo.Patente} devuelto correctamente. Ahora está nuevamente disponible para arriendo.";
            return RedirectToPage("./Index");
        }
    }
}
