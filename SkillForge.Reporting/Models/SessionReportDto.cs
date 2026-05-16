namespace SkillForge.Reporting.Models
{
    public class SessionReportDto
    {
        public int SessionId { get; set; }

        public int CourseId { get; set; }

        public int InstructorId { get; set; }

        public int RoomId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Capacity { get; set; }
    }
}