using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillForge.API.Models;

namespace SkillForge.MVC.Controllers
{
    [Authorize(Roles = "Instructor,TrainingCoordinator")]
    public class InstructorSessionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public InstructorSessionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> MySessions()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == user.Id);

            if (instructor == null && !User.IsInRole("TrainingCoordinator"))
                return NotFound("Instructor profile not found.");

            var query = _context.Sessions
                .Include(s => s.Course)
                .Include(s => s.Room)
                .Include(s => s.Instructor)
                .AsQueryable();

            if (instructor != null && User.IsInRole("Instructor"))
            {
                query = query.Where(s => s.InstructorId == instructor.InstructorId);
            }

            var sessions = await query.ToListAsync();

            return View(sessions);
        }

        public async Task<IActionResult> Trainees(int sessionId)
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Trainee)
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .Include(e => e.Result)
                .Where(e => e.SessionId == sessionId)
                .ToListAsync();

            ViewBag.SessionId = sessionId;
            return View(enrollments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordResult(int enrollmentId, string gradeOrScore, string passFail)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == user.Id);

            if (instructor == null)
                return NotFound("Instructor profile not found.");

            var existingResult = await _context.Results.FirstOrDefaultAsync(r => r.EnrollmentId == enrollmentId);

            if (existingResult == null)
            {
                var result = new Result
                {
                    EnrollmentId = enrollmentId,
                    GradeOrScore = gradeOrScore,
                    PassFail = passFail,
                    UpdatedByInstructorId = instructor.InstructorId,
                    UpdatedAt = DateTime.Now
                };

                _context.Results.Add(result);
            }
            else
            {
                existingResult.GradeOrScore = gradeOrScore;
                existingResult.PassFail = passFail;
                existingResult.UpdatedByInstructorId = instructor.InstructorId;
                existingResult.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Result saved successfully.";
            return RedirectToAction(nameof(MySessions));
        }
    }
}