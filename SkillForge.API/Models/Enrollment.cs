using System.ComponentModel.DataAnnotations;

namespace SkillForge.API.Models
{
    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }
        public int TraineeId { get; set; }
        public Trainee? Trainee { get; set; }
        public int SessionId { get; set; }
        public Session? Session { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } = string.Empty;

        public Result? Result { get; set; }

        public Certificate? Certificate { get; set; }

        public ICollection<Payment>? Payments { get; set; }
    }
}
