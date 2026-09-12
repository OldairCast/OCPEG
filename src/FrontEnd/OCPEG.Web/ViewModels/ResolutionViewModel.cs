using System.ComponentModel.DataAnnotations;

namespace OCPEG.Web.ViewModels
{
    public class ResolutionViewModel
    {
        [Required]
        public int CallNumber { get; set; } = 0;
        public DateTime? Date { get; set; }
        public string? Comment { get; set; }
        public string? User { get; set; }
    }
}
