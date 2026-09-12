using OCPEG.Domain.BusinessObject.Common;

namespace OCPEG.Domain.BusinessObject.Message
{
    public class EmailHeaderVO : BaseMessage
    {
        public int CallNumber { get; set; }

        public DateTime? Date { get; set; }

        public string? Comment { get; set; }

        public string? CustomerName { get; set; }

        public string? CustomerEmail { get; set; }
    }

}
