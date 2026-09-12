using OCPEG.Domain.BusinessObject.Common;
using static OCPEG.Domain.Enum.Enums;

namespace OCPEG.Domain.BusinessObject
{
    public class Call
    {
        public int CallNumber { get; set; }

        public int UserId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? FinishDate { get; set; }

        public DateTime? PreviousDate { get; set; }

        public ComboOption? Customer { get; set; }

        public Product? Product { get; set; }

        public PriorityEn Priority { get; set; }

        public string? Comment { get; set; } 

        public StatusEn Status { get; set; }

        public ComboOption? User { get; set; }

    }

}
