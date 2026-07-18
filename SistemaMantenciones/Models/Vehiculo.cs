using System;
using System.Collections.Generic;

namespace SistemaMantenciones.Models;

public partial class Vehiculo
{
    public string codigo { get; set; } = null!;

    public string patente { get; set; } = null!;

    public string marca { get; set; } = null!;

    public string modelo { get; set; } = null!;

    public string tipo { get; set; } = null!;

    public int kilometraje { get; set; }

    public string estado { get; set; } = null!;

    public int precioArriendoDiario { get; set; }

    public virtual ICollection<Mantenicion> manteniciones { get; set; } = new List<Mantenicion>();
}
