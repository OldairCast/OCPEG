using OCPEG.Domain.Dto;

namespace OCPEG.Domain.BusinessObject.Common
{
    public class Message
    {
        public int CallNumber { get; set; }
        public DateTime? Date { get; set; }

        public CustomerDto? Customer { get; set; }

        public string? Comment { get; set; }
    }
}
