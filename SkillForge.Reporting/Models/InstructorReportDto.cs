namespace SkillForge.Reporting.Models
{
    public class InstructorReportDto
    {
        public int InstructorId { get; set; }
        public string InstructorName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int TotalSessions { get; set; }
    }
}