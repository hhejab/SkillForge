using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkillForge.API.Models;

namespace SkillForge.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/Categories
        [HttpGet]
        public IActionResult GetCategories()
        {
            return Ok(_context.Categories.ToList());
        }
        // POST: api/Categories
        [HttpPost]
        public IActionResult CreateCategory([FromBody] Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return Ok(category);
        }
    }
}
