using System;
using System.Collections.Generic;

namespace SistemaArriendos.Models;

public partial class Cliente
{
    public string rut { get; set; } = null!;

    public string nombre { get; set; } = null!;

    public string direccion { get; set; } = null!;

    public virtual ICollection<Arriendo> arriendos { get; set; } = new List<Arriendo>();
}
