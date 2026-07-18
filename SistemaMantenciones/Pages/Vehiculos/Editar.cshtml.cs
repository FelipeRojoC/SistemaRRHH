using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaMantenciones.Models;

namespace SistemaMantenciones.Pages.Vehiculos
{
    public class EditarModel : PageModel
    {
        private readonly ArriendosMantencionesDbContext _contextoDb;

        public EditarModel(ArriendosMantencionesDbContext contextoDb)
        {
            _contextoDb = contextoDb;
        }

        [BindProperty]
        public Vehiculo vehiculo { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _contextoDb.vehiculos == null)
            {
                return NotFound();
            }

            var v = await _contextoDb.vehiculos.FirstOrDefaultAsync(m => m.codigo == id);
            if (v == null)
            {
                return NotFound();
            }
            vehiculo = v;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("vehiculo.manteniciones");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var patenteDuplicada = await _contextoDb.vehiculos.AnyAsync(v => v.patente == vehiculo.patente && v.codigo != vehiculo.codigo);
            if (patenteDuplicada)
            {
                ModelState.AddModelError("vehiculo.patente", "La patente ya se encuentra asignada a otro vehiculo.");
                return Page();
            }

            _contextoDb.Attach(vehiculo).State = EntityState.Modified;

            try
            {
                await _contextoDb.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!vehiculoExists(vehiculo.codigo))
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

        private bool vehiculoExists(string id)
        {
            return (_contextoDb.vehiculos?.Any(e => e.codigo == id)).GetValueOrDefault();
        }
    }
}
