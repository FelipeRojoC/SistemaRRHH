using System;
using System.Collections.Generic;

namespace SistemaMantenciones.Models;

public partial class Cliente
{
    public string Rut { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public virtual ICollection<Arriendo> Arriendos { get; set; } = new List<Arriendo>();
}
