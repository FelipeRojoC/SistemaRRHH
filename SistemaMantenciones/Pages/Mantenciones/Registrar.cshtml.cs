using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using SistemaMantenciones.Models;

namespace SistemaMantenciones.Pages.Mantenciones
{
    public class RegistrarModel : PageModel
    {
        private readonly ArriendosMantencionesDbContext _context;

        public RegistrarModel(ArriendosMantencionesDbContext context)
        {
            _context = context;
        }

        public SelectList VehiculosList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            // Cargar vehículos para el select en la vista
            var vehiculos = await _context.Vehiculos
                .Where(v => v.Estado != "De Baja") // No se le hacen mantenciones a vehículos dados de baja
                .Select(v => new { v.Codigo, Nombre = $"{v.Codigo} - {v.Marca} {v.Modelo} ({v.Patente})" })
                .ToListAsync();

            VehiculosList = new SelectList(vehiculos, "Codigo", "Nombre");

            Mantencion = new Mantencion
            {
                Fecha = DateTime.Now
            };

            return Page();
        }

        [BindProperty]
        public Mantencion Mantencion { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            // 1. Remover el objeto de navegación de la validación ya que no es un input del formulario
            ModelState.Remove("Mantencion.CodigoVehiculoNavigation");

            if (!ModelState.IsValid || _context.Mantencions == null || Mantencion == null)
            {
                // Si falla, recargamos el select
                var vehiculos = await _context.Vehiculos
                    .Where(v => v.Estado != "De Baja")
                    .Select(v => new { v.Codigo, Nombre = $"{v.Codigo} - {v.Marca} {v.Modelo} ({v.Patente})" })
                    .ToListAsync();
                VehiculosList = new SelectList(vehiculos, "Codigo", "Nombre");
                return Page();
            }

            // 2. Validar dinámicamente si el RUT del mecánico existe en la base de datos de Recursos Humanos (rrhh_db)
            var connectionString = _context.Database.GetConnectionString();
            bool mecanicoExiste = false;

            if (!string.IsNullOrEmpty(connectionString))
            {
                // Reutilizamos el host, usuario y clave local del programador actual, cambiando solo la base de datos a rrhh_db
                var connectionBuilder = new MySqlConnectionStringBuilder(connectionString)
                {
                    Database = "rrhh_db"
                };

                try
                {
                    using (var conn = new MySqlConnection(connectionBuilder.ConnectionString))
                    {
                        await conn.OpenAsync();
                        using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM Mecanico WHERE Rut = @rut", conn))
                        {
                            cmd.Parameters.AddWithValue("@rut", Mantencion.RutMecanico);
                            var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                            mecanicoExiste = count > 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Mantencion.RutMecanico", $"No se pudo verificar el mecánico en Recursos Humanos: {ex.Message}");
                    
                    var vehiculos = await _context.Vehiculos
                        .Where(v => v.Estado != "De Baja")
                        .Select(v => new { v.Codigo, Nombre = $"{v.Codigo} - {v.Marca} {v.Modelo} ({v.Patente})" })
                        .ToListAsync();
                    VehiculosList = new SelectList(vehiculos, "Codigo", "Nombre");
                    return Page();
                }
            }

            if (!mecanicoExiste)
            {
                ModelState.AddModelError("Mantencion.RutMecanico", "El RUT ingresado no corresponde a ningún mecánico registrado en el sistema de Recursos Humanos.");
                
                var vehiculos = await _context.Vehiculos
                    .Where(v => v.Estado != "De Baja")
                    .Select(v => new { v.Codigo, Nombre = $"{v.Codigo} - {v.Marca} {v.Modelo} ({v.Patente})" })
                    .ToListAsync();
                VehiculosList = new SelectList(vehiculos, "Codigo", "Nombre");
                return Page();
            }

            // Registrar la mantención
            _context.Mantencions.Add(Mantencion);

            // Regla de Negocio: Al ingresar una mantención, el vehículo pasa a estado "En Mantencion" automáticamente
            var vehiculo = await _context.Vehiculos.FindAsync(Mantencion.CodigoVehiculo);
            if (vehiculo != null)
            {
                vehiculo.Estado = "En Mantencion";
                _context.Attach(vehiculo).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "¡La mantención ha sido registrada con éxito y el vehículo se ha marcado 'En Mantención'!";

            return RedirectToPage("./Registrar");
        }
    }
}
