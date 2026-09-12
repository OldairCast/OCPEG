using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.BusinessObject.Common;
using static OCPEG.Domain.Enum.Enums;

namespace OCPEG.Domain.Dto
{
    public class CallDto
    {
        public string Id { get; set; } = string.Empty;
        
        public string CallNumber { get; set; } = string.Empty;

        public DateTime? StartDate { get; set; }

        public DateTime? FinishDate { get; set; }

        public ComboOption? Customer { get; set; }

        public Product? Product { get; set; }

        public PriorityEn Priority { get; set; }

        public string Comment { get; set; } = string.Empty;

        public StatusEn Status { get; set; }

        public ComboOption? User { get; set; }
    }
}
