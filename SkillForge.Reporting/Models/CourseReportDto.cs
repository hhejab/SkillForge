namespace SkillForge.Reporting.Models
{
    public class CourseReportDto
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int TotalEnrollments { get; set; }
    }
}