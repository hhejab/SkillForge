using System.ComponentModel.DataAnnotations;

namespace SkillForge.API.Models
{
    public class VerificationStatus
    {
        [Key]
        public int VerificationStatusId { get; set; }

        public string StatusName { get; set; } = string.Empty;

        public ICollection<Certificate>? Certificates { get; set; }
    }
}