using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaArriendos.Models;
using SistemaArriendos.Protos;

namespace SistemaArriendos.Pages.Arriendos
{
    public class CrearModel : PageModel
    {
        private readonly ArriendosMantencionesDbContext _contextoDb;
        private readonly ServicioMantencion.ServicioMantencionClient _clienteGrpc;

        public CrearModel(ArriendosMantencionesDbContext contextoDb, ServicioMantencion.ServicioMantencionClient clienteGrpc)
        {
            _contextoDb = contextoDb;
            _clienteGrpc = clienteGrpc;
        }

        public SelectList listaVehiculosDisponibles { get; set; } = default!;
        public SelectList listaClientes { get; set; } = default!;

        [BindProperty]
        public Arriendo arriendo { get; set; } = default!;

        public async Task<IActionResult> onGetAsync()
        {
            await cargarListasAsync();

            arriendo = new Arriendo
            {
                fechaInicio = DateTime.Today,
                fechaFin = DateTime.Today.AddDays(1),
                estado = "Activo"
            };

            return Page();
        }

        public async Task<IActionResult> onPostAsync()
        {
            ModelState.Remove("arriendo.rutClienteNavigation");

            if (!ModelState.IsValid || arriendo == null)
            {
                await cargarListasAsync();
                return Page();
            }

            if (arriendo.fechaFin <= arriendo.fechaInicio)
            {
                ModelState.AddModelError("arriendo.fechaFin", "La fecha de fin debe ser posterior a la fecha de inicio.");
                await cargarListasAsync();
                return Page();
            }

            // Obtener vehiculo desde el servicio gRPC
            VehiculoRespuesta? v = null;
            try
            {
                v = await _clienteGrpc.obtieneVehiculoAsync(new ObtieneVehiculoPeticion { Id = arriendo.codigoVehiculo });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("arriendo.codigoVehiculo", $"No se pudo obtener el vehiculo desde el servicio gRPC: {ex.Message}");
                await cargarListasAsync();
                return Page();
            }

            if (v == null)
            {
                ModelState.AddModelError("arriendo.codigoVehiculo", "El vehiculo seleccionado no existe.");
                await cargarListasAsync();
                return Page();
            }

            if (v.Estado != "Activo")
            {
                ModelState.AddModelError("arriendo.codigoVehiculo", $"No se puede arrendar este vehiculo: su estado actual es '{v.Estado}'. Solo se permiten vehiculos en estado 'Activo'.");
                await cargarListasAsync();
                return Page();
            }

            var c = await _contextoDb.clientes.FindAsync(arriendo.rutCliente);
            if (c == null)
            {
                ModelState.AddModelError("arriendo.rutCliente", "El cliente seleccionado no existe.");
                await cargarListasAsync();
                return Page();
            }

            int dias = (int)Math.Ceiling((arriendo.fechaFin - arriendo.fechaInicio).TotalDays);
            if (dias < 1) dias = 1;

            arriendo.precioDiario = v.PrecioArriendoDiario;
            arriendo.precioTotal = v.PrecioArriendoDiario * dias;
            arriendo.estado = "Activo";

            // Cambiar el estado del vehiculo a "Arrendado" via gRPC
            try
            {
                var respuestaGrpc = await _clienteGrpc.cambiaEstadoVehiculoAsync(new CambiaEstadoVehiculoPeticion
                {
                    Id = arriendo.codigoVehiculo,
                    Estado = "Arrendado"
                });

                if (!respuestaGrpc.Exito)
                {
                    ModelState.AddModelError("arriendo.codigoVehiculo", $"Fallo al cambiar el estado del vehiculo via gRPC: {respuestaGrpc.Mensaje}");
                    await cargarListasAsync();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("arriendo.codigoVehiculo", $"Fallo la conexion gRPC para cambiar el estado del vehiculo: {ex.Message}");
                await cargarListasAsync();
                return Page();
            }

            _contextoDb.arriendos.Add(arriendo);
            await _contextoDb.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Arriendo registrado con exito por {dias} dia(s). Total: ${arriendo.precioTotal:N0}";
            return RedirectToPage("./Index");
        }

        private async Task cargarListasAsync()
        {
            var disponibles = new List<object>();
            try
            {
                var respuestaVehiculos = await _clienteGrpc.obtieneVehiculosAsync(new ObtieneVehiculosPeticion());
                if (respuestaVehiculos != null)
                {
                    foreach (var v in respuestaVehiculos.Vehiculos)
                    {
                        if (v.Estado == "Activo")
                        {
                            disponibles.Add(new
                            {
                                codigo = v.Codigo,
                                nombre = $"{v.Codigo} - {v.Marca} {v.Modelo} ({v.Patente}) - ${v.PrecioArriendoDiario}/dia"
                            });
                        }
                    }
                }
            }
            catch
            {
                // Manejar error de conexion
            }

            listaVehiculosDisponibles = new SelectList(disponibles, "codigo", "nombre");

            var clientes = await _contextoDb.clientes
                .OrderBy(c => c.nombre)
                .Select(c => new { rut = c.rut, nombre = $"{c.nombre} ({c.rut})" })
                .ToListAsync();

            listaClientes = new SelectList(clientes, "rut", "nombre");
        }
    }
}
