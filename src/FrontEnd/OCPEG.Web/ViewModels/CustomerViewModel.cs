using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.Dto;

namespace OCPEG.Web.ViewModels
{
    public class CustomerViewModel
    {
        public List<CustomerDto>? Customers { get; set; }

        public CustomerDto? Customer { get; set; }

        public string? Photo { get; set; }

        public List<CustomerType>? Type { get; set; }
    }
}
