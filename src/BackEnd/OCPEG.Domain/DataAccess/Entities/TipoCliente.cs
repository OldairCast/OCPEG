namespace OCPEG.Domain.DataAccess.Entities
{
    public partial class TipoCliente
    {
        public TipoCliente()
        {
            Clientes = new HashSet<Cliente>();
        }

        public int Id { get; set; }
        public string? Nome { get; set; }

        public virtual ICollection<Cliente> Clientes { get; set; }
    }
}
