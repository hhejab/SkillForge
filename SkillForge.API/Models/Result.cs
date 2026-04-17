using System.ComponentModel.DataAnnotations;

namespace SkillForge.API.Models
{
    public class Result
    {
        [Key]
        public int ResultId { get; set; }
        public int EnrollmentId { get; set; }
        public string GradeOrScore { get; set; } = string.Empty;
        public string PassFail { get; set; } = string.Empty;
        public int UpdatedByInstructorId { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
