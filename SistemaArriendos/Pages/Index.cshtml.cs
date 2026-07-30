using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaArriendos.Models;

namespace SistemaArriendos.Pages;

public class IndexModel : PageModel
{
    private readonly ArriendosMantencionesDbContext _contextoDb;

    public IndexModel(ArriendosMantencionesDbContext contextoDb)
    {
        _contextoDb = contextoDb;
    }

    public int vehiculosDisponibles { get; set; }
    public int arriendosActivos { get; set; }
    public int totalClientes { get; set; }

    public async Task OnGetAsync()
    {
        totalClientes = await _contextoDb.clientes.CountAsync();
        arriendosActivos = await _contextoDb.arriendos.CountAsync(a => a.estado == "Activo");
        vehiculosDisponibles = await _contextoDb.vehiculosCache.CountAsync(v => v.estado == "Activo");
    }
}
