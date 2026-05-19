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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CertificatesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CertificatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Certificates
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Certificate>>> GetCertificates()
        {
            return await _context.Certificates.ToListAsync();
        }

        // GET: api/Certificates/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Certificate>> GetCertificate(int id)
        {
            var certificate = await _context.Certificates.FindAsync(id);

            if (certificate == null)
            {
                return NotFound();
            }

            return certificate;
        }

        // GET: api/Certificates/verify?traineeId=1&certificateCode=SF-CERT-2026-0001
        [AllowAnonymous]
        [HttpGet("verify")]
        public async Task<IActionResult> VerifyCertificate(int traineeId, string certificateCode)
        {
            var certificate = await _context.Certificates
                .Include(c => c.Enrollment)
                    .ThenInclude(e => e!.Trainee)
                .Include(c => c.Enrollment)
                    .ThenInclude(e => e!.Session)
                        .ThenInclude(s => s!.Course)
                .Include(c => c.VerificationStatus)
                .FirstOrDefaultAsync(c =>
                    c.CertificateCode == certificateCode &&
                    c.Enrollment != null &&
                    c.Enrollment.TraineeId == traineeId);

            if (certificate == null || certificate.Enrollment == null)
            {
                return NotFound(new
                {
                    message = "Certificate not found."
                });
            }

            var enrollment = certificate.Enrollment;
            var trainee = enrollment.Trainee;
            var session = enrollment.Session;
            var course = session?.Course;
            var verification = certificate.VerificationStatus;

            if (trainee == null || session == null || course == null || verification == null)
            {
                return NotFound(new
                {
                    message = "Certificate has incomplete related data."
                });
            }

            return Ok(new
            {
                traineeName = trainee.FullName,
                traineeId = enrollment.TraineeId,
                certificateCode = certificate.CertificateCode,
                course = course.Title,
                issueDate = certificate.IssueDate,
                status = verification.StatusName
            });
        }

        // PUT: api/Certificates/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCertificate(int id, Certificate certificate)
        {
            if (id != certificate.CertificateId)
            {
                return BadRequest();
            }

            _context.Entry(certificate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CertificateExists(id))
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

        // POST: api/Certificates
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Certificate>> PostCertificate(Certificate certificate)
        {
            _context.Certificates.Add(certificate);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCertificate", new { id = certificate.CertificateId }, certificate);
        }

        // DELETE: api/Certificates/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCertificate(int id)
        {
            var certificate = await _context.Certificates.FindAsync(id);
            if (certificate == null)
            {
                return NotFound();
            }

            _context.Certificates.Remove(certificate);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CertificateExists(int id)
        {
            return _context.Certificates.Any(e => e.CertificateId == id);
        }
    }
}
