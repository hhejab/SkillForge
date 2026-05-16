using System.ComponentModel.DataAnnotations;

namespace SkillForge.API.Models
{
    public class Track
    {
        [Key]
        public int TrackId { get; set; }

        public string TrackName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ICollection<Course>? Courses { get; set; }
    }
}