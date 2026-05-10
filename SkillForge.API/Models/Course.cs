using System.ComponentModel.DataAnnotations;

namespace SkillForge.API.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CertificationTrack { get; set; } = string.Empty;
        public int Duration { get; set; }
    }
}
