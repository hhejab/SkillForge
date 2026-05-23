using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SkillForge.API.Models;
using SkillForge.MVC.Hubs;

namespace SkillForge.MVC.Controllers
{
    // Only trainees can access these actions
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

        // GET: TraineeEnrollments/AvailableSessions
        // Shows all sessions the trainee can browse and enroll in
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

        // POST: TraineeEnrollments/Enroll
        // Handles the enroll button click — validates capacity and prerequisites before creating the enrollment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int sessionId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            // Resolve the logged-in user to their Trainee profile
            var trainee = await _context.Trainees
                .FirstOrDefaultAsync(t => t.UserId == user.Id);

            if (trainee == null)
                return NotFound("Trainee profile not found.");

            // Load the session with its current enrollment count
            var session = await _context.Sessions
                .Include(s => s.Enrollments)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session == null)
                return NotFound();

            // Prevent duplicate enrollment in the same session
            var alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e => e.TraineeId == trainee.TraineeId && e.SessionId == sessionId);

            if (alreadyEnrolled)
            {
                TempData["Error"] = "You are already enrolled in this session.";
                return RedirectToAction(nameof(AvailableSessions));
            }

            // Enforce session capacity limit
            var enrolledCount = await _context.Enrollments
                .CountAsync(e => e.SessionId == sessionId);

            if (enrolledCount >= session.Capacity)
            {
                TempData["Error"] = "This session is full.";
                return RedirectToAction(nameof(AvailableSessions));
            }

            // Check that the trainee has passed all prerequisite courses before enrolling
            var prerequisites = await _context.CoursePrerequisites
                .Where(cp => cp.CourseId == session.CourseId)
                .ToListAsync();

            foreach (var prereq in prerequisites)
            {
                // A prerequisite is satisfied when the trainee has a Pass result in any session of that course
                var hasPassed = await _context.Enrollments
                    .Include(e => e.Session)
                    .Include(e => e.Result)
                    .AnyAsync(e => e.TraineeId == trainee.TraineeId
                                && e.Session!.CourseId == prereq.PrerequisiteCourseId
                                && e.Result != null
                                && e.Result.PassFail == "Pass");

                if (!hasPassed)
                {
                    TempData["Error"] = "You must complete the prerequisite course before enrolling in this session.";
                    return RedirectToAction(nameof(AvailableSessions));
                }
            }

            // Find the "Pending" enrollment status (initial state when trainee self-enrolls)
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

            // Broadcast the updated enrollment count to all connected clients via SignalR
            var newCount = await _context.Enrollments
                .CountAsync(e => e.SessionId == sessionId);

            await _hubContext.Clients.All.SendAsync("EnrollmentUpdated", sessionId, newCount);

            TempData["Success"] = "Enrollment submitted successfully.";
            return RedirectToAction(nameof(MyEnrollments));
        }

        // GET: TraineeEnrollments/MyEnrollments
        // Shows the trainee's own enrollments with results, certificates, and track progress
        public async Task<IActionResult> MyEnrollments()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var trainee = await _context.Trainees
                .FirstOrDefaultAsync(t => t.UserId == user.Id);

            if (trainee == null)
                return NotFound("Trainee profile not found.");

            // Load enrollments with all related data needed for the view (status, result, certificate, track)
            var enrollments = await _context.Enrollments
                .Include(e => e.Session!)
                    .ThenInclude(s => s.Course!)
                        .ThenInclude(c => c.Track)
                .Include(e => e.EnrollmentStatus)
                .Include(e => e.Result)
                .Include(e => e.Certificate)
                .Where(e => e.TraineeId == trainee.TraineeId)
                .ToListAsync();

            // Build certification progress per track:
            // For each track the trainee has enrolled courses in, calculate how many they've passed
            var enrolledTrackIds = enrollments
                .Where(e => e.Session?.Course?.TrackId != null)
                .Select(e => e.Session!.Course!.TrackId)
                .Distinct()
                .ToList();

            var tracks = await _context.Tracks
                .Include(t => t.Courses)
                .Where(t => enrolledTrackIds.Contains(t.TrackId))
                .ToListAsync();

            var trackProgress = tracks.Select(track =>
            {
                var trackCourseIds = track.Courses?.Select(c => c.CourseId).ToList() ?? new List<int>();
                int passed = enrollments.Count(e =>
                    trackCourseIds.Contains(e.Session?.Course?.CourseId ?? 0) &&
                    e.Result?.PassFail == "Pass");
                int total = trackCourseIds.Count;

                return new
                {
                    TrackName = track.TrackName,
                    Passed = passed,
                    Total = total,
                    // Eligible when all required courses in the track are passed
                    IsEligible = total > 0 && passed == total
                };
            }).ToList();

            ViewBag.TrackProgress = trackProgress;

            return View(enrollments);
        }
    }
}
