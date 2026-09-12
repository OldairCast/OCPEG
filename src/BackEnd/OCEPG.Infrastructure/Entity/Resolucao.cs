using System;
using System.Collections.Generic;

namespace OCEPG.Infrastructure.Entity;

public partial class Resolucao
{
    public int Protocolo { get; set; }

    public DateTime? DataSolucao { get; set; }

    public string? Comentario { get; set; }

    public int UsuarioId { get; set; }

    public virtual Atendimento ProtocoloNavigation { get; set; } = null!;
}
