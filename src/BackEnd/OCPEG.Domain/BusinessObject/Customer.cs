using OCPEG.Domain.DataAccess.Base;
using System.ComponentModel.DataAnnotations;

namespace OCPEG.Domain.BusinessObject
{
    public class Customer : BaseEntity
    {
        public long CustomerId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string? Name { get; set; } = string.Empty;

        public string? Email { get; set; } = string.Empty;

        public string? Telephone { get; set; } = string.Empty;
        public short? Age { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public string? Text { get; set; } = string.Empty;
        public decimal? MonthlyPayment { get; set; }
        public int? TypeId { get; set; }
    }

}
