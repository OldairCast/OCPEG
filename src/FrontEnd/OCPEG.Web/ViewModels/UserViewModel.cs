using OCPEG.Domain.BusinessObject.Common;
using OCPEG.Domain.Dto;

namespace OCPEG.Web.ViewModels
{
    public class UserViewModel
    {
        public UserDto? User { get; set; }

        public List<ComboOption>? Function { get; set; }

        public List<ComboOption>? Role { get; set; }

        public string? FunctionId { get; set; }

        public string? RoleId { get; set; }
    }
}
