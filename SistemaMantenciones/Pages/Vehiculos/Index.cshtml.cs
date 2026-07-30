using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaMantenciones.Messaging;
using SistemaMantenciones.Models;

namespace SistemaMantenciones.Pages.Vehiculos
{
    public class IndexModel : PageModel
    {
        private readonly ArriendosMantencionesDbContext _contextoDb;
        private readonly PublicadorVehiculo _publicador;

        public IndexModel(ArriendosMantencionesDbContext contextoDb, PublicadorVehiculo publicador)
        {
            _contextoDb = contextoDb;
            _publicador = publicador;
        }

        public IList<Vehiculo> listaVehiculos { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_contextoDb.vehiculos != null)
            {
                listaVehiculos = await _contextoDb.vehiculos.ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostCambiaEstadoAsync(string id, string nuevoEstado)
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

                _publicador.Publicar(new VehiculoMensaje(
                    v.codigo, v.patente, v.marca, v.modelo, v.tipo, v.kilometraje, v.estado, v.precioArriendoDiario));

                TempData["SuccessMessage"] = "Estado del vehiculo actualizado correctamente.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
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
