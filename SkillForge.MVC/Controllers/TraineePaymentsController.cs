using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillForge.API.Models;
using SkillForge.MVC.Models;

namespace SkillForge.MVC.Controllers
{
    [Authorize(Roles = "Trainee")]
    public class TraineePaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TraineePaymentsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> MyPayments()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var trainee = await _context.Trainees
                .FirstOrDefaultAsync(t => t.UserId == user.Id);

            if (trainee == null)
                return NotFound("Trainee profile not found.");

            var payments = await _context.Payments
                .Include(p => p.PaymentStatus)
                .Include(p => p.Enrollment!)
                    .ThenInclude(e => e.Session!)
                        .ThenInclude(s => s.Course)
                .Where(p => p.Enrollment != null && p.Enrollment.TraineeId == trainee.TraineeId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return View(payments);
        }

        [HttpGet]
        public async Task<IActionResult> Pay(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var trainee = await _context.Trainees
                .FirstOrDefaultAsync(t => t.UserId == user.Id);

            if (trainee == null)
                return NotFound("Trainee profile not found.");

            var payment = await _context.Payments
                .Include(p => p.PaymentStatus)
                .Include(p => p.Enrollment!)
                    .ThenInclude(e => e.Session!)
                        .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(p =>
                    p.PaymentId == id &&
                    p.Enrollment != null &&
                    p.Enrollment.TraineeId == trainee.TraineeId);

            if (payment == null)
                return NotFound();

            if (payment.PaymentStatusId == 2)
            {
                TempData["Error"] = "This payment is already paid.";
                return RedirectToAction(nameof(MyPayments));
            }

            var model = new CardPaymentViewModel
            {
                PaymentId = payment.PaymentId,
                Amount = payment.Amount
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(CardPaymentViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;

            if (model.ExpiryYear < currentYear ||
                (model.ExpiryYear == currentYear && model.ExpiryMonth < currentMonth))
            {
                ModelState.AddModelError("", "Card is expired.");
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var trainee = await _context.Trainees
                .FirstOrDefaultAsync(t => t.UserId == user.Id);

            if (trainee == null)
                return NotFound("Trainee profile not found.");

            var payment = await _context.Payments
                .Include(p => p.Enrollment)
                .FirstOrDefaultAsync(p =>
                    p.PaymentId == model.PaymentId &&
                    p.Enrollment != null &&
                    p.Enrollment.TraineeId == trainee.TraineeId);

            if (payment == null)
                return NotFound();

            if (payment.PaymentStatusId == 2)
            {
                TempData["Error"] = "This payment is already paid.";
                return RedirectToAction(nameof(MyPayments));
            }

            payment.PaymentStatusId = 2;
            payment.PaymentMethod = "Card";
            payment.PaymentDate = DateTime.Now;

            await _context.SaveChangesAsync();

            var passedResult = await _context.Results
                .FirstOrDefaultAsync(r =>
                    r.EnrollmentId == payment.EnrollmentId &&
                    r.PassFail == "Pass");

            var certificateExists = await _context.Certificates
                .AnyAsync(c => c.EnrollmentId == payment.EnrollmentId);

            if (passedResult != null && !certificateExists)
            {
                var certificateCode = $"SF-CERT-{DateTime.Now.Year}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";

                _context.Certificates.Add(new Certificate
                {
                    EnrollmentId = payment.EnrollmentId,
                    CertificateCode = certificateCode,
                    IssueDate = DateTime.Now,
                    VerificationStatusId = 2
                });

                await _context.SaveChangesAsync();

                TempData["Success"] = $"Payment completed successfully. Certificate generated: {certificateCode}";
                return RedirectToAction(nameof(MyPayments));
            }

            TempData["Success"] = "Payment completed successfully.";
            return RedirectToAction(nameof(MyPayments));
        }
    }
}