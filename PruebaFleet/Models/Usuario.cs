using System;
using System.Collections.Generic;

namespace PruebaFleet.Models;

public partial class Usuario
{
    public int UserId { get; set; }

    public Guid UserGuid { get; set; }

    public string UserNombre { get; set; } = null!;

    public string UserPApellido { get; set; } = null!;

    public string UserSApellido { get; set; } = null!;

    public string UserEmail { get; set; } = null!;

    public int UserGenderId { get; set; }

    public int UserAge { get; set; }

    public string UserPass { get; set; } = null!;

    public DateTime UserDcreate { get; set; }

    public byte UserStatus { get; set; }
}
