using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SkillForge.API.Models;
using SkillForge.MVC.ViewModels;

namespace SkillForge.MVC.Controllers
{
    // TODO: Uncomment the line below when the authentication team has implemented login/roles.
    // [Authorize(Roles = "Coordinator")]
    public class SessionsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SessionsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // ─── INDEX ──────────────────────────────────────────────────────────────────

        // GET: /Sessions
        public async Task<IActionResult> Index()
        {
            var sessions = await _db.Sessions
                .Include(s => s.Course)
                .Include(s => s.Instructor)
                .OrderBy(s => s.StartDate)
                .ToListAsync();

            // Single query to get enrollment counts for all sessions at once
            var sessionIds = sessions.Select(s => s.SessionId).ToList();
            var enrollmentCounts = await _db.Enrollments
                .Where(e => sessionIds.Contains(e.SessionId))
                .GroupBy(e => e.SessionId)
                .Select(g => new { SessionId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.SessionId, x => x.Count);

            ViewBag.EnrollmentCounts = enrollmentCounts;
            return View(sessions);
        }

        // ─── DETAILS ────────────────────────────────────────────────────────────────

        // GET: /Sessions/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var session = await _db.Sessions
                .Include(s => s.Course)
                .Include(s => s.Instructor)
                .FirstOrDefaultAsync(s => s.SessionId == id);

            if (session == null)
                return NotFound();

            ViewBag.EnrollmentCount = await _db.Enrollments.CountAsync(e => e.SessionId == id);
            return View(session);
        }

        // ─── CREATE ─────────────────────────────────────────────────────────────────

        // GET: /Sessions/Create
        public async Task<IActionResult> Create()
        {
            var vm = new SessionFormViewModel
            {
                // Sensible defaults for the form
                StartDate = DateTime.Today.AddDays(1).AddHours(9),
                EndDate   = DateTime.Today.AddDays(1).AddHours(11),
                Capacity  = 20,
                Courses     = await GetCoursesListAsync(),
                Instructors = await GetInstructorsListAsync()
            };
            return View(vm);
        }

        // POST: /Sessions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SessionFormViewModel vm)
        {
            await ValidateSessionAsync(vm, excludeSessionId: null);

            if (!ModelState.IsValid)
            {
                vm.Courses     = await GetCoursesListAsync();
                vm.Instructors = await GetInstructorsListAsync();
                return View(vm);
            }

            var session = new Session
            {
                CourseId     = vm.CourseId,
                InstructorId = vm.InstructorId,
                Room         = vm.Room.Trim(),
                StartDate    = vm.StartDate,
                EndDate      = vm.EndDate,
                Capacity     = vm.Capacity
            };

            _db.Sessions.Add(session);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Session scheduled successfully (ID: {session.SessionId}).";
            return RedirectToAction(nameof(Index));
        }

        // ─── EDIT ───────────────────────────────────────────────────────────────────

        // GET: /Sessions/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var session = await _db.Sessions.FindAsync(id);
            if (session == null)
                return NotFound();

