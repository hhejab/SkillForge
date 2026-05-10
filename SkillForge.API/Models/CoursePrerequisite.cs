using System.ComponentModel.DataAnnotations;

namespace SkillForge.API.Models
{
    public class CoursePrerequisite
    {
        [Key]
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int PrerequisiteCourseId { get; set; }
    }
}
