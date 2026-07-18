using System;
using System.Collections.Generic;

namespace SistemaMantenciones.Models;

public partial class Mantenicion
{
    public int id { get; set; }

    public string codigoVehiculo { get; set; } = null!;

    public DateTime fecha { get; set; }

    public int horas { get; set; }

    public string descripcion { get; set; } = null!;

    public virtual Vehiculo codigoVehiculoNavigation { get; set; } = null!;
}
