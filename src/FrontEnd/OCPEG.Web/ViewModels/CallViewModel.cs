using OCPEG.Domain;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.BusinessObject.Common;
using static OCPEG.Domain.Enum.Enums;

namespace OCPEG.Web.ViewModels
{
    public class CallViewModel
    {
        public string? Id { get; set; }

        public string? CallNumber { get; set; }

        public string? StartDate { get; set; }
        
        public string? FinishDate { get; set; }

        public ComboOption? Customer { get; set; }

        public int? CustomerId { get; set; }

        public List<Product>? Product { get; set; }

        public Product? ProductSel { get; set; }

        public int? ProductId { get; set; }

        public List<ComboOption>? Priority { get; set; }

        public ComboOption? PrioritySel { get; set; }

        public byte? PriorityId { get; set; }

        public string? Comment { get; set; }

        public ComboOption? StatusSel { get; set; }

        public StatusEn? Status { get; set; }

    }
}
