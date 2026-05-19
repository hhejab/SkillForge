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
                .Include(e => e.Session!)
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

            var normalizedPassFail =
                passFail == "true" || passFail == "True" || passFail == "Pass"
                    ? "Pass"
                    : "Fail";

            var existingResult = await _context.Results
                .FirstOrDefaultAsync(r => r.EnrollmentId == enrollmentId);

            if (existingResult == null)
            {
                var result = new Result
                {
                    EnrollmentId = enrollmentId,
                    GradeOrScore = gradeOrScore,
                    PassFail = normalizedPassFail,
                    UpdatedByInstructorId = instructor.InstructorId,
                    UpdatedAt = DateTime.Now
                };

                _context.Results.Add(result);
            }
            else
            {
                existingResult.GradeOrScore = gradeOrScore;
                existingResult.PassFail = normalizedPassFail;
                existingResult.UpdatedByInstructorId = instructor.InstructorId;
                existingResult.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            if (normalizedPassFail == "Pass")
            {
                var paymentPaid = await _context.Payments
                    .AnyAsync(p => p.EnrollmentId == enrollmentId && p.PaymentStatusId == 2);

                var existingCertificate = await _context.Certificates
                    .FirstOrDefaultAsync(c => c.EnrollmentId == enrollmentId);

                if (existingCertificate != null)
                {
                    TempData["Success"] = $"Result saved. Certificate already exists: {existingCertificate.CertificateCode}";
                    return RedirectToAction(nameof(MySessions));
                }

                if (paymentPaid)
                {
                    var certificateCode = $"SF-CERT-{DateTime.Now.Year}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";

                    _context.Certificates.Add(new Certificate
                    {
                        EnrollmentId = enrollmentId,
                        CertificateCode = certificateCode,
                        IssueDate = DateTime.Now,
                        VerificationStatusId = 2
                    });

                    await _context.SaveChangesAsync();

                    TempData["Success"] = $"Result saved and certificate generated: {certificateCode}";
                    return RedirectToAction(nameof(MySessions));
                }

                TempData["Success"] = "Result saved. Certificate will be generated after payment is completed.";
                return RedirectToAction(nameof(MySessions));
            }

            TempData["Success"] = "Result saved successfully.";
            return RedirectToAction(nameof(MySessions));
        }
    }
}