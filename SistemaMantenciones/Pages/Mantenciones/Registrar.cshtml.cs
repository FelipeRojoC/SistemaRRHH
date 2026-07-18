using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaMantenciones.Models;

namespace SistemaMantenciones.Pages.Mantenciones
{
    public class RegistrarModel : PageModel
    {
        private readonly ArriendosMantencionesDbContext _contextoDb;

        public RegistrarModel(ArriendosMantencionesDbContext contextoDb)
        {
            _contextoDb = contextoDb;
        }

        public SelectList listaVehiculos { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var vehiculos = await _contextoDb.vehiculos
                .Where(v => v.estado != "De Baja")
                .Select(v => new { v.codigo, nombre = $"{v.codigo} - {v.marca} {v.modelo} ({v.patente})" })
                .ToListAsync();

            listaVehiculos = new SelectList(vehiculos, "codigo", "nombre");

            mantenicion = new Mantenicion
            {
                fecha = DateTime.Now
            };

            return Page();
        }

        [BindProperty]
        public Mantenicion mantenicion { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("mantenicion.codigoVehiculoNavigation");

            if (!ModelState.IsValid || _contextoDb.manteniciones == null || mantenicion == null)
            {
                var vehiculos = await _contextoDb.vehiculos
                    .Where(v => v.estado != "De Baja")
                    .Select(v => new { v.codigo, nombre = $"{v.codigo} - {v.marca} {v.modelo} ({v.patente})" })
                    .ToListAsync();
                listaVehiculos = new SelectList(vehiculos, "codigo", "nombre");
                return Page();
            }

            _contextoDb.manteniciones.Add(mantenicion);

            var v = await _contextoDb.vehiculos.FindAsync(mantenicion.codigoVehiculo);
            if (v != null)
            {
                v.estado = "En Mantenicion";
                _contextoDb.Attach(v).State = EntityState.Modified;
            }

            await _contextoDb.SaveChangesAsync();

            TempData["SuccessMessage"] = "La mantenicion ha sido registrada con exito y el vehiculo se ha marcado En Mantenicion";

            return RedirectToPage("./Registrar");
        }
    }
}
