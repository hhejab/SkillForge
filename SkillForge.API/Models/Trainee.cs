namespace SkillForge.API.Models
{
    public class Trainee
    {
        public int TraineeId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public ICollection<Enrollment>? Enrollments { get; set; }
    }
}