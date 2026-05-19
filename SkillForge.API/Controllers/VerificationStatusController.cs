using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillForge.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkillForge.API.Controllers
{
    [Authorize(Roles = "TrainingCoordinator")]
    [Route("api/[controller]")]
    [ApiController]
    public class VerificationStatusController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VerificationStatusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/VerificationStatus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VerificationStatus>>> GetVerificationStatuses()
        {
            return await _context.VerificationStatuses.ToListAsync();
        }

        // GET: api/VerificationStatus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VerificationStatus>> GetVerificationStatus(int id)
        {
            var verificationStatus = await _context.VerificationStatuses.FindAsync(id);

            if (verificationStatus == null)
            {
                return NotFound();
            }

            return verificationStatus;
        }

        // PUT: api/VerificationStatus/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVerificationStatus(int id, VerificationStatus verificationStatus)
        {
            if (id != verificationStatus.VerificationStatusId)
            {
                return BadRequest();
            }

            _context.Entry(verificationStatus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VerificationStatusExists(id))
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

        // POST: api/VerificationStatus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<VerificationStatus>> PostVerificationStatus(VerificationStatus verificationStatus)
        {
            _context.VerificationStatuses.Add(verificationStatus);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVerificationStatus", new { id = verificationStatus.VerificationStatusId }, verificationStatus);
        }

        // DELETE: api/VerificationStatus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVerificationStatus(int id)
        {
            var verificationStatus = await _context.VerificationStatuses.FindAsync(id);
            if (verificationStatus == null)
            {
                return NotFound();
            }

            _context.VerificationStatuses.Remove(verificationStatus);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VerificationStatusExists(int id)
        {
            return _context.VerificationStatuses.Any(e => e.VerificationStatusId == id);
        }
    }
}
