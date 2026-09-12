using OCPEG.Domain.BusinessObject.Common;

namespace OCPEG.Domain.Dto
{
    public record UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool Active { get; set; } = true;
        public bool Admin { get; set; } = false;
        public short Function { get; set; }
        public string RefreshToken { get; set; } = string.Empty;

        public string ProfileId { get; set; } = string.Empty;

        public List<ComboOption>? Profile { get; set; }
    }
}
