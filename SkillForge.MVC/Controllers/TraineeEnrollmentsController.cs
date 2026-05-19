using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SkillForge.API.Models;
using SkillForge.MVC.Hubs;

namespace SkillForge.MVC.Controllers
{
    [Authorize(Roles = "Trainee")]
    public class TraineeEnrollmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<EnrollmentHub> _hubContext;

        public TraineeEnrollmentsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IHubContext<EnrollmentHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> AvailableSessions()
        {
            var sessions = await _context.Sessions
                .Include(s => s.Course)
                .Include(s => s.Instructor)
                .Include(s => s.Room)
                .Include(s => s.Enrollments)
                .ToListAsync();

            return View(sessions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int sessionId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.UserId == user.Id);

            if (trainee == null)
                return NotFound("Trainee profile not found.");

            var session = await _context.Sessions
                .Include(s => s.Enrollments)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session == null)
                return NotFound();

            var alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e => e.TraineeId == trainee.TraineeId && e.SessionId == sessionId);

            if (alreadyEnrolled)
            {
                TempData["Error"] = "You are already enrolled in this session.";
                return RedirectToAction(nameof(AvailableSessions));
            }

            var enrolledCount = await _context.Enrollments.CountAsync(e => e.SessionId == sessionId);

            if (enrolledCount >= session.Capacity)
            {
                TempData["Error"] = "This session is full.";
                return RedirectToAction(nameof(AvailableSessions));
            }

            var pendingStatus = await _context.EnrollmentStatuses
                .FirstOrDefaultAsync(s => s.StatusName == "Pending");

            var enrollment = new Enrollment
            {
                TraineeId = trainee.TraineeId,
                SessionId = sessionId,
                EnrollmentDate = DateTime.Now,
                EnrollmentStatusId = pendingStatus?.EnrollmentStatusId ?? 1
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            var newCount = await _context.Enrollments.CountAsync(e => e.SessionId == sessionId);

            await _hubContext.Clients.All.SendAsync("EnrollmentUpdated", sessionId, newCount);

            TempData["Success"] = "Enrollment submitted successfully.";
            return RedirectToAction(nameof(MyEnrollments));
        }

        public async Task<IActionResult> MyEnrollments()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.UserId == user.Id);

            if (trainee == null)
                return NotFound("Trainee profile not found.");

            var enrollments = await _context.Enrollments
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .Include(e => e.EnrollmentStatus)
                .Where(e => e.TraineeId == trainee.TraineeId)
                .ToListAsync();

            return View(enrollments);
        }
    }
}