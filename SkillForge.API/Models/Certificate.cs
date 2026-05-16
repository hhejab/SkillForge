using System.ComponentModel.DataAnnotations;

namespace SkillForge.API.Models
{
    public class Certificate
    {
        [Key]
        public int CertificateId { get; set; }
        public int EnrollmentId { get; set; }
        public Enrollment? Enrollment { get; set; }
        public string CertificateCode { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public int VerificationStatusId { get; set; }
        public VerificationStatus? VerificationStatus { get; set; }
    }
}
