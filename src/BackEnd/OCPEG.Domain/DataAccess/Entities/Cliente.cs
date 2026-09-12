namespace OCPEG.Domain.DataAccess.Entities
{
    public partial class Cliente
    {
        public Cliente()
        {
            Atendimentos = new HashSet<Atendimento>();
        }

        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public short? Idade { get; set; }
        public DateTime? DataInscricao { get; set; }
        public string? Texto { get; set; }
        public decimal? Mensalidade { get; set; }
        public int? TipoId { get; set; }

        public virtual TipoCliente? Tipo { get; set; }

        public virtual ICollection<Atendimento> Atendimentos { get; set; }

    }
}
