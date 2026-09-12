
namespace OCPEG.Domain.DataAccess.Entities
{
    public partial class ClienteImagem
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public required string Descricao { get; set; }
        public required string ContentType { get; set; }
        public required byte[] Dados { get; set; }
        public DateTime DataUpload { get; set; }
    }
}
