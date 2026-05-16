using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillForge.API.Models;

namespace SkillForge.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentStatusController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentStatusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/EnrollmentStatus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnrollmentStatus>>> GetEnrollmentStatuses()
        {
            return await _context.EnrollmentStatuses.ToListAsync();
        }

        // GET: api/EnrollmentStatus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EnrollmentStatus>> GetEnrollmentStatus(int id)
        {
            var enrollmentStatus = await _context.EnrollmentStatuses.FindAsync(id);

            if (enrollmentStatus == null)
            {
                return NotFound();
            }

            return enrollmentStatus;
        }

        // PUT: api/EnrollmentStatus/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEnrollmentStatus(int id, EnrollmentStatus enrollmentStatus)
        {
            if (id != enrollmentStatus.EnrollmentStatusId)
            {
                return BadRequest();
            }

            _context.Entry(enrollmentStatus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EnrollmentStatusExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/EnrollmentStatus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EnrollmentStatus>> PostEnrollmentStatus(EnrollmentStatus enrollmentStatus)
        {
            _context.EnrollmentStatuses.Add(enrollmentStatus);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEnrollmentStatus", new { id = enrollmentStatus.EnrollmentStatusId }, enrollmentStatus);
        }

        // DELETE: api/EnrollmentStatus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnrollmentStatus(int id)
        {
            var enrollmentStatus = await _context.EnrollmentStatuses.FindAsync(id);
            if (enrollmentStatus == null)
            {
                return NotFound();
            }

            _context.EnrollmentStatuses.Remove(enrollmentStatus);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EnrollmentStatusExists(int id)
        {
            return _context.EnrollmentStatuses.Any(e => e.EnrollmentStatusId == id);
        }
    }
}
