using OCPEG.Domain.DataAccess.Base;

namespace OCPEG.Domain.BusinessObject
{
    public class CustomerType : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
    }
}
