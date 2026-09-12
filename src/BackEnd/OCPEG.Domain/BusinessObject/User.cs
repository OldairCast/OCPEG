using OCPEG.Domain.DataAccess.Base;

namespace OCPEG.Domain.BusinessObject
{
    public sealed class User: BaseEntity
    {
        //public int Id { get; set; }; => herda de BaseEntity
        public string? Name { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public string? RetypePassword { get; set; } = string.Empty;
        public string? PasswordHash { get; set; } = string.Empty;
        public bool Active { get; set; } = true;
        public bool Admin { get; set; } = false;
        public short Function { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; set; }
        public Guid UserIdentifier { get; set; } = Guid.NewGuid();
    }
}
