namespace SkillForge.Reporting.Models
{
    public class SessionReportDto
    {
        public int SessionId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Capacity { get; set; }
        public int EnrolledCount { get; set; }
    }
}