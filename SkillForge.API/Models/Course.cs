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
        public Category? Category { get; set; }
        public int TrackId { get; set; }
        public Track? Track { get; set; }
        public int Duration { get; set; }
        public ICollection<Session>? Sessions { get; set; }
    }
}
