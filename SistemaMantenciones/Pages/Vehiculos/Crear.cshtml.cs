using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaMantenciones.Models;

namespace SistemaMantenciones.Pages.Vehiculos
{
    public class CrearModel : PageModel
    {
        private readonly ArriendosMantencionesDbContext _context;

        public CrearModel(ArriendosMantencionesDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            // Inicializamos el modelo con estado por defecto 'Activo'
            Vehiculo = new Vehiculo
            {
                Estado = "Activo"
            };
            return Page();
        }

        [BindProperty]
        public Vehiculo Vehiculo { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.Vehiculos == null || Vehiculo == null)
            {
                return Page();
            }

            // Validar que el código de vehículo no esté duplicado
            var existente = await _context.Vehiculos.FindAsync(Vehiculo.Codigo);
            if (existente != null)
            {
                ModelState.AddModelError("Vehiculo.Codigo", "El código del vehículo ya se encuentra registrado.");
                return Page();
            }

            // Validar que la patente no esté duplicada
            var patenteExistente = await _context.Vehiculos.AnyAsync(v => v.Patente == Vehiculo.Patente);
            if (patenteExistente)
            {
                ModelState.AddModelError("Vehiculo.Patente", "La patente del vehículo ya se encuentra registrada.");
                return Page();
            }

            _context.Vehiculos.Add(Vehiculo);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
