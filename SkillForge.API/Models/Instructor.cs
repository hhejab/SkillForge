namespace SkillForge.API.Models
{
    public class Instructor
    {
        public int InstructorId { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? Specialization { get; set; }

        public ICollection<Session>? Sessions { get; set; }

        public ICollection<Result>? UpdatedResults { get; set; }
    }
}