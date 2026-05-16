using System.ComponentModel.DataAnnotations;

namespace SkillForge.API.Models
{
    public class EnrollmentStatus
    {
        [Key]
        public int EnrollmentStatusId { get; set; }

        public string StatusName { get; set; } = string.Empty;

        public ICollection<Enrollment>? Enrollments { get; set; }
    }
}