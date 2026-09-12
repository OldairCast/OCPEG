using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.BusinessObject.Common;

namespace OCPEG.Application.Communication.Responses
{
    public class UserResponse
    {
        public User? User { get; set; }

        public List<ComboOption>? Functions { get; set; }
    }
}
