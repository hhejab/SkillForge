using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillForge.API.Models
{
    public class Session
    {
        [Key]
        public int SessionId { get; set; }
        public int CourseId { get; set; }
        public int InstructorId { get; set; }
        public string Room { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Capacity { get; set; }

        // Navigation properties for EF Core JOIN queries (no DB schema change required)
        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

        [ForeignKey("InstructorId")]
        public User? Instructor { get; set; }
    }
}
