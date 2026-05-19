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
    [Authorize(Roles = "TrainingCoordinator")]
    public class SessionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sessions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Sessions.Include(s => s.Course).Include(s => s.Instructor).Include(s => s.Room);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Sessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Sessions
                .Include(s => s.Course)
                .Include(s => s.Instructor)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(m => m.SessionId == id);
            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        // GET: Sessions/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Description");
            ViewData["InstructorId"] = new SelectList(_context.Instructors, "InstructorId", "Email");
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomName");
            return View();
        }

        // POST: Sessions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SessionId,CourseId,InstructorId,RoomId,StartDate,EndDate,Capacity")] Session session)
        {
            if (ModelState.IsValid)
            {
                _context.Add(session);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Description", session.CourseId);
            ViewData["InstructorId"] = new SelectList(_context.Instructors, "InstructorId", "Email", session.InstructorId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomName", session.RoomId);
            return View(session);
        }

        // GET: Sessions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Sessions.FindAsync(id);
            if (session == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Description", session.CourseId);
            ViewData["InstructorId"] = new SelectList(_context.Instructors, "InstructorId", "Email", session.InstructorId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomName", session.RoomId);
            return View(session);
        }

        // POST: Sessions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SessionId,CourseId,InstructorId,RoomId,StartDate,EndDate,Capacity")] Session session)
        {
            if (id != session.SessionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(session);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SessionExists(session.SessionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Description", session.CourseId);
            ViewData["InstructorId"] = new SelectList(_context.Instructors, "InstructorId", "Email", session.InstructorId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomName", session.RoomId);
            return View(session);
        }

        // GET: Sessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Sessions
                .Include(s => s.Course)
                .Include(s => s.Instructor)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(m => m.SessionId == id);
            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        // POST: Sessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var session = await _context.Sessions.FindAsync(id);
            if (session != null)
            {
                _context.Sessions.Remove(session);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SessionExists(int id)
        {
            return _context.Sessions.Any(e => e.SessionId == id);
        }
    }
}
