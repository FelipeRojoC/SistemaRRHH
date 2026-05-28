using System;
using System.Collections.Generic;

namespace SistemaArriendos.Models;

public partial class Arriendo
{
    public int Id { get; set; }

    public string CodigoVehiculo { get; set; } = null!;

    public string RutCliente { get; set; } = null!;

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public int PrecioDiario { get; set; }

    public int PrecioTotal { get; set; }

    public virtual Vehiculo CodigoVehiculoNavigation { get; set; } = null!;

    public virtual Cliente RutClienteNavigation { get; set; } = null!;
}
