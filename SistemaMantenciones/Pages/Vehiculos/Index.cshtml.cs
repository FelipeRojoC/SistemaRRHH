using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaMantenciones.Models;

namespace SistemaMantenciones.Pages.Vehiculos
{
    public class IndexModel : PageModel
    {
        private readonly ArriendosMantencionesDbContext _contextoDb;

        public IndexModel(ArriendosMantencionesDbContext contextoDb)
        {
            _contextoDb = contextoDb;
        }

        public IList<Vehiculo> listaVehiculos { get; set; } = default!;

        public async Task onGetAsync()
        {
            if (_contextoDb.vehiculos != null)
            {
                listaVehiculos = await _contextoDb.vehiculos.ToListAsync();
            }
        }

        public async Task<IActionResult> onPostCambiaEstadoAsync(string id, string nuevoEstado)
        {
            if (id == null || _contextoDb.vehiculos == null)
            {
                return NotFound();
            }

            var v = await _contextoDb.vehiculos.FindAsync(id);
            if (v != null)
            {
                v.estado = nuevoEstado;
                _contextoDb.Entry(v).State = EntityState.Modified;
                await _contextoDb.SaveChangesAsync();
                TempData["SuccessMessage"] = "Estado del vehiculo actualizado correctamente.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> onPostDeleteAsync(string id)
        {
            if (id == null || _contextoDb.vehiculos == null)
            {
                return NotFound();
            }

            var v = await _contextoDb.vehiculos.FindAsync(id);
            if (v != null)
            {
                _contextoDb.vehiculos.Remove(v);
                await _contextoDb.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
