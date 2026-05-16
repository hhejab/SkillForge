namespace SkillForge.Reporting.Models
{
    public class CourseReportDto
    {
        public int CourseId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public int TrackId { get; set; }

        public int Duration { get; set; }
    }
}