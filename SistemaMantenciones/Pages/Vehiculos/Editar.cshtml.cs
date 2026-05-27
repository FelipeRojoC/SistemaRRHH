using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaMantenciones.Models;

namespace SistemaMantenciones.Pages.Vehiculos
{
    public class EditarModel : PageModel
    {
        private readonly ArriendosMantencionesDbContext _context;

        public EditarModel(ArriendosMantencionesDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Vehiculo Vehiculo { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.Vehiculos == null)
            {
                return NotFound();
            }

            var vehiculo = await _context.Vehiculos.FirstOrDefaultAsync(m => m.Codigo == id);
            if (vehiculo == null)
            {
                return NotFound();
            }
            Vehiculo = vehiculo;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Validar que la patente no esté duplicada en otro vehículo
            var patenteDuplicada = await _context.Vehiculos.AnyAsync(v => v.Patente == Vehiculo.Patente && v.Codigo != Vehiculo.Codigo);
            if (patenteDuplicada)
            {
                ModelState.AddModelError("Vehiculo.Patente", "La patente ya se encuentra asignada a otro vehículo.");
                return Page();
            }

            _context.Attach(Vehiculo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehiculoExists(Vehiculo.Codigo))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool VehiculoExists(string id)
        {
          return (_context.Vehiculos?.Any(e => e.Codigo == id)).GetValueOrDefault();
        }
    }
}
