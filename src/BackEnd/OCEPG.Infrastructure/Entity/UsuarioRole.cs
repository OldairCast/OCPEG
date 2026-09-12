using System;
using System.Collections.Generic;

namespace OCEPG.Infrastructure.Entity;

public partial class UsuarioRole
{
    public int Id { get; set; }

    public short RoleId { get; set; }

    public int UserId { get; set; }
}
