
namespace OCPEG.Domain.BusinessObject
{
    public class CustomerImage
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public required string Nome { get; set; }
        public required string ContentType { get; set; }
        public required byte[] Dados { get; set; }
        public DateTime DataUpload { get; set; }
    }
}
