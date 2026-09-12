namespace OCPEG.Domain.DataAccess.Entities
{
    public partial class Atendimento
    {
        public int Protocolo { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataConclusao { get; set; }
        public string? Comentario { get; set; }
        public int? IdCliente { get; set; }
        public int? IdAssunto { get; set; }
        public byte? Prioridade { get; set; }
        public byte? StatusAtend { get; set; }
        public int? UsuarioId { get; set; }

        public virtual Assunto? IdAssuntoNavigation { get; set; }
        public virtual Cliente? IdClienteNavigation { get; set; }
        public virtual Resolucao? Resolucao { get; set; }
    }
}
