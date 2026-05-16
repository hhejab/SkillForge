namespace SkillForge.Reporting.Models
{
    public class EnrollmentReportDto
    {
        public int EnrollmentId { get; set; }

        public int TraineeId { get; set; }

        public int SessionId { get; set; }

        public DateTime EnrollmentDate { get; set; }

        public int EnrollmentStatusId { get; set; }
    }
}