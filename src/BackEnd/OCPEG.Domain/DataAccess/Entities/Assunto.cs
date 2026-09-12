namespace OCPEG.Domain.DataAccess.Entities
{
    public partial class Assunto
    {
        public Assunto()
        {
            Atendimentos = new HashSet<Atendimento>();
        }

        public int Id { get; set; }
        public string? Descricao { get; set; }

        public virtual ICollection<Atendimento> Atendimentos { get; set; }
    }
}
