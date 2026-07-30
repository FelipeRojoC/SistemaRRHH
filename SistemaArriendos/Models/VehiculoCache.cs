namespace SistemaArriendos.Models;

// Copia local de solo lectura de los vehiculos, alimentada por los mensajes de cola_mantencion.
// Reemplaza las consultas gRPC a Mantenciones: Arriendos ya no pregunta nada por RPC.
public class VehiculoCache
{
    public string codigo { get; set; } = null!;
    public string patente { get; set; } = null!;
    public string marca { get; set; } = null!;
    public string modelo { get; set; } = null!;
    public string tipo { get; set; } = null!;
    public int kilometraje { get; set; }
    public string estado { get; set; } = null!;
    public int precioArriendoDiario { get; set; }
}
