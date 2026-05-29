using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaArriendos.Models;

namespace SistemaArriendos.Pages.Arriendos
{
    public class CrearModel : PageModel
    {
        private readonly ArriendosMantencionesDbContext _context;

        public CrearModel(ArriendosMantencionesDbContext context)
        {
            _context = context;
        }

        public SelectList VehiculosDisponiblesList { get; set; } = default!;
        public SelectList ClientesList { get; set; } = default!;

        [BindProperty]
        public Arriendo Arriendo { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            await CargarSelectsAsync();

            Arriendo = new Arriendo
            {
                FechaInicio = DateTime.Today,
                FechaFin = DateTime.Today.AddDays(1)
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Estos campos los completamos en el servidor
            ModelState.Remove("Arriendo.CodigoVehiculoNavigation");
            ModelState.Remove("Arriendo.RutClienteNavigation");
            ModelState.Remove("Arriendo.PrecioDiario");
            ModelState.Remove("Arriendo.PrecioTotal");

            if (!ModelState.IsValid || Arriendo == null)
            {
                await CargarSelectsAsync();
                return Page();
            }

            // 1. Validar fechas
            if (Arriendo.FechaFin <= Arriendo.FechaInicio)
            {
                ModelState.AddModelError("Arriendo.FechaFin", "La fecha de fin debe ser posterior a la fecha de inicio.");
                await CargarSelectsAsync();
                return Page();
            }

            // 2. Validar que el vehículo exista y esté en estado 'Activo'
            var vehiculo = await _context.Vehiculos.FindAsync(Arriendo.CodigoVehiculo);
            if (vehiculo == null)
            {
                ModelState.AddModelError("Arriendo.CodigoVehiculo", "El vehículo seleccionado no existe.");
                await CargarSelectsAsync();
                return Page();
            }

            // Regla de negocio: no se puede arrendar un vehículo Arrendado, En Mantencion o De Baja
            if (vehiculo.Estado != "Activo")
            {
                ModelState.AddModelError("Arriendo.CodigoVehiculo",
                    $"No se puede arrendar este vehículo: su estado actual es '{vehiculo.Estado}'. Solo se permiten vehículos en estado 'Activo'.");
                await CargarSelectsAsync();
                return Page();
            }

            // 3. Validar que el cliente exista
            var cliente = await _context.Clientes.FindAsync(Arriendo.RutCliente);
            if (cliente == null)
            {
                ModelState.AddModelError("Arriendo.RutCliente", "El cliente seleccionado no existe.");
                await CargarSelectsAsync();
                return Page();
            }

            // 4. Calcular precios desde el lado del servidor (no confiar en el cliente)
            int dias = (int)Math.Ceiling((Arriendo.FechaFin - Arriendo.FechaInicio).TotalDays);
            if (dias < 1) dias = 1;

            Arriendo.PrecioDiario = vehiculo.PrecioArriendoDiario;
            Arriendo.PrecioTotal = vehiculo.PrecioArriendoDiario * dias;

            // 5. Registrar arriendo y actualizar estado del vehículo
            _context.Arriendos.Add(Arriendo);

            vehiculo.Estado = "Arrendado";
            _context.Attach(vehiculo).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Arriendo registrado: {vehiculo.Marca} {vehiculo.Modelo} ({vehiculo.Patente}) para {cliente.Nombre}, total ${Arriendo.PrecioTotal:N0} por {dias} día(s).";
            return RedirectToPage("./Index");
        }

        private async Task CargarSelectsAsync()
        {
            var disponibles = await _context.Vehiculos
                .Where(v => v.Estado == "Activo")
                .OrderBy(v => v.Codigo)
                .Select(v => new
                {
                    v.Codigo,
                    Nombre = $"{v.Codigo} - {v.Marca} {v.Modelo} ({v.Patente}) — ${v.PrecioArriendoDiario}/día"
                })
                .ToListAsync();
            VehiculosDisponiblesList = new SelectList(disponibles, "Codigo", "Nombre");

            var clientes = await _context.Clientes
                .OrderBy(c => c.Nombre)
                .Select(c => new { c.Rut, Nombre = $"{c.Nombre} ({c.Rut})" })
                .ToListAsync();
            ClientesList = new SelectList(clientes, "Rut", "Nombre");
        }
    }
}
