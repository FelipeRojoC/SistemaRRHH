using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaArriendos.Models;

namespace SistemaArriendos.Pages;

public class IndexModel : PageModel
{
    private readonly ArriendosMantencionesDbContext _context;

    public IndexModel(ArriendosMantencionesDbContext context)
    {
        _context = context;
    }

    public int VehiculosDisponibles { get; set; }
    public int ArriendosActivos { get; set; }
    public int TotalClientes { get; set; }

    public async Task OnGetAsync()
    {
        VehiculosDisponibles = await _context.Vehiculos.CountAsync(v => v.Estado == "Activo");
        ArriendosActivos = await _context.Arriendos.CountAsync(a => a.FechaFin >= DateTime.Now);
        TotalClientes = await _context.Clientes.CountAsync();
    }
}
