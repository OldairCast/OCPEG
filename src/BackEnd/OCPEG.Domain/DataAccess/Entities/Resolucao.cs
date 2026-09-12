namespace OCPEG.Domain.DataAccess.Entities
{
    public partial class Resolucao
    {
        public int Protocolo { get; set; }
        public DateTime? DataSolucao { get; set; }
        public string? Comentario { get; set; }
        public int UsuarioId { get; set; }

        public virtual Atendimento ProtocoloNavigation { get; set; } = null!;
    }
}
