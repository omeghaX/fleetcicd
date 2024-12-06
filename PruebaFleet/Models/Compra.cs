using System;
using System.Collections.Generic;

namespace PruebaFleet.Models;

public partial class Compra
{
    public Guid CompId { get; set; }

    public string CompProducto { get; set; } = null!;

    public string CompCategoria { get; set; } = null!;

    public string CompTipoGarantia { get; set; } = null!;

    public Guid CompUsuarioGuid { get; set; }

    public DateTime CompDCreate { get; set; }

    public byte CompStatus { get; set; }
}
