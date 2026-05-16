using System.ComponentModel.DataAnnotations;

namespace SkillForge.API.Models
{
    public class Session
    {
        [Key]
        public int SessionId { get; set; }
        public int CourseId { get; set; }
        public Course? Course { get; set; }
        public int InstructorId { get; set; }
        public Instructor? Instructor { get; set; }
        public int RoomId { get; set; }
        public Room? Room { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Capacity { get; set; }
        public ICollection<Enrollment>? Enrollments { get; set; }
    }
}
