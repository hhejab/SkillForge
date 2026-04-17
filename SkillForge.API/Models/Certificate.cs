using System.ComponentModel.DataAnnotations;

namespace SkillForge.API.Models
{
    public class Certificate
    {
        [Key]
        public int CertificateId { get; set; }
        public int EnrollmentId { get; set; }
        public string CertificateCode { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public string VerificationStatus { get; set; } = string.Empty;
    }
}
