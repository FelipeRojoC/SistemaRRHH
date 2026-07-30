using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaArriendos.Messaging;
using SistemaArriendos.Models;

namespace SistemaArriendos.Pages.Arriendos
{
    public class IndexModel : PageModel
    {
        private readonly ArriendosMantencionesDbContext _contextoDb;
        private readonly PublicadorArriendo _publicador;

        public IndexModel(ArriendosMantencionesDbContext contextoDb, PublicadorArriendo publicador)
        {
            _contextoDb = contextoDb;
            _publicador = publicador;
        }

        public IList<Arriendo> listaArriendos { get; set; } = default!;
        public Dictionary<string, VehiculoCache> mapaVehiculos { get; set; } = new();

        public async Task OnGetAsync()
        {
            listaArriendos = await _contextoDb.arriendos
                .Include(a => a.rutClienteNavigation)
                .OrderByDescending(a => a.id)
                .ToListAsync();

            mapaVehiculos = await _contextoDb.vehiculosCache.ToDictionaryAsync(v => v.codigo);
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
            await _contextoDb.SaveChangesAsync();

            // Avisa a Mantenciones via cola_arriendo para que devuelva el vehiculo a Activo.
            _publicador.Publicar(new ArriendoMensaje("ArriendoCerrado", arriendo.codigoVehiculo));

            TempData["SuccessMessage"] = "Arriendo cerrado exitosamente y vehiculo devuelto a estado Activo.";

            return RedirectToPage();
        }
    }
}
