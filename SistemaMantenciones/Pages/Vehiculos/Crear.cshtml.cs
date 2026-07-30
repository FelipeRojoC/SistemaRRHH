using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaMantenciones.Messaging;
using SistemaMantenciones.Models;

namespace SistemaMantenciones.Pages.Vehiculos
{
    public class CrearModel : PageModel
    {
        private readonly ArriendosMantencionesDbContext _contextoDb;
        private readonly PublicadorVehiculo _publicador;

        public CrearModel(ArriendosMantencionesDbContext contextoDb, PublicadorVehiculo publicador)
        {
            _contextoDb = contextoDb;
            _publicador = publicador;
        }

        public IActionResult OnGet()
        {
            vehiculo = new Vehiculo
            {
                estado = "Activo"
            };
            return Page();
        }

        [BindProperty]
        public Vehiculo vehiculo { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("vehiculo.manteniciones");

            if (!ModelState.IsValid || _contextoDb.vehiculos == null || vehiculo == null)
            {
                return Page();
            }

            var existente = await _contextoDb.vehiculos.FindAsync(vehiculo.codigo);
            if (existente != null)
            {
                ModelState.AddModelError("vehiculo.codigo", "El codigo del vehiculo ya se encuentra registrado.");
                return Page();
            }

            var patenteExistente = await _contextoDb.vehiculos.AnyAsync(v => v.patente == vehiculo.patente);
            if (patenteExistente)
            {
                ModelState.AddModelError("vehiculo.patente", "La patente del vehiculo ya se encuentra registrada.");
                return Page();
            }

            _contextoDb.vehiculos.Add(vehiculo);
            await _contextoDb.SaveChangesAsync();

            _publicador.Publicar(new VehiculoMensaje(
                vehiculo.codigo, vehiculo.patente, vehiculo.marca, vehiculo.modelo,
                vehiculo.tipo, vehiculo.kilometraje, vehiculo.estado, vehiculo.precioArriendoDiario));

            return RedirectToPage("./Index");
        }
    }
}
