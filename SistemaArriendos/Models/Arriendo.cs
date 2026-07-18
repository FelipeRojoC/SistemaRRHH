using System;
using System.Collections.Generic;

namespace SistemaArriendos.Models;

public partial class Arriendo
{
    public int id { get; set; }

    public string codigoVehiculo { get; set; } = null!;

    public string rutCliente { get; set; } = null!;

    public DateTime fechaInicio { get; set; }

    public DateTime fechaFin { get; set; }

    public int precioDiario { get; set; }

    public int precioTotal { get; set; }

    public string estado { get; set; } = null!;

    public virtual Cliente rutClienteNavigation { get; set; } = null!;
}
