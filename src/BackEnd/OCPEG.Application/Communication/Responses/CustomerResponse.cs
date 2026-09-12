namespace OCPEG.Application.Communication.Responses
{
    public class CustomerResponse
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? Telephone { get; set; }
        public short? Age { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public string? Photo { get; set; }
        public string? Text { get; set; }
        public decimal? MonthlyPayment { get; set; }
        public int? TypeId { get; set; }

    }
}
