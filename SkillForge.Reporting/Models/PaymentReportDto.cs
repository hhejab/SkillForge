namespace SkillForge.Reporting.Models
{
    public class PaymentReportDto
    {
        public int PaymentId { get; set; }

        public int EnrollmentId { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public int PaymentStatusId { get; set; }
    }
}