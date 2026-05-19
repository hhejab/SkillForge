using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillForge.API.Models;

namespace SkillForge.API.Controllers
{
    [Authorize(Roles = "TrainingCoordinator")]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("enrollments-by-course")]
        public async Task<IActionResult> GetEnrollmentsByCourse()
        {
            var report = await _context.Courses
                .Select(c => new
                {
                    courseId = c.CourseId,
                    courseTitle = c.Title,
                    category = c.Category != null ? c.Category.CategoryName : "",
                    totalEnrollments = _context.Enrollments
                        .Count(e => e.Session != null && e.Session.CourseId == c.CourseId)
                })
                .ToListAsync();

            return Ok(report);
        }

        [HttpGet("instructor-workload")]
        public async Task<IActionResult> GetInstructorWorkload()
        {
            var report = await _context.Instructors
                .Select(i => new
                {
                    instructorId = i.InstructorId,
                    instructorName = i.FullName,
                    email = i.Email,
                    totalSessions = _context.Sessions
                        .Count(s => s.InstructorId == i.InstructorId)
                })
                .ToListAsync();

            return Ok(report);
        }

        [HttpGet("revenue-summary")]
        public async Task<IActionResult> GetRevenueSummary()
        {
            var report = await _context.Payments
                .Include(p => p.PaymentStatus)
                .Select(p => new
                {
                    paymentId = p.PaymentId,
                    enrollmentId = p.EnrollmentId,
                    amount = p.Amount,
                    paymentDate = p.PaymentDate,
                    paymentMethod = p.PaymentMethod,
                    status = p.PaymentStatus != null ? p.PaymentStatus.StatusName : ""
                })
                .ToListAsync();

            return Ok(report);
        }

        [HttpGet("sessions-summary")]
        public async Task<IActionResult> GetSessionsSummary()
        {
            var report = await _context.Sessions
                .Include(s => s.Course)
                .Include(s => s.Instructor)
                .Include(s => s.Room)
                .Select(s => new
                {
                    sessionId = s.SessionId,
                    courseTitle = s.Course != null ? s.Course.Title : "",
                    instructorName = s.Instructor != null ? s.Instructor.FullName : "",
                    roomName = s.Room != null ? s.Room.RoomName : "",
                    startDate = s.StartDate,
                    endDate = s.EndDate,
                    capacity = s.Capacity,
                    enrolledCount = _context.Enrollments.Count(e => e.SessionId == s.SessionId)
                })
                .ToListAsync();

            return Ok(report);
        }
    }
}