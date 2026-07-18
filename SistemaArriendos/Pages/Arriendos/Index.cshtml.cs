using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaArriendos.Models;
using SistemaArriendos.Protos;

namespace SistemaArriendos.Pages.Arriendos
{
    public class IndexModel : PageModel
    {
        private readonly ArriendosMantencionesDbContext _contextoDb;
        private readonly ServicioMantencion.ServicioMantencionClient _clienteGrpc;

        public IndexModel(ArriendosMantencionesDbContext contextoDb, ServicioMantencion.ServicioMantencionClient clienteGrpc)
        {
            _contextoDb = contextoDb;
            _clienteGrpc = clienteGrpc;
        }

        public IList<Arriendo> listaArriendos { get; set; } = default!;
        public Dictionary<string, VehiculoRespuesta> mapaVehiculos { get; set; } = new();

        public async Task OnGetAsync()
        {
            listaArriendos = await _contextoDb.arriendos
                .Include(a => a.rutClienteNavigation)
                .OrderByDescending(a => a.id)
                .ToListAsync();

            try
            {
                var respuestaVehiculos = await _clienteGrpc.obtieneVehiculosAsync(new ObtieneVehiculosPeticion());
                if (respuestaVehiculos != null)
                {
                    foreach (var v in respuestaVehiculos.Vehiculos)
                    {
                        mapaVehiculos[v.Codigo] = v;
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"No se pudo obtener informacion de los vehiculos desde el servicio gRPC: {ex.Message}";
            }
        }

        public async Task<IActionResult> OnPostCerrarArriendoAsync(int id)
        {
            var arriendo = await _contextoDb.arriendos.FindAsync(id);
            if (arriendo == null)
            {
                return NotFound();
            }

            arriendo.estado = "Cerrado";
            _contextoDb.Entry(arriendo).State = EntityState.Modified;

            try
            {
                // Cambiar el estado del vehiculo en el sistema de mantenciones a "Activo"
                var respuestaGrpc = await _clienteGrpc.cambiaEstadoVehiculoAsync(new CambiaEstadoVehiculoPeticion
                {
                    Id = arriendo.codigoVehiculo,
                    Estado = "Activo"
                });

                if (!respuestaGrpc.Exito)
                {
                    TempData["ErrorMessage"] = $"El arriendo se cerro localmente, pero no se pudo cambiar el estado del vehiculo en mantenciones: {respuestaGrpc.Mensaje}";
                }
                else
                {
                    TempData["SuccessMessage"] = "Arriendo cerrado exitosamente y vehiculo devuelto a estado Activo.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"El arriendo se cerro localmente, pero fallo la conexion gRPC: {ex.Message}";
            }

            await _contextoDb.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
