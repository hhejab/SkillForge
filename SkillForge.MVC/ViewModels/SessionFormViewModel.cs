using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SkillForge.MVC.ViewModels
{
    /// <summary>
    /// ViewModel for the Create and Edit session forms.
    /// Using a dedicated ViewModel (instead of binding the Session entity directly)
    /// avoids overposting vulnerabilities and keeps the form decoupled from the DB model.
    /// </summary>
    public class SessionFormViewModel
    {
        // 0 on Create; the actual SessionId on Edit
        public int SessionId { get; set; }

        [Required(ErrorMessage = "Please select a course.")]
        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Please select an instructor.")]
        [Display(Name = "Instructor")]
        public int InstructorId { get; set; }

        [Required(ErrorMessage = "Room is required.")]
        [StringLength(100, ErrorMessage = "Room name cannot exceed 100 characters.")]
        [Display(Name = "Room / Classroom")]
        public string Room { get; set; } = string.Empty;

        [Required(ErrorMessage = "Start date and time are required.")]
        [Display(Name = "Start Date & Time")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date and time are required.")]
        [Display(Name = "End Date & Time")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, 10000, ErrorMessage = "Capacity must be at least 1.")]
        [Display(Name = "Capacity")]
        public int Capacity { get; set; }

        // Populated by the controller — not bound from the form POST
        public IEnumerable<SelectListItem> Courses { get; set; } = [];
        public IEnumerable<SelectListItem> Instructors { get; set; } = [];
    }
}
