using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.BusinessObject.Common;
using static OCPEG.Domain.Enum.Enums;

namespace OCPEG.Application.Communication.Responses
{
    public class CallResponse
    {
        public int CallNumber { get; set; }

        public DateTime? PreviousDate { get; set; }

        public ComboOption? Customer { get; set; }

        public Product? Product { get; set; }

        public PriorityEn Priority { get; set; }

        public string? Comment { get; set; }

        public StatusEn Status { get; set; }
    }
}
