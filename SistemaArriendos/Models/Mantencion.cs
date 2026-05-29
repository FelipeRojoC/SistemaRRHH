using System;
using System.Collections.Generic;

namespace SistemaArriendos.Models;

public partial class Mantencion
{
    public int Id { get; set; }

    public string CodigoVehiculo { get; set; } = null!;

    public DateTime Fecha { get; set; }

    public string RutMecanico { get; set; } = null!;

    public int Horas { get; set; }

    public virtual Vehiculo CodigoVehiculoNavigation { get; set; } = null!;
}
