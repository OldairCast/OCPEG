using System;
using System.Collections.Generic;

namespace OCEPG.Infrastructure.Entity;

public partial class Usuario
{
    public int Id { get; set; }

    public string? Nome { get; set; }

    public string? Email { get; set; }

    public string? Telefone { get; set; }

    public string? PasswordHash { get; set; }

    public bool? Active { get; set; }

    public bool? Administrador { get; set; }

    public short? Funcao { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    public Guid? UserIdentifier { get; set; }

    public virtual ICollection<Atendimento> Atendimentos { get; set; } = new List<Atendimento>();
}