            var vm = new SessionFormViewModel
            {
                SessionId    = session.SessionId,
                CourseId     = session.CourseId,
                InstructorId = session.InstructorId,
                Room         = session.Room,
                StartDate    = session.StartDate,
                EndDate      = session.EndDate,
                Capacity     = session.Capacity,
                Courses      = await GetCoursesListAsync(),
                Instructors  = await GetInstructorsListAsync()
            };
            return View(vm);
        }

        // POST: /Sessions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SessionFormViewModel vm)
        {
            if (id != vm.SessionId)
                return BadRequest();

            // Validate capacity against existing enrollments before other checks
            if (ModelState.IsValid)
            {
                var enrollmentCount = await _db.Enrollments.CountAsync(e => e.SessionId == id);
                if (vm.Capacity < enrollmentCount)
                {
                    ModelState.AddModelError("Capacity",
                        $"Capacity cannot be reduced below the current enrollment count ({enrollmentCount} trainees).");
                }
            }

            await ValidateSessionAsync(vm, excludeSessionId: id);

            if (!ModelState.IsValid)
            {
                vm.Courses     = await GetCoursesListAsync();
                vm.Instructors = await GetInstructorsListAsync();
                return View(vm);
            }

            var session = await _db.Sessions.FindAsync(id);
            if (session == null)
                return NotFound();

            session.CourseId     = vm.CourseId;
            session.InstructorId = vm.InstructorId;
            session.Room         = vm.Room.Trim();
            session.StartDate    = vm.StartDate;
            session.EndDate      = vm.EndDate;
            session.Capacity     = vm.Capacity;

            await _db.SaveChangesAsync();

            TempData["Success"] = "Session updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ─── DELETE ─────────────────────────────────────────────────────────────────

        // GET: /Sessions/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var session = await _db.Sessions
                .Include(s => s.Course)
                .Include(s => s.Instructor)
                .FirstOrDefaultAsync(s => s.SessionId == id);

            if (session == null)
                return NotFound();

            ViewBag.EnrollmentCount = await _db.Enrollments.CountAsync(e => e.SessionId == id);
            return View(session);
        }

        // POST: /Sessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var session = await _db.Sessions.FindAsync(id);
            if (session == null)
                return NotFound();

            _db.Sessions.Remove(session);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Session deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ─── HELPERS ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Validates end-date, instructor availability, and room availability.
        /// Pass excludeSessionId = null for Create, the current SessionId for Edit
        /// so the session doesn't conflict with itself.
        /// </summary>
        private async Task ValidateSessionAsync(SessionFormViewModel vm, int? excludeSessionId)
        {
            // Check end is after start first; skip overlap checks if dates are invalid
            if (vm.EndDate <= vm.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date/time must be after start date/time.");
                return;
            }

            // Overlap logic: two time ranges overlap when
            //   existing.StartDate < newEndDate  AND  existing.EndDate > newStartDate
            var instructorConflict = await _db.Sessions.AnyAsync(s =>
                s.SessionId != (excludeSessionId ?? 0) &&
                s.InstructorId == vm.InstructorId &&
                s.StartDate < vm.EndDate &&
                s.EndDate > vm.StartDate);

            if (instructorConflict)
                ModelState.AddModelError("InstructorId",
                    "This instructor is already scheduled for another session during the selected time slot.");

            // Room comparison is case-insensitive trim so "Lab 1" == "lab 1"
            var trimmedRoom = vm.Room?.Trim() ?? string.Empty;
            var roomConflict = await _db.Sessions.AnyAsync(s =>
                s.SessionId != (excludeSessionId ?? 0) &&
                s.Room.ToLower() == trimmedRoom.ToLower() &&
                s.StartDate < vm.EndDate &&
                s.EndDate > vm.StartDate);

            if (roomConflict)
                ModelState.AddModelError("Room",
                    "This room is already booked for another session during the selected time slot.");
        }

        /// <summary>Returns all courses as dropdown items, ordered by title.</summary>
        private async Task<IEnumerable<SelectListItem>> GetCoursesListAsync()
        {
            return await _db.Courses
                .OrderBy(c => c.Title)
                .Select(c => new SelectListItem(c.Title, c.CourseId.ToString()))
                .ToListAsync();
        }

        /// <summary>
        /// Returns all users whose role is "Instructor".
        /// Falls back to all users if no Instructor role exists yet (dev convenience).
        /// </summary>
        private async Task<IEnumerable<SelectListItem>> GetInstructorsListAsync()
        {
            var instructorRole = await _db.Roles
                .FirstOrDefaultAsync(r => r.RoleName == "Instructor");

            IQueryable<User> query = _db.Users.OrderBy(u => u.FullName);

            // If the Instructor role exists in the DB, filter by it
            if (instructorRole != null)
                query = query.Where(u => u.RoleId == instructorRole.RoleId);

            return await query
                .Select(u => new SelectListItem(u.FullName, u.UserId.ToString()))
                .ToListAsync();
        }
    }
}
