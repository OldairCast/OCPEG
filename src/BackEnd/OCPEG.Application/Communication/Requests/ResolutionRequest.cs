using OCPEG.Domain.BusinessObject.Common;

namespace OCPEG.Application.Communication.Requests
{
    public class ResolutionRequest
    {
        public int CallNumber { get; set; }
        public DateTime? Date { get; set; }
        public string? Comment { get; set; }
        public ComboOption? User { get; set; }
    }
}
