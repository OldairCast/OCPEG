using OCPEG.Domain.BusinessObject.Common;

namespace OCPEG.Domain.BusinessObject
{
    public class Resolution
    {
        public int CallNumber { get; set; }
        public DateTime? Date { get; set; }
        public string? Comment { get; set; } = string.Empty;
        public ComboOption? User { get; set; }
        
    }
}
