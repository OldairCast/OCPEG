namespace OCPEG.Domain.DataAccess.Entities
{
    public partial class Usuario
    {
        public Usuario()
        {
            Atendimentos = new HashSet<Atendimento>();
        }

        public int Id { get; set; }
        public string? Nome { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        
        public string? Telefone { get; set; } = string.Empty;
        public string? PasswordHash { get; set; } = string.Empty;
        public bool Active { get; set; } = true;
        public bool Administrador { get; set; } = false;
        public short Funcao { get; set; }
        public string? RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; set; }
        public Guid UserIdentifier { get; set; } = Guid.NewGuid();

        public virtual ICollection<Atendimento> Atendimentos { get; set; }
    }

}
