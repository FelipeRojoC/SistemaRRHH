using System;
using System.Collections.Generic;

namespace SistemaArriendos.Models;

public partial class Vehiculo
{
    public string Codigo { get; set; } = null!;

    public string Patente { get; set; } = null!;

    public string Marca { get; set; } = null!;

    public string Modelo { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public int Kilometraje { get; set; }

    public string Estado { get; set; } = null!;

    public int PrecioArriendoDiario { get; set; }

    public virtual ICollection<Arriendo> Arriendos { get; set; } = new List<Arriendo>();

    public virtual ICollection<Mantencion> Mantencions { get; set; } = new List<Mantencion>();
}
