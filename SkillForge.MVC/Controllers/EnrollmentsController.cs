using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SkillForge.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkillForge.MVC.Controllers
{
    // Only the Training Coordinator can manage enrollments via this admin CRUD controller
    [Authorize(Roles = "TrainingCoordinator")]
    public class EnrollmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Enrollments — lists all enrollments across all trainees and sessions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Enrollments
                .Include(e => e.EnrollmentStatus)
                .Include(e => e.Session)
                .Include(e => e.Trainee);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Enrollments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var enrollment = await _context.Enrollments
                .Include(e => e.EnrollmentStatus)
                .Include(e => e.Session)
                .Include(e => e.Trainee)
                .FirstOrDefaultAsync(m => m.EnrollmentId == id);

            if (enrollment == null)
                return NotFound();

            return View(enrollment);
        }

        // GET: Enrollments/Create — coordinator can manually create an enrollment
        public IActionResult Create()
        {
            ViewData["EnrollmentStatusId"] = new SelectList(_context.EnrollmentStatuses, "EnrollmentStatusId", "StatusName");
            ViewData["SessionId"] = new SelectList(_context.Sessions, "SessionId", "SessionId");
            ViewData["TraineeId"] = new SelectList(_context.Trainees, "TraineeId", "Email");
            return View();
        }

        // POST: Enrollments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EnrollmentId,TraineeId,SessionId,EnrollmentDate,EnrollmentStatusId")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["EnrollmentStatusId"] = new SelectList(_context.EnrollmentStatuses, "EnrollmentStatusId", "StatusName", enrollment.EnrollmentStatusId);
            ViewData["SessionId"] = new SelectList(_context.Sessions, "SessionId", "SessionId", enrollment.SessionId);
            ViewData["TraineeId"] = new SelectList(_context.Trainees, "TraineeId", "Email", enrollment.TraineeId);
            return View(enrollment);
        }

        // GET: Enrollments/Edit/5 — coordinator can update enrollment status (e.g. Approve, Cancel)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
                return NotFound();

            ViewData["EnrollmentStatusId"] = new SelectList(_context.EnrollmentStatuses, "EnrollmentStatusId", "StatusName", enrollment.EnrollmentStatusId);
            ViewData["SessionId"] = new SelectList(_context.Sessions, "SessionId", "SessionId", enrollment.SessionId);
            ViewData["TraineeId"] = new SelectList(_context.Trainees, "TraineeId", "Email", enrollment.TraineeId);
            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EnrollmentId,TraineeId,SessionId,EnrollmentDate,EnrollmentStatusId")] Enrollment enrollment)
        {
            if (id != enrollment.EnrollmentId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // If the record was deleted between load and save, return 404
                    if (!EnrollmentExists(enrollment.EnrollmentId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["EnrollmentStatusId"] = new SelectList(_context.EnrollmentStatuses, "EnrollmentStatusId", "StatusName", enrollment.EnrollmentStatusId);
            ViewData["SessionId"] = new SelectList(_context.Sessions, "SessionId", "SessionId", enrollment.SessionId);
            ViewData["TraineeId"] = new SelectList(_context.Trainees, "TraineeId", "Email", enrollment.TraineeId);
            return View(enrollment);
        }

        // GET: Enrollments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var enrollment = await _context.Enrollments
                .Include(e => e.EnrollmentStatus)
                .Include(e => e.Session)
                .Include(e => e.Trainee)
                .FirstOrDefaultAsync(m => m.EnrollmentId == id);

            if (enrollment == null)
                return NotFound();

            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment != null)
                _context.Enrollments.Remove(enrollment);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnrollmentExists(int id)
        {
            return _context.Enrollments.Any(e => e.EnrollmentId == id);
        }
    }
}
