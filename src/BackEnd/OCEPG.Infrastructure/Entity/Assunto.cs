using System;
using System.Collections.Generic;

namespace OCEPG.Infrastructure.Entity;

public partial class Assunto
{
    public int Id { get; set; }

    public string? Descricao { get; set; }

    public virtual ICollection<Atendimento> Atendimentos { get; set; } = new List<Atendimento>();
}
