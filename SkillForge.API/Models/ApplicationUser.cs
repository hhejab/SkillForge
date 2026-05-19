using Microsoft.AspNetCore.Identity;

namespace SkillForge.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
    }
}