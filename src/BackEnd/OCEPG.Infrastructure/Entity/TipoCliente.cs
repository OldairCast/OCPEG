using System;
using System.Collections.Generic;

namespace OCEPG.Infrastructure.Entity;

public partial class TipoCliente
{
    public int Id { get; set; }

    public string? Nome { get; set; }

    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
}
