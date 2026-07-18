using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaArriendos.Models;
using SistemaArriendos.Protos;

namespace SistemaArriendos.Pages;

public class IndexModel : PageModel
{
    private readonly ArriendosMantencionesDbContext _contextoDb;
    private readonly ServicioMantencion.ServicioMantencionClient _clienteGrpc;

    public IndexModel(ArriendosMantencionesDbContext contextoDb, ServicioMantencion.ServicioMantencionClient clienteGrpc)
    {
        _contextoDb = contextoDb;
        _clienteGrpc = clienteGrpc;
    }

    public int vehiculosDisponibles { get; set; }
    public int arriendosActivos { get; set; }
    public int totalClientes { get; set; }

    public async Task OnGetAsync()
    {
        totalClientes = await _contextoDb.clientes.CountAsync();
        arriendosActivos = await _contextoDb.arriendos.CountAsync(a => a.estado == "Activo");

        try
        {
            var respuestaVehiculos = await _clienteGrpc.obtieneVehiculosAsync(new ObtieneVehiculosPeticion());
            if (respuestaVehiculos != null)
            {
                int count = 0;
                foreach (var v in respuestaVehiculos.Vehiculos)
                {
                    if (v.Estado == "Activo")
                    {
                        count++;
                    }
                }
                vehiculosDisponibles = count;
            }
        }
        catch
        {
            vehiculosDisponibles = 0;
        }
    }
}
