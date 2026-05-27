using System;
using System.Collections.Generic;

namespace SistemaRRHH.Models;

public partial class Mecanico
{
    public string Rut { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public int SueldoBase { get; set; }

    public int ValorHora { get; set; }
}
